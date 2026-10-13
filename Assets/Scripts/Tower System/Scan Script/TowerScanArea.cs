using UnityEngine;
using MoreMountains.Tools;

public class TowerScanArea : MonoBehaviour
{
    [SerializeField] protected Tower towerOwner;
    [SerializeField] private float detectionInterval;
    [SerializeField] private int maxDetected;
    [SerializeField] private bool detectionWithoutInterval;
    
    [Header("Sound Effect and ambient")]
    [SerializeField] protected AudioClip scanSound;
    [SerializeField] protected float scanSoundInterval = 1f; // seconds between sounds
    [SerializeField] protected float volumeScanSound;
    [SerializeField] private bool onlyPlayWhenTargetFound;

    private bool _scanSound;
    [SerializeField] private float _scanSoundTimer;
    
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
        if (detectionWithoutInterval)
            ScanArea();
        else
        {
            _detectionTimer += deltaTime;

            if (_detectionTimer >= detectionInterval)
            {
                _detectionTimer -= detectionInterval;
                ScanArea();
            }   
        }
        HandleScanSound(deltaTime);
        
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
    
    private void HandleScanSound(float deltaTime)
    {
        if (scanSound == null || scanSoundInterval <= 0f) return;
        if (onlyPlayWhenTargetFound && CurrentTarget == null) return;
        
        _scanSoundTimer += deltaTime;
        if (_scanSoundTimer < scanSoundInterval)
        {
            
            return;
        }

        _scanSoundTimer -= scanSoundInterval;

        MMSoundManagerSoundPlayEvent.Trigger(
            scanSound,
            MMSoundManager.MMSoundManagerTracks.Sfx,
            transform.position,
            volume: volumeScanSound);
    }
}
