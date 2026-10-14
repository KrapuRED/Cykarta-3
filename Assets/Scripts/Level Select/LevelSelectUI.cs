using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text levelDescText;
    [SerializeField] private Image backgroundImage;
    
    private LevelStatus _levelStatus;
    private TransitionHelper _transitionHelper;
    private CanvasGroup _canvasGroup;
    private string _sceneName;
    private int _level;
    
    private void OnEnable()
    {
        GameEvents.OnSelectingLevel.AddListener(SetLevelSelectedUI);
    }

    private void OnDisable()
    {
        GameEvents.OnSelectingLevel.RemoveListener(SetLevelSelectedUI);
        
    }

    private void SetLevelSelectedUI(int level, string levelName, string sceneName,  LevelStatus levelStatus)
    {
        _level =  level;
        _levelStatus = levelStatus;
        levelText.text = levelName;
        _sceneName = sceneName;

        if (_levelStatus != LevelStatus.Lock)
        {
            backgroundImage.color = Color.green;
        }
        else
        {
            backgroundImage.color = Color.darkGray;
        }
        
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }
    
    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _transitionHelper = GetComponent<TransitionHelper>();
    }

    public void EnterLevel()
    {
        if (_levelStatus == LevelStatus.Lock) return;
        
        _transitionHelper.DoTransitionDirect(_sceneName);
    }
    
    public void EnterStory()
    {
        if (_levelStatus == LevelStatus.Lock) return;
        
        _transitionHelper.DoTransitionDirectToStory(_level, DialogueType.Before);
    }
}
