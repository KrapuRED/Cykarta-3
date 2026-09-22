using UnityEngine;

[System.Serializable]
public class TowerRunTimeData
{
   public string towerName;
   public int  towerLevel;
   public int  towerRange;
   public int  towerCapacity;
   public float  towerProcessingSpeed;
   public float  towerAttackSpeed;
   public bool isTowerUnLockToTarget;
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
   [SerializeField] private TowerDataSO towerData;
   [SerializeField] private TowerRunTimeData towerRunTimeData;
   [SerializeField] private Transform towerBody;
   [SerializeField] private Transform visualRange;
   [SerializeField] protected LayerMask enemyLayerMask;

   protected TowerScanArea TowerScanArea { get; private set; }

   public string TowerID { get; private set; }
   public TowerRunTimeData TowerRunTimeData => towerRunTimeData;
   public bool IsLocked { get;  set; }
   public LayerMask EnemyLayerMask => enemyLayerMask;
   protected Transform CurrentTarget;
   public Transform Target => CurrentTarget;
   public TowerDataSO TowerData => towerData;
   
   
   protected bool IsBeenPlace { get; private set; }
   private Vector3 GridPosition { get;  set; }

   private float _currentRotation;

   private void Awake()
   {
      towerRunTimeData = new TowerRunTimeData
      {
         towerName = towerData.towerName,
         towerLevel = 1,
         towerRange =  towerData.baseTowerRanger,
         towerCapacity = towerData.baseTowerMaxCapacity,
         towerProcessingSpeed = towerData.baseTowerProcessingSpeed,
         towerAttackSpeed = towerData.baseTowerAttackSpeed,
         isTowerUnLockToTarget = true
      };
      
      TowerScanArea = entitySystemContainer != null
         ? entitySystemContainer.GetComponentInChildren<TowerScanArea>()
         : GetComponentInChildren<TowerScanArea>();
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

   private void HandleShowTowerRangeDetection(string towerID)
   {
      if (string.IsNullOrEmpty(towerID))
      {
         visualRange.gameObject.SetActive(false); }
      
      if (!string.IsNullOrEmpty(towerID) && towerID == TowerID)
      {
         visualRange.gameObject.SetActive(true);
      }
   }
   
   private void HandleHideTowerRangeDetection()
   {
      if (IsBeenPlace) return;
         InitializeTower();
      
      visualRange.gameObject.SetActive(false);
   }

   private void Start()
   {
      TowerID = TowerManager.Instance.GetTowerID(towerData.towerName);
      gameObject.name = TowerID;

      SetVisualDetectRange();
      TowerManager.Instance.RegisterTower(this);
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

   private void SetVisualDetectRange()
   {
      if (visualRange == null)
      {
         Debug.LogError($"[{name} SetVisualDetectRange] cannot visual range for this tower!");
         return;
      }
      
      float currentRange = towerRunTimeData != null 
         ? towerRunTimeData.towerRange 
         : towerData.baseTowerRanger;
      
      // Set Diameter of the range
      float diameter = currentRange * 2f;

      // Compensate for parent scale
      Vector3 parentScale = visualRange.parent != null
         ? visualRange.parent.lossyScale
         : Vector3.one;
      
      //Show visual by currentRange 
      visualRange.localScale = new Vector3(diameter/ parentScale.x, diameter/parentScale.y, 1f);
   }

   #region ======= BUILD TOWER ======
   
   protected virtual void InitializeTower()
   {
      IsBeenPlace = true;
      
   }
   
   public void RotateTower(TowerRotation towerRotation)
   {
      if (towerRotation == TowerRotation.Right)
      {
         _currentRotation -= 90f;
      }
      else
      {
         _currentRotation += 90f;
      }
      
      _currentRotation %= 360f; 

      towerBody.rotation = Quaternion.Euler(0, 0, _currentRotation);
   }

   public void UpgradeTower(TowerDataSO upgradeTowerData)
   {
      
   }

   public void RemoveTower(GridZone resetZone = GridZone.Build)
   {
      GridManager.Instance.ChangeGridMode(GridMode.None);
      GridManager.Instance.BuildingGridMap.ClearTower(GridPosition, resetZone);
      DestroyEntity();
   }
   #endregion

   private void OnDrawGizmos()
   {
      Gizmos.color = Color.red;
      float currentRange = towerRunTimeData != null 
         ? towerRunTimeData.towerRange 
         : towerData.baseTowerRanger;

      Gizmos.DrawWireSphere(transform.position, currentRange);
   }
}
