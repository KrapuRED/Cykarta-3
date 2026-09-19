using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataStateMachine
{
    public string nameStateCondition;
    public StateSO state;
    public ConditionSO condition;
}

public class StateMachine : MonoBehaviour
{
    [SerializeField] private Entity entityOwner;
    [SerializeField] private List<DataStateMachine> dataStateMachines = new();
    [SerializeField] private StateSO activeState;

    public void UpdateStateMachine(float deltaTime)
    {
        foreach (var data in dataStateMachines)
        {
            if (data.condition.CheckCondition(entityOwner))
            {
                StateSO nextState = data.state;

                if (nextState != activeState)
                {
                    activeState?.ExitState(entityOwner);
                    activeState = nextState;
                    activeState?.EnterState(entityOwner);
                }

                break;
            }
        }

        activeState?.ExecuteState(entityOwner, deltaTime);
    }

    public void ResetCondition()
    {
        activeState?.ExitState(entityOwner);
        activeState = null;
    }
}
