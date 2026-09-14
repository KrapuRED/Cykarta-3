using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public string waveDataName;
    public int waveDelay;
    public List<SpawnerData> waveSpawnerDatas = new();
}

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager Instance  { get; private set; }
    [SerializeField] private List<WaveData> waveDataList = new();
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    
}
