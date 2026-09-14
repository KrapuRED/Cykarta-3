using UnityEngine;
using UnityEngine.InputSystem;
using UtilTools;


public class BuildingInput : MonoBehaviour
{
    [SerializeField] private InputActionReference clickGrid;
    [SerializeField] private InputActionReference clickGetValueGrid;
    
    private void OnEnable()
    {
        clickGrid.action.Enable();
        clickGetValueGrid.action.Enable();
        
        clickGrid.action.performed += OnClickGrid;
        clickGetValueGrid.action.performed += OnDebugClickGrid;
    }

    private void OnDisable()
    {
        clickGrid.action.performed -= OnClickGrid;
        clickGetValueGrid.action.performed -= OnDebugClickGrid;
    }
    
    private void OnClickGrid(InputAction.CallbackContext _)
    {
        GridManager.Instance.IncreaseValueCell(UtilTools.UtilsClass.GetMouseWorldPositionWithZ());
    }
    
    private void OnDebugClickGrid(InputAction.CallbackContext _)
    {
        GridManager.Instance.GetValueCell(UtilTools.UtilsClass.GetMouseWorldPositionWithZ());
    }
}
