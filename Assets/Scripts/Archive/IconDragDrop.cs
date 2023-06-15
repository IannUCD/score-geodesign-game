using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class IconDragDrop : MonoBehaviour
{
    public AbstractMap map;
    public Image iconPrefab;
    public ScrollRect scrollBar;
    public static GameObject currentDraggedObject;
    private Image draggingIcon;

    void Start()
    {
        
        GameObject scrollObject = GameObject.FindGameObjectWithTag("EBAScrollView");
        scrollBar = scrollObject.GetComponent<ScrollRect>();

        // Get the MapboxAbstractMap component on a different GameObject with a specified tag
        GameObject mapObject = GameObject.FindGameObjectWithTag("Map");
        map = mapObject.GetComponent<AbstractMap>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Over Scrollbar");
        // Check if the event target is an image in the scroll bar
        if (eventData.pointerCurrentRaycast.gameObject.transform.parent == scrollBar.transform)
        {
            
            // Instantiate a new icon prefab and set its position to the mouse position
            draggingIcon = Instantiate(iconPrefab);
            draggingIcon.transform.SetParent(map.transform);
            draggingIcon.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // If there is a dragging icon, update its position to the mouse position
        if (draggingIcon != null)
        {
            draggingIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // If there is a dragging icon, convert its position to a world position on the map
        if (draggingIcon != null)
        {
            // Calculate the latitude and longitude of the pin based on the dragging icon's position
            Vector2d latLong = map.WorldToGeoPosition(draggingIcon.transform.position);

            // Destroy the dragging icon
            Destroy(draggingIcon.gameObject);

            // Instantiate a new icon prefab and set its position to the world position of the pin
            Image icon = Instantiate(iconPrefab);
            icon.transform.SetParent(map.transform);
            icon.transform.position = map.GeoToWorldPosition(latLong);
        }
    }
}
