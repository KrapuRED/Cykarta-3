using TMPro;
using UnityEngine;

public class TowerCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private TMP_Text towerCost;

    public TowerDataSO TowerData { get; private set; }

    public void SetTowerCardUI(TowerDataSO towerData)
    {
       TowerData = towerData;
       
       towerName.text = towerData.towerName;
       towerCost.text = $"${towerData.towerCost}";
    }

    public void SelectTowerCardUI()
    {
        Debug.Log($"{name} SelectTowerCardUI");
        
        GridManager.Instance.HighlightBuildGridZone();
        
        GameEvents.OnShowTowerCardDetail.Invoke(TowerData);
    }
}
