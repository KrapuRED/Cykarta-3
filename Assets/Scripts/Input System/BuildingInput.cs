using UnityEngine;
using UnityEngine.InputSystem;
using UtilTools;


public class BuildingInput : MonoBehaviour
{
    [SerializeField] private InputActionReference clickGrid;
    [SerializeField] private InputActionReference clickSellGrid;
    
    [SerializeField] private GridPlacement gridPlacement;
    
    private void OnEnable()
    {
        clickGrid.action.Enable();
        clickSellGrid.action.Enable();
        
        clickGrid.action.performed += OnClickGrid;
        clickSellGrid.action.performed += OnClickSelectTower;
    }

    private void OnDisable()
    {
        clickGrid.action.performed -= OnClickGrid;
        clickSellGrid.action.performed -= OnClickSelectTower;
    }
    
    private void OnClickGrid(InputAction.CallbackContext _)
    {
        Vector3 mouseWorldPosition = UtilTools.UtilsClass.GetMouseWorldPositionWithZ();

        GridZone gridZone = GridZone.None;
        
        GridManager.Instance.GetGridMapZoneCell(mouseWorldPosition, out gridZone);
        
        if (gridZone == GridZone.Build)
        {
            Vector3 gridPosition = GridManager.Instance.GridToWorldPosition(mouseWorldPosition);
            gridPlacement.PlaceTower(gridPosition);
            
            GridManager.Instance.UnhighlightBuildGridZone();
            GameEvents.OnHideTowerCardDetail.Invoke();
        }
    }

    private void OnClickSelectTower(InputAction.CallbackContext _)
    {
        Vector3 mouseWorldPosition = UtilTools.UtilsClass.GetMouseWorldPositionWithZ();

        GridZone gridZone;
        
        GridManager.Instance.GetGridMapZoneCell(mouseWorldPosition, out gridZone);
        
        if (gridZone == GridZone.Occupied)
        {
            Vector3 gridPosition = GridManager.Instance.GridToWorldPosition(mouseWorldPosition);
            gridPlacement.GetActiveTower(gridPosition);
        }
    }
}
