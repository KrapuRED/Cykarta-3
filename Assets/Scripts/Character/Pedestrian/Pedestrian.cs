using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestrian : Entity
{
    [SerializeField] private Transform endPoint;
    [SerializeField] private List<Vector3> waypoints = new ();
    [SerializeField] private bool _hasWaypoints;

    private void Start()
    {
       StartCoroutine(DelayedUpdate());
    }

    public void GetWaypoints(Transform endPoint)
    {
        var gridMap = GridManager.Instance.BuildingGridMap;
        
        GridManager.Instance.GetGridMapZoneCell(transform.position, out var myZone);
        
        var path = GridPathfinder.FindPath(gridMap, transform.position, endPoint.position, myZone);

        if (path != null)
        {
            waypoints.Clear();
            waypoints.AddRange(path);
            _hasWaypoints = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;

        Gizmos.color = Color.green;

        // Draw lines connecting consecutive waypoints
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }

        // Optional: Draw a sphere at each waypoint to mark the nodes
        foreach (var waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint, 0.2f);
        }
    }
    
    private IEnumerator DelayedUpdate()
    {
        yield return new WaitForEndOfFrame();
        GetWaypoints(endPoint);
    }
}
