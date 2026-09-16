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

public class Tower : MonoBehaviour
{
   [SerializeField] private TowerDataSO towerData;
   [SerializeField] private TowerRunTimeData towerRunTimeData;
   
   public TowerDataSO TowerData => towerData;
   public string towerID { get; private set; }
   public TowerRunTimeData TowerRunTimeData => towerRunTimeData;
   
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

   public void UpdateTowerStateMachine()
   {
      
   }
}
