using UnityEngine;
using TMPro;

public class TowerCardUpgradeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private TMP_Text towerLevel;

    [SerializeField] private CanvasGroup canvasGroup;
    
    private Tower _towerData;
    
    private void OnEnable()
    {
        GameEvents.OnShowTowerCardUpgrade.AddListener(SetTowerCardUpgradeUI);
    }

    private void OnDisable()
    {
        GameEvents.OnShowTowerCardUpgrade.RemoveListener(SetTowerCardUpgradeUI);
    }
    
    private void SetTowerCardUpgradeUI(Tower towerData)
    {
        _towerData =  towerData;
        var towerRunTimeData = towerData.TowerRunTimeData;
        
        towerName.text = towerRunTimeData.towerName;
        towerLevel.text = $"level {towerRunTimeData.towerLevel}";
        
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        
        GameEvents.OnShowTowerDetectRange.Invoke(towerData.TowerID);
        Debug.Log($"[{name} (PlaceTower)] Selected tower : {towerRunTimeData.towerName} ID : {towerData.TowerID}");
    }

    public void HideTowerCardUpgradeUI()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        
        _towerData = null;
        GameEvents.OnHideTowerDetectRange.Invoke();
    }
    
    public void RemoveTowerCardUpgradeUI()
    {
        _towerData.RemoveTower();
        
        HideTowerCardUpgradeUI();
    }
}
