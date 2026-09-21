using UnityEngine;

public class TowerScanArea : MonoBehaviour
{
    [SerializeField] private Tower towerOwner;
    [SerializeField] private float detectionInterval;
    [SerializeField] private int maxDetected;

    private float _detectionTimer;
    private Collider2D[] _hits;
    private Transform _currentTarget;
    private ContactFilter2D _filter;
    
    private void Awake()
    {
        _hits = new Collider2D[maxDetected];
        _detectionTimer = Random.Range(0, detectionInterval);
        
        _filter = new ContactFilter2D();
        _filter.SetLayerMask(towerOwner.EnemyLayerMask);
        _filter.useTriggers = true;
    }

    public void OnDetecting(float deltaTime, out Transform targetToLock)
    {
        _detectionTimer += deltaTime;

        if (_detectionTimer >= detectionInterval)
        {
            _detectionTimer -= detectionInterval;
            ScanArea();
        }
        
        targetToLock = _currentTarget;
    }

    private Transform PickNearestTarget(int count)
    {
        Transform best = null;
        float bestSqr = float.MaxValue;
        Vector2 origin = transform.position;

        for (int i = 0; i < count; i++)
        {
            float sqr = ((Vector2)_hits[i].transform.position - origin).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = _hits[i].transform;
            }
        }
        
        Debug.Log($"[{name} (PickTarget)] The Nearest Target from {towerOwner.TowerRunTimeData.towerName} is {best.name}");
        return best;
    }

    private void ScanArea()
    {
        float radius = towerOwner.TowerRunTimeData.towerRange; // multiply by cell size if range is in grid cells
        int count = Physics2D.OverlapCircle(transform.position, radius, _filter, _hits);

        _currentTarget = count > 0 ? PickNearestTarget(count) : null;
    }
}
