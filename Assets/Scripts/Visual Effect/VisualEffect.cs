using UnityEngine;

public abstract class VisualEffect : MonoBehaviour
{
    public abstract void PlayAnimation();
    public abstract void RevertAnimation();
    public abstract void StopAnimation();
}
