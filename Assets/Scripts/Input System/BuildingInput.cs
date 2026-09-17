using UnityEngine;
using UnityEngine.InputSystem;
using UtilTools;


public class BuildingInput : MonoBehaviour
{
    [SerializeField] private InputActionReference onMousePosition;
    [SerializeField] private InputActionReference clickGrid;
    [SerializeField] private InputActionReference clickSellGrid;
    [SerializeField] private InputActionReference holdCardAction;
    
    [SerializeField] private GridPlacement gridPlacement;
    
    private void OnEnable()
    {
        clickGrid.action.Enable();
        clickSellGrid.action.Enable();
        onMousePosition.action.Enable();
        holdCardAction.action.Enable();
        
        clickGrid.action.performed += OnClickGrid;
        clickSellGrid.action.performed += OnClickSelectTower;
        onMousePosition.action.performed += OnPositionMouse;
        holdCardAction.action.canceled += OnReleaseHoldButton;
    }

    private void OnDisable()
    {
        clickGrid.action.performed -= OnClickGrid;
        clickSellGrid.action.performed -= OnClickSelectTower;
        onMousePosition.action.performed -= OnPositionMouse;
        holdCardAction.action.canceled -= OnReleaseHoldButton;
        
    }
    
    private void OnPositionMouse(InputAction.CallbackContext ctx)
    {
        Vector3 mouseWorldPosition = UtilTools.UtilsClass.GetMouseWorldPosition();

        GridZone gridZoneData = GridZone.None;
        GridManager.Instance.GetGridMapZoneCell(mouseWorldPosition, out gridZoneData);

        var towerPreview = GridManager.Instance.InstancePreviewTower;
        if (towerPreview != null)
            towerPreview.transform.position = mouseWorldPosition;
        
        GameEvents.OnShowGridZone.Invoke(gridZoneData);
    }

    private void OnReleaseHoldButton(InputAction.CallbackContext _)
    {
        Vector3 mouseWorldPosition = UtilTools.UtilsClass.GetMouseWorldPositionWithZ();

        GridZone gridZone = GridZone.None;
        GridManager.Instance.GetGridMapZoneCell(mouseWorldPosition, out gridZone);

        if (gridZone == GridZone.Build)
        {
            Vector3 gridPosition = GridManager.Instance.GridToWorldPosition(mouseWorldPosition);
            gridPlacement.PlaceTower(gridPosition);
        }
        
        GridManager.Instance.UnhighlightBuildGridZone();
        GameEvents.OnHideTowerCardDetail.Invoke();
    }
    
    private void OnClickGrid(InputAction.CallbackContext _)
    {
        Vector3 mouseWorldPosition = UtilTools.UtilsClass.GetMouseWorldPositionWithZ();

        GridZone gridZone = GridZone.None;
        GridManager.Instance.GetGridMapZoneCell(mouseWorldPosition, out gridZone);

        if (gridZone == GridZone.Occupied)
        {
            Vector3 gridPosition = GridManager.Instance.GridToWorldPosition(mouseWorldPosition);
            gridPlacement.GetActiveTower(gridPosition);
        }
    }

    private void OnClickSelectTower(InputAction.CallbackContext _)
    {

    }
}
