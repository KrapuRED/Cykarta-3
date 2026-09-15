using UnityEngine;

[CreateAssetMenu(fileName = "TowerDataSO", menuName = "Tower Data/TowerDataSO")]
public class TowerDataSO : ScriptableObject
{
    public string towerName;
    public int towerCost;
    
    [Header("Tower Base Status")]
    public int baseTowerRanger;
}
