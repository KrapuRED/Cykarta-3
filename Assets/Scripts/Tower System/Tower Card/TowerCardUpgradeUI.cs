using UnityEngine;
using TMPro;

public class TowerCardUpgradeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private TMP_Text towerLevel;
    [SerializeField] private TMP_Text towerStatus;
    [SerializeField] private TMP_Text towerCost;
    [SerializeField] private TowerPreviewUpgradeUI towerPreviewUpgradeUI;
    
    [SerializeField] private CanvasGroup canvasGroup;
    
    private Tower _towerData;
    private UpgradeTowerData _upgradeTowerData;
    
    private void OnEnable()
    {
        GameEvents.OnShowTowerCardUpgrade.AddListener(SetTowerCardUpgradeUI);
        GameEvents.OnUpdateVisualCurrency.AddListener(HandleCostText);
    }

    private void OnDisable()
    {
        GameEvents.OnShowTowerCardUpgrade.RemoveListener(SetTowerCardUpgradeUI);
        GameEvents.OnUpdateVisualCurrency.RemoveListener(HandleCostText);
    }

    private string BuildCurrentStatusText(TowerRunTimeData towerRunTimeData)
    {
        string bs = string.Empty;
        
        foreach (var s in  towerRunTimeData.currentTowerStatusData)
        {
            bs += $"{s.status} : {s.valueStatus}\n";
        }
        
        return bs;
    }
    
    private void SetTowerCardUpgradeUI(Tower towerData)
    {
        _towerData =  towerData;
        var towerRunTimeData = towerData.TowerRunTimeData;
        _upgradeTowerData = _towerData.GetNextUpgrade();
        
        towerName.text = towerRunTimeData.towerName;
        towerLevel.text = $"level {towerRunTimeData.towerLevel}";
        towerStatus.text = BuildCurrentStatusText(towerRunTimeData);
        
        if (_upgradeTowerData != null)
                    HandleCostText(_upgradeTowerData.upgradeCost);
        
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        
        GameEvents.OnShowTowerDetectRange.Invoke(towerData.TowerID);
    }

    public void ShowTowerPreviewUpgradeUI()
    {
        Debug.Log($"[{name}] ShowTowerPreviewUpgradeUI");
        
        if (_upgradeTowerData  == null) return;
        
        towerPreviewUpgradeUI.ShowPreview(_towerData.TowerRunTimeData, _upgradeTowerData);
    }

    public void BuyTowerUpgrade()
    {
        if (_upgradeTowerData  == null || !CurrencyManager.Instance.IsCurrentCurrencyEnough(_upgradeTowerData.upgradeCost)) return;
        
        _towerData.UpgradeTower(_upgradeTowerData);
        
       towerPreviewUpgradeUI.HidePreview();
       HideTowerCardUpgradeUI();
    }
    
    public void HideTowerPreviewUpgradeUI()
    {
       towerPreviewUpgradeUI.HidePreview();
    }
    
    public void HideTowerCardUpgradeUI()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        
        _towerData = null;
        _upgradeTowerData = null;
        
        HideTowerPreviewUpgradeUI();
        
        GameEvents.OnHideTowerDetectRange.Invoke();
    }
    
    private void HandleCostText(int cost)
    {
        if (_upgradeTowerData ==  null) return;
        
        towerCost.color = CurrencyManager.Instance.IsCurrentCurrencyEnough(_upgradeTowerData.upgradeCost) ? Color.green : Color.red; 
        towerCost.text = $"${_upgradeTowerData.upgradeCost}";
    }
    
    public void RemoveTowerCardUpgradeUI()
    {
        _towerData.RemoveTower();
        
        HideTowerCardUpgradeUI();
    }
}
