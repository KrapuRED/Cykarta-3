using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerRunTimeData
{
   public string towerName;
   public int  towerLevel;
   public List<TowerStatusData> currentTowerStatusData = new ();
   public bool isTowerUnLockToTarget;
   
   public float GetCurrentStatusValue(TowerStatus status, float fallback = 0f)
   {
      foreach (var s in currentTowerStatusData)
      {
         if (s.status == status)
            return s.valueStatus;
      }
      return fallback;
   }
}

[System.Serializable]
public enum TowerRotation
{
   Right,
   Left
}

[System.Serializable]
public enum TowerState
{
   Scan,
   LockToTarget,
   ProcessingTrash
}

public class Tower : Entity
{
   [SerializeField] protected TowerDataSO towerData;
   [SerializeField] protected TowerRunTimeData towerRunTimeData;
   [SerializeField] private Transform towerBody;
   [SerializeField] protected Transform visualRange;
   [SerializeField] protected LayerMask enemyLayerMask;

   protected TowerScanArea TowerScanArea { get; private set; }

   public string TowerID { get; private set; }
   public TowerRunTimeData TowerRunTimeData => towerRunTimeData;
   public bool IsLocked { get;  set; }
   public LayerMask EnemyLayerMask => enemyLayerMask;
   protected Transform CurrentTarget;
   public Transform Target => CurrentTarget;
   public TowerDataSO TowerData => towerData;
   public float CurrentRotation { get; private set; }
   
   
   protected bool IsBeenPlace { get; private set; }
   private Vector3 GridPosition { get;  set; }


   private void Awake()
   {
      InitializerTowerData();
      
      TowerScanArea = entitySystemContainer != null
         ? entitySystemContainer.GetComponentInChildren<TowerScanArea>()
         : GetComponentInChildren<TowerScanArea>();
      
      TowerID = TowerManager.Instance.GetTowerID(towerData.towerName);
      gameObject.name = TowerID;

      if (visualRange != null)
         SetVisualDetectRange();
      
      TowerManager.Instance.RegisterTower(this);
   }

   private void OnEnable()
   {
      GameEvents.OnShowTowerDetectRange.AddListener(HandleShowTowerRangeDetection);
      GameEvents.OnHideTowerDetectRange.AddListener(HandleHideTowerRangeDetection);
   }

   private void OnDisable()
   {
      GameEvents.OnShowTowerDetectRange.RemoveListener(HandleShowTowerRangeDetection);
      GameEvents.OnHideTowerDetectRange.RemoveListener(HandleHideTowerRangeDetection);
   }

   private void InitializerTowerData()
   {
      var clonedStatuses = new List<TowerStatusData>();
      foreach (var s in towerData.baseStatuses)
      {
         clonedStatuses.Add(s.Clone());
      }

      towerRunTimeData = new TowerRunTimeData 
      {
         towerName = towerData.towerName,
         towerLevel = 1,
         currentTowerStatusData = clonedStatuses,
         isTowerUnLockToTarget = true
      };
   }

   private void HandleShowTowerRangeDetection(string towerID)
   {
      if (visualRange == null)
         return;
      
      if (string.IsNullOrEmpty(towerID))
      {
         visualRange.gameObject.SetActive(false); 
      }
      
      if (!string.IsNullOrEmpty(towerID) && towerID == TowerID)
      {
         SetVisualDetectRange();
         visualRange.gameObject.SetActive(true);
      }
   }
   
   private void HandleHideTowerRangeDetection()
   {
      Debug.Log($"[{nameof(Tower)}] {TowerID} is being hidden!");
      
      if (visualRange == null)
         return;

      visualRange.gameObject.SetActive(false);
   }

   public void SetGridPosition(Vector3 gridPosition)
   {
      GridPosition = gridPosition;
   }

   public virtual void OnDetectingArea(float deltaTime)
   {
      
   }

   public virtual void LockToTarget(Transform target)
   {
      
   }

   public virtual void OnUpdateTower(float deltaTime)
   {
      
   }

   protected virtual void SetVisualDetectRange()
   {
      
   }

   protected virtual void ReleaseObject()
   {
      
   }

   #region ======= BUILD TOWER ======
   
   public void InitializeTower()
   {
      IsBeenPlace = true;
      
      var entityData = EntityManager.Instance.GetEntityRunTimeData(TowerData.towerName, string.Empty, null,this);
      InitializeEntity(entityData);
   }
   
   public void RotateTower(TowerRotation towerRotation)
   {
      if (towerRotation == TowerRotation.Right)
      {
         CurrentRotation -= 90f;
      }
      else
      {
         CurrentRotation += 90f;
      }
      
      CurrentRotation %= 360f; 

      towerBody.rotation = Quaternion.Euler(0, 0, CurrentRotation);
   }

   public void UpgradeTower(UpgradeTowerData upgradeTowerData)
   {
      TowerRunTimeData.towerLevel++;
      foreach (var upgrade in upgradeTowerData.upgradeStatuses)
      {
         if (upgrade == null) continue;

         foreach (var runtimeData in TowerRunTimeData.currentTowerStatusData)
         {
            if (runtimeData.status == upgrade.status)
            {
               runtimeData.valueStatus += upgrade.valueStatus;
            }
         }
      }
   }
   
   public UpgradeTowerData GetNextUpgrade()
   {
      if (towerData == null)
         return null;

      int nextUpgradeIndex = towerRunTimeData.towerLevel - 1;
      if (nextUpgradeIndex < 0 || nextUpgradeIndex >= towerData.upgrades.Count)
         return null;

      return towerData.upgrades[nextUpgradeIndex];
   }

   public bool HasNextUpgrade => GetNextUpgrade() != null;

   public void RemoveTower(GridZone resetZone = GridZone.Build)
   {
      GridManager.Instance.ChangeGridMode(GridMode.None);
      GridManager.Instance.BuildingGridMap.ClearTower(GridPosition, resetZone);
      ReleaseObject();
         
      DestroyEntity();
   }
   #endregion
}
