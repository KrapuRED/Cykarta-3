using UnityEngine;

public class IrresponsibleThinkScan : TowerScanArea
{
    public override Transform PickTarget(int count)
    {
        Transform best = null;

        for (int i = 0; i < count; i++)
        {
            if (Hits[i].TryGetComponent<Entity>(out var entityData))
            {
                if (entityData.EntityRunTimeData.entityState == EntityState.IrresponsibleThinking)
                {
                    best = Hits[i].transform;
                }
            }
        }
        
        if (best != null)
            Debug.Log($"[{name}] PickTarget there are target that doing Irresponsible Thinking! {best.name}");
        
        return best;
    }
}
