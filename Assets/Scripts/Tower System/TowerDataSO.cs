using UnityEngine;

[CreateAssetMenu(fileName = "TowerDataSO", menuName = "Tower Data/TowerDataSO")]
public class TowerDataSO : ScriptableObject
{
    public string towerName;
    public int towerCost;
    public string towerDescription;
    public Tower prefabObjectTower;
    
    [Header("Tower Base Status")]
    public int baseTowerRanger;
    public int baseTowerMaxCapacity;
    public float baseTowerProcessingSpeed;
    public float baseTowerAttackSpeed;
}
