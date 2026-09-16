using UnityEngine;
using TMPro;

public class TowerCardUpgradeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private TMP_Text towerLevel;

    [SerializeField] private CanvasGroup canvasGroup;
    
    private void OnEnable()
    {
        GameEvents.OnShowTowerCardUpgrade.AddListener(SetTowerCardUpgradeUI);
        GameEvents.OnHideTowerCardDetail.AddListener(HideTowerCardUpgradeUI);
    }

    private void OnDisable()
    {
        GameEvents.OnShowTowerCardUpgrade.RemoveListener(SetTowerCardUpgradeUI);
        GameEvents.OnHideTowerCardDetail.RemoveListener(HideTowerCardUpgradeUI);
    }
    
    public void SetTowerCardUpgradeUI(Tower towerData)
    {
        var towerRunTimeData = towerData.TowerRunTimeData;
        
        towerName.text = towerRunTimeData.towerName;
        towerLevel.text = $"level {towerRunTimeData.towerLevel}";
        
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    private void HideTowerCardUpgradeUI()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}
