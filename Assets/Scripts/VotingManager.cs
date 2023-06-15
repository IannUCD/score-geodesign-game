using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class VotingManager : MonoBehaviourPunCallbacks
{
    public TMP_Text playerText;
    public TMP_Text timerText;
    public TMP_Text listText;
    public TMP_Text budgetText;
    public Button yesButton;
    public Button noButton;
    public List<int> yesResults = new List<int>();
    public List<int> noResults = new List<int>();

    public Dictionary<int, int> totalRoundYesCounts = new Dictionary<int, int>();
    public Dictionary<int, int> totalRoundNoCounts = new Dictionary<int, int>();

    public Dictionary<int, int> playerRoundYesCounts = new Dictionary<int, int>();
    public Dictionary<int, int> playerRoundNoCounts = new Dictionary<int, int>();



    private int currentIndex;
    private List<int> playerIDs;
   // private Dictionary<int, Dictionary<int, bool>> roundVoteData; // Stores the vote data for each player in each round
    private float roundStartTime;
    private bool votingActive = false;

    public GameObject lobbyManager;
    MapSelection mapSelection;

    private void Update()
    {
        if (votingActive)
        {
            float elapsedTime = Time.time - roundStartTime;
            float timeLeft = 30 - elapsedTime;

            if (timeLeft > 0)
            {
                timerText.text = string.Format("Time left: {0:0}", timeLeft);
            }
            /*else //Taking this out for now
            {
                //EndRound();
                Vote(false);
            }*/
        }
    }

    public void StartVoting(List<int> ids)
    {
        currentIndex = 0;
        playerIDs = ids;
        //roundVoteData = new Dictionary<int, Dictionary<int, bool>>();
        roundStartTime = Time.time;
        votingActive = true;
        Debug.Log("Player ID List (Voting): " + playerIDs.Count);
        UpdatePlayerText();
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        // Set initial values of totalRoundYesCounts and totalRoundNoCounts for each player. Can probably delete
        foreach (int playerID in playerIDs)
        {
            Player currentPlayer = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
            currentPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable()
        {
            { "totalRoundYesCounts", new Dictionary<int, int>() },
            { "totalRoundNoCounts", new Dictionary<int, int>() }
        });
        }
    }


    private void UpdatePlayerText()
    {
        int currentPlayerID = playerIDs[currentIndex];
        Player currentPlayer = PhotonNetwork.CurrentRoom.GetPlayer(currentPlayerID);
        playerText.text = "Vote for Role " + currentPlayer.CustomProperties[RoleSelection.RolePropKey];
        Debug.Log("Current Player ID: " + currentPlayerID);
        Debug.Log("Current Player ID: " + currentPlayerID.ToString());

        List<string> myList = new List<string>();
        if (currentPlayer.CustomProperties.ContainsKey("ListProperty"))
        {
            string[] listArray = (string[])currentPlayer.CustomProperties["ListProperty"];
            myList = new List<string>(listArray);
        }

        // Clear the existing text in listText
        listText.text = "";

        // Loop through myList and append each item to listText
        foreach (string item in myList)
        {
            listText.text += item + "\n";
        }

        /*if (currentPlayer.CustomProperties.ContainsKey("Budget"))
        {
            int budget = (int)currentPlayer.CustomProperties["Budget"];
            budgetText.text = "Budget: " + budget.ToString();
        }*/


        //None of this ended up working. The idea was to clear all pins and then just place the ones the a specific role has placed. For now we'll just show them all with associated role names
        // Retrieve pin choices for the current player from the map selection script
        /*mapSelection = FindObjectOfType<MapSelection>();
        List<Vector3> pinChoices;
        if (mapSelection.playerPins.TryGetValue(currentPlayerID.ToString(), out pinChoices))
        {
            // Destroy all pins in the scene
            GameObject[] pins = GameObject.FindGameObjectsWithTag("Pins");
            foreach (GameObject pin in pins)
            {
                Destroy(pin);
            }

            // Display the pin choices for the current player
            foreach (Vector3 pinPosition in pinChoices)
            {
                // Retrieve the pin name from the Photon custom properties
                string pinName = (string)PhotonNetwork.LocalPlayer.CustomProperties["PinName"];

                // Instantiate the pin prefab at the desired location
                GameObject pinPrefab = Resources.Load<GameObject>(pinName);
                GameObject pin = Instantiate(pinPrefab, pinPosition, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log("No pin choices found for current player.");
        }*/
    }


    [PunRPC]
    public void Vote(bool vote)
    {
       // int currentPlayerID = playerIDs[currentIndex];

        // Store the vote data for the current player in the current round
        /*if (!roundVoteData.ContainsKey(currentIndex))
        {
            roundVoteData[currentIndex] = new Dictionary<int, bool>();
        }
        roundVoteData[currentIndex][currentPlayerID] = vote;*/

        //Debug.Log(PhotonNetwork.LocalPlayer.NickName + " voted " + (vote ? "yes" : "no") + " for Role " + PhotonNetwork.CurrentRoom.GetPlayer(currentPlayerID).CustomProperties[RoleSelection.RolePropKey]);

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        if (vote)
        {
            // Update the totalRoundYesCounts for the current player - We can probably kill this
            //PhotonNetwork.LocalPlayer.CustomProperties[roleKey].ToString();
            //Player currentPlayer = PhotonNetwork.CurrentRoom.GetPlayer(currentPlayerID);

            // Update the VoteListPropKey for the current player
            List<bool> voteList;
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("VoteListPropKey"))
            {
                voteList = new List<bool>((bool[])PhotonNetwork.LocalPlayer.CustomProperties["VoteListPropKey"]);
            }
            else
            {
                voteList = new List<bool>();
            }

            voteList.Add(true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "VoteListPropKey", voteList.ToArray() } });
        }
        else
        {
            //Player currentPlayer = PhotonNetwork.CurrentRoom.GetPlayer(currentPlayerID);

            // Update the VoteListPropKey for the current player
            List<bool> voteList;
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("VoteListPropKey"))
            {
                voteList = new List<bool>((bool[])PhotonNetwork.LocalPlayer.CustomProperties["VoteListPropKey"]);
            }
            else
            {
                voteList = new List<bool>();
            }

            voteList.Add(false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "VoteListPropKey", voteList.ToArray() } });

        }

        StartCoroutine(StartNextVoting());
    }




    private IEnumerator StartNextVoting()
    {
        yield return new WaitForSeconds(5f); // Delay before starting the next voting
        
        currentIndex++;

        if (currentIndex < playerIDs.Count)
        {
            UpdatePlayerText();
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
            roundStartTime = Time.time; // Reset the round start time
        }
        else
        {
            EndRound();
        }
    }

    public void EndRound()
    {
        votingActive = false;
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        Dictionary<int, int> roundYesCounts = new Dictionary<int, int>();
        Dictionary<int, int> roundNoCounts = new Dictionary<int, int>();

       /* foreach (var roundVote in roundVoteData)
        {
            int roundNumber = roundVote.Key;
            Dictionary<int, bool> voteData = roundVote.Value;

            int yesCount = 0;
            int noCount = 0;

            foreach (var voteEntry in voteData)
            {
                int playerId = voteEntry.Key;
                bool vote = voteEntry.Value;

                if (vote)
                    yesCount++;
                else
                    noCount++;
            }

            roundYesCounts[roundNumber] = yesCount;
            roundNoCounts[roundNumber] = noCount;
        }*/

        foreach (var roundVoteCount in roundYesCounts)
        {
            int roundNumber = roundVoteCount.Key;
            int yesCount = roundVoteCount.Value;
            int noCount = roundNoCounts[roundNumber];

            Debug.Log("Round " + roundNumber + " - Yes: " + yesCount + ", No: " + noCount);
        }

        lobbyManager.GetComponent<LobbyManager>().OnFinishVoting();
    }

}