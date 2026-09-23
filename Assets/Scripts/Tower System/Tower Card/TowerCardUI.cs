using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TowerCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private TMP_Text towerCost;

    [Header("Input Action Settings")]
    [SerializeField] private string actionMapName;
    [SerializeField] private InputActionReference holdCardAction;
    
    private bool _isSelected;
    private TowerDataSO TowerData { get;  set; }

    private void OnEnable()
    {
        holdCardAction.action.Enable();

        holdCardAction.action.performed += OnHoldButton;
        
        GameEvents.OnDeselectTowerCard.AddListener(DeselectTowerCardUI);
        GameEvents.OnUpdateVisualCurrency.AddListener(HandleCostText);
    }

    private void OnDisable()
    {
        holdCardAction.action.performed -= OnHoldButton;
        
        GameEvents.OnDeselectTowerCard.RemoveListener(DeselectTowerCardUI);
        GameEvents.OnUpdateVisualCurrency.RemoveListener(HandleCostText);
    }
    
    private void OnHoldButton(InputAction.CallbackContext context)
    {
        if (!_isSelected || !CurrencyManager.Instance.IsCurrentCurrencyEnough(TowerData.towerCost)) 
            return;
        
        _isSelected = false;
        GridManager.Instance.HighlightBuildGridZone();
    }

    private void HandleCostText(int cost)
    {
        towerCost.color = CurrencyManager.Instance.IsCurrentCurrencyEnough(TowerData.towerCost) ? Color.green : Color.red; 
        towerCost.text = $"${TowerData.towerCost}";
    }
    
    public void SetTowerCardUI(TowerDataSO towerData)
    {
       TowerData = towerData;
       
       towerName.text = towerData.towerName;
       HandleCostText(towerData.towerCost);
    }

    public void SelectTowerCardUI()
    {
        if (_isSelected)
        {
            DeselectTowerCardUI();
            return;
        }
        
        Debug.Log($"{name} SelectTowerCardUI");
        GameEvents.OnDeselectTowerCard.Invoke();
        _isSelected = true;
        GameEvents.OnShowTowerCardDetail.Invoke(TowerData);
    }

    public void DeselectTowerCardUI()
    {
        if (!_isSelected) return;
        
        _isSelected = false;
    }
}
