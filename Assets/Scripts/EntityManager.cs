using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityRunTimeData
{
    public string entityName;
    public string entityID; 
    public float currentIrresponsibleThinkingMeter;
    public float irresponsibleThinkingIncreaseRate;
    public Entity entity;
}

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance {get; private set;}

    private Dictionary<string, EntityRunTimeData> _indexEntityRunTimeData = new();
    private Dictionary<string, int> _indexActiveEntityRunTimeData = new();
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public EntityRunTimeData GetEntityRunTimeData(string entityName, string spawnerID, Entity entity = null)
    {
        string entityID = string.Empty;
        int indexEntity = 0;

        if (_indexActiveEntityRunTimeData.TryGetValue(entityName, out indexEntity))
        {
            indexEntity++;
        }

        if (string.IsNullOrEmpty(entityID))
        {
            entityID =$"{entityName}_{indexEntity}";
        }
        else
        {
            entityID =$"{spawnerID}_{entityName}_{indexEntity}";
        }
        
        EntityRunTimeData entityRunTimeData = new EntityRunTimeData
        {
            entityName = entityName,
            entityID = entityID,
            currentIrresponsibleThinkingMeter = 0,
            irresponsibleThinkingIncreaseRate = 0
        };
        
        _indexActiveEntityRunTimeData[entityName] = indexEntity;
        _indexEntityRunTimeData[entityName] = entityRunTimeData;
        
        return entityRunTimeData;
    }
    
    public void RegisterEntity(Entity entityData)
    {
        
    }

    public void UnregisterEntity(Entity entityData)
    {
        
    }
}
