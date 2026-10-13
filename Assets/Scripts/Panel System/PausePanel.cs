using MoreMountains.Tools;
using UnityEngine;

public class PausePanel : Panel
{
    [SerializeField] private float musicFadeDuration = 0.5f;
   
   public override void OpenPanel()
   {
      canvasGroup.alpha = 1;
      canvasGroup.blocksRaycasts = true;
      canvasGroup.interactable = true;
      
      GameEvents.OnDeselectTowerCard.Invoke();
      
      GridManager.Instance.ChangeGridMode(GridMode.None);
      GridManager.Instance.UnhighlightBuildGridZone();

      FadeMusic(0);
      
      SpawnerManager.Instance.Pause();
      EntityManager.Instance.Pause();
   }

   public override void ClosePanel()
   {
      canvasGroup.alpha = 0;
      canvasGroup.blocksRaycasts = false;
      canvasGroup.interactable = false;
      
      FadeMusic(1);
      
      
      SpawnerManager.Instance.Resume();
      EntityManager.Instance.Resume();
   }

   public void ExitLevel()
   {
      if (TransitionHelper == null)
         TransitionHelper = GetComponent<TransitionHelper>();
      
      ObjectiveManager.Instance.SaveData();
      
      TransitionHelper.DoTransition();
   }
   
   private void FadeMusic( float targetVolume)
   {
      Debug.Log($"[{name}] FadeInMusic");
        
      MMSoundManagerTrackFadeEvent.Trigger(
         MMSoundManagerTrackFadeEvent.Modes.PlayFade,
         MMSoundManager.MMSoundManagerTracks.Sfx,
         musicFadeDuration,
         targetVolume,
         new MMTweenType(MMTween.MMTweenCurve.EaseInOutQuartic));
      
   }
}
