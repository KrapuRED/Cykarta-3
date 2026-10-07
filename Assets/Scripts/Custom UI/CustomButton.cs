using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    
    [SerializeField] private UnityEvent onHoverEnter;
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private UnityEvent onHoverExit;

    public bool IsPointerInside { get; private set; }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        onHoverEnter?.Invoke();
        Debug.Log($"[{name}] - Hover in");
        
        IsPointerInside = true;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit?.Invoke();
        Debug.Log($"[{name}] - Hover Out");
        
        IsPointerInside = false;
    }
}
