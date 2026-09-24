using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pedestrian : Entity, IIrresponsibleThinkable
{
    [SerializeField] private Slider irresponsibleThinkingSlider;
    [SerializeField] private List<TrashChangeData> trashData = new();
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private List<Vector3> waypoints = new ();
    
    [SerializeField] private int indexWaypoint;
    private bool _hasWaypoints;
    
    public IrresponsibleThinkingData IrresponsibleThinkingData { get;  set; }
    public float ThinkingCheckTimer { get; set; }

    public override void OnMoveEntity(float deltaTime)
    {
        base.OnMoveEntity(deltaTime);
        
        if (EntityRunTimeData.entityState == EntityState.IrresponsibleThinking) return;
        
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
            DestroyEntity();

        IsCanMove = _hasWaypoints;
        indexWaypoint = 0;
    }
    
    public void InitializePedestrian(Transform endPoint, float speedMovement, EntityRunTimeData runTimeData)
    {
        IrresponsibleThinkingData = new IrresponsibleThinkingData
        {
            chanceIrresponsibleThinking = runTimeData.irresponsibleThinkingBaseData.chanceIrresponsibleThinking,
            chanceTimeIrresponsibleThinking = runTimeData.irresponsibleThinkingBaseData.chanceTimeIrresponsibleThinking,
            maxIrresponsibleThinkingMeter = runTimeData.irresponsibleThinkingBaseData.maxIrresponsibleThinkingMeter,
            currentIrresponsibleThinkingMeter = 0,
            irresponsibleThinkingIncreaseRate = runTimeData.irresponsibleThinkingBaseData.irresponsibleThinkingIncreaseRate
        };
        
        GetWaypoints(endPoint);
        moveSpeed = speedMovement;
        
        InitializeEntity(runTimeData);
    }

    #region === CheckIrresponsibleThinking ===

    public override void OnCheckIrresponsibleThinking(float deltaTime) => CheckIrresponsibleThinking(deltaTime);
    public void CheckIrresponsibleThinking(float deltaTime)
    {
        if (EntityRunTimeData.entityState == EntityState.IrresponsibleThinking) return;

        ThinkingCheckTimer += deltaTime;
        float interval = IrresponsibleThinkingData.chanceTimeIrresponsibleThinking;
        if (ThinkingCheckTimer < interval) return;

        ThinkingCheckTimer -= interval;
        
        float roll = Random.Range(0f, 100f);
        if (roll <= IrresponsibleThinkingData.chanceIrresponsibleThinking)
        {
            EntityRunTimeData.entityState = EntityState.IrresponsibleThinking;
            irresponsibleThinkingSlider.gameObject.SetActive(true);
        }
    }

    #endregion
    
    #region === CheckIrresponsibleThinking ===

    public TrashType GetTrashType()
    {
        float total  = 0;
        foreach (var trash in trashData)
        {
            if (trash == null) continue;
            total += trash.trashChance;
        }
        
        var trashType = TrashType.Light;
        
        if (!Mathf.Approximately(total, 100f))
        {
            Debug.LogWarning($"Trash chances sum to {total}, expected 100. Returning Light.");
            return trashType;
        }
        
        float roll = Random.Range(0f, total);
        float cumulativeRoll = 0;
        
        foreach (var trash in trashData)
        {
            if (trash == null) continue;

            cumulativeRoll += trash.trashChance;
            if (roll <= cumulativeRoll)
                trashType =  trash.trashType;
        }
        
        return trashType;
    }
    
    public override void OnIncreaseIrresponsibleThinking(float deltaTime) =>  IncreaseIrresponsibleThinking(deltaTime);
    public void IncreaseIrresponsibleThinking(float deltaTime)
    {
        if (EntityRunTimeData.entityState == EntityState.Moving) return;
        
        float increase = IrresponsibleThinkingData.irresponsibleThinkingIncreaseRate * deltaTime;
        IrresponsibleThinkingData.currentIrresponsibleThinkingMeter += increase;
        
        irresponsibleThinkingSlider.value = IrresponsibleThinkingData.currentIrresponsibleThinkingMeter;
        
        if (IrresponsibleThinkingData.currentIrresponsibleThinkingMeter >=
            IrresponsibleThinkingData.maxIrresponsibleThinkingMeter)
        {
            IrresponsibleThinkingData.currentIrresponsibleThinkingMeter = 0;
            ThinkingCheckTimer = 0;
            EntityRunTimeData.entityState = EntityState.Moving;
            SpawnerTrash.Instance.SpawnTrash(transform.position, GetTrashType());
            
            irresponsibleThinkingSlider.gameObject.SetActive(false);
        }
    }

    #endregion
   
    
    #region === CheckIrresponsibleThinking ===
   
    private void DecreaseIrresponsibleThinking(float deltaTime)
    {
        float decrease = IrresponsibleThinkingData.irresponsibleThinkingIncreaseRate * deltaTime;
        
        IrresponsibleThinkingData.currentIrresponsibleThinkingMeter -= decrease;
        irresponsibleThinkingSlider.value = IrresponsibleThinkingData.currentIrresponsibleThinkingMeter;
        
        if (IrresponsibleThinkingData.currentIrresponsibleThinkingMeter <= 0)
        {
            IrresponsibleThinkingData.currentIrresponsibleThinkingMeter = 0;
            EntityRunTimeData.entityState = EntityState.Moving;
            
            irresponsibleThinkingSlider.gameObject.SetActive(false);
            
            Debug.LogWarning($"[{name} (DecreaseIrresponsibleThinking)] Is not do the Irresponsible Think!");
        }
    }
    public override void OnDecreaseIrresponsibleThinking(float deltaTime) => DecreaseIrresponsibleThinking(deltaTime);

    #endregion

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
