using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveSpawnerData
{
    public string spawnerName;
    public SpawnerIrresponsible spawnerIrresponsible;
    public float maxSpawnRate;
    public float minSpawnRate;
    public List<SpawnerData> waveSpawnerData = new();
}

[System.Serializable]
public class WaveData
{
    public string waveDataName;
    public int waveDelay;
    public List<WaveSpawnerData> waveSpawnerData = new();
}

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager Instance  { get; private set; }
    [SerializeField] private List<WaveData> waveDataList = new();

    public bool startSpawn = false;
    
    private WaveData _currentWaveData;
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

    private void Start()
    {
        if (!startSpawn) return;
        
        _currentWaveData = waveDataList[_waveIndex];
        List<SpawnerIrresponsible> spawners = new(); 
        
        foreach (var spawnerSystem in _currentWaveData.waveSpawnerData)
        {
            if (spawnerSystem.spawnerIrresponsible == null) continue;

            spawnerSystem.spawnerIrresponsible.AddOrChangeSpawnerData(spawnerSystem);
            spawners.Add(spawnerSystem.spawnerIrresponsible);
        }
        
        StartCoroutine(DelaySpanwenActivation(spawners));
    }

    private IEnumerator DelaySpanwenActivation(List<SpawnerIrresponsible>  spawners)
    {
        yield return new WaitForSeconds(_currentWaveData.waveDelay);
        foreach (var spawner in spawners)
            spawner.StartSpawner();
    }
}
