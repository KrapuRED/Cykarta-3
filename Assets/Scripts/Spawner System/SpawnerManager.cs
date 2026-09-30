using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public bool startSpawn = false;
    
    private List<SpawnerIrresponsible> _activeSpawners = new ();
    private int _waveIndex;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CheckAllIrresponsibleSpawnerIsDone()
    {
        bool isDoneAll = _activeSpawners.TrueForAll(spawner => !spawner.IsSpawnerActive);
        
        if (isDoneAll)
        {
            ObjectiveManager.Instance.NextWave();
            _activeSpawners.Clear();
        }
    }

    public void StartSpawn(WaveData currentWaveData)
    {
        
        foreach (var spawnerSystem in currentWaveData.waveSpawnerData)
        {
            if (spawnerSystem.spawnerIrresponsible == null) continue;

            spawnerSystem.spawnerIrresponsible.AddOrChangeSpawnerData(spawnerSystem);
            _activeSpawners.Add(spawnerSystem.spawnerIrresponsible);
            
            StartCoroutine(DelaySpanwenActivation(_activeSpawners, currentWaveData.waveDelay));
        }
    }

    private IEnumerator DelaySpanwenActivation(List<SpawnerIrresponsible> spawners, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var spawner in spawners)
            spawner.StartSpawner();
    }
}
