using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    [Header("Scene Transition References")]
    [SerializeField] private Transform sceneTransitionContainer;
    [SerializeField] private List<Transition> transitions = new();
    
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

        yield return transition.TransitionIn();
        
        do
        {
            //progressBar.value = scene.progress;
            yield return null;
        } while (scene.progress < 0.9f);

        yield return new WaitForSeconds(1f);

        scene.allowSceneActivation = true;
        
        yield return new WaitUntil(() => scene.isDone);
        
        yield return transition.TransitionOut();
        isTrasitioning = false;
        
        GameEvents.OnGameStart?.Invoke();
        
    }

}
