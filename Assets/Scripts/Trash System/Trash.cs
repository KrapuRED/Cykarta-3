using System.Collections.Generic;
using UnityEngine;

public class Trash : Entity
{
    [SerializeField] private TrashDataSO trashData;
    [SerializeField] private float moveSpeed;
    [SerializeField] private List<Vector3> waypoints = new ();
    
    [SerializeField] private int indexWaypoint;
    private bool _hasWaypoints;
    
    public TrashData TrashData { get; private set; }

    private void Awake()
    {
        TrashData = new TrashData
        {
            trashName = trashData.displayName,
            trashType = trashData.trashType,
            trashWeight = trashData.trashWeight,
            trashState = TrashState.Grounded
        };
    }
    
    public override void OnMoveEntity(float deltaTime)
    {
        base.OnMoveEntity(deltaTime);
        
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
        
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * deltaTime);
    }
    
    private void GetWaypoints(Transform endPoint)
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
        else
        {
            DestroyEntity();
        }

        IsCanMove = _hasWaypoints;
        indexWaypoint = 0;
    }
    
    public void InitializeTrash(Transform endPoint, float speedMovement, EntityRunTimeData entityData)
    {
        GetWaypoints(endPoint);
        moveSpeed = speedMovement;
        
        TrashData.trashState = TrashState.Grounded;
        InitializeEntity(entityData);
    }
    
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;

        Gizmos.color = Color.cornflowerBlue;

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
    
    public void HoldMovement() => IsCanMove = false;
    public void UnholdMovement() => IsCanMove = true;
}
