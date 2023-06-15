using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayRoleDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject messagePanel;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        messagePanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        messagePanel.SetActive(false);
    }
}
