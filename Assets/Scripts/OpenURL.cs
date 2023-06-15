using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public string url = "https://score-eu-project.eu/";

    public void Open()
    {
        Application.OpenURL(url);
    }
}
