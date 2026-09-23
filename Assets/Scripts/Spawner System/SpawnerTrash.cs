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
   Airborne
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
     
      var entityData = EntityManager.Instance.GetEntityRunTimeData(trashData.spawnData.displayName, spawnerID, entityComponent);
      
      if (newTrash.TryGetComponent<Trash>(out var trash))
         StartCoroutine(ThrowRoutine(trash, entityData, trashData.throwDuration, spawnPosition, landingPosition));
   }

   private IEnumerator ThrowRoutine(Trash trash, EntityRunTimeData entityRunTimeData ,float duration, Vector3 from, Vector3 to)
   {
      trash.TrashData.trashState = TrashState.Airborne;
      
      float t = 0;
      while (t < 1f)
      {
         if (trash == null) yield break;
         
         t+= Time.deltaTime / duration;
         float clamped = Mathf.Clamp01(t);
         
         Vector3 pos = Vector3.Lerp(from, to, clamped);
         pos.y += arcHeight * 4f * clamped * (1f - clamped); // parabola, peaks at t = 0.5
         trash.transform.position = pos;
         
         yield return null;
      }
      
      trash.transform.position = to;
      trash.InitializeTrash(endWaterPath, 2f,entityRunTimeData );
   }
}
