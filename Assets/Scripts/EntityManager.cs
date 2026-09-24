using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityRunTimeData
{
    public string entityName;
    public string entityID; 
    public Entity entity;
    public IrresponsibleThinkingBaseData irresponsibleThinkingBaseData;
    public EntityState entityState;
    public float stateTimer;
    public float cooldown;
}

[System.Serializable]
public class IrresponsibleThinkingBaseData
{
    public float chanceIrresponsibleThinking;
    public float chanceTimeIrresponsibleThinking;
    public float maxIrresponsibleThinkingMeter; 
    public float irresponsibleThinkingIncreaseRate;
}

[System.Serializable]
public enum EntityState
{
    Moving,
    IrresponsibleThinking
}

[System.Serializable]
public class IrresponsibleThinkingData 
{
    public float chanceIrresponsibleThinking;
    public float chanceTimeIrresponsibleThinking;
    public float maxIrresponsibleThinkingMeter;
    public float currentIrresponsibleThinkingMeter;
    public float irresponsibleThinkingIncreaseRate;
    public bool isIrresponsibleThinking;
}

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance {get; private set;}
    
    public bool paused = false;
    
    private readonly List<Entity> _entities = new();
    private readonly List<Entity> _pendingRemove = new();
    
    private Dictionary<string, EntityRunTimeData> _activeEntityRunTimeData = new();
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

    private void Update()
    {
        if (paused) return;
        
        if (_pendingRemove.Count > 0)
        {
            foreach (var e in _pendingRemove) _entities.Remove(e);
            _pendingRemove.Clear();
        }

        if (_entities.Count > 0)
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < _entities.Count; i++)
                _entities[i].OnEntityUpdate(deltaTime);
        }
    }

    public EntityRunTimeData GetEntityRunTimeData(string entityName, string spawnerID , IrresponsibleThinkingBaseData baseData = null ,Entity entity = null)
    {
        string entityID = string.Empty;
        int indexEntity = 0;

        if (_indexActiveEntityRunTimeData.TryGetValue(entityName, out indexEntity))
        {
            indexEntity++;
        }

        if (string.IsNullOrEmpty(spawnerID))
        {
            entityID = $"{entityName}_{indexEntity}";
        }
        else
        {
            entityID = $"{spawnerID}_{entityName}_{indexEntity}";
        }
        
        EntityRunTimeData entityRunTimeData = new EntityRunTimeData
        {
            entityName = entityName,
            entityID = entityID,
            irresponsibleThinkingBaseData = baseData,
            entity =  entity
        };
        
        _indexActiveEntityRunTimeData[entityName] = indexEntity;
        _activeEntityRunTimeData[entityID] = entityRunTimeData;
        
        GameEvents.OnShowDebugEntity.Invoke(entityName, entityID, spawnerID);
        
        return entityRunTimeData;
    }
    
    public void RegisterEntity(Entity e)
    {
        if (e == null || _entities.Contains(e)) return;
        Debug.LogWarning($"[{name} (RegisterEntity)] Success registering entity {e.name}");
        _entities.Add(e);
    }

    public void UnregisterEntity(Entity e)
    {
        if (e == null || _pendingRemove.Contains(e)) return;
        
       _pendingRemove.Add(e);
        
        if (e.EntityRunTimeData != null)
            _activeEntityRunTimeData.Remove(e.EntityRunTimeData.entityID);
    }
}
