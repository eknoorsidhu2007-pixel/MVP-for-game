using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool isDestination = false;
    public bool isWrongTurn = false; // Leads to dead end
    public Waypoint nextWaypoint;
    public Waypoint alternateRoute; // For wrong turns that loop back
    public string landmarkName = "";

    [Header("Visuals")]
    public Color gizmoColor = Color.yellow;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 2f);
        
        if (nextWaypoint != null)
        {
            Gizmos.color = isWrongTurn ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, nextWaypoint.transform.position);
        }
    }
}