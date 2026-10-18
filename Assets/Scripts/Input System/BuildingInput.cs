using UnityEngine;
using UnityEngine.InputSystem;
using UtilTools;
using UnityEngine.EventSystems;

public class BuildingInput : MonoBehaviour
{
    [SerializeField] private bool inputActive;
    [SerializeField] private InputActionReference onMousePosition;
    [SerializeField] private InputActionReference clickGrid;
    [SerializeField] private InputActionReference clickSellGrid;
    
    [SerializeField] private GridPlacement gridPlacement;
    
    private GridManager _gridManager;
    
    private void OnEnable()
    {
        clickGrid.action.Enable();
        clickSellGrid.action.Enable();
        onMousePosition.action.Enable();
        
        clickGrid.action.performed += OnClickGrid;
        clickSellGrid.action.performed += OnClickSellTower;
        onMousePosition.action.performed += OnPositionMouse;
    }

    private void OnDisable()
    {
        clickGrid.action.performed -= OnClickGrid;
        clickSellGrid.action.performed -= OnClickSellTower;
        onMousePosition.action.performed -= OnPositionMouse;
        
    }
    
    private bool IsPointerOverUI() => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    
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
    
    private void OnClickGrid(InputAction.CallbackContext _)
    {
        if (IsPointerOverUI()) return;
        
        Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPositionWithZ();
        GridManager.Instance.GetGridMapZoneCell(mouseWorldPosition, out GridZone gridZone);

        switch (GridManager.Instance.CurrentGridMode)
        {
            case GridMode.Building:
                if (gridZone != GridZone.Build) break; // invalid tile, let them retry

                Vector3 gridPosition = GridManager.Instance.GridToWorldPosition(mouseWorldPosition);
                gridPlacement.PlaceTower(gridPosition);   // needs prefabTower + Building mode
                GridManager.Instance.UnhighlightBuildGridZone();          // destroys preview, clears prefab
                GridManager.Instance.ChangeGridMode(GridMode.Confirmation);
                GameEvents.OnHideTowerCardDetail.Invoke();
                GameEvents.OnShowConfirmationUI.Invoke();
                break;

            case GridMode.None:
                if (gridZone == GridZone.Occupied)
                    gridPlacement.GetActiveTower(GridManager.Instance.GridToWorldPosition(mouseWorldPosition));
                break;
        }
    }

    private void OnClickSellTower(InputAction.CallbackContext _)
    {
        GameEvents.OnDeselectTowerCard.Invoke();
        
        GridManager.Instance.CancelBuild();
    }
    
}
