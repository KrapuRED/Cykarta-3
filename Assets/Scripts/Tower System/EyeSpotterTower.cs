using UnityEngine;

public class EyeSpotterTower : Tower, IRotateHeadTowerable
{
    protected override void InitializeTower()
    {
        Debug.Log($"[{name}] Initializing Tower System {TowerRunTimeData.towerName} level tower : {TowerRunTimeData.towerLevel}");
        
        base.InitializeTower();
        
        var entityData = EntityManager.Instance.GetEntityRunTimeData(TowerData.towerName, string.Empty, this);
        InitializeEntity(entityData);
    }

    public override void OnDetectingArea(float deltaTime)
    {
        if (!IsBeenPlace)
        {
            Debug.LogWarning($"[{name}] OnDetectingArea Is Been Place {IsBeenPlace}");
            return;
        }
        
        TowerScanArea.OnDetecting(deltaTime, out Transform targetToLock);
        if (targetToLock != null)
        {
            if (targetToLock.TryGetComponent<IIrresponsibleThinkable>(out var thinkable) &&
               thinkable.IrresponsibleThinkingData.currentIrresponsibleThinkingMeter >= 25f &&
               !thinkable.IrresponsibleThinkingData.isIrresponsibleThinking)
            {
                CurrentTarget = targetToLock;
                LockToTarget(targetToLock);
            }
            else
            {
                RotateHead();
            }
        }
        else RotateHead();
    }

    public override void LockToTarget(Transform targetToLock)
    {
        IsLocked = true;
        LockRotationToTarget(targetToLock);
        
        if (targetToLock.TryGetComponent<IIrresponsibleThinkable>(out var thinkable))
            thinkable.IrresponsibleThinkingData.isIrresponsibleThinking = true;
    }

    #region === Main Method ===

    private void DecreaseIrresponsibleThinking(float deltaTime)
    {
        if (CurrentTarget == null)
            return;
        
        CurrentTarget.TryGetComponent<Entity>(out var entityData);
        entityData.OnDecreaseIrresponsibleThinking(deltaTime);

        if (entityData.EntityRunTimeData.entityState == EntityState.Moving)
        {
            if (CurrentTarget.TryGetComponent<IIrresponsibleThinkable>(out var thinkable)) 
                thinkable.IrresponsibleThinkingData.isIrresponsibleThinking = false;
            
            CurrentTarget = null;
            IsLocked = false;
        }
    }

    public override void OnUpdateTower(float deltaTime) => DecreaseIrresponsibleThinking(deltaTime);

    #endregion
    
    #region ==== Interface Implement ====
    
    public void RotateHead()
    {
        Debug.Log($"[{name}] Rotating Head");
    }

    public void LockRotationToTarget(Transform target)
    {
        
    }
    
    #endregion
    
}
