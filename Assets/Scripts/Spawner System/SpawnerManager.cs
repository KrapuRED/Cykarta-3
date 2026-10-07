using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

#region Spawner Data & System

    [System.Serializable]
    public class SpawnerData
    {
        public string spawnerDataName;
        public int maxSpawnCount;
        public int currentSpawnCount;
        public bool isReachMaxSpawnCount;
        public SpawnableDataSO spawnData;
    }

    [System.Serializable]
    public class WaveSpawnerData
    {
        public string spawnerName;
        public SpawnerIrresponsible spawnerIrresponsible;
        public float maxSpawnRate;
        public float minSpawnRate;
        public List<SpawnerData> spawnerData = new();
    }

    [System.Serializable]
    public class WaveData
    {
        public string waveDataName;
        public int waveDelay;
        public List<WaveSpawnerData> waveSpawnerData = new();
    }

#endregion

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager Instance  { get; private set; }

    [SerializeField] private DelayTimerObjectiveUI delayTimerObjectiveUI;
    
    public bool startSpawn = false;
    
    [SerializeField] private List<SpawnerIrresponsible> _activeSpawners = new ();
    private bool _isPaused;
    private bool _isDelayRunning;
    private bool _isSpawnerActive;
    private Coroutine _delaySpanwerActivationCoroutine ;

    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (_isPaused) return;
        
        foreach (var spawner in _activeSpawners)
        {
            if (spawner == null) continue;
            
            spawner.UpdateSpawner();
        }
    }

    public void CheckAllIrresponsibleSpawnerIsDone()
    {
        bool isDoneAll = _activeSpawners.TrueForAll(spawner => !spawner.IsSpawnerActive);
        
        if (isDoneAll)
        {
            _activeSpawners.Clear();
            ObjectiveManager.Instance.NextWave();
        }
    }

    public void StartSpawn(WaveData currentWaveData, int waveIndex)
    {
        foreach (var spawnerSystem in currentWaveData.waveSpawnerData)
        {
            if (spawnerSystem.spawnerIrresponsible == null)
            {
                Debug.LogWarning($"Spawner System in {spawnerSystem.spawnerName} doesn't exist!");
                continue;
            }

            spawnerSystem.spawnerIrresponsible.AddOrChangeSpawnerData(spawnerSystem);
            _activeSpawners.Add(spawnerSystem.spawnerIrresponsible);
            
            _delaySpanwerActivationCoroutine = StartCoroutine(DelaySpanwerActivation(_activeSpawners, currentWaveData.waveDelay, waveIndex));
        }
    }

    private IEnumerator DelaySpanwerActivation(List<SpawnerIrresponsible> spawners, float delay, int waveIndex)
    {
        Debug.Log($"Delay spawner activation: {waveIndex + 1}");
        
        delayTimerObjectiveUI.ShowDelayTimerObjectiveUI();
        float remainingTime = delay;
        
        while (remainingTime > 0)
        {
            if (!_isPaused)
            {
                remainingTime -= Time.deltaTime;
                delayTimerObjectiveUI.UpdateTimerText(waveIndex + 1, remainingTime);   
            }
            
            yield return null;
        }
        
        delayTimerObjectiveUI.HideDelayTimerObjectiveUI();
        
        foreach (var spawner in spawners)
            spawner.StartSpawner();

        _isDelayRunning = false;
        _isSpawnerActive = true;
        _delaySpanwerActivationCoroutine = null;
    }

    public void Pause()
    {
        _isPaused = true;
    }

    public void Resume()
    {
        _isPaused = false;
        
        if (!_isDelayRunning && _delaySpanwerActivationCoroutine == null)
            _isSpawnerActive = true;
    }
}
