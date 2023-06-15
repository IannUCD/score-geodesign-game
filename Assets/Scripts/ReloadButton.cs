using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ReloadButton : MonoBehaviour
{
    public void ReloadGame()
    {
        // Leave the Photon room
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0); // Load scene 0
    }

}
