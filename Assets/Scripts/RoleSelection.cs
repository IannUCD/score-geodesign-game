using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class RoleSelection : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public Button[] roleButtons; // an array of buttons that represent roles
    public Button[] removeButtons; // an array of buttons that can remove roles
    public Button startButton; // a button that becomes active when all roles are selected

    private bool[] roleSelected; // an array that keeps track of which roles are selected
    //private int numRolesSelected = 0; // a counter for the number of roles that are currently selected
    private int selectedRoleIndex = -1; // the index of the currently selected role, -1 if no role is selected
    private const string NumRolesSelectedPropKey = "NumRolesSelected";
    private const string StartButtonStatePropKey = "StartButtonState";
    public const string RolePropKey = "Role";

    private bool firstRole = false;


    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable initialProps = new ExitGames.Client.Photon.Hashtable();
                initialProps[NumRolesSelectedPropKey] = 0;
                PhotonNetwork.CurrentRoom.SetCustomProperties(initialProps);
            }
        }


        roleSelected = new bool[roleButtons.Length]; // initialize the roleSelected array
        for (int i = 0; i < roleButtons.Length; i++)
        {
            int roleIndex = i; // store the index of this button for use in the onClick event

            // assign the onClick event for this button
            roleButtons[i].onClick.AddListener(() => { SelectRole(roleIndex); });
        }

        for (int i = 0; i < removeButtons.Length; i++)
        {
            int roleIndex = i; // store the index of this button for use in the onClick event

            // assign the onClick event for this button
            removeButtons[i].onClick.AddListener(() => { RemoveRole(roleIndex); });
            removeButtons[i].gameObject.SetActive(false);
        }

        // Check the initial state of the start button
        object startButtonStateObj;
        if (PhotonNetwork.CurrentRoom.CustomProperties != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(StartButtonStatePropKey, out startButtonStateObj))
        {
            bool startButtonState = (bool)startButtonStateObj;
            startButton.interactable = startButtonState;
        }
        else
        {
            startButton.interactable = false;
        }
    }

    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(StartButtonStatePropKey))
            {
                if (firstRole)
                {

                    // Get the updated start button state from the custom properties
                    bool startButtonState = (bool)PhotonNetwork.CurrentRoom.CustomProperties[StartButtonStatePropKey];

                    // Update the interactability of the local start button
                    startButton.interactable = startButtonState;

                    // Debug.Log("Start Button State: " + startButtonState);
                }
            }
        }
    }


    // called when a player selects a role
    void SelectRole(int roleIndex)
    {
        // if the player has already selected a role, do nothing
        if (selectedRoleIndex != -1)
            return;

        // if the role is not already selected and no other player has already selected it
        if (!roleSelected[roleIndex] && !RoleTaken(roleIndex))
        {
            
            // if the player has already selected a role, remove the previous role first
            if (selectedRoleIndex != -1)
            {
                RemoveRole(selectedRoleIndex);
            }
            
            // select the new role
            selectedRoleIndex = roleIndex;
            roleSelected[roleIndex] = true;
            roleButtons[roleIndex].interactable = false;
            removeButtons[roleIndex].gameObject.SetActive(true);

            //string roleName = "Role " + roleIndex; // Replace with your desired role name
            string roleName = roleButtons[roleIndex].GetComponentInChildren<Text>().text;

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props[RolePropKey] = roleName;
            Debug.Log("Role Name: "+roleName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            // update the custom property for this role
            props["role_" + roleIndex] = PhotonNetwork.LocalPlayer.ActorNumber;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            // increment the counter for the number of roles selected
            int totalRolesSelected = 0;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(NumRolesSelectedPropKey, out object totalRolesObj))
            {
                totalRolesSelected = (int)totalRolesObj;
            }

            totalRolesSelected++;
            props[NumRolesSelectedPropKey] = totalRolesSelected;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            // update the start button state and interactability
            bool startButtonState = totalRolesSelected == roleButtons.Length;
            //startButtonState &= roleSelected[roleIndex]; // Update start button state based on the selected role
            //startButton.interactable = startButtonState;

            // update the StartButtonState custom property
            props[StartButtonStatePropKey] = startButtonState;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            firstRole = true;
        }
    }

    // called when a player removes a role
    void RemoveRole(int roleIndex)
    {
        // if the role is selected
        if (roleSelected[roleIndex])
        {
            // deselect the role
            roleSelected[roleIndex] = false;
            roleButtons[roleIndex].interactable = true;
            removeButtons[roleIndex].gameObject.SetActive(false);

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props[RolePropKey] = null;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            // update the custom property for this role
            props["role_" + roleIndex] = null;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            // decrement the counter for the number of roles selected
            int totalRolesSelected = 0;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(NumRolesSelectedPropKey, out object totalRolesObj))
            {
                totalRolesSelected = (int)totalRolesObj;
            }

            totalRolesSelected--;

            props[NumRolesSelectedPropKey] = totalRolesSelected;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            // update the start button interactability
            bool startButtonState = false;
            startButton.interactable = startButtonState;

            // update the StartButtonState custom property
            props[StartButtonStatePropKey] = startButtonState;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            selectedRoleIndex = -1;
        }
    }




    // checks if a role has already been taken by another player
    bool RoleTaken(int roleIndex)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.ContainsKey("role_" + roleIndex))
            {
                return true;
            }
        }
        return false;
    }


    //This override can maybe be used instead of the Update to check button state
    /*
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            // Update the start button state for all players
            object numRolesSelectedObj;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(NumRolesSelectedPropKey, out numRolesSelectedObj))
            {
                int totalRolesSelected = (int)numRolesSelectedObj;
                bool startButtonState = totalRolesSelected == roleButtons.Length;

                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props[StartButtonStatePropKey] = startButtonState;
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }
    }
    */
}