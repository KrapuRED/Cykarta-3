using UnityEngine;

public class PausePanel : Panel
{
   public override void OpenPanel()
   {
      canvasGroup.alpha = 1;
      canvasGroup.blocksRaycasts = true;
      canvasGroup.interactable = true;
      
      GameEvents.OnDeselectTowerCard.Invoke();
      
      GridManager.Instance.ChangeGridMode(GridMode.None);
      GridManager.Instance.UnhighlightBuildGridZone();
      
      SpawnerManager.Instance.Pause();
      EntityManager.Instance.Pause();
   }

   public override void ClosePanel()
   {
      canvasGroup.alpha = 0;
      canvasGroup.blocksRaycasts = false;
      canvasGroup.interactable = false;
      
      SpawnerManager.Instance.Resume();
      EntityManager.Instance.Resume();
   }
}
