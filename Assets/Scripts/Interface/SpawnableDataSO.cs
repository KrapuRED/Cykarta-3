using UnityEngine;

public abstract  class SpawnableDataSO : ScriptableObject
{
    public string displayName;
    public float entitySpeed;
    public GameObject entityPrefab;
}
