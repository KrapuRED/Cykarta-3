using UnityEngine;
using MoreMountains.Tools;
using DG.Tweening;

public class DefeatPanel : Panel
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
      
        SpawnerManager.Instance.Pause();
        EntityManager.Instance.Pause();
        
        MMSoundManagerSoundPlayEvent.Trigger(openPanelSound, MMSoundManager.MMSoundManagerTracks.Music, transform.position);
    }

    public override void ClosePanel()
    {
        PlayAnimation();
      
        SpawnerManager.Instance.Resume();
        EntityManager.Instance.Resume();
    }
    
    public void ExitLevel()
    {
        if (TransitionHelper == null)
            TransitionHelper = GetComponent<TransitionHelper>();

        PlayAnimation();
        
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
