using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

[System.Serializable]
public class DialogueDataRunTime
{
    public string dialogueName;
    public int level;
    public DialogueType dialogueType; 
    public List<DialogueSO> dialogueData = new();
    public bool isComplete;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [SerializeField] private bool dialogueStart;
    [SerializeField] private DialogueCharacterController dialogueCharacterController;
    [SerializeField] private List<DialogueDataRunTime> dialogueDataRunTimes = new();
    [SerializeField] private int currentLevel = 0;
    
    private DialogueDataRunTime _selectedDialogueDataRunTime;
    private DialogueSO _currentDialogueData;
    [SerializeField] private int _dialogueDataIndex;
    [SerializeField] private int _dialogueIndex;
    [SerializeField] private int _levelIndex;
    private bool _isOpeningTutorial;
    private Coroutine _dialogueCoroutine;
    [SerializeField] private DialogueType _currentType;
    
    [Header("Sound Effect and ambient")]
    [SerializeField] private AudioClip ambientSound;
    
    [SerializeField] private UnityEvent visualEffectIn = new();
    [SerializeField] private UnityEvent visualEffectOut = new();
    
    public bool IsDialogueRunning { get; private set; }
    public bool IsSkipDialogue { get; private set; }
    public bool IsSkipDialogueLine { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region Event System
    private void OnEnable()
    {
        GameEvents.OnStartDialogue.AddListener(StartDialogue);
    }

    private void OnDisable()
    {
        GameEvents.OnStartDialogue.RemoveListener(StartDialogue);
        
    }
    #endregion
    
    private void Start()
    {
        if (dialogueStart)
            StartDialogue(currentLevel, _currentType);
        
        MMSoundManagerSoundPlayEvent.Trigger(ambientSound, MMSoundManager.MMSoundManagerTracks.Music, transform.position);
    }

    private void Update()
    {
        if (!IsDialogueRunning) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!IsSkipDialogueLine)
            {
                // Show All Dialogue Line
                IsSkipDialogueLine = true;
                GameEvents.OnSkipDialogueLine.Invoke();
            }
            else
            {
                IsSkipDialogueLine = false;
                ContinueDialogue();
            }
            
        }
    }

    private void DisplayDialogue()
    {
        var line = _currentDialogueData.dialogueLines[_dialogueIndex];
        
        if (!string.IsNullOrEmpty(line.characterName) && dialogueCharacterController != null)
            dialogueCharacterController.ShowCharacter(line.characterName);
        
        GameEvents.OnDisplayDialogue.Invoke(line.characterName, line.dialogueLine);
    }

    private bool IsMultipleDialogue(DialogueDataRunTime dialogueDataRunTime)
    {
        return dialogueDataRunTime.dialogueData.Count > 1;
    }

    private void ChangeDialogueData()
    {
        if (dialogueDataRunTimes.Count > 0)
            _dialogueDataIndex++;

        if (dialogueCharacterController != null)
        {
            dialogueCharacterController.HideAllCharacters();
            dialogueCharacterController.ClearCharacters();
        }
        
        IsSkipDialogue = false;
        
        _currentDialogueData  = _selectedDialogueDataRunTime.dialogueData[_dialogueDataIndex];
        _dialogueIndex++;
            
        DisplayDialogue();
    }

    public void StartDialogue(int level, DialogueType type)
    {
        if (IsDialogueRunning) return;
        
        _currentType = type;
        currentLevel = level;
        
        Debug.Log($"[{name}] Current Level : {level} Current Type : {_currentType}");
        
        if (_dialogueCoroutine != null) StopCoroutine(_dialogueCoroutine);
        _dialogueCoroutine = StartCoroutine(WaitAndStarDialogue());
        
        visualEffectIn?.Invoke();
    }

    private void ContinueDialogue()
    {
        //if (!IsDialogueRunning || TransitionManager.Instance.isTrasitioning) return;
        
        _dialogueIndex++;
        IsSkipDialogueLine = false;

        if (_dialogueIndex < _currentDialogueData.dialogueLines.Count)
        {
            DisplayDialogue();
        }
        else if (IsMultipleDialogue(_selectedDialogueDataRunTime) && _dialogueDataIndex < _selectedDialogueDataRunTime.dialogueData.Count - 1)
        {
            _dialogueIndex = -1;
            ChangeDialogueData();
        }
        else
        {
            StopDialogue();
        }
    }

    public void SkipDialogue()
    {
        if (!IsDialogueRunning) return;

        if (IsSkipDialogue) return;
        
        IsSkipDialogue = true;
        
        if (IsMultipleDialogue(_selectedDialogueDataRunTime) && _dialogueDataIndex < _selectedDialogueDataRunTime.dialogueData.Count - 1)
        {
            _dialogueIndex = -1;
            ChangeDialogueData();
        }
        else
        {
            StopDialogue();
        }
    }

    public bool IsAllDoneDialogue()
    {
        bool allDoneDialogue = true;

        foreach (var dialogueData in dialogueDataRunTimes)
        {
            if (!dialogueData.isComplete)
                allDoneDialogue = false;
        }
        
        return allDoneDialogue;
    }

    private bool IsOpeningDialogueDone()
    {
        var dialogueData = dialogueDataRunTimes[0];
        return dialogueData.isComplete; 
    }
    
    public void StopDialogue()
    {
        if (!IsDialogueRunning) return;

        _selectedDialogueDataRunTime.isComplete = true;
        
        IsDialogueRunning = false;
        IsSkipDialogue = false;

        LevelStatus status = LevelProgress.Get(currentLevel);
       OnDialogueFinished();
    }
    
    private void OnDialogueFinished()
    {
        dialogueCharacterController.HideAllCharacters();
        
        visualEffectOut?.Invoke();
        GameEvents.OnHideDialogue.Invoke();

        if (_currentType == DialogueType.Before)
            TransitionManager.Instance.TransitionToScene($"Main_Level_{currentLevel}", "FadeOut");
        else
            TransitionManager.Instance.TransitionToScene("Main_SelectLevel", "FadeOut");
    }

    private IEnumerator WaitAndStarDialogue()
    {
        if (TransitionManager.Instance != null && TransitionManager.Instance.isTrasitioning)
        {
            yield return new WaitWhile(() => TransitionManager.Instance.isTrasitioning);
        }
        if (IsDialogueRunning) yield break;
        
        bool isAfter = _currentType == DialogueType.After;
        var dialogueData = dialogueDataRunTimes.Find(x => 
            x.level == currentLevel && x.dialogueType == _currentType);
        
        if (dialogueData == null)
        {
            Debug.LogWarning($"[{name} (StartDialogue)] There are no dialogue data for {currentLevel} day!");
            yield break;
        }

        if (dialogueData.isComplete)
        {
            Debug.LogWarning($"[{name} (StartDialogue)] This dialogueData is complete! {dialogueData.dialogueName}");
            OnDialogueFinished();
            yield break;
        }

        IsDialogueRunning = true;
        
        _selectedDialogueDataRunTime  = dialogueData;
        _dialogueIndex = -1;
        _dialogueDataIndex = -1;

        ChangeDialogueData();
    }
}
