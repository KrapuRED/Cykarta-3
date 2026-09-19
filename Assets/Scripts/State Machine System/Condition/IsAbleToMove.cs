using UnityEngine;

[CreateAssetMenu(fileName = "IsAbleToMove", menuName = "Scriptable Objects/IsAbleToMove")]
public class IsAbleToMove : ConditionSO
{
    public override bool CheckCondition(Entity entity)
    {
        return entity.IsCanMove;
    }
}
