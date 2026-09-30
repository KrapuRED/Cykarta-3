using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set;}

    [SerializeField] private int objectiveHealthPoints;
    [SerializeField] private List<WaveData> waveDataList = new();
    
    [Header("References")]
    [SerializeField] private ObjectiveUI enemyCounter;
    [SerializeField] private ObjectiveUI proctectionLevel;
    
    public int CurrentObjectiveHealthPoints { get; private set; }
    private int _waveIndex = -1;
    private int _maxEnemyCounter;
    private int _currentEnemyCounter;
    
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
        CurrentObjectiveHealthPoints = objectiveHealthPoints;
        
        proctectionLevel.UpdateObjectiveUI(new ProtectionObjectiveDataUI
        {
            currentProtectionLevel = objectiveHealthPoints
        });
        
        NextWave();
    }

    public void NextWave()
    {
        _waveIndex++;
        var currentWaveData = waveDataList[_waveIndex];
        var spawnData = currentWaveData.waveSpawnerData;
        List<SpawnerData> data =  new List<SpawnerData>();

        foreach (var ws in spawnData)
        {
            data.AddRange(ws.spawnerData);
        }

        _maxEnemyCounter = CountTotalEnemy(data);
        
        enemyCounter.UpdateObjectiveUI(new EnemyCounterObjectiveDataUI
        {
            currentEnemyCounter = 0,
            maxEnemyCounter = _maxEnemyCounter
        });
        
        
        SpawnerManager.Instance.StartSpawn(currentWaveData);
        Debug.Log($"Total Wave Count: {waveDataList.Count}");
        Debug.Log($"Wave Index: {_waveIndex} {currentWaveData.waveDataName} total Enemy : {_maxEnemyCounter}");
    }

    private int CountTotalEnemy(List<SpawnerData> spawnerData)
    {
        if (spawnerData.Count <= 0)
        {
            Debug.LogWarning("No spawner data found");
            return -1;
        }
        
        int totalEnemies = 0;
        foreach (var data in spawnerData)
        {
            if (data == null) continue;
            totalEnemies += data.maxSpawnCount;
        }
        
        return totalEnemies;
    }

    public void UpdateObjectiveEnemyCounter()
    {
        Debug.LogWarning($"{name} Updating Enemy Counter!");
        
        _currentEnemyCounter++;
        
        enemyCounter.UpdateObjectiveUI(new EnemyCounterObjectiveDataUI
        {
            currentEnemyCounter = _currentEnemyCounter,
            maxEnemyCounter = _maxEnemyCounter
        });
        
        if (_currentEnemyCounter >= _maxEnemyCounter)
            SpawnerManager.Instance.CheckAllIrresponsibleSpawnerIsDone();
    }
}
