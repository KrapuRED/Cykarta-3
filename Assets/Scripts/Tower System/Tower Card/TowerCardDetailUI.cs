using System;
using TMPro;
using UnityEngine;

public class TowerCardDetailUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerNameText;
    [SerializeField] private TMP_Text towerCostText;
    [SerializeField] private TMP_Text towerDescriptionText;
    [SerializeField] private TMP_Text towerStatusDescriptionText;

    [SerializeField] private CanvasGroup canvasGroup;

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
        
        
        
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    private void HideTowerDetailUI()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}
