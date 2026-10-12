using UnityEngine;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private int prevLevel;
    [SerializeField] private string sceneName;
    [SerializeField] private bool starterLevel = false;
    
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private LevelStatus _levelStatus;

    private bool IsUnlocked()
    {
        if (starterLevel) return true;

        LevelStatus prev = LevelProgress.Get(prevLevel);
        return prev == LevelStatus.Success;
    }
    
    public void InitializeLevelSelect()
    {
        levelText.text = $"Level {level}";
        
        _levelStatus = IsUnlocked() ? LevelProgress.Get(level) : LevelStatus.Lock;

        Color c = _levelStatus switch
        {
            LevelStatus.Success     => Color.green,
            LevelStatus.Failed      => Color.red,
            LevelStatus.NotPlayed   => Color.yellow,
            LevelStatus.Lock        => Color.darkSlateGray,
            _                       => Color.white
        };
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        Debug.Log($"LevelSelect {name} Level Status {_levelStatus}");
        spriteRenderer.color = c;
    }
    
    public void OnLevelSelected()
    {
        if (_levelStatus == LevelStatus.Lock) return;
        
        Debug.Log($"Level Select {name}");
        string levelName = $"Level {level}"; 
        string scene = $"{sceneName}_{level}";
        
        //TransitionManager.Instance.TransitionToScene(scene, "FadeOut");
        GameEvents.OnSelectingLevel.Invoke(levelName,scene, _levelStatus);
    }
}
