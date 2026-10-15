using UnityEngine;
using DG.Tweening;

public abstract class VisualEffect : MonoBehaviour
{
    public Tween Tween { get; protected set; }
    
    public abstract void PlayAnimation();
    public abstract void RevertAnimation();
    public abstract void StopAnimation();
}
