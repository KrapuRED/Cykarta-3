using UnityEngine;
using MoreMountains.Tools;

public class EyeSpotterTower : Tower, IRotateHeadTowerable
{
    [Header("Rotate Head Tower Config")]
    [SerializeField] private Transform towerHead;
    [SerializeField] private float spriteAngleOffset = -90f; // -90 if head sprite faces up, 0 if it faces right
    [SerializeField] private float lockRotationSpeed = 720f;
    [SerializeField] private float speedRotation;
   
    [Header("Sound Effect and ambient")]
    [SerializeField] protected AudioClip scanSound;
    [SerializeField] protected float scanSoundInterval = 1f; // seconds between sounds
    [SerializeField] protected float volumeScanSound;
    private bool _scanSound;
    private float _scanSoundTimer;
    
    private float _currentTargetAngle;
    private bool _isDecreasing;
    private const float DecreaseStartPercent = 0.25f;
    
    private void Start()
    {
        TowerScanArea.TargetFilter = CanLock;
    }
    
    private bool CanLock(Transform t)
    {
        if (!t.TryGetComponent<Entity>(out var entity) ||
            !t.TryGetComponent<IIrresponsibleThinkable>(out var thinkable)) return false;

        if (entity.EntityRunTimeData.entityState != EntityState.IrresponsibleThinking) return false;

        var owner = thinkable.IrresponsibleThinkingData.lockedBy;
        return owner == null || owner == this;
    }
    
    public override void OnDetectingArea(float deltaTime)
    {
        if (!IsBeenPlace)
        {
            Debug.LogWarning($"[{name}] OnDetectingArea Is Been Place {IsBeenPlace}");
            return;
        }
        
        if (IsLocked && CurrentTarget != null)
        {
            LockRotationToTarget(CurrentTarget);
            return;
        }
     
        Debug.Log($"{name} OnDetectingArea");
        
        TowerScanArea.OnDetecting(deltaTime, out Transform targetToLock);
        if (targetToLock != null && CanLock(targetToLock))
        {
            CurrentTarget = targetToLock;
            LockToTarget(targetToLock);
        }
        else RotateHead();
    }

    public override void LockToTarget(Transform targetToLock)
    {
        if (targetToLock.TryGetComponent<IIrresponsibleThinkable>(out var thinkable))
            thinkable.IrresponsibleThinkingData.lockedBy = this;

        IsLocked = true;
        LockRotationToTarget(targetToLock);
    }

    #region === Main Method ===

    public override void OnUpdateTower(float deltaTime)
    {
        if (!IsLocked) return;
        
        HandleScanSound(deltaTime);
        
        if (CurrentTarget == null ||
            !CurrentTarget.TryGetComponent<Entity>(out var entity) ||
            !CurrentTarget.TryGetComponent<IIrresponsibleThinkable>(out var thinkable))
        {
            ReleaseTarget();
            return;
        }
        
        if (entity.EntityRunTimeData.entityState != EntityState.IrresponsibleThinking)
        {
            ReleaseTarget();
            return;
        }
        
        var data = thinkable.IrresponsibleThinkingData;
        LockRotationToTarget(CurrentTarget);
        
        if (!_isDecreasing)
        {
            if (data.currentIrresponsibleThinkingMeter / data.maxIrresponsibleThinkingMeter <=
                DecreaseStartPercent) return;
            
            _isDecreasing = true;
            data.isBeingDecreased = true;
        }

        entity.OnDecreaseIrresponsibleThinking(deltaTime);
    }

    private void ReleaseTarget()
    {
        if (CurrentTarget != null &&
            CurrentTarget.TryGetComponent<IIrresponsibleThinkable>(out var t))
        {
            if (t.IrresponsibleThinkingData.lockedBy == this)
                t.IrresponsibleThinkingData.lockedBy = null;

            t.IrresponsibleThinkingData.isBeingDecreased = false;
        }

        CurrentTarget = null;
        IsLocked = false;
        _isDecreasing = false;
    }
    #endregion

    protected override void SetVisualDetectRange()
    {
        if (visualRange == null)
        {
            Debug.LogError($"[{name} SetVisualDetectRange] cannot visual range for this tower!");
            return;
        }
      
        float currentRange = towerRunTimeData != null 
            ? TowerRunTimeData.GetCurrentStatusValue(TowerStatus.Range)
            : (int)towerData.GetBaseStatusValue(TowerStatus.Range);
      
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
        _currentTargetAngle -= Time.deltaTime * speedRotation;
        _currentTargetAngle %= 360;
        
        towerHead.rotation = Quaternion.Euler(0, 0, _currentTargetAngle);
    }

    public void LockRotationToTarget(Transform target)
    {
        Vector2 dir = target.position - towerHead.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + spriteAngleOffset;

        _currentTargetAngle = Mathf.MoveTowardsAngle(_currentTargetAngle, targetAngle, lockRotationSpeed * Time.deltaTime);
        towerHead.rotation  = Quaternion.Euler(0f, 0f, _currentTargetAngle);
    }
    
    #endregion
    
    private void HandleScanSound(float deltaTime)
    {
        if (scanSound == null || scanSoundInterval <= 0f) return;
        
        _scanSoundTimer += deltaTime;
        if (_scanSoundTimer < scanSoundInterval)
        {
            return;
        }

        _scanSoundTimer -= scanSoundInterval;

        MMSoundManagerSoundPlayEvent.Trigger(
            scanSound,
            MMSoundManager.MMSoundManagerTracks.Sfx,
            transform.position,
            volume: volumeScanSound);
    }
}
