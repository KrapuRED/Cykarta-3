using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerCardDetailUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerNameText;
    [SerializeField] private TMP_Text towerCostText;
    [SerializeField] private TMP_Text towerDescriptionText;
    [SerializeField] private TMP_Text towerStatusDescriptionText;
    [SerializeField] private Image towerIcon;

    [SerializeField] private VisualEffect slideEffect;
    
    
    private void OnEnable()
    {
        GameEvents.OnShowTowerCardDetail.AddListener(SetTowerDetailUI);
        GameEvents.OnHideTowerCardDetail.AddListener(HideTowerDetailUI);
    }

    private void OnDisable()
    {
        GameEvents.OnShowTowerCardDetail.RemoveListener(SetTowerDetailUI);
        GameEvents.OnHideTowerCardDetail.RemoveListener(HideTowerDetailUI);
    }

    private void SetTowerDetailUI(TowerDataSO towerData)
    {
        towerNameText.text        = towerData.towerName;
        towerCostText.text        = $"$ {towerData.towerCost}";
        towerDescriptionText.text = towerData.towerDescription;
        towerIcon.sprite = towerData.towerIcon;
        
        slideEffect.PlayAnimation();
    }

    private void HideTowerDetailUI()
    {
        slideEffect.RevertAnimation();
        
        towerNameText.text        = string.Empty;
        towerCostText.text        = string.Empty;
        towerDescriptionText.text = string.Empty;
        towerIcon.sprite          = null;
    }
}
