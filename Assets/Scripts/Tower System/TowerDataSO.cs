using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TowerStatus
{
    Range,
    MaxCapacity,
    ProcessingSpeed,
    AttackSpeed
}

[System.Serializable]
public class TowerStatusData
{
    public string towerStatus;
    public TowerStatus status;
    public float valueStatus;
    [Tooltip("Check this if a LOWER value is actually an improvement (e.g. a duration/cooldown stat). " +
             "Leave unchecked for stats where higher is better (e.g. Range, Capacity).")]
    public bool isLowerValueBetter;
    
    public TowerStatusData Clone()
    {
        return new TowerStatusData
        {
            towerStatus = towerStatus,
            status = status,
            valueStatus = valueStatus,
            isLowerValueBetter = isLowerValueBetter
        };
    }
}

[System.Serializable]
public class UpgradeTowerData
{
    public string upgradeName;
    public int upgradeRequirement;
    public int upgradeCost;
    public List<TowerStatusData> upgradeStatuses = new ();
    
}

[CreateAssetMenu(fileName = "TowerDataSO", menuName = "Tower Data/TowerDataSO")]
public class TowerDataSO : ScriptableObject
{
    public string towerName;
    public int towerCost;
    public string towerDescription;
    public Tower prefabObjectTower;
    
    [Header("Tower Base Status")]
    public List<TowerStatusData> baseStatuses = new ();
    
    public List<UpgradeTowerData> upgrades = new List<UpgradeTowerData>() ;
    
    public float GetBaseStatusValue(TowerStatus status, float fallback = 0f)
    {
        foreach (var s in baseStatuses)
        {
            if (s.status == status)
                return s.valueStatus;
        }
        return fallback;
    }
}


