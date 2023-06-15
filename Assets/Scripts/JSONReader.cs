using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class JSONReader : MonoBehaviour
{
    public JurisdictionConfig jurisdictionConfig;

    public TextAsset textJSON;
    public string jsonURL;

    public TMP_Dropdown dropdown;

    public List<string> dropdownNames = new List<string>();

    [System.Serializable]
    public class Jurisdiction{
        public string name;
        public string description;
        public string[] eba_array;
        public int total_available_budget;
        public string currency;
        public string logo;
        public string[] latLng;
    }

    [System.Serializable]
    public class JurisdictionList
    {
        public Jurisdiction[] jurisdiction;
    }

    public JurisdictionList myJurisdictions = new JurisdictionList();

    IEnumerator getData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(jsonURL))
        {
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log("Couldn't pull data");
            }
            else
            {
                Debug.Log("Success");
                Debug.Log("Json: " +request.downloadHandler.text);
                ProcessJsonData(request.downloadHandler.text);
            }
        }
    }

    void Start()
    {
        StartCoroutine(getData());
        Debug.Log(myJurisdictions.jurisdiction.Length);
        //myJurisdictions = JsonUtility.FromJson<JurisdictionList>(textJSON.text);
    }

    private void ProcessJsonData(string _url)
    {
        myJurisdictions = JsonUtility.FromJson<JurisdictionList>(_url);
        Debug.Log(myJurisdictions.jurisdiction[0].name);
        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

        foreach (Jurisdiction jurisdiction in myJurisdictions.jurisdiction)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(jurisdiction.name);
            dropdownOptions.Add(optionData);
        }

        dropdown.AddOptions(dropdownOptions);
    }


    private void Update()
    {
        jurisdictionConfig.setName(myJurisdictions.jurisdiction[dropdown.value].name);
        jurisdictionConfig.setDescription(myJurisdictions.jurisdiction[dropdown.value].description);
        jurisdictionConfig.setEBA(myJurisdictions.jurisdiction[dropdown.value].eba_array);
        jurisdictionConfig.setBudget(myJurisdictions.jurisdiction[dropdown.value].total_available_budget);
        jurisdictionConfig.setCurrency(myJurisdictions.jurisdiction[dropdown.value].currency);
        jurisdictionConfig.setLogo(myJurisdictions.jurisdiction[dropdown.value].logo);
        jurisdictionConfig.setLngLat(myJurisdictions.jurisdiction[dropdown.value].latLng);
    }

}
