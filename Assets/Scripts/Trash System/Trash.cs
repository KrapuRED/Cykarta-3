using System.Collections;
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
    public Coroutine ThrowCoroutine { get; private set; }

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
    
    private IEnumerator ThrowRoutine(EntityRunTimeData entityRunTimeData, Transform endWaterPath, float arcHeight ,float duration, Vector3 from, Vector3 to)
    {
        TrashData.trashState = TrashState.Airborne;
      
        float t = 0;
        while (t < 1f)
        {
            t+= Time.deltaTime / duration;
            float clamped = Mathf.Clamp01(t);
         
            Vector3 pos = Vector3.Lerp(from, to, clamped);
            pos.y += arcHeight * 4f * clamped * (1f - clamped);
            transform.position = pos;
         
            yield return null;
        }
      
        transform.position = to;
        InitializeTrash(endWaterPath, 2f,entityRunTimeData );
    }

    public void ThrowTrash(EntityRunTimeData entityRunTimeData, Transform endWaterPath, float arcHeight ,float duration, Vector3 from, Vector3 to)
    {
        ThrowCoroutine = StartCoroutine(ThrowRoutine(entityRunTimeData, endWaterPath, arcHeight, duration, from, to));
    }
    
    public void StopThrow()
    {
        if (ThrowCoroutine == null) return;

        StopCoroutine(ThrowCoroutine);
        ThrowCoroutine = null;
    }

    public void Grab()
    {
        StopThrow();
        HoldMovement();
        TrashData.trashState = TrashState.Grabbed;
    }
    
    public void HoldMovement() => IsCanMove = false;
    public void UnholdMovement() => IsCanMove = true;
}
