using UnityEngine;

[CreateAssetMenu(fileName = "TowerLockToTargetState", menuName = "State Machine/State/TowerLockToTargetState")]
public class TowerLockToTargetState : StateSO
{
    public override void EnterState(Entity entity)
    {
        
        
    }

    public override void ExecuteState(Entity entity, float deltaTime)
    {
        entity.TryGetComponent<Tower>(out var tower);
        
        tower.OnUpdateTower(deltaTime);
    }

    public override void ExitState(Entity entity)
    {
        
    }
}
