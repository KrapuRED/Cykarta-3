using UnityEngine;

[CreateAssetMenu(fileName = "IrresponsibleThinkingState", menuName = "Scriptable Objects/IrresponsibleThinkingState")]
public class IrresponsibleThinkingState : StateSO
{
    public override void EnterState(Entity entity)
    {
        
    }

    public override void ExecuteState(Entity entity, float deltaTime)
    {
        entity.OnIncreaseIrresponsibleThinking(deltaTime);
    }

    public override void ExitState(Entity entity)
    {
        
    }
}
