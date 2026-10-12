using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLineData
{
    public string characterName;
    public string dialogueLine;
}

[CreateAssetMenu(fileName = "DIalogueSO", menuName = "Scriptable Objects/DIalogueSO")]
public class DialogueSO : ScriptableObject
{
    public string locationDialogue;
    public List<DialogueLineData> dialogueLines = new ();
}
