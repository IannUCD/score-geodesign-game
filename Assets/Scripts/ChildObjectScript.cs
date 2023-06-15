using UnityEngine;
using Photon.Pun;
using TMPro;

public class ChildObjectScript : MonoBehaviourPun
{
    private TMP_Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    [PunRPC]
    private void UpdateText(string text)
    {
        textComponent = GetComponent<TMP_Text>();
        // Update the text value of the TMP_Text component on the child object
        if (textComponent != null)
        {
            textComponent.text = text;
        }
    }
}
