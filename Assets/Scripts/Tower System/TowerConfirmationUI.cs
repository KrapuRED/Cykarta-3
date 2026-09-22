using System;
using UnityEngine;

public class TowerConfirmationUI : MonoBehaviour
{
    [SerializeField] private Tower ownerTower;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private bool _isBeenConfirmed;
    
    private void OnEnable()
    {
        GameEvents.OnShowConfirmationUI.AddListener(ShowTowerConfirmationUI);
    }

    private void OnDisable()
    {
        GameEvents.OnShowConfirmationUI.RemoveListener(ShowTowerConfirmationUI);
    }

    private void ShowTowerConfirmationUI()
    {
        if (_isBeenConfirmed) return;
        
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    private void HideTowerConfirmationUI()
    {
        if (!_isBeenConfirmed) return;
        
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void ConfirmTower()
    {
        GridManager.Instance.ChangeGridMode(GridMode.None);

        CurrencyManager.Instance.UseCurrency(ownerTower.TowerData.towerCost);
        _isBeenConfirmed = true;
        HideTowerConfirmationUI();
        
        GameEvents.OnHideTowerDetectRange.Invoke();
    }
    
    public void RotateTowerToRight() => ownerTower.RotateTower(TowerRotation.Right);
    public void RotateTowerToLeft() => ownerTower.RotateTower(TowerRotation.Left);
    public void RemoveTower() => ownerTower.RemoveTower();
}
