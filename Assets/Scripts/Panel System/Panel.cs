using System;
using UnityEngine;

public abstract class Panel : MonoBehaviour
{
    [Header("Panel Configuration")]
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected PanelType panelType;
    
    protected TransitionHelper TransitionHelper;

    public PanelType PanelType => panelType;

    public abstract void OpenPanel();

    public abstract void ClosePanel();
}
