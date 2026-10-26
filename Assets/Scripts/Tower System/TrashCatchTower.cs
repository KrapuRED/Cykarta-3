using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCatchTower : Tower, IRecycleTrash
{
    [SerializeField] private bool enableOvercapacity;
    [SerializeField] private TrashState catchTrashState;
    [SerializeField] private float durationRecycle;
    
    [SerializeField] private SpriteRenderer netSpriteRenderer;
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

    private bool _isCatching;
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

        // Recycle only when full AND no net is in flight
        if (CurrentCapacity >= MaxCapacity && !_isCatching)
        {
            IsRecycleTrash = true;
            StartCoroutine(RecycleTrash());
            return;
        }

        if (_isCatching) return;   // one catch at a time

        TowerScanArea.OnDetecting(deltaTime, out Transform target);

        if (target != null && target.TryGetComponent<Trash>(out Trash trash)
                           && !AccumulatedTrashes.Contains(trash)
                           && trash.TrashData.trashState == catchTrashState
                           && IsEnoughSpace(trash.TrashData.trashWeight))
        {
            if (trash.HoldBy != null) return;
            
            _isCatching = true;
            
            if (_netRoutine != null) return;
            
            _netRoutine = StartCoroutine(ShootNetRoutine(trash));
        }
    }
    
    private void UpdateLine(Vector3 endPos)
    {
        grappleLine.useWorldSpace = true;
        grappleLine.positionCount = 2;
        grappleLine.SetPosition(0, grappleOrigin.position);
        grappleLine.SetPosition(1, endPos);
        
    }

    private void ResetGrappler()
    {
        if (netSpriteRenderer != null) netSpriteRenderer.enabled = false;
        if (grappleLine != null)
        {
            grappleLine.SetPosition(0, grappleOrigin.position);
            grappleLine.SetPosition(1, grappleOrigin.position);
            grappleLine.enabled = false;
        }
        _netRoutine = null;
    }
    
    private IEnumerator ShootNetRoutine(Trash trash)
    {
        try
        {
            netSpriteRenderer.enabled = false;
            grappleLine.enabled = false;

            yield return new WaitForSeconds(delayShooting);
            if (trash == null) yield break;

            Vector3 hookPos = grappleOrigin.position;
            grappleLine.enabled = true;

            while (Vector3.Distance(hookPos, endTransform.position) > catchDistance)
            {
                hookPos = Vector3.MoveTowards(hookPos, endTransform.position, shootSpeed * Time.deltaTime);
                UpdateLine(hookPos);
                yield return null;
            }

            if (trash == null) yield break;
            
            //netSpriteRenderer.transform.position = endTransform.position;
            netSpriteRenderer.enabled = true;
            
            trash.HoldMovement(this);
            yield return new WaitForSeconds(netShowDelay);

            Vector3 retractPos = endTransform.position;
            
            while (trash != null &&
                   Vector3.Distance(trash.transform.position, grappleOrigin.position) > catchDistance)
            {
                retractPos = Vector3.MoveTowards(
                    retractPos, grappleOrigin.position, dragSpeed * Time.deltaTime);
                
                trash.transform.position = retractPos;   // trash rides the hook
                
                //netSpriteRenderer.transform.position = retractPos;
                UpdateLine(retractPos);
                yield return null;
            }

            if (trash == null) yield break;   // destroyed while dragging

            if (!AccumulatedTrashes.Contains(trash))
            {
                AccumulatedTrashes.Add(trash);
                CurrentCapacity += trash.TrashData.trashWeight;
            }
        }
        finally
        {
            ResetGrappler();       // always runs, even on yield break or an exception
            _isCatching = false;
        }
    }
    
    public IEnumerator RecycleTrash()
    {
        RecycleProcessingUI.ShowProcessingUI();

        var trashes = new List<Trash>(AccumulatedTrashes);
        AccumulatedTrashes.Clear();

        try
        {
            float elapsed = 0f;
            while (elapsed < durationRecycle)
            {
                elapsed += Time.deltaTime;
                RecycleProcessingUI.UpdateProgress(durationRecycle > 0f ? elapsed / durationRecycle : 1f);
                yield return null;
            }

            int moneyGain = 0;
            foreach (var trash in trashes)
            {
                if (trash == null) continue;   // destroyed while waiting
                moneyGain += trash.TrashData.trashReward;
                trash.DestroyEntity();
            }

            CurrencyManager.Instance.ReceiveCurrency(moneyGain);
        }
        finally
        {
            CurrentCapacity = 0;
            RecycleProcessingUI.HideProcessingUI();
            IsRecycleTrash = false;
        }
    }

    public bool IsAlreadyColleted(Trash trash)
    {
        return AccumulatedTrashes.Contains(trash);
    }
    
    public bool IsEnoughSpace(int weight)
    {
        if (CurrentCapacity + weight <= MaxCapacity) return true;
        return enableOvercapacity && CurrentCapacity < MaxCapacity;
    }
}
