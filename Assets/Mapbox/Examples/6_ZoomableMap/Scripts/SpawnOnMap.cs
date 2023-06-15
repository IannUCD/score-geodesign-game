using UnityEngine;
using UnityEngine.UI;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class PinDropper : MonoBehaviour
{
    public AbstractMap map;
    public Image iconPrefab;
    public Scrollbar scrollBar;

    private void Start()
    {
        // Set up an event listener for the scrollbar's onValueChanged event
        scrollBar.onValueChanged.AddListener(OnScrollBarValueChanged);
    }

    private void OnScrollBarValueChanged(float value)
    {
        // Calculate the latitude and longitude of the pin based on the scrollbar's value
        Vector2d latLong = new Vector2d(50.0f, -120.0f + value * 240.0f);

        // Convert the latitude and longitude to a world position on the map
        Vector3 worldPos = map.GeoToWorldPosition(latLong);

        // Instantiate the icon prefab and set its position to the world position of the pin
        Image icon = Instantiate(iconPrefab);
        icon.transform.SetParent(map.transform);
        icon.transform.position = worldPos;
    }
}