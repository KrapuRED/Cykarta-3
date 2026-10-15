using UnityEngine;
using TMPro;
using MoreMountains.Tools;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private int prevLevel;
    [SerializeField] private string sceneName;
    [SerializeField] private bool starterLevel = false;
    
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private VisualEffect effect;
    [SerializeField] private LevelStatus _levelStatus;

    [SerializeField] private AudioClip hoverInSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip hoverOutSound;
    [SerializeField] private float volumeSound;
    
    public LevelStatus LevelStatus => _levelStatus;
    
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
        MMSoundManagerSoundPlayEvent.Trigger(clickSound, MMSoundManager.MMSoundManagerTracks.Music, transform.position, volume:volumeSound);
        
        GameEvents.OnSelectingLevel.Invoke(level, levelName,scene, _levelStatus);
    }

    public void OnLevelHoverIn()
    {
        effect.PlayAnimation();
        
        Debug.Log($"LevelSelect {name} On Level Hover In!");
    }
    
    public void OnLevelHoverOut()
    {
        effect.RevertAnimation();
    }
}
