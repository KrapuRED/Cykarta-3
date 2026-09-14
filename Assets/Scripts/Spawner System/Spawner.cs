using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ObjectSpawnType
{
    None, 
    Vehicle,
    Pedestrian,
    Trash
}

[System.Serializable]
public class SpawnerData
{
    public string spawnerDataName;
    public ObjectSpawnType objectSpawnType;
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
    
    private void Start()
    {
        currentSpawnRate = Random.Range(minSpawnRate, maxSpawnRate);
        prevSpawnRate = currentSpawnRate;
        _isSpawnerActive = true;
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
            _isSpawnerActive = false;
            return;
        }
        
        spawnerData.currentSpawnCount++;
        
        Debug.Log($"[{name} - (OnSpawning)] Spawning {spawnerData.spawnerDataName} at {prevSpawnRate:F1}s");
        currentSpawnRate = Random.Range(minSpawnRate, maxSpawnRate);
        prevSpawnRate = currentSpawnRate;
    }
}
