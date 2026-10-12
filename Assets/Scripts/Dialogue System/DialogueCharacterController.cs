using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public enum CharacterPosition
{
    Left,
    Right
}

[System.Serializable]
public class PointPosition
{
    public string positonName;
    public CharacterPosition positionType;
    public Transform positon;
}

public class DialogueCharacterController : MonoBehaviour
{
    [Header("Character Settings")]
    [SerializeField] private Transform characterContainer;
    [SerializeField] private List<DialogueCharacter> characterControllers = new List<DialogueCharacter>();
    
    [Header("Point Positions")]
    [SerializeField] private Transform pointPositionContainer;
    [SerializeField] private List<PointPosition> pointPositions = new List<PointPosition>();
    
    private DialogueCharacter _currentDialogueCharacter;
    private int MAX_CHARACTERS = 2;
    private int _currentTurn = 0;
    
    private class ActiveCharacterData 
    {
        public PointPosition pointData;
        public int lastUsedTurn;
    }
    private Dictionary<DialogueCharacter, ActiveCharacterData> _activeCharacters = new ();
    
    
    private void Awake()
    {
        characterControllers.Clear();
        characterControllers = characterContainer.GetComponentsInChildren<DialogueCharacter>().ToList();
        
        Init();
    }

    private void Init()
    {
        pointPositions.Clear();
        
        foreach (Transform point in pointPositionContainer.transform)
        {
            bool isRight = point.localPosition.x > 1;
            PointPosition newPositon = new PointPosition
            {
                positonName = point.name,
                positionType = isRight ? CharacterPosition.Right : CharacterPosition.Left,
                positon = point.transform
            };
            
            pointPositions.Add(newPositon);
        }
    }
    
    private PointPosition GetCharacterPositon()
    {
        // Find The Point Position that empty in _activeCharacters
        foreach (var point in pointPositions)
        {
            bool isOccupied = _activeCharacters.Values.Any(x => x.pointData == point);
            if (!isOccupied)
                return point;
        }
        
        return null;
    }
    
    private void LeastRecentUseCharacter(DialogueCharacter newCharacter)
    {
        // 1) Find the Least Recent Use Character on the list
        var lruEntry = _activeCharacters.OrderBy(kvp => kvp.Value.lastUsedTurn).First();
        DialogueCharacter lruCharacter = lruEntry.Key;
        PointPosition recycledPoint = lruEntry.Value.pointData;
        
        // 2) Remove Character and Reset indicator from list
        lruCharacter.FullHideCharacter();
        _activeCharacters.Remove(lruCharacter);
        
        // 3) Add The New Character to list 
        ActiveCharacterData newData = new ActiveCharacterData
        {
            pointData = recycledPoint,
            lastUsedTurn = _currentTurn
        };
        _activeCharacters.Add(newCharacter, newData);
        
        // 4) Move Character to position
        // 5) Show Character
        newCharacter.MovePointPosition(recycledPoint.positon);
        newCharacter.ShowCharacter();
    }

    public void ShowCharacter(string characterName)
    {
        _currentTurn++;
        string formattedName = $"Character - {characterName}";

        var targetCharacter = characterControllers.Find(x => x.CharacterName == formattedName);
        if (targetCharacter == null)
        {
            Debug.LogError($"[{name} (SwapCharacter)] Failed to Swap Character {characterName} because it doesn't exist");
            return;
        }
        Debug.Log($"[{name} (SwapCharacter)] Swap Character {targetCharacter.CharacterName}");

        if (!_activeCharacters.ContainsKey(targetCharacter))
        {
            if (_activeCharacters.Count < MAX_CHARACTERS)
            {
                PointPosition emptyPoint = GetCharacterPositon();
                if (emptyPoint == null)
                {
                    Debug.Log($"[{name} (ShowCharacter)] There's no empty point at this time!");
                    return;
                }
                
                ActiveCharacterData newData = new ActiveCharacterData
                {
                    pointData = emptyPoint,
                    lastUsedTurn = _currentTurn
                };
                
                _activeCharacters.Add(targetCharacter, newData);
                targetCharacter.MovePointPosition(emptyPoint.positon);
                targetCharacter.ShowCharacter();
            }
            else
            {
                LeastRecentUseCharacter(targetCharacter);
            }
        }
        else
        {
            _activeCharacters[targetCharacter].lastUsedTurn = _currentTurn;
        }
        
        _currentDialogueCharacter = targetCharacter;
        _currentDialogueCharacter.ShowCharacter();
        
        foreach (var kvp in _activeCharacters)
        {
            if (kvp.Key != _currentDialogueCharacter)
                kvp.Key.DimCharacter();
        }
    }

    public void HideAllCharacters()
    {
        foreach (var kvp in _activeCharacters)
        {
            if (kvp.Key == null)
                continue;
            
            kvp.Key.FullHideCharacter();
        }
    }

    public void ClearCharacters()
    {
        _activeCharacters.Clear();
    }
}
