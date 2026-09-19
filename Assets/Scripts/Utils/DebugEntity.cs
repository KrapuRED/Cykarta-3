using TMPro;
using UnityEngine;
using UtilTools;

public class DebugEntity : MonoBehaviour
{
    public TMP_Text entityListText;

    private string _allEntityList;

    private void OnEnable()
    {
        GameEvents.OnShowDebugEntity.AddListener(UpdateDebugEntity);
    }

    private void OnDisable()
    {
        GameEvents.OnShowDebugEntity.RemoveListener(UpdateDebugEntity);
    }

    private void UpdateDebugEntity(string entityName, string entityID, string spawnerID)
    {
        string newEntity = $"{entityName}:{entityID}:{spawnerID}.";
        _allEntityList = UtilsClass.AddStringBelow(_allEntityList, newEntity);
        
        if (entityListText != null)
        {
            entityListText.text = _allEntityList;
        }
    }
}
