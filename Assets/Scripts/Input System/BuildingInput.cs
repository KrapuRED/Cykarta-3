using UnityEngine;
using UnityEngine.InputSystem;
using UtilTools;


public class BuildingInput : MonoBehaviour
{
    [SerializeField] private bool inputActive;
    [SerializeField] private InputActionReference onMousePosition;
    [SerializeField] private InputActionReference clickGrid;
    [SerializeField] private InputActionReference clickSellGrid;
    [SerializeField] private InputActionReference holdCardAction;
    
    [SerializeField] private GridPlacement gridPlacement;
    
    private GridManager _gridManager;
    
    private void OnEnable()
    {
        clickGrid.action.Enable();
        clickSellGrid.action.Enable();
        onMousePosition.action.Enable();
        holdCardAction.action.Enable();
        
        clickGrid.action.performed += OnClickGrid;
        clickSellGrid.action.performed += OnClickSellTower;
        onMousePosition.action.performed += OnPositionMouse;
        holdCardAction.action.canceled += OnReleaseHoldButton;
    }

    private void OnDisable()
    {
        clickGrid.action.performed -= OnClickGrid;
        clickSellGrid.action.performed -= OnClickSellTower;
        onMousePosition.action.performed -= OnPositionMouse;
        holdCardAction.action.canceled -= OnReleaseHoldButton;
        
    }
    
    private void OnPositionMouse(InputAction.CallbackContext ctx)
    {
        if (!inputActive) return;
        
        Vector3 mouseWorldPosition = UtilTools.UtilsClass.GetMouseWorldPosition();
        GridManager.Instance.GetGridMapZoneCell(mouseWorldPosition, out var gridZoneData);

        var towerPreview = GridManager.Instance.InstancePreviewTower;
        if (towerPreview != null)
            towerPreview.transform.position = mouseWorldPosition;
        
        GameEvents.OnShowGridZone.Invoke(gridZoneData);
    }

    private void OnReleaseHoldButton(InputAction.CallbackContext _)
    {
        if (_gridManager == null) _gridManager = GridManager.Instance;

        if (_gridManager.CurrentGridMode == GridMode.Confirmation)
        {
            return;
        }
        
        Vector3 mouseWorldPosition = UtilTools.UtilsClass.GetMouseWorldPositionWithZ();

        GridZone gridZone = GridZone.None;
        _gridManager.GetGridMapZoneCell(mouseWorldPosition, out gridZone);

        if (gridZone == GridZone.Build)
        {
            Vector3 gridPosition = _gridManager.GridToWorldPosition(mouseWorldPosition);
            gridPlacement.PlaceTower(gridPosition);
            
            GridManager.Instance.UnhighlightBuildGridZone();
            GameEvents.OnShowConfirmationUI.Invoke();
            
            _gridManager.ChangeGridMode(GridMode.Confirmation);
        }
        else
        {
            GridManager.Instance.ChangeGridMode(GridMode.None);
        }
        
        GameEvents.OnHideTowerCardDetail.Invoke();
    }
    
    private void OnClickGrid(InputAction.CallbackContext _)
    {
        if (_gridManager == null) _gridManager = GridManager.Instance;
        
        Vector3 mouseWorldPosition = UtilTools.UtilsClass.GetMouseWorldPositionWithZ();

        GridZone gridZone = GridZone.None;
        _gridManager.GetGridMapZoneCell(mouseWorldPosition, out gridZone);

        if (gridZone == GridZone.Occupied)
        {
            Vector3 gridPosition = _gridManager.GridToWorldPosition(mouseWorldPosition);
            gridPlacement.GetActiveTower(gridPosition);
        }
    }

    private void OnClickSellTower(InputAction.CallbackContext _)
    {
        Vector3 mouseWorldPosition = UtilTools.UtilsClass.GetMouseWorldPosition();
        GridManager.Instance.GetGridMapZoneCell(mouseWorldPosition, out var gridZoneData);
        
        Debug.Log($"[{name} GetGridMapZoneCell] Position {mouseWorldPosition} GridZone {gridZoneData}");
        
        //gridPlacement.RemoveTower(mouseWorldPosition);
    }
}
