using UnityEngine;

public class TransitionHelper : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private string transitionName;

    public void DoTransition() => TransitionManager.Instance.TransitionToScene(sceneName, transitionName);
    
    public void DoTransitionDirect(string _sceneName) =>  TransitionManager.Instance.TransitionToScene(_sceneName, transitionName);
}
