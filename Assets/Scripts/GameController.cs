using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviourPunCallbacks
{
    public float gameTime = 300f; // Time in seconds
    public float timeRemaining;
    public int budget;
    public int currentCost;
    public Scrollbar scrollbar;
    public TMP_Text timerText;
    public TMP_Text budgetText;

    public GameObject lobbyManager;
    public MapSelection mapSelection;
    public JurisdictionConfig jurisdictionConfig;
    public DropdownItemSelector dropdownItemSelector;
    public GameObject dataPanel;
    public TMP_Text nameLabel;
    public TMP_Text descriptionLabel;
    public TMP_Text costLabel;
    public TMP_Text roleLabel;

    public GameObject dropdownGO;
    public GameObject promptGO;
    public GameObject warningGO;

    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = gameTime;
        budget = jurisdictionConfig.getBudget();
        string roleKey = RoleSelection.RolePropKey;
        string roleValue = PhotonNetwork.LocalPlayer.CustomProperties[roleKey].ToString();
        roleLabel.text = "You are playing as the " + roleValue + " role";
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Update timer
            timeRemaining -= Time.deltaTime;
            //Not going to waiting room like this right now
            /*if (timeRemaining < 0f || budget == 0)
            {
                lobbyManager.GetComponent<LobbyManager>().WaitingRoom();
            }*/

            // Update timer display
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Update budget display
        budgetText.text = string.Format("Budget: € {0}", budget);
    }

    // Buy an item
    public void BuyItem(int cost)
    {
        if (budget >= cost)
        {
            budget -= cost;
            scrollbar.value = 0f; // Reset scrollbar position
        }
    }

    public void UseBudget()
    {
        if (currentCost <= budget)
        {
            budget -= currentCost;
            mapSelection.mapInteractable = true; // Set mapInteractable to true using the reference
            //RemoveSelectedItem();
            dataPanel.SetActive(false);
            Debug.Log("Budget: " + budget);
            dropdownGO.SetActive(false);
            promptGO.SetActive(true);
            budgetText.text = string.Format("Budget: € {0}", budget);
        }
        else
        {
            // Create warning message
            warningGO.SetActive(true);
            Debug.Log("Insufficient budget for this transaction");
        }
    }

    public void SetCurrentCost(int _cost)
    {
        currentCost = _cost;
    }

    private void RemoveSelectedItem()
    {
        int selectedIndex = dropdownItemSelector.dropdown.value;
        dropdownItemSelector.dropdown.options.RemoveAt(selectedIndex);
        dropdownItemSelector.dropdown.RefreshShownValue();
    }

    public void DisplayEBAData(string name, string description, int cost)
    {
        nameLabel.text = name;
        descriptionLabel.text = description;
        costLabel.text = string.Format("Cost: € {0}", cost);
    }

    public void ClosePanel()
    {
        warningGO.SetActive(false);
        dataPanel.SetActive(false);
    }


    // Load waiting room scene
    /*[PunRPC]
    void LoadWaitingRoom()
    {
        SceneManager.LoadScene("WaitingRoom");
    }*/
}
