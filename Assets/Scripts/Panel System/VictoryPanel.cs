using UnityEngine;
using MoreMountains.Tools;
using DG.Tweening;

public class VictoryPanel : Panel
{
    [Header("Sound Effect and ambient")]
    [SerializeField] private AudioClip openPanelSound; 
    
    [SerializeField] private VisualEffect slideEffect;
    [SerializeField] private VisualEffect fadeEffect;
    
    private bool _isOpen;
    private Tween _tween;
    
    public override void OpenPanel()
    {
        PlayAnimation();
        
        GameEvents.OnDeselectTowerCard.Invoke();
      
        GridManager.Instance.ChangeGridMode(GridMode.None);
        GridManager.Instance.UnhighlightBuildGridZone();
      
        MMSoundManagerSoundPlayEvent.Trigger(openPanelSound, MMSoundManager.MMSoundManagerTracks.Music, transform.position);
        
        GameEvents.OnGamePause.Invoke();
        EntityManager.Instance.Pause();
        
    }

    public override void ClosePanel()
    {
        PlayAnimation();
        
        GameEvents.OnGameResume.Invoke();
    }
    
    public void ExitLevel()
    {
        if (TransitionHelper == null)
            TransitionHelper = GetComponent<TransitionHelper>();
      
        ObjectiveManager.Instance.SaveData();
        
        TransitionHelper.DoTransitionDirectToStory(ObjectiveManager.Instance.LevelIndex, DialogueType.After);
    }

    private void PlayAnimation()
    {
        if (_isOpen)
        {
            Debug.Log("Close Panel");
            _tween?.Kill();
            // Play FadeEffect after that play slideEffect
            fadeEffect.RevertAnimation();
            fadeEffect.Tween.OnComplete(() => slideEffect.RevertAnimation());
        }
        else
        {
            Debug.Log("Open Panel");
            
            _tween?.Kill();
            // Play slideEffect after that play FadeEffect
            slideEffect.PlayAnimation();
            slideEffect.Tween.OnComplete(() => fadeEffect.PlayAnimation());
        }
        _isOpen = !_isOpen;
    }
}
