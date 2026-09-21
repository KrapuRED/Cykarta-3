using UnityEngine;

[CreateAssetMenu(fileName = "DetectingAreaState", menuName = "State Machine/State/DetectingAreaState")]
public class DetectingAreaState : StateSO
{
    public override void EnterState(Entity entity)
    {
        
    }

    public override void ExecuteState(Entity entity, float deltaTime)
    {
        Debug.Log($"{name} is ExecuteState");
        var tower = entity.GetComponent<Tower>();
        
        tower.OnDetectingArea(deltaTime);
    }

    public override void ExitState(Entity entity)
    {
        
    }
}
