using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TrashType
{
   Light,
   Medium,
   Heavy
}
[System.Serializable]
public enum TrashState
{
   Grounded,
   Airborne,
   Grabbed
}

[System.Serializable]
public class TrashSpawnData
{
   public string trashName;
   public TrashType trashType;
   public float throwDuration;
   public SpawnableDataSO spawnData;
}

[System.Serializable]
public class TrashData
{
   public string trashName;
   public TrashState trashState;
   public TrashType trashType;
   public int trashWeight;
}

[System.Serializable]
public class TrashChangeData
{
   public string trashName;
   public TrashType trashType;
   public float trashChance;
}

public class SpawnerTrash : Spawner
{
   public static SpawnerTrash Instance { get; private set; }

   [SerializeField] private Transform endWaterPath;
   [SerializeField] private float arcHeight = 1.5f;
   [SerializeField] private List<TrashSpawnData> trashDatas = new();
   
   private void Awake()
   {
      if (Instance != null)
      {
         Destroy(gameObject);
         return;
      }

      Instance = this;
   }

   public void SpawnTrash(Vector3 spawnPosition, TrashType trashType)
   {
      var gridMap = GridManager.Instance.BuildingGridMap;
      
      if (!GridPathfinder.TryFindNearPath(gridMap, spawnPosition, spawnZone, out Vector3 landingPosition))
      {
         Debug.LogWarning($"[{name}] No {spawnZone} cell found for trash.");
         return;
      }
      
      // Spawn Trash
      var trashData = trashDatas.Find(x => x.trashType == trashType);
      
      var newTrash = Instantiate(trashData.spawnData.entityPrefab, spawnPosition, Quaternion.identity, gridMap.transform);
      if (!newTrash.TryGetComponent<Entity>(out var entityComponent))
      {
         Destroy(newTrash.gameObject);
         return;
      }
     
      var entityData = EntityManager.Instance.GetEntityRunTimeData(trashData.spawnData.displayName, spawnerID, null,entityComponent);
      
      if (newTrash.TryGetComponent<Trash>(out var trash))
         trash.ThrowTrash(entityData, endWaterPath, arcHeight , trashData.throwDuration, spawnPosition, landingPosition);
   }
}
