using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    //public InputField roomInputField;
    public TMP_InputField roomInput;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public GameObject gamePanel;
    public GameObject waitingPanel;
    public GameObject votingPanel;
    public GameObject resultsPanel;
    public GameObject ebaDescriptionPanel;
    public Text ebaNameText;
    public Text ebaDescriptionText;
    public Text roomName;
    public int numPlayersFinished = 0;
    public int numPlayersFinishedVoting = 0;
    public int maxPlayers = 4;
    public TMP_Text waitingRoomText;
    public TMP_Text winnerText;
    public int currentWinner = 0;
    public bool votingFinished = false;

    public GameObject resultRowPrefab; // Prefab for the row in the table
    public Transform resultTableContent; // Transform of the content area in the table


    private const byte FINISH_BUTTON_CLICKED_EVENT = 1;

    //Lobby dropdown and start buttons
    public TMP_Dropdown dropdown;
    public Button startGameButton;
    public GameObject warningMessage;

    public RoomItem roomItemPrefab;
    
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;
    private Dictionary<int, int> votes = new Dictionary<int, int>();


    //Other scripts
    public GameObject mapSelection;
    public GameObject votingManager;
    
    PhotonView photonView;

    private void Start()
    {
        PhotonNetwork.JoinLobby();
        startGameButton.interactable = false;
        warningMessage.SetActive(true);
        photonView = GetComponent<PhotonView>();
        // Add a listener to the dropdown's OnValueChanged event
        /*dropdown.onValueChanged.AddListener(delegate {
            // When the dropdown value changes, check if it's been set to a valid value
            if (dropdown.value != 0)
            {
                // If so, enable the button
                startGameButton.interactable = true;
            }
            else
            {
                // Otherwise, disable the button
                startGameButton.interactable = false;
            }
        });*/

        // Add a listener to the button's onClick event
        startGameButton.onClick.AddListener(delegate {
            // When the button is clicked, check if it's interactable
            if (startGameButton.interactable)
            {
                // If so, do whatever you want the button to do
                Debug.Log("Button clicked!");
            }
            else
            {
                // Otherwise, display a message to the player
                Debug.Log("Button NOT clicked!");
                warningMessage.SetActive(true);
            }
        });

    }

    public void OnClickCreate()
    {
        Debug.Log("Room Input: " + roomInput.text);
        if(roomInput.text.Length >= 1)
        {
            Debug.Log("Before room created");
            PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions() { MaxPlayers = 4});
            Debug.Log("After room created");
        }
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
        
    }

    public void UpdateRoomList(List<RoomInfo> list)
    {
        foreach(RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach(RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
    {
        foreach(PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if(PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItemsList.Add(newPlayerItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public void LoadGame()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
        gamePanel.SetActive(true);

        mapSelection.GetComponent<MapSelection>().LoadGameSceneData();
    }

    public void WaitingRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
        gamePanel.SetActive(false);
        waitingPanel.SetActive(true);
        votingPanel.SetActive(false);
    }

    public void VotingRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
        gamePanel.SetActive(false);
        waitingPanel.SetActive(false);
        votingPanel.SetActive(true);
    }

    public void ResultsRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
        gamePanel.SetActive(false);
        waitingPanel.SetActive(false);
        votingPanel.SetActive(false);
        resultsPanel.SetActive(true);
    }

    public void ExpertDetailsActive()
    {
        ebaDescriptionPanel.SetActive(true);
    }

    public void ExpertDetailsDeactive()
    {
        ebaDescriptionPanel.SetActive(false);
    }

    public void ExpertNameSetText(string _text)
    {
        ebaNameText.text = _text;
    }

    public void ExpertDetailsSetText(string _text)
    {
        ebaDescriptionText.text = _text;
    }

    public void OnFinishButtonClick()
    {
        WaitingRoom();
        photonView.RPC("PlayerFinished", RpcTarget.All);
    }

    [PunRPC]
    void PlayerFinished()
    {
        numPlayersFinished++;
        if (numPlayersFinished == maxPlayers)
        {
            //Load voting screen here
            List<int> playerIDs = GetPlayerIDs();
            waitingRoomText.text = "All players finished!";
            numPlayersFinished = 0; //Reset value for future waiting room use
  
            votingManager.GetComponent<VotingManager>().StartVoting(playerIDs);
            VotingRoom();
            
        }
        else
        {
            waitingRoomText.text = numPlayersFinished + " out of " + maxPlayers + " players finished.";
        }
    }

    [PunRPC]
    void PlayerFinishedVoting()
    {
        numPlayersFinishedVoting++;
        if (numPlayersFinishedVoting == maxPlayers)
        {
            waitingRoomText.text = "All players finished!";
            DisplayResults();
            ResultsRoom();
            
        }
        else
        {
            waitingRoomText.text = numPlayersFinishedVoting + " out of " + maxPlayers + " players finished.";
        }
    }

    List<int> GetPlayerIDs()
    {
        List<int> playerIDs = new List<int>();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerIDs.Add(player.ActorNumber);
            Debug.Log("Player ID: " + player.ActorNumber);
        }
        return playerIDs;
    }

    public void OnFinishVoting()
    {
        WaitingRoom();
        photonView.RPC("PlayerFinishedVoting", RpcTarget.All);
    }



    public void DisplayResults()
    {

        // Clear existing rows from the table
        foreach (Transform child in resultTableContent)
        {
            Destroy(child.gameObject);
        }

        // Get the player IDs of all players in the room
        List<int> playerIDs = GetPlayerIDs();

        // Create a list of roles
        List<string> roles = new List<string>();
        foreach (int playerID in playerIDs)
        {
            Player currentPlayer = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
            string role = currentPlayer.CustomProperties[RoleSelection.RolePropKey].ToString();
            if (!roles.Contains(role))
            {
                roles.Add(role);
            }
        }

        foreach (int playerID in playerIDs)
        {
            Player currentPlayer = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
            string role = currentPlayer.CustomProperties[RoleSelection.RolePropKey].ToString();
            bool[] voteList = currentPlayer.CustomProperties["VoteListPropKey"] as bool[];
            Debug.Log("Role: " +role);
            foreach (bool vote in voteList)
            {
                Debug.Log("Vote: " + vote);
            }
        }

        int count = 0;
        // Iterate over each role
        foreach (string role in roles)
        {
            // Create a new row for the role
            GameObject newRow = Instantiate(resultRowPrefab, resultTableContent);
            TMP_Text playerNameText = newRow.transform.Find("RoleText").GetComponent<TMP_Text>();
            TMP_Text voteResultText = newRow.transform.Find("RoleResults").GetComponent<TMP_Text>();

            // Calculate the total yes and no votes for the role
            int totalYesCount = 0;
            int totalNoCount = 0;
            foreach (int playerID in playerIDs)
            {
                Player currentPlayer = PhotonNetwork.CurrentRoom.GetPlayer(playerID);

                if (currentPlayer.CustomProperties.TryGetValue("VoteListPropKey", out object voteListObj) && voteListObj is bool[] voteList)
                {
                    //if (voteList.Length > count && voteList[count])
                    if (voteList[count])
                    {
                        totalYesCount += 1;
                        Debug.Log("Voting Yes from: " + playerID + " With a total count of: " + totalYesCount);
                    }
                    else
                    {
                        totalNoCount += 1;
                        Debug.Log("Voting No from: " + playerID + " With a total count of: " + totalNoCount);
                    }
                }
                else
                {
                    Debug.Log("Invalid or missing vote list for player: " + playerID);
                }
            }


            if (totalYesCount > currentWinner)
            {
                winnerText.text = role;
                currentWinner = totalYesCount;
            }
            else if (totalYesCount == currentWinner)
            {
                winnerText.text += "\n" + role;
            }


            // Set the role name
            playerNameText.text = "" + role;

            // Set the vote result text
            voteResultText.text = "Yes: " + totalYesCount + ", No: " + totalNoCount;
            count++;
        }
    }
}

