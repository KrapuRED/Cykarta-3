using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestrian : Entity
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private List<Vector3> waypoints = new ();
    
    [SerializeField] private int indexWaypoint;
    [SerializeField] private bool _hasWaypoints;
    
    private void Update()
    {
        if (!_hasWaypoints) return;
        
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        if (waypoints.Count <= 0)
        {
            DestroyEntity();
            return;
        }
        
        if (Vector3.Distance(transform.position, waypoints[indexWaypoint]) < 0.1f)
        {
            indexWaypoint++;
            if (indexWaypoint >= waypoints.Count)
            {
                DestroyEntity();
                return;
            }
        }
        
        Vector3 targetPosition = waypoints[indexWaypoint];
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        transform.Translate(direction * moveSpeed * Time.deltaTime);
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

        indexWaypoint = 0;
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
}
