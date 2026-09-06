using UnityEngine;
using UnityEngine.UI;

public class NavigatorMap : MonoBehaviour
{
    [Header("UI")]
    public RectTransform mapPanel;
    public RectTransform truckIcon;
    public RectTransform waypointPrefab;
    public RectTransform destinationIcon;

    [Header("World Bounds")]
    public Vector3 worldCenter;
    public Vector3 worldSize = new Vector3(1000f, 0, 1000f);

    private TruckController truck;
    private RouteManager route;

    private void Start()
    {
        truck = FindObjectOfType<TruckController>();
        route = RouteManager.Instance;
        DrawRoute();
    }

    private void Update()
    {
        if (truck != null)
        {
            Vector2 mapPos = WorldToMap(truck.transform.position);
            truckIcon.anchoredPosition = mapPos;
            truckIcon.rotation = Quaternion.Euler(0, 0, -truck.transform.eulerAngles.y);
        }
    }

    private void DrawRoute()
    {
        if (route?.startWaypoint == null) return;

        Waypoint wp = route.startWaypoint;
        while (wp != null)
        {
            RectTransform icon = Instantiate(waypointPrefab, mapPanel);
            icon.anchoredPosition = WorldToMap(wp.transform.position);
            
            if (wp.isDestination)
            {
                icon.GetComponent<Image>().color = Color.green;
            }
            else if (wp.isWrongTurn)
            {
                icon.GetComponent<Image>().color = Color.red;
            }

            wp = wp.nextWaypoint;
        }
    }

    private Vector2 WorldToMap(Vector3 worldPos)
    {
        float x = (worldPos.x - worldCenter.x) / worldSize.x;
        float y = (worldPos.z - worldCenter.z) / worldSize.z;
        
        return new Vector2(
            x * mapPanel.rect.width,
            y * mapPanel.rect.height
        );
    }
}