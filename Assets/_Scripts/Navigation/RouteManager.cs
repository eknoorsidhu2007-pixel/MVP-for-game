using Mirror;
using UnityEngine;

public class RouteManager : NetworkBehaviour
{
    public static RouteManager Instance;

    [Header("Route")]
    public Waypoint startWaypoint;
    public Waypoint currentWaypoint;
    public float reachDistance = 10f;

    [SyncVar] public bool destinationReached = false;
    [SyncVar] public float totalRouteDistance = 0f;
    [SyncVar] public float distanceTraveled = 0f;

    private TruckController truck;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        currentWaypoint = startWaypoint;
        CalculateTotalDistance();
    }

    [ServerCallback]
    private void Update()
    {
        if (GameManager.Instance?.currentState != GameState.Driving) return;
        
        truck = truck ?? FindObjectOfType<TruckController>();
        if (truck == null || currentWaypoint == null) return;

        float dist = Vector3.Distance(truck.transform.position, currentWaypoint.transform.position);
        
        if (dist <= reachDistance)
        {
            if (currentWaypoint.isDestination)
            {
                destinationReached = true;
                GameManager.Instance.EndRun(true);
                return;
            }

            // Progress to next
            currentWaypoint = currentWaypoint.nextWaypoint;
        }

        // Update distance traveled approximation
        distanceTraveled = CalculateDistanceTraveled();
    }

    [Server]
    private void CalculateTotalDistance()
    {
        float dist = 0f;
        Waypoint wp = startWaypoint;
        while (wp != null && wp.nextWaypoint != null)
        {
            dist += Vector3.Distance(wp.transform.position, wp.nextWaypoint.transform.position);
            wp = wp.nextWaypoint;
        }
        totalRouteDistance = dist;
    }

    [Server]
    private float CalculateDistanceTraveled()
    {
        // Simplified: distance from start to current position along waypoints
        float dist = 0f;
        Waypoint wp = startWaypoint;
        while (wp != null && wp != currentWaypoint && wp.nextWaypoint != null)
        {
            dist += Vector3.Distance(wp.transform.position, wp.nextWaypoint.transform.position);
            wp = wp.nextWaypoint;
        }
        if (wp != null && truck != null)
        {
            dist += Vector3.Distance(wp.transform.position, truck.transform.position);
        }
        return dist;
    }

    public float GetProgressPercent()
    {
        if (totalRouteDistance <= 0) return 0f;
        return Mathf.Clamp01(distanceTraveled / totalRouteDistance);
    }
}