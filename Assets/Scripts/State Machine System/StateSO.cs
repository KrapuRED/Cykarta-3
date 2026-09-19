using UnityEngine;

[CreateAssetMenu(fileName = "StateSO", menuName = "Scriptable Objects/StateSO")]
public abstract class StateSO : ScriptableObject
{
    public abstract void EnterState(Entity entity);
    public abstract void ExecuteState(Entity entity, float deltaTime);
    public abstract void ExitState(Entity entity);
}
