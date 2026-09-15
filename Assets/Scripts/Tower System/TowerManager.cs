using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance {  get; private set; }

    [SerializeField] private List<Tower> activeTowers = new List<Tower>();
    
    private Dictionary<string, int> activeTowerDict = new Dictionary<string, int>();
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public string GetTowerID(string towerName)
    {
        string towerID = string.Empty;

        int indexTower = 0;
        if (activeTowerDict.TryGetValue(towerName, out indexTower))
        {
            indexTower ++;
            towerID = $"{towerName}_{indexTower}";
        }
        else
        {
            towerID = $"{towerName}_{indexTower}";
        }
        activeTowerDict[towerName] = indexTower;
        
        return towerID;
    }
    
    public void RegisterTower(Tower tower)
    {
        activeTowers.Add(tower);
    }
}
