using UnityEngine;
using UnityEngine.InputSystem;
using UtilTools;


public class BuildingInput : MonoBehaviour
{
    [SerializeField] private InputActionReference clickGrid;
    [SerializeField] private InputActionReference clickGetValueGrid;
    
    [SerializeField] private GridPlacement gridPlacement;
    
    private void OnEnable()
    {
        clickGrid.action.Enable();
        clickGetValueGrid.action.Enable();
        
        clickGrid.action.performed += OnClickGrid;
    }

    private void OnDisable()
    {
        clickGrid.action.performed -= OnClickGrid;
    }
    
    private void OnClickGrid(InputAction.CallbackContext _)
    {
        Vector3 mouseWorldPosition = UtilTools.UtilsClass.GetMouseWorldPositionWithZ();

        GridZone gridZone;
        
        GridManager.Instance.GetGridMapZoneCell(mouseWorldPosition, out gridZone);
        
        if (gridZone == GridZone.Build)
        {
            Vector3 gridPosition = GridManager.Instance.GridToWorldPosition(mouseWorldPosition);
            gridPlacement.PlaceTower(gridPosition);
        }
    }
}
