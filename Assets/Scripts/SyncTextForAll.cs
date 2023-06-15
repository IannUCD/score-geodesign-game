using UnityEngine;
using TMPro;
using Photon.Pun;

public class SyncTextForAll : MonoBehaviourPun, IPunObservable
{
    private TMP_Text textComponent;
    private string syncedText;

    public void Start()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    public void SetText(string newText)
    {
        textComponent = GetComponent<TMP_Text>();
        syncedText = newText;
        Debug.Log("New text: " + syncedText);
        if (textComponent != null)
        {
            Debug.Log("You in here m8");
            textComponent.text = syncedText;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the synced text value to other players
            stream.SendNext(syncedText);
        }
        else
        {
            // Receive the synced text value from the owner and update the text component
            syncedText = (string)stream.ReceiveNext();
            if (textComponent != null)
            {
                textComponent.text = syncedText;
            }
        }
    }
}
