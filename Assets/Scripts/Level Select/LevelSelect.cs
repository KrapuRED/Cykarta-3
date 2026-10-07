using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public void OnLevelSelected()
    {
        Debug.Log($"Level Select {name}");
    }
}
