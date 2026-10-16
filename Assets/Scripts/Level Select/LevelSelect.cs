using UnityEngine;
using TMPro;
using MoreMountains.Tools;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private int prevLevel;
    [SerializeField] private string sceneName;
    [SerializeField] private bool starterLevel = false;
    [SerializeField] private string levelName;
    
    [Header("Visual")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private SpriteRenderer frameProgress;
    [SerializeField] private SpriteRenderer iconProgress;
    [SerializeField] private Sprite defaultFrameProgress;
    [SerializeField] private Sprite completedFrameProgress;
    [SerializeField] private Sprite defaultIconProgress;
    [SerializeField] private Sprite lockIconProgress;
    [SerializeField] private Sprite unlockIconProgress;
    [SerializeField] private Sprite completedIconProgress;
    [SerializeField] private VisualEffect effect;
    private LevelStatus _levelStatus;

    [Header("Sounds")]
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
        
        bool unlocked = IsUnlocked();
        LevelStatus raw = LevelProgress.Get(level);
        Debug.Log($"[{name}] level={level} prevLevel={prevLevel} starter={starterLevel} unlocked={unlocked} raw={raw}");

        _levelStatus = unlocked ? raw : LevelStatus.Lock;

        if (_levelStatus == LevelStatus.Success)
            frameProgress.sprite = completedFrameProgress;
        else
            frameProgress.sprite = defaultFrameProgress;
        
        Sprite iconProgressSprite = _levelStatus switch
        {
            LevelStatus.Success     => completedIconProgress,
            LevelStatus.Failed      => unlockIconProgress,
            LevelStatus.NotPlayed   => unlockIconProgress,
            LevelStatus.Lock        => lockIconProgress,
            _                       => defaultIconProgress
        };
        
        Debug.Log($"LevelSelect {name} Level Status {_levelStatus}");
        iconProgress.sprite = iconProgressSprite;
    }
    
    public void OnLevelSelected()
    {
        if (_levelStatus == LevelStatus.Lock) return;
        
        Debug.Log($"Level Select {name}");
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
