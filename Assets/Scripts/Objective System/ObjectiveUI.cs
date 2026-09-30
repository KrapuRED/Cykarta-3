using System;
using UnityEngine;

public abstract class ObjectiveDataUI { }

public class ProtectionObjectiveDataUI : ObjectiveDataUI
{
    public int currentProtectionLevel;
}

public class EnemyCounterObjectiveDataUI : ObjectiveDataUI
{
    public int currentEnemyCounter;
    public int maxEnemyCounter;
}

public class WaveCounterObjectiveDataUI : ObjectiveDataUI
{
    public int maxWaveCounter;
    public int currentWaveCounter;
}

public abstract class ObjectiveUI : MonoBehaviour
{
    public abstract void UpdateObjectiveUI(ObjectiveDataUI data);
}

public abstract class ObjectiveUI<TData> : ObjectiveUI where TData : ObjectiveDataUI
{
    public sealed override void UpdateObjectiveUI(ObjectiveDataUI data)
    {
        if (data is TData typed)
            Refresh(typed);
        else
            Debug.LogError($"{nameof(GameObject)} expected {typeof(TData).Name} but got {data?.GetType().Name}");
    }

    protected abstract void Refresh(TData data);
}