using System;
using UnityEngine;

public class Tower : MonoBehaviour
{
   [SerializeField] private TowerDataSO towerData;
   
   public TowerDataSO TowerData => towerData;
   public string towerID { get; private set; }

   private void Start()
   {
      towerID = TowerManager.Instance.GetTowerID(towerData.towerName);
      gameObject.name = towerID;
      
      TowerManager.Instance.RegisterTower(this);
   }

   public void UpdateTowerStateMachine()
   {
      
   }
}
