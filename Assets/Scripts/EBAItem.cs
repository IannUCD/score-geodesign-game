 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EBAItem : MonoBehaviour
{
    public string id;
    public string name;
    public string description;
    public string icon;
    public int cost;

    public Text nameText;

    LobbyManager manager;
    GameController controller;

    private void Start()
    {
        manager = FindObjectOfType<LobbyManager>();
        controller = FindObjectOfType<GameController>();
        nameText.text = name;
    }

    public void DisplayExpertDetails()
    {
        manager.ExpertDetailsActive();
        manager.ExpertNameSetText(name);
        manager.ExpertDetailsSetText(description);
        controller.SetCurrentCost(cost);
    }

    public void SetID(string _id)
    {
        id = _id;
    }

    public string GetID()
    {
        return id;
    }

    public void SetName(string _name)
    {
        name = _name;
    }

    public string GetName()
    {
        return name;
    }

    public void SetDescription(string _description)
    {
        description = _description;
    }

    public string GetDescription()
    {
        return description;
    }

    public void SetIcon(string _icon)
    {
        icon = _icon;
    }

    public string GetIcon()
    {
        return icon;
    }

    public void SetCost(int _cost)
    {
        cost = _cost;
    }

    public int GetCost()
    {
        return cost;
    }
}
