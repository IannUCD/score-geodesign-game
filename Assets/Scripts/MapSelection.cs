using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;


public class MapSelection : MonoBehaviour
{
    public GameObject pinPrefab; // The game object to use as the pin
    public Dictionary<string, List<Vector3>> playerPins = new Dictionary<string, List<Vector3>>();
    public List<Button> buttons = new List<Button>();
    public Button selectedButton;
    public bool mapInteractable;
    public GameObject gameController;

    public string pinName;
    public string ebaName;

    public GameObject dropdownGO;
    public GameObject promptGO;

    private AbstractMap _map;

    PhotonView photonView;

    public GameObject waitingRoomPanel;
    public GameObject mapPanel;
    public GameObject votePanel;
    public Text playerNameText;
    public Text scoreText;
    private List<int> voteResults;
    private int votesReceived;

    private void Update()
    {
        if (mapInteractable && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            Debug.Log("Click");

            if (Physics.Raycast(ray, out RaycastHit hit, 100000f))
            {
                GameObject pinPrefab = Resources.Load<GameObject>(pinName);
                if (pinPrefab != null)
                {

                    GameObject pin = PhotonNetwork.Instantiate(pinPrefab.name, new Vector3(hit.point.x, hit.point.y, 125), Quaternion.identity);
                    pin.SetActive(true);



                    // Find the child object within the pin game object
                    Transform childTransform = pin.transform.Find("PlayerRole");
                    if (childTransform != null)
                    {
                        TMP_Text childText = childTransform.GetComponent<TMP_Text>();
                        if (childText != null)
                        {
                            string roleKey = RoleSelection.RolePropKey;
                            string roleValue = PhotonNetwork.LocalPlayer.CustomProperties[roleKey].ToString();
                            childText.text = roleValue;

                            // Synchronize the text value across the network
                            PhotonView photonView = childText.GetComponent<PhotonView>();
                            if (photonView != null && photonView.IsMine)
                            {
                                photonView.RPC("UpdateText", RpcTarget.OthersBuffered, roleValue);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("TMP_Text component not found on the child object.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Child object not found within the pin prefab.");
                    }


                    // Store the pin name and ID in Photon custom properties
                    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
                    customProperties["PinName"] = pinName;
                    customProperties["PinId"] = pin.name;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
                }
                else
                {
                    Debug.Log("Prefab not found: " + pinName);
                }

                // Add the pin position to the dictionary for the current player
                string currentPlayerId = PhotonNetwork.LocalPlayer.UserId;
                if (!playerPins.ContainsKey(currentPlayerId))
                {
                    playerPins.Add(currentPlayerId, new List<Vector3>());
                }
                playerPins[currentPlayerId].Add(hit.point);

                // Add the string to the list
                List<string> myList = new List<string>();
                if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("ListProperty"))
                {
                    string[] listArray = (string[])PhotonNetwork.LocalPlayer.CustomProperties["ListProperty"];
                    myList = new List<string>(listArray);
                }
                myList.Add(ebaName);

                // Update the custom property for the list
                ExitGames.Client.Photon.Hashtable customPropertiesList = new ExitGames.Client.Photon.Hashtable();
                customPropertiesList["ListProperty"] = myList.ToArray();
                PhotonNetwork.LocalPlayer.SetCustomProperties(customPropertiesList);

                mapInteractable = false;

                dropdownGO.SetActive(true);
                promptGO.SetActive(false);
            }
        }
    }


    private void SetSelectedButton(Button button)
    {
        // Set the selected button and enable map interaction
        selectedButton = button;
        mapInteractable = true;
    }

    private void FinishSelection()
    {
        // Broadcast the player pins to all other players
        photonView.RPC("BroadcastPins", RpcTarget.Others, PhotonNetwork.LocalPlayer.UserId, playerPins[PhotonNetwork.LocalPlayer.UserId]);
    }

    [PunRPC]
    public void BroadcastPins(string playerName, List<Vector3> pinPositions)
    {
        // Display the pins for the player with the given name
        foreach (Vector3 pinPosition in pinPositions)
        {
            Instantiate(pinPrefab, pinPosition, Quaternion.identity);
        }
    }

    private void AddBoxCollidersToChildren()
    {
        foreach (Transform child in transform)
        {
            // Add a box collider to the child object
            BoxCollider collider = child.gameObject.AddComponent<BoxCollider>();

            // Adjust the size of the collider to match the child object's size
            Vector3 size = child.GetComponent<Renderer>().bounds.size;
            collider.size = size;

            // Center the collider on the child object's position
            collider.center = child.localPosition;
        }
    }

    public void LoadGameSceneData()
    {
        // Initialize the dictionary with an empty list for each player
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.UserId != null)
            {
                playerPins.Add(player.UserId, new List<Vector3>());
            }
            else
            {
                Debug.Log("Player.UserId is null for player: " + player.NickName);
            }
        }

        Button[] allButtons = GameObject.FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            if (button.CompareTag("EBAItem"))
            {
                buttons.Add(button);
            }
        }

        // Add event listeners to the buttons
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => SetSelectedButton(button));
        }

        // Disable map interaction
        mapInteractable = false;
        voteResults = new List<int>();

        _map = GetComponent<AbstractMap>();
        AddBoxCollidersToChildren();
    }
}
