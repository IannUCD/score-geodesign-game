using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class JSONEBA : MonoBehaviour
{
    //public JurisdictionConfig jurisdictionConfig;

    public string jsonURL;

    public EBAItem ebaItemPrefab;
    public Transform contentObject;

    public TMP_Dropdown dropdown;
    public DropdownItemSelector itemSelector;

    public List<EBAItem> ebaListItems = new List<EBAItem>();
    //public Dropdown dropdown;

    //public List<string> dropdownNames = new List<string>();

    [System.Serializable]
    public class EBA
    {
        public string id;
        public string name;
        public string description;
        public string icon;
        public int cost;
    }

    [System.Serializable]
    public class EBAList
    {
        public EBA[] eba;
    }

    public EBAList myEBAs = new EBAList();

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
                Debug.Log("Json: " + request.downloadHandler.text);
                ProcessJsonData(request.downloadHandler.text);
                Debug.Log("Blep");
            }
        }
    }

    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownItemSelected);
        StartCoroutine(getData());
        Debug.Log(myEBAs.eba.Length);
        //myJurisdictions = JsonUtility.FromJson<JurisdictionList>(textJSON.text);
    }

    private void ProcessJsonData(string _url)
    {
        myEBAs = JsonUtility.FromJson<EBAList>(_url);
        Debug.Log(myEBAs.eba[0].name);


        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

        foreach (EBA eba in myEBAs.eba)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(eba.name);
            EBAItem newEBA = Instantiate(ebaItemPrefab, contentObject);
            newEBA.SetID(eba.id);
            newEBA.SetName(eba.name);
            newEBA.SetDescription(eba.description);
            newEBA.SetIcon(eba.icon);
            newEBA.SetCost(eba.cost);
            ebaListItems.Add(newEBA);
            dropdownOptions.Add(optionData);
        }

        dropdown.AddOptions(dropdownOptions);
    }

    void OnDropdownItemSelected(int index)
    {
        itemSelector.OnDropdownItemSelected(index);
    }
}