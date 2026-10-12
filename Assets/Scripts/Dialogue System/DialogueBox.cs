using System;
using TMPro;
using UnityEngine;
using Febucci.UI;
using Febucci.UI.Core;
using MoreMountains.Tools;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private TMP_Text detailCharacterText;
    [SerializeField] private TMP_Text dialogueBoxText;
    
    [SerializeField] private AudioClip dialogueBlip;
    
    [SerializeField] private TypewriterCore typewriter;
    TextAnimatorSettings _settings;
    
    private void Awake()
    {
        UnityEngine.Assertions.Assert.IsNotNull(typewriter, $"Text Animator Player component is null in {gameObject.name}");
        _settings = TextAnimatorSettings.Instance;
        UnityEngine.Assertions.Assert.IsNotNull(_settings, $"Text Animator Settings is null.");
    }
    
    private void OnEnable()
    {
        GameEvents.OnDisplayDialogue.AddListener(UpdateDialogueUI);
        GameEvents.OnSkipDialogueLine.AddListener(SkipDialogueLine);
        
        typewriter.onCharacterVisible.AddListener(PlayTypeSound);
    }

    private void OnDisable()
    {
        GameEvents.OnDisplayDialogue.RemoveListener(UpdateDialogueUI);
        GameEvents.OnSkipDialogueLine.RemoveListener(SkipDialogueLine);
    }

    private void PlayTypeSound(Char character)
    {
        if (DialogueManager.Instance.IsSkipDialogue) return;
        
        if (Char.IsWhiteSpace(character))
            return;

        if (dialogueBlip != null)
            MMSoundManagerSoundPlayEvent.Trigger(dialogueBlip, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
    }
    
    private void UpdateDialogueUI(string charName, string line)
    {
        if (detailCharacterText != null)
            detailCharacterText.text = charName;
        
        if (DialogueManager.Instance.IsSkipDialogueLine) return;
        
        typewriter.ShowText(line);
    }

    private void SkipDialogueLine() => typewriter.SkipTypewriter();
}

