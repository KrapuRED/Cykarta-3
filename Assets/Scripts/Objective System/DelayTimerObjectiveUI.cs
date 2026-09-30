using System;
using TMPro;
using UnityEngine;

public class DelayTimerObjectiveUI : MonoBehaviour
{
   [SerializeField] private TMP_Text timerText;
   [SerializeField] private CanvasGroup canvasGroup;

   private void Awake()
   {
      HideDelayTimerObjectiveUI();
   }

   public void ShowDelayTimerObjectiveUI()
   {
      Debug.Log("ShowDelayTimerObjectiveUI");
      canvasGroup.alpha = 1;
   }

   public void HideDelayTimerObjectiveUI()
   {
      canvasGroup.alpha = 0;
   }
   
   public void UpdateTimerText(int waveIndex, float time)
   {
      int minutes = (int)time / 60;
      int seconds = (int)time % 60;
      
      timerText.text = $"Wave {waveIndex} begin {minutes}:{seconds}";
   }
}
