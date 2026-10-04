using System;
using UnityEngine;

public class CircelScanArea : TowerScanArea
{
    public System.Func<Transform, bool> TargetFilter;
    
    protected override Transform PickTarget(int count)
    {
        Transform best = null;
        float bestSqr = float.MaxValue;
        Vector2 origin = transform.position;

        for (int i = 0; i < count; i++)
        {
            Transform t = Hits[i].transform;
            if (TargetFilter != null && !TargetFilter(t)) continue;

            float sqr = ((Vector2)t.position - origin).sqrMagnitude;
            if (sqr < bestSqr) { bestSqr = sqr; best = t; }
        }
        return best;
    }

    protected override void ScanArea()
    {
        float radius = (int)towerOwner.TowerRunTimeData.GetCurrentStatusValue(TowerStatus.Range); 
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
            ? (int)towerOwner.TowerRunTimeData.GetCurrentStatusValue(TowerStatus.Range)
            : (int)towerOwner.TowerData.GetBaseStatusValue(TowerStatus.Range);
    }
}
