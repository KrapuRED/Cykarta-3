using UnityEngine;

[CreateAssetMenu(fileName = "TrashDataSO", menuName = "Scriptable Objects/TrashDataSO")]
public class TrashDataSO : SpawnableDataSO
{
    public TrashType trashType;
    public int trashWeight;
}
