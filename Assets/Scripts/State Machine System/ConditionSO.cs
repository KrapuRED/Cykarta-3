using UnityEngine;

[CreateAssetMenu(fileName = "ConditionSO", menuName = "Scriptable Objects/ConditionSO")]
public abstract class ConditionSO : ScriptableObject
{
    public abstract bool CheckCondition(Entity entity);
}
