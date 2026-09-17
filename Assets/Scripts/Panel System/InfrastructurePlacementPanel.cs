using System;
using UnityEngine;
using TMPro;

public class InfrastructurePlacementPanel : Panel
{
   [SerializeField] private TMP_Text indicatorPlacementText; 
   [SerializeField] private CanvasGroup hudCanvasGroup;
   
   private bool _isPanelActive;
   
   public override void OpenPanel()
   {
      _isPanelActive = true;
      
      canvasGroup.alpha = 1;
      canvasGroup.blocksRaycasts = true;
      canvasGroup.interactable = true;
      
      hudCanvasGroup.alpha = 0;
      hudCanvasGroup.blocksRaycasts = false;
      hudCanvasGroup.interactable = false;
   }

   public override void ClosePanel()
   {
      _isPanelActive = false;
      
      canvasGroup.alpha = 0;
      canvasGroup.blocksRaycasts = false;
      canvasGroup.interactable = false;
      
      hudCanvasGroup.alpha = 1;
      hudCanvasGroup.blocksRaycasts = true;
      hudCanvasGroup.interactable = true;

   }

   private void OnEnable()
   {
      GameEvents.OnShowGridZone.AddListener(UpdateIndicatorPlacement);
   }

   private void OnDisable()
   {
      GameEvents.OnShowGridZone.RemoveListener(UpdateIndicatorPlacement);
   }

   private void UpdateIndicatorPlacement(GridZone gridZone)
   {
      if (!_isPanelActive) return;
      
      if (gridZone == GridZone.Build)
      {
         indicatorPlacementText.text = "Valid Tile";
      }
      else
      {
         indicatorPlacementText.text = "Invalid Tile";
      }
   } 
}
