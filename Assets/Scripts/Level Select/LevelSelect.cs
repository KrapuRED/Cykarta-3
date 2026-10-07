using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private string sceneName;
    
    public void OnLevelSelected()
    {
        Debug.Log($"Level Select {name}");
        string scene = $"{sceneName}_{level}";
        
        TransitionManager.Instance.TransitionToScene(scene, "FadeOut");
    }
}
