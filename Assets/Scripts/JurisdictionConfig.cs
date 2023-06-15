using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JurisdictionConfig : MonoBehaviour
{
    public static JurisdictionConfig instance;

    public string name;
    public string description;
    public string[] eba_array;
    public int total_available_budget;
    public string currency;
    public string logo;
    public string[] latLng;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void setName(string _name)
    {
        name = _name;
    }

    public void setDescription(string _description)
    {
        description = _description;
    }

    public void setEBA(string[] _ebaArray)
    {
        eba_array = _ebaArray;
    }

    public void setBudget(int _budget)
    {
        total_available_budget = _budget;
    }

    public void setCurrency(string _currency)
    {
        currency = _currency;
    }

    public void setLogo(string _logo)
    {
        logo = _logo;
    }

    public void setLngLat(string[] _lnglat)
    {
        latLng = _lnglat;
    }

    public int getBudget()
    {
        return total_available_budget;
    }

}
