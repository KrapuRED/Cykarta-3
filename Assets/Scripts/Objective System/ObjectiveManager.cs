using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set;}

    [SerializeField] private List<WaveData> waveDataList = new();
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
        _waveIndex = 0;
        var currentWaveData = waveDataList[_waveIndex];
        var spawnData = currentWaveData.waveSpawnerData;
        
        //Debug.Log($"Wave Index: {_waveIndex} total Enemy : {CountTotalEnemy()}");
    }

    private int CountTotalEnemy(List<SpawnerData> waveDatas)
    {
        int totalEnemies = 0;
        
        foreach (var spawnerData in waveDatas)
        {
            
        }
        
        return totalEnemies;
    }
}
