using UnityEngine;

[CreateAssetMenu(fileName = "MovingEntityState", menuName = "Scriptable Objects/MovingEntityState")]
public class MovingEntityState : StateSO
{
    public override void EnterState(Entity entity)
    {
        
    }

    public override void ExecuteState(Entity entity, float deltaTime)
    {
        if (!entity.IsCanMove) return;
        
        entity.OnMoveEntity(deltaTime);
    }

    public override void ExitState(Entity entity)
    {
        
    }
}
