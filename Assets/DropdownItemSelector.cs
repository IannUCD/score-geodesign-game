using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropdownItemSelector : MonoBehaviour
{
    public JSONEBA jsonEBA;
    public GameController gameController;

    public GameObject dataPanel;
    public TMP_Text nameLabel;
    public TMP_Text descriptionLabel;
    public TMP_Text costLabel;

    MapSelection mapManager;

    public TMP_Dropdown dropdown; // Add this variable to reference the dropdown component

    public void OnDropdownItemSelected(int index)
    {
        JSONEBA.EBA selectedEBA = jsonEBA.myEBAs.eba[index];
        DisplayEBAData(selectedEBA);

        // Call the GameController method to set the current cost
        gameController.SetCurrentCost(selectedEBA.cost);
    }

    private void DisplayEBAData(JSONEBA.EBA ebaData)
    {
        nameLabel.text = ebaData.name;
        descriptionLabel.text = ebaData.description;
        costLabel.text = string.Format("Cost: {0}", ebaData.cost);
        mapManager = FindObjectOfType<MapSelection>();
        mapManager.pinName = ebaData.icon;
        mapManager.ebaName = ebaData.name;

        dataPanel.SetActive(true);
    }
}
