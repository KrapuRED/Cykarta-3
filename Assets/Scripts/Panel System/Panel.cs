using UnityEngine;

public enum PanelType
{
    None,
    Building
}

public abstract class Panel : MonoBehaviour
{
    [Header("Panel Configuration")]
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected PanelType panelType;
    
    public PanelType PanelType => panelType;

    public abstract void OpenPanel();

    public abstract void ClosePanel();
}
