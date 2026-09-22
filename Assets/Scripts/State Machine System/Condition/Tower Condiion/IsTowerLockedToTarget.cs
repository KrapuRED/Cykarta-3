using UnityEngine;

[CreateAssetMenu(fileName = "IsTowerLockedToTarget", menuName = "State Machine/Condition/IsTowerLockedToTarget")]
public class IsTowerLockedToTarget : ConditionSO
{
    public override bool CheckCondition(Entity entity)
    {
        entity.TryGetComponent<Tower>(out var tower);

        return tower.IsLocked;
    }
}
