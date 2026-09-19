using System.Collections.Generic;
using UnityEngine;

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
public class WayPointData
{
    public string wayPointDataName;
    public Transform startPoint;
    public Transform endPoint;
}

public class SpawnerIrresponsible : Spawner
{
    [Header("Spawner Configuration")]
    [SerializeField] private float maxSpawnRate;
    [SerializeField] private float minSpawnRate;
    [SerializeField] private List<SpawnerData> spawnerDatas = new();
    [SerializeField] private List<WayPointData> wayPointDatas = new();
    // Where the Character / Vehicle should Spawn
    
    [SerializeField] private float currentSpawnRate;
    [SerializeField] private float prevSpawnRate;
    private bool _isSpawnerActive;

    private void Update()
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

        if (spawnerData.currentSpawnCount >= spawnerData.maxSpawnCount)
            spawnerData.isReachMaxSpawnCount = true;
        
        return spawnerData.isReachMaxSpawnCount;
    }

    private bool AllReachedMax() => spawnerDatas.TrueForAll(d => d.currentSpawnCount >= d.maxSpawnCount);
    
    private WayPointData GetWayPointData()
    {
        int index = Random.Range(0, wayPointDatas.Count);
        return wayPointDatas[index];
    }

    public override void OnSpawning()
    {
        if (spawnerDatas.Count == 0 || AllReachedMax()) { _isSpawnerActive = false; return; }
        
        int index = Random.Range(0, spawnerDatas.Count);
        if (index >= spawnerDatas.Count)
            return;
        
        var spawnerData = spawnerDatas[index];
        if (IsSpawnerDataReachMax(spawnerData.spawnerDataName))
        {
            return;
        }
        
        if (spawnerData.spawnData != null && !spawnerData.isReachMaxSpawnCount)
        {
            spawnerData.currentSpawnCount++;
            var spawnData = spawnerData.spawnData;
            
            var waypointData = GetWayPointData();
            var entity = Instantiate(spawnData.entityPrefab, waypointData.startPoint.position, Quaternion.identity);
            if (!entity.TryGetComponent<Entity>(out var entityComponent))
            {
                Destroy(entity.gameObject);
                return;
            }
            
            var entityData = EntityManager.Instance.GetEntityRunTimeData(spawnerData.spawnData.displayName, spawnerID, entityComponent);
            entity.name = $"{entityData.entityID}";
            
            var entitiySpeed = Random.Range(spawnData.minEntitySpeed, spawnData.maxEntitySpeed);
            
            switch (spawnZone)
            {
                case GridZone.Pedestrian:
                    if (entity.TryGetComponent<Pedestrian>(out var pedestrian))
                    {
                        pedestrian.InitializePedestrian(waypointData.endPoint, entitiySpeed, entityData);
                    }
                    break;
                
                case GridZone.Vehicle:
                    if (entity.TryGetComponent<Vehicle>(out var vehicle))
                    {
                        vehicle.InitializeVehicle(waypointData.endPoint, entitiySpeed, entityData);
                    }
                    break;
            }
            Debug.Log($"[{name} - OnSpawning] Spawning {entityData.entityID} {spawnerData.currentSpawnCount} / {spawnerData.maxSpawnCount}");
        }

        currentSpawnRate = Random.Range(minSpawnRate, maxSpawnRate);
        prevSpawnRate = currentSpawnRate;
    }

    public override void StartSpawner()
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
            spawnerDatas.Add(new SpawnerData
            {
                spawnerDataName = spawnerData.spawnerDataName,
                maxSpawnCount = spawnerData.maxSpawnCount,
                currentSpawnCount = 0,
                isReachMaxSpawnCount = false,
                spawnData = spawnerData.spawnData
            });
            
            Debug.Log($"[{name} (AddOrChangeSpawnerData)] new {data.spawnerDataName} {data.currentSpawnCount} / {data.maxSpawnCount}");
        }
    }
}
