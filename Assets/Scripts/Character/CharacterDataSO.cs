using UnityEngine;

[System.Serializable]
public enum CharacterType
{
    None,
    Common,
    Delinquent
}

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Character Data/CharacterDataSO")]
public class CharacterDataSO : SpawnableDataSO
{
    [Header("Character Data")]
    public CharacterType characterType;
}
