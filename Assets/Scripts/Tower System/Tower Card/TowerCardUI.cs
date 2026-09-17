using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class TowerCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private TMP_Text towerCost;

    [Header("Input Action Settings")]
    [SerializeField] private string actionMapName;
    [SerializeField] private InputActionReference holdCardAction;
    
    public TowerDataSO TowerData { get; private set; }

    private void OnEnable()
    {
        holdCardAction.action.Enable();

        holdCardAction.action.performed += OnHoldButton;
    }

    private void OnDisable()
    {
        holdCardAction.action.performed -= OnHoldButton;
    }
    
    private void OnHoldButton(InputAction.CallbackContext context)
    {
        GridManager.Instance.HighlightBuildGridZone();
    }
    
    public void SetTowerCardUI(TowerDataSO towerData)
    {
       TowerData = towerData;
       
       towerName.text = towerData.towerName;
       towerCost.text = $"${towerData.towerCost}";
    }

    public void SelectTowerCardUI()
    {
        Debug.Log($"{name} SelectTowerCardUI");

        GameEvents.OnShowTowerCardDetail.Invoke(TowerData);
    }
}
