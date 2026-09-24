using System;
using UnityEngine;

public class CircelScanArea : TowerScanArea
{
    protected override Transform PickTarget(int count)
    {
        Transform best = null;

        for (int i = 0; i < count; i++)
        {
            best = Hits[i].transform;
        }
        
        return best;
    }

    protected override void ScanArea()
    {
        float radius = towerOwner.TowerRunTimeData.towerRange; 
        int count = Physics2D.OverlapCircle(transform.position, radius, Filter, Hits);

        CurrentTarget = count > 0 ? PickTarget(count) : null;
    }

    private void OnDrawGizmos()
    {
        float radius = GetRange();
        if (radius <= 0f) return;

        Gizmos.color = new Color(1f, 0.3f, 0.3f, 1f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private float GetRange()
    {

        if (towerOwner == null) return 0f;

        return towerOwner.TowerRunTimeData != null
            ? towerOwner.TowerRunTimeData.towerRange
            : towerOwner.TowerData.baseTowerRanger;
    }
}
