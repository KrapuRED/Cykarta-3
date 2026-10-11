using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private List<SpawnerData> spawnerDatas = new();
    [SerializeField] private List<WayPointData> wayPointDatas = new();
    [SerializeField] private Transform containerSpawner;
    // Where the Character / Vehicle should Spawn
    
    [SerializeField] private float currentSpawnRate;
    [SerializeField] private float prevSpawnRate;
    [SerializeField] public bool isSpawnerActive;

    
    private float _maxSpawnRate;
    private float _minSpawnRate;
    public bool IsSpawnerActive { get; private set; }

    public override void UpdateSpawner()
    {
        if (!IsSpawnerActive) return;
        
        currentSpawnRate -= Time.deltaTime;
       
        if (currentSpawnRate <= 0)
        {
            OnSpawning();
        }
        
        isSpawnerActive = IsSpawnerActive;
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
        if (spawnerDatas.Count == 0 || AllReachedMax()) 
        { 
            IsSpawnerActive = false; 
            return;
        }
        
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
            var entity = Instantiate(spawnData.entityPrefab, waypointData.startPoint.position, Quaternion.identity, parent:containerSpawner);
            if (!entity.TryGetComponent<Entity>(out var entityComponent))
            {
                Destroy(entity.gameObject);
                return;
            }
            
            var entityData = EntityManager.Instance.GetEntityRunTimeData(spawnerData.spawnData.displayName, spawnerID, spawnData.irresponsibleThinkingBaseData,entityComponent);
            entity.name = $"{entityData.entityID}";
            
            var entitiySpeed = Random.Range(spawnData.minEntitySpeed, spawnData.maxEntitySpeed);
            
            switch (spawnZone)
            {
                case GridZone.Pedestrian:
                    entity.TryGetComponent<Pedestrian>(out var pedestrian);
                    pedestrian.InitializePedestrian(waypointData.endPoint, entitiySpeed, entityData);
                    break;
                
                case GridZone.Vehicle:
                    entity.TryGetComponent<Vehicle>(out var vehicle);
                    vehicle.InitializeVehicle(waypointData.endPoint, entitiySpeed, entityData);
                    break;
            }
            //Debug.Log($"[{name} - OnSpawning] Spawning {entityData.entityID} {spawnerData.currentSpawnCount} / {spawnerData.maxSpawnCount}");
        }

        currentSpawnRate = Random.Range(_minSpawnRate, _maxSpawnRate);
        prevSpawnRate = currentSpawnRate;
        CheckCount();
    }

    public override void StartSpawner()
    {
        if (IsSpawnerActive) return;
        
        currentSpawnRate = Random.Range(_minSpawnRate, _maxSpawnRate);
        prevSpawnRate = currentSpawnRate;
        IsSpawnerActive = true;
    }
    
    public void AddOrChangeSpawnerData(WaveSpawnerData newSpawnerData)
    {
        if (newSpawnerData ==  null)
        {
            Debug.LogError($"[{name} - AddOrChangeSpawnerData] The newSpawnerData is NULL!");
            return;
        }
        
        IsSpawnerActive = false;
        _maxSpawnRate = newSpawnerData.maxSpawnRate;
        _minSpawnRate = newSpawnerData.minSpawnRate;
        spawnerDatas.Clear();
        
        foreach (var spawnerData in newSpawnerData.spawnerData)
        {
            if (spawnerData == null)
            {
                continue;
            }
            
            spawnerDatas.Add(new SpawnerData
            {
                spawnerDataName = spawnerData.spawnerDataName,
                maxSpawnCount = spawnerData.maxSpawnCount,
                currentSpawnCount = 0,
                isReachMaxSpawnCount = false,
                spawnData = spawnerData.spawnData
            });
        }
    }

    private void CheckCount()
    {
        int totalCount = 0;
        foreach (var spawnerData in spawnerDatas)
        {
            totalCount += spawnerData.currentSpawnCount;
        }
        
        Debug.Log($"[{name} - CheckCount] TotalCount: {totalCount}");
    }
}
