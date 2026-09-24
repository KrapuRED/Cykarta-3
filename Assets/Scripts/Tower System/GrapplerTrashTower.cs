using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrapplerTrashTower : Tower, IRotateHeadTowerable, IRecycleTrash
{
    private enum GrapplerState { Idle, Tracking, Shooting, Dragging, Cooldown }
    
    [Header("Timing")]
    [SerializeField] private float durationTrackingTarget = 1.5f;
    [SerializeField] private float cooldownAfterCatch = 0.5f;
    [SerializeField] private float durationRecycle;
    
    [Header("Grappler")]
    [SerializeField] private Transform headTransform;   // the part that rotates
    [SerializeField] private Transform grappleOrigin;   // where the line starts (muzzle)
    [SerializeField] private LineRenderer grappleLine;  // 2 points: origin -> hook
    [SerializeField] private float shootSpeed = 25f;
    [SerializeField] private float dragSpeed = 8f;
    [SerializeField] private float catchDistance = 0.3f;
    [SerializeField] private float rotateSpeed = 720f;  // deg/sec
    
    public int MaxCapacity { get; set; }
    public int CurrentCapacity { get; set; }
    public float DurationRecycleTrash { get; set; }
    public List<Trash> AccumulatedTrashes { get; set; }
    public bool IsRecycleTrash { get; set; }
    
    [SerializeField] private GrapplerState _state = GrapplerState.Idle;
    private Trash _targetTrash;
    private Coroutine _grappleRoutine;

    private void Start()
    {
        if (grappleLine != null) grappleLine.enabled = false;
        
        MaxCapacity = TowerRunTimeData.towerCapacity;
        durationRecycle = TowerData.baseTowerProcessingSpeed;
        AccumulatedTrashes = new List<Trash>();
    }
    
    private void OnDisable()
    {
        // Tower sold / disabled mid-grapple
        if (_grappleRoutine != null) StopCoroutine(_grappleRoutine);
        ResetGrappler();
    }
    
     public override void OnDetectingArea(float deltaTime)
    {
        if (!IsBeenPlace) return;
        
        if (_state != GrapplerState.Idle ||  IsRecycleTrash) return;
        
        TowerScanArea.OnDetecting(deltaTime, out Transform targetToLock);
        if (targetToLock == null) return;
 
        if (targetToLock.TryGetComponent<Trash>(out var trash) 
            && !AccumulatedTrashes.Contains(trash)
            && trash.TrashData.trashState == TrashState.Airborne 
            && IsEnoughSpace(trash.TrashData.trashWeight))
        {
            _targetTrash = trash;
            _grappleRoutine = StartCoroutine(GrappleRoutine());
        }
    }

    public override void LockToTarget(Transform targetToLock)
    {
        IsLocked = true;
        LockRotationToTarget(targetToLock);
    }

    protected override void SetVisualDetectRange()
    {
        if (visualRange == null)
        {
            Debug.LogError($"[{name} SetVisualDetectRange] cannot visual range for this tower!");
            return;
        }
      
        float currentRange = towerRunTimeData != null 
            ? TowerRunTimeData.towerRange 
            : TowerData.baseTowerRanger;
      
        // Set Diameter of the range
        float diameter = currentRange * 2f;

        // Compensate for parent scale
        Vector3 parentScale = visualRange.parent != null
            ? visualRange.parent.lossyScale
            : Vector3.one;
      
        //Show visual by currentRange 
        visualRange.localScale = new Vector3(diameter/ parentScale.x, diameter/parentScale.y, 1f);
    }

    #region ==== Interface Implement ====
    
    public void RotateHead()
    {

    }

    public void LockRotationToTarget(Transform target)
    {
        if (headTransform == null || target == null) return;
 
        Vector2 dir = target.position - headTransform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
 
        headTransform.rotation = Quaternion.RotateTowards(
            headTransform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }
    
    public IEnumerator RecycleTrash()
    {
        Debug.Log($"[{name}] Start Recycle Trash");
        var trashes = new List<Trash>(AccumulatedTrashes);
        AccumulatedTrashes.Clear();
        
        yield return new WaitForSeconds(durationRecycle);

        foreach (var trash in trashes)
        {
            trash.DestroyEntity();
        }

        Debug.Log($"[{name}] Done Recycle Trash");
        CurrentCapacity = 0;
        
        yield return null;
        IsRecycleTrash = false;
    }
    
    #endregion

    private void OnTrashCaught(Trash trash)
    {
        Debug.Log($"[{name}] Caught {trash.name}");
        AccumulatedTrashes.Add(trash);
        CurrentCapacity += trash.TrashData.trashWeight;
        
        if (CurrentCapacity >= MaxCapacity)
        {
            Debug.Log($"[{name}] Hitting Max Capacity {CurrentCapacity}/{MaxCapacity}");
            IsRecycleTrash = true;
            StartCoroutine(RecycleTrash());
        }
    }
    
    private bool IsTargetValid()
    {
        if (_targetTrash == null) return false;
        if (_targetTrash.TrashData.trashState != TrashState.Airborne) return false;
        
        float range = towerRunTimeData != null ? TowerRunTimeData.towerRange : TowerData.baseTowerRanger;
        return Vector3.Distance(transform.position, _targetTrash.transform.position) <= range;
    }
    
    private void UpdateLine(Vector3 endPos)
    {
        grappleLine.positionCount = 2;
        grappleLine.SetPosition(0, grappleOrigin.position);
        grappleLine.SetPosition(1, endPos);
    }

    private void DisableTrashPhysics(Trash trash)
    {
        trash.Grab();
        trash.HoldMovement();
    }

    private void ResetGrappler()
    {
        if (grappleLine != null) grappleLine.enabled = false;
        _targetTrash = null;
        _grappleRoutine = null;
        _state = GrapplerState.Idle;
        IsLocked = false;
    }
    
    private IEnumerator GrappleRoutine()
    {
        // Track the trash for durationTrackingTarget
        _state = GrapplerState.Tracking;
        float timer = 0f;
        while (timer < durationTrackingTarget)
        {
            if (!IsTargetValid()) { ResetGrappler(); yield break; }
            
            LockToTarget(_targetTrash.transform);
            timer += Time.deltaTime;
            yield return null;
        }
        
        // After that Shoot Grappler to TrashPosition
        _state = GrapplerState.Shooting;
        Vector3 hookPos = grappleOrigin.position;
        grappleLine.enabled = true;

        while (true)
        {
            if (!IsTargetValid()) { ResetGrappler(); yield break; }
 
            // Aim at the trash's CURRENT position so the hook still lands if it moved
            Vector3 targetPos = _targetTrash.transform.position;
            hookPos = Vector3.MoveTowards(hookPos, targetPos, shootSpeed * Time.deltaTime);
            UpdateLine(hookPos);
 
            if (Vector3.Distance(hookPos, targetPos) <= catchDistance) break;
            yield return null;
        }
        
        // ---------- 3. DRAG ----------
        _state = GrapplerState.Dragging;
        _targetTrash.TrashData.trashState = TrashState.Grabbed;
        DisableTrashPhysics(_targetTrash);
 
        Transform trashTf = _targetTrash.transform;
        while (Vector3.Distance(trashTf.position, grappleOrigin.position) > catchDistance)
        {
            if (_targetTrash == null) { ResetGrappler(); yield break; } // destroyed by something else
 
            trashTf.position = Vector3.MoveTowards(
                trashTf.position, grappleOrigin.position, dragSpeed * Time.deltaTime);
            UpdateLine(trashTf.position);
            yield return null;
        }
 
        // ---------- 4. ARRIVED ----------
        OnTrashCaught(_targetTrash);
 
        _state = GrapplerState.Cooldown;
        grappleLine.enabled = false;
        yield return new WaitForSeconds(cooldownAfterCatch);
 
        ResetGrappler();
    }
    
    public bool IsAlreadyColleted(Trash trash)
    {
        return AccumulatedTrashes.Contains(trash);
    }
    
    public bool IsEnoughSpace(int weight)
    {
        return CurrentCapacity + weight <= MaxCapacity;
    }
}
