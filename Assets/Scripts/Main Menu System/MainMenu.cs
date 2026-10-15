using UnityEngine;
using MoreMountains.Tools;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip mainMenuSound;
    [SerializeField] private float volumeMainMenuSound;

    [Header("Music")]
    [SerializeField, Range(0f, 1f)] private float loweredMusicVolume = 0.2f;
    [SerializeField] private float musicFadeDuration = 0.5f;
    [SerializeField] private TransitionHelper helper1;
    [SerializeField] private TransitionHelper helper2;
    
    private int _soundID;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _soundID = GetInstanceID();
        
        MMSoundManagerSoundPlayEvent.Trigger(
            mainMenuSound,
            MMSoundManager.MMSoundManagerTracks.Sfx,
            transform.position,
            loop: true,
            volume: volumeMainMenuSound,
            ID: _soundID);
    }

    public void OnPlayButton()
    {
        helper1.DoTransition();
    }

    public void OnCreditsButton()
    {
        helper2.DoTransition();
    }
    
    public void OnQuitButton()
    {
        Application.Quit();
    }
    
    private void FadeMusic( float targetVolume)
    {
        Debug.Log($"[{name}] FadeInMusic");
        
        MMSoundManagerTrackFadeEvent.Trigger(
            MMSoundManagerTrackFadeEvent.Modes.PlayFade,
            MMSoundManager.MMSoundManagerTracks.Music,
            musicFadeDuration,
            targetVolume,
            new MMTweenType(MMTween.MMTweenCurve.EaseInOutQuartic));
        
        MMSoundManagerTrackFadeEvent.Trigger(
            MMSoundManagerTrackFadeEvent.Modes.PlayFade,
            MMSoundManager.MMSoundManagerTracks.Other,
            musicFadeDuration,
            targetVolume,
            new MMTweenType(MMTween.MMTweenCurve.EaseInOutQuartic));
    }
}
