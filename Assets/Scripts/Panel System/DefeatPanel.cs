using UnityEngine;
using MoreMountains.Tools;

public class DefeatPanel : Panel
{
    [Header("Sound Effect and ambient")]
    [SerializeField] private AudioClip openPanelSound; 
    
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
        
        MMSoundManagerSoundPlayEvent.Trigger(openPanelSound, MMSoundManager.MMSoundManagerTracks.Music, transform.position);
    }

    public override void ClosePanel()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
      
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
}
