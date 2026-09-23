using UnityEngine;

public class TowerScanArea : MonoBehaviour
{
    [SerializeField] protected Tower towerOwner;
    [SerializeField] private float detectionInterval;
    [SerializeField] private int maxDetected;

    protected Collider2D[] Hits;
    protected Transform CurrentTarget;
    protected ContactFilter2D Filter;
    
    private float _detectionTimer;
    
    private void Awake()
    {
        Hits = new Collider2D[maxDetected];
        _detectionTimer = Random.Range(0, detectionInterval);
        
        Filter = new ContactFilter2D();
        Filter.SetLayerMask(towerOwner.EnemyLayerMask);
        Filter.useTriggers = true;
    }

    public void OnDetecting(float deltaTime, out Transform targetToLock)
    {
        _detectionTimer += deltaTime;

        if (_detectionTimer >= detectionInterval)
        {
            _detectionTimer -= detectionInterval;
            ScanArea();
        }
        
        targetToLock = CurrentTarget;
    }

    protected virtual Transform PickTarget(int count)
    {
        Transform best = null;

        return best;
    }

    protected virtual void ScanArea()
    {
        
    }
}
