using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private static PanelManager _instance;

    [SerializeField] private Transform panelContainer;
    [SerializeField] private List<Panel> panels = new();
    
    private Dictionary<PanelType, Panel> _panelLookup = new();
    private Panel _activePanel;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        panels = panelContainer.GetComponentsInChildren<Panel>().ToList();

        foreach (var panelBase in panels)
        {
            _panelLookup[panelBase.PanelType] = panelBase;
        }
    }

    private void OnEnable()
    {
        GameEvents.OnRequestOpenPanel.AddListener(HandelOpenPanel);
        GameEvents.OnRequestClosePanel.AddListener(HandelClosePanel);
    }

    private void OnDisable()
    {
        OnRemoveListeners();
    }

    private void OnDestroy()
    {
        OnRemoveListeners();
    }

    private void OnRemoveListeners()
    {
        GameEvents.OnRequestOpenPanel.RemoveListener(HandelOpenPanel);
        GameEvents.OnRequestClosePanel.RemoveListener(HandelClosePanel);
    }

    private void HandelOpenPanel(PanelType panelType)
    {
        if (_activePanel != null) _activePanel.ClosePanel();
        _panelLookup[panelType].OpenPanel();
        _activePanel = _panelLookup[panelType];
    }

    private void HandelClosePanel(PanelType panelType)
    {
        _panelLookup[panelType].ClosePanel();
        if (_activePanel == _panelLookup[panelType]) _activePanel = null;
    }
}
