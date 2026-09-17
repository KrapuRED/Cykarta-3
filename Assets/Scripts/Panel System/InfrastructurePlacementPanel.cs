using System;
using UnityEngine;
using TMPro;

public class InfrastructurePlacementPanel : Panel
{
   [SerializeField] private TMP_Text indicatorPlacementText; 
   [SerializeField] private TMP_Text instructionPlacementText; 
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
      
      indicatorPlacementText.gameObject.SetActive(true);
      instructionPlacementText.gameObject.SetActive(true);
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
      GameEvents.OnShowConfirmationUI.AddListener(HideText);
   }

   private void OnDisable()
   {
      GameEvents.OnShowGridZone.RemoveListener(UpdateIndicatorPlacement);
      GameEvents.OnShowConfirmationUI.RemoveListener(HideText);
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

   private void HideText()
   {
      indicatorPlacementText.gameObject.SetActive(false);
      instructionPlacementText.gameObject.SetActive(false);
   }
}
