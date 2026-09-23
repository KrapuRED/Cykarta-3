using UnityEngine;

public class EyeSpotterTower : Tower, IRotateHeadTowerable
{
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
        Debug.Log($"[{name}] Rotating Head");
    }

    public void LockRotationToTarget(Transform target)
    {
        
    }
    
    #endregion
    
}
