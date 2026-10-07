using System;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private int prevLevel;
    [SerializeField] private string sceneName;
    [SerializeField] private bool starterLevel = false;
    
    private SpriteRenderer _spriteRenderer;

    private LevelStatus _levelStatus;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private bool IsUnlocked()
    {
        if (starterLevel) return true;

        LevelStatus prev = LevelProgress.Get(prevLevel);
        return prev == LevelStatus.Success || prev == LevelStatus.Failed;
    }
    
    public void InitializeLevelSelect()
    {
        _levelStatus = IsUnlocked() ? LevelProgress.Get(level) : LevelStatus.Lock;

        Color c = _levelStatus switch
        {
            LevelStatus.Success     => Color.green,
            LevelStatus.Failed      => Color.red,
            LevelStatus.NotPlayed   => Color.yellow,
            LevelStatus.Lock        => Color.darkSlateGray,
            _                       => Color.white
        };
        
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        
        Debug.Log($"LevelSelect {name} Level Status {_levelStatus}");
        _spriteRenderer.color = c;
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
