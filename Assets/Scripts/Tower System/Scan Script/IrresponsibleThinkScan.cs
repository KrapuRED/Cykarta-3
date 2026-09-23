using UnityEngine;

public class IrresponsibleThinkScan : TowerScanArea
{
    protected override Transform PickTarget(int count)
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

    protected override void ScanArea()
    {
        float radius = towerOwner.TowerRunTimeData.towerRange; 
        int count = Physics2D.OverlapCircle(transform.position, radius, Filter, Hits);

        CurrentTarget = count > 0 ? PickTarget(count) : null;
    }
}
