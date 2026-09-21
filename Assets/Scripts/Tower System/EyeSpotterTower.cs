using UnityEngine;

public class EyeSpotterTower : Tower, IRotateHeadTowerable
{
    
    public override void InitializeTower()
    {
        if (IsBeenPlace) return;
        
        Debug.Log($"[{name}] Initializing Tower System {TowerRunTimeData.towerName} level tower : {TowerRunTimeData.towerLevel}");
        
        base.InitializeTower();
        
        var entityData = EntityManager.Instance.GetEntityRunTimeData(TowerData.towerName, string.Empty,this);
        InitializeEntity(entityData);
    }

    public override void OnDetectingArea(float deltaTime)
    {
        if (!IsBeenPlace) return;
        RotateHead();
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
