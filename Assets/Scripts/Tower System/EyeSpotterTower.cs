using UnityEngine;

public class EyeSpotterTower : Tower, IRotateHeadTowerable
{
    [SerializeField] private float limitFrame;
    
    public override void InitializeTower()
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
        
        if (targetToLock != null) LockToTarget(targetToLock);
        else RotateHead();
    }

    public override void LockToTarget(Transform targetToLock)
    {
        LockRotationToTarget(targetToLock);
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
