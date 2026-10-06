using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInput : GeneralInput
{
   [SerializeField] private InputActionReference pauseAction;

   [SerializeField] private bool _paused;
   
   private void OnEnable()
   {
      pauseAction.action.Enable();
      
      pauseAction.action.performed += _ => PauseGame();

   }

   private void OnDisable()
   {
      pauseAction.action.performed -= _ => PauseGame();

   }

   public void PauseGame()
   {
      if (_paused) return;
      
      _paused = true;
      GameEvents.OnRequestOpenPanel.Invoke(PanelType.Pause);
   }
   
   public void ResumeGame()
   {
      if (!_paused) return;

      _paused = false;
      GameEvents.OnRequestClosePanel.Invoke(PanelType.Pause);
   }
}
