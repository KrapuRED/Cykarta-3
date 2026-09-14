using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnerData
{
    public string spawnerDataName;
    public int maxSpawnCount;
    public int currentSpawnCount;
    // Character / Vehicle Data
} 

public class Spawner : MonoBehaviour
{
    [Header("Spawner Configuration")]
    [SerializeField] private float maxSpawnRate;
    [SerializeField] private float minSpawnRate;
    [SerializeField] private List<SpawnerData> spawnerDatas = new();
    // Where the Character / Vehicle should Spawn
    
    [SerializeField] private float currentSpawnRate;
    [SerializeField] private float prevSpawnRate;
    private bool _isSpawnerActive;

    private void FixedUpdate()
    {
        if (!_isSpawnerActive) return;
        
        currentSpawnRate -= Time.deltaTime;
        if (currentSpawnRate <= 0)
        {
            OnSpawning();
        }
    }

    private bool IsSpawnerDataReachMax(string spawnerDataName)
    {
        var spawnerData = spawnerDatas.Find(x => x.spawnerDataName == spawnerDataName);
        return spawnerData.currentSpawnCount >= spawnerData.maxSpawnCount;
    }

    private void OnSpawning()
    {
        int index = Random.Range(0, spawnerDatas.Count);
        var spawnerData = spawnerDatas[index];

        if (IsSpawnerDataReachMax(spawnerData.spawnerDataName))
        {
            spawnerDatas.Remove(spawnerData);
            return;
        }
        
        spawnerData.currentSpawnCount++;
        
        Debug.Log($"[{name} - OnSpawning] Spawning {spawnerData.spawnerDataName} {spawnerData.currentSpawnCount} / {spawnerData.maxSpawnCount}");

        currentSpawnRate = Random.Range(minSpawnRate, maxSpawnRate);
        prevSpawnRate = currentSpawnRate;
    }

    public void StartSpawner()
    {
        if (_isSpawnerActive) return;
        
        currentSpawnRate = Random.Range(minSpawnRate, maxSpawnRate);
        prevSpawnRate = currentSpawnRate;
        _isSpawnerActive = true;
    }
    
    public void AddOrChangeSpawnerData(WaveSpawnerData newSpawnerData)
    {
        if (newSpawnerData ==  null)
        {
            Debug.LogError($"[{name} - AddOrChangeSpawnerData] The newSpawnerData is NULL!");
            return;
        }
        
        _isSpawnerActive = false;
        maxSpawnRate = newSpawnerData.maxSpawnRate;
        minSpawnRate = newSpawnerData.minSpawnRate;
        
        spawnerDatas.Clear();
        
        foreach (var spawnerData in newSpawnerData.waveSpawnerData)
        {
            if (spawnerData == null)
            {
                continue;
            }
            
            var data = spawnerData;
            spawnerDatas.Add(data);
            
            Debug.Log($"[{name} (AddOrChangeSpawnerData)] new {data.spawnerDataName} {data.currentSpawnCount} / {data.maxSpawnCount}");
        }
    }
}
