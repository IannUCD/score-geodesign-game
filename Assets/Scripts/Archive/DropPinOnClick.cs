//DEPRICATED
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class DropPinOnClick : MonoBehaviour
{
    public GameObject pinPrefab; // The game object to instantiate as the pin
    private AbstractMap _map;

    private void Start()
    {
        _map = GetComponent<AbstractMap>();
        AddBoxCollidersToChildren();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Debug.Log("Mouse down");
                // Instantiate the pin at the hit location
                Instantiate(pinPrefab, hit.point, Quaternion.identity);
            }
        }
    }

    private void AddBoxCollidersToChildren()
    {
        foreach (Transform child in transform)
        {
            // Add a box collider to the child object
            BoxCollider collider = child.gameObject.AddComponent<BoxCollider>();

            // Adjust the size of the collider to match the child object's size
            Vector3 size = child.GetComponent<Renderer>().bounds.size;
            collider.size = size;

            // Center the collider on the child object's position
            collider.center = child.localPosition;
        }
    }
}