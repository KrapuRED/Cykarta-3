using UnityEngine;

[CreateAssetMenu(fileName = "IrresponsibleThinkingCondition", menuName = "Scriptable Objects/IrresponsibleThinkingCondition")]
public class IrresponsibleThinkingCondition : ConditionSO
{
    public override bool CheckCondition(Entity entity)
    {
        entity.OnCheckIrresponsibleThinking(Time.deltaTime);
        return entity.EntityRunTimeData.entityState == EntityState.IrresponsibleThinking;
    }
}
