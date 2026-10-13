using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MoreMountains.Tools;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    
    [SerializeField] private UnityEvent onHoverEnter;
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private UnityEvent onHoverExit;

    [Header("Sound Effect and ambient")]
    [SerializeField] private AudioClip hoverInSound;  
    [SerializeField] private AudioClip clickSound;  
    [SerializeField] private AudioClip hoverOutSound;  
    
    public bool IsPointerInside { get; private set; }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        onHoverEnter?.Invoke();
        MMSoundManagerSoundPlayEvent.Trigger(hoverInSound, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
        
        Debug.Log($"[{name}] - Hover in");
        
        IsPointerInside = true;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        MMSoundManagerSoundPlayEvent.Trigger(clickSound, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
        
        onClick?.Invoke();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit?.Invoke();
        MMSoundManagerSoundPlayEvent.Trigger(hoverOutSound, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
        
        Debug.Log($"[{name}] - Hover Out");
        
        IsPointerInside = false;
    }
}
