using System;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set;}

    [SerializeField] private int levelIndex;
    
    [SerializeField] private int objectiveHealthPoints;
    [SerializeField] private List<WaveData> waveDataList = new();
    
    [Header("References")]
    [SerializeField] private ObjectiveUI enemyCounter;
    [SerializeField] private ObjectiveUI proctectionLevel;
    [SerializeField] private ObjectiveUI waveCounter;
    
    [Header("Sound Effect and ambient")]
    [SerializeField] private AudioClip ambientSound;
    [SerializeField] private AudioClip bgmSound;
    [SerializeField] private AudioClip objectiveHurtSound;
    [SerializeField] private float volumeAmbientSound;
    
    public int CurrentObjectiveHealthPoints { get; private set; }
    public int LevelIndex => levelIndex;
    private int _waveIndex = -1;
    private int _maxEnemyCounter;
    private int _currentEnemyCounter;
    private int _soundID;
    
    [SerializeField] private bool startLevel;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        _soundID = GetInstanceID();
        
        Instance = this;
    }

    #region  Event System

    private void OnEnable()
    {
        GameEvents.OnGameStart.AddListener(StartLevel);
    }
    
    private void OnDisable()
    {
        GameEvents.OnGameStart.RemoveListener(StartLevel);
        
        StopAmbient();
    }

    #endregion

    private void Start()
    {
        if (startLevel)
            StartLevel();
    }

    #region Objective Sterter and Next Wave
    
    private void StartLevel()
    {
        CurrentObjectiveHealthPoints = objectiveHealthPoints;
        
        proctectionLevel.UpdateObjectiveUI(new ProtectionObjectiveDataUI
        {
            currentProtectionLevel = objectiveHealthPoints
        });
        
        StartAmbient();
        NextWave();
    }

    public void NextWave()
    {
        _waveIndex++;
        if (_waveIndex >= waveDataList.Count)
        {
            Debug.LogWarning("No wave data found");
            GameEvents.OnRequestOpenPanel.Invoke(PanelType.Victory);
            return;
        }
        
        var currentWaveData = waveDataList[_waveIndex];
        var spawnData = currentWaveData.waveSpawnerData;
        _currentEnemyCounter = 0;
        
        List<SpawnerData> data =  new List<SpawnerData>();
        
        foreach (var ws in spawnData)
        {
            data.AddRange(ws.spawnerData);
        }

        SpawnerManager.Instance.StartSpawn(currentWaveData, _waveIndex);
        
        _maxEnemyCounter = CountTotalEnemy(data);
        enemyCounter.UpdateObjectiveUI(new EnemyCounterObjectiveDataUI
        {
            currentEnemyCounter = 0,
            maxEnemyCounter = _maxEnemyCounter
        });
        
        waveCounter.UpdateObjectiveUI(new WaveCounterObjectiveDataUI
        {
            currentWaveCounter = _waveIndex + 1,
            maxWaveCounter = waveDataList.Count
        });

        Debug.Log($"Wave Index: {_waveIndex} {currentWaveData.waveDataName} total Enemy : {_maxEnemyCounter}");
    }
    
    #endregion

    #region Objective UI

    private int CountTotalEnemy(List<SpawnerData> spawnerData)
    {
        if (spawnerData.Count <= 0)
        {
            Debug.LogWarning("No spawner data found");
            return -1;
        }
        
        int totalEnemies = 0;
        foreach (var data in spawnerData)
        {
            if (data == null) continue;
            totalEnemies += data.maxSpawnCount;
        }
        
        return totalEnemies;
    }

    public void UpdateObjectiveEnemyCounter()
    {
        Debug.LogWarning($"{name} Updating Enemy Counter!");
        
        _currentEnemyCounter++;
        
        enemyCounter.UpdateObjectiveUI(new EnemyCounterObjectiveDataUI
        {
            currentEnemyCounter = _currentEnemyCounter,
            maxEnemyCounter = _maxEnemyCounter
        });

        if (_currentEnemyCounter >= _maxEnemyCounter)
        {
            CurrencyManager.Instance.ReceiveCurrency(waveDataList[_waveIndex].waveReward);
            SpawnerManager.Instance.CheckAllIrresponsibleSpawnerIsDone();
        }
    }

    public void UpdateObjectiveProctectionLevel(int weight)
    {
        CurrentObjectiveHealthPoints -= weight;
        MMSoundManagerSoundPlayEvent.Trigger(objectiveHurtSound, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
        
        proctectionLevel.UpdateObjectiveUI(new ProtectionObjectiveDataUI
        {
            currentProtectionLevel = CurrentObjectiveHealthPoints
        });
        
        if (CurrentObjectiveHealthPoints <= 0)
        {
            GameEvents.OnRequestOpenPanel.Invoke(PanelType.Defeat);
        }
        
    }
    
    #endregion

    private LevelStatus GetLevelStatus()
    {
        if (objectiveHealthPoints <= 0)
            return LevelStatus.Failed;
        else if (objectiveHealthPoints <= 0 && _waveIndex <= waveDataList.Count)
            return LevelStatus.Failed;
        else
            return LevelStatus.Success;
        
        return LevelStatus.NotPlayed;
    }
    
    #region Sound

    private void StartAmbient()
    {
        MMSoundManagerSoundPlayEvent.Trigger(
            ambientSound,
            MMSoundManager.MMSoundManagerTracks.Music,
            transform.position,
            loop: true,
            volume: volumeAmbientSound,
            ID: _soundID);
    }

    private void StopAmbient()
    {
        MMSoundManagerSoundControlEvent.Trigger(
            MMSoundManagerSoundControlEventTypes.Stop,
            _soundID);
    }

    #endregion
    
    public void SaveData() => LevelProgress.Set(levelIndex, GetLevelStatus());
    
}
