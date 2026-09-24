using UnityEngine;

public abstract  class SpawnableDataSO : ScriptableObject
{
    public string displayName;
    public float maxEntitySpeed;
    public float minEntitySpeed;
    public IrresponsibleThinkingBaseData irresponsibleThinkingBaseData;
    public GameObject entityPrefab;
}
