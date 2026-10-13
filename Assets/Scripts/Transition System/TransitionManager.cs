using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    [Header("Scene Transition References")]
    [SerializeField] private Transform sceneTransitionContainer;
    [SerializeField] private List<Transition> transitions = new();
    
    [Header("Music")]
    [SerializeField, Range(0f, 1f)] private float loweredMusicVolume = 0.2f;
    [SerializeField] private float musicFadeDuration = 0.5f;
    
    public  bool isTrasitioning { get; private set;}
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        transitions.Clear();
        transitions = sceneTransitionContainer.GetComponentsInChildren<Transition>(true).ToList();
     
        LevelProgress.ResetAll(3);
        
        DontDestroyOnLoad(gameObject);
    }

    public void TransitionToScene(string sceneName, string transitionName)
    {
        if (isTrasitioning) return;
        isTrasitioning = true;
        
        string transition = $"Transition - {transitionName}";
        
        StartCoroutine(LoadSceneAsync(sceneName, transition));
    }
    
    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        Transition transition = transitions.First(t => t.name == transitionName);

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        FadeMusic(loweredMusicVolume);
        yield return transition.TransitionIn();
        //Reduce MMSoundManager Music
        
        do
        {
            //progressBar.value = scene.progress;
            yield return null;
        } while (scene.progress < 0.9f);

        yield return new WaitForSeconds(1f);

        scene.allowSceneActivation = true;
        
        yield return new WaitUntil(() => scene.isDone);
        
        FadeMusic(1f);
        yield return transition.TransitionOut();
        
        isTrasitioning = false;
        
        GameEvents.OnGameStart?.Invoke();
        
    }

    public void TransitionToStoryScene(int level, DialogueType dialogueType, string transitionName)
    {
        if (isTrasitioning) return;
        isTrasitioning = true;
        
        string transition = $"Transition - {transitionName}";
        
        StartCoroutine(LoadStorySceneAsync("Test_Story", level, dialogueType, transition));
    }
    
    private IEnumerator LoadStorySceneAsync(string sceneName, int level, DialogueType dialogueType, string transitionName)
    {
        Transition transition = transitions.First(t => t.name == transitionName);

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        FadeMusic(loweredMusicVolume);
        
        yield return transition.TransitionIn();
        
        do
        {
            //progressBar.value = scene.progress;
            yield return null;
        } while (scene.progress < 0.9f);

        yield return new WaitForSeconds(1f);

        scene.allowSceneActivation = true;
        
        yield return new WaitUntil(() => scene.isDone);
        
        FadeMusic(1f);
        yield return transition.TransitionOut();
        
        isTrasitioning = false;
        
        yield return new WaitUntil(() => DialogueManager.Instance != null);

        GameEvents.OnStartDialogue.Invoke(level, dialogueType);
        
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
