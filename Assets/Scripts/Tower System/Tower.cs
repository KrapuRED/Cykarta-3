using UnityEngine;

[System.Serializable]
public class TowerRunTimeData
{
   public string towerName;
   public int towerLevel;
   public float towerRange;
   public float towerAttackSpeed;
   public float towerCapacity;
}

[System.Serializable]
public enum TowerRotation
{
   Right,
   Left
}

public class Tower : MonoBehaviour
{
   [SerializeField] private TowerDataSO towerData;
   [SerializeField] private TowerRunTimeData towerRunTimeData;
   [SerializeField] private Transform towerBody;
   
   public TowerDataSO TowerData => towerData;
   public string towerID { get; private set; }
   public Vector3 GridPosition { get; private set; }
   
   public TowerRunTimeData TowerRunTimeData => towerRunTimeData;
   
   private float _currentRotation;
   
   private void Start()
   {
      towerID = TowerManager.Instance.GetTowerID(towerData.towerName);
      gameObject.name = towerID;
      
      towerRunTimeData = new TowerRunTimeData
      {
         towerName = towerData.towerName,
         towerLevel = 1
      };
      
      TowerManager.Instance.RegisterTower(this);
   }

   public void SetGridPosition(Vector3 gridPosition)
   {
      GridPosition = gridPosition;
   }
   
   public void UpdateTowerStateMachine()
   {
      
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

   public void RemoveTower(GridZone resetZone = GridZone.Build)
   {
      GridManager.Instance.ChangeGridMode(GridMode.None);
      GridManager.Instance.BuildingGridMap.ClearTower(GridPosition, resetZone);
      Destroy(gameObject);
   }
}
