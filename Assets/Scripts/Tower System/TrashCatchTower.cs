using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCatchTower : Tower, IRecycleTrash
{
    [SerializeField] private TrashState catchTrashState;
    [SerializeField] private float durationRecycle;
    
    [SerializeField] private SpriteRenderer netSpriteRenderer;
    [SerializeField] private Transform headTransform;   // the part that rotates
    [SerializeField] private Transform endTransform;   // the part that rotates
    [SerializeField] private Transform grappleOrigin;   // where the line starts (muzzle)
    [SerializeField] private LineRenderer grappleLine;  // 2 points: origin -> hook
    [SerializeField] private float shootSpeed = 25f;
    [SerializeField] private float dragSpeed = 8f;
    [SerializeField] private float catchDistance = 0.3f;
    [SerializeField] private float delayShooting;
    [SerializeField] private float netShowDelay = 0.5f;   // pause after the net lands, before dragging
    [SerializeField] private float netLingerTime = 0.3f;  // keep the net visible after the trash arrives
    
    public int MaxCapacity { get; set; }
    public int CurrentCapacity { get; set; }
    public float DurationRecycleTrash { get; set; }
    public List<Trash> AccumulatedTrashes { get; set; }
    public bool IsRecycleTrash { get; set; }


    private Coroutine _netRoutine;
    
    private void Start()
    {
        MaxCapacity = (int)TowerRunTimeData.GetCurrentStatusValue(TowerStatus.MaxCapacity);
        durationRecycle = TowerData.GetBaseStatusValue(TowerStatus.ProcessingSpeed);
        AccumulatedTrashes = new List<Trash>();
    }
    
    public override void OnDetectingArea(float deltaTime)
    {
        if (!IsBeenPlace || IsRecycleTrash) return;

        TowerScanArea.OnDetecting(deltaTime, out Transform target);
        
        if (target != null && target.TryGetComponent<Trash>(out Trash trash)
            && !AccumulatedTrashes.Contains(trash)
            && trash.TrashData.trashState == catchTrashState
            && IsEnoughSpace(trash.TrashData.trashWeight))
        {
            _netRoutine = StartCoroutine(ShootNetRoutine(trash));
        }
        
        if (CurrentCapacity >= MaxCapacity)
        {
            IsRecycleTrash = true;
            StartCoroutine(RecycleTrash());
        }
    }

    protected override void SetVisualDetectRange()
    {
        
    }
    
    private void UpdateLine(Vector3 endPos)
    {
        grappleLine.positionCount = 2;
        grappleLine.SetPosition(0, grappleOrigin.position);
        grappleLine.SetPosition(1, endPos);
    }

    private void ResetGrappler()
    {
        if (netSpriteRenderer != null) netSpriteRenderer.enabled = false;
        if (grappleLine != null) grappleLine.enabled = false;
        _netRoutine = null;
    }
    
    private IEnumerator ShootNetRoutine(Trash trash)
    {
        // Wait for second (Delay Shooting)
        netSpriteRenderer.enabled = false;
        grappleLine.enabled = false;
        
        yield return new WaitForSeconds(delayShooting);
        
        // ---------- 1. Shoot net ----------
        Vector3 hookPos = grappleOrigin.position;
        grappleLine.enabled = true;

        while (true)
        {
            Vector3 targetPos = endTransform.position;
            hookPos = grappleOrigin.position;
            UpdateLine(hookPos);
            
            if (Vector3.Distance(hookPos, targetPos) <= catchDistance)
            {
                netSpriteRenderer.enabled = true;
                break;
            }
            yield return null;
        }
        
        if (trash == null) { ResetGrappler(); yield break; }
        
        trash.HoldMovement();
        yield return new WaitForSeconds(netShowDelay);
        if (trash == null) { ResetGrappler(); yield break; }
        
        // ---------- 3. DRAG ----------
        Transform trashTf = trash.transform;
        while (trash != null &&
               Vector3.Distance(trashTf.position, grappleOrigin.position) > catchDistance)
        {
            trashTf.position = Vector3.MoveTowards(
                trashTf.position, grappleOrigin.position, dragSpeed * Time.deltaTime);

            UpdateLine(trashTf.position);
            yield return null;
        }
 
        if (trash == null) { ResetGrappler(); yield break; }
        
        // ---------- 4. ARRIVED ----------
        if (!AccumulatedTrashes.Contains(trash))
        {
            AccumulatedTrashes.Add(trash);
            CurrentCapacity += trash.TrashData.trashWeight;
        }
        
        yield return new WaitForSeconds(netLingerTime);
        ResetGrappler();
    }
    
    public IEnumerator RecycleTrash()
    {
        Debug.Log($"[{name}] Start Recycle Trash");
        var trashes = new List<Trash>(AccumulatedTrashes);
        AccumulatedTrashes.Clear();
        
        yield return new WaitForSeconds(durationRecycle);

        foreach (var trash in trashes)
        {
            if (trash == null)
            {
                continue;
            }
            
            trash.DestroyEntity();
        }

        Debug.Log($"[{name}] Done Recycle Trash");
        CurrentCapacity = 0;
        
        yield return null;
        IsRecycleTrash = false;
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
