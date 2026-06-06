using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform[] waypoints;

    public Transform GetRandomWaypoint()
    {
        if (waypoints.Length == 0) return null;
        return waypoints[Random.Range(0, waypoints.Length)];
    }
}