using UnityEngine;
using DG.Tweening;

public class VisualScaleButton : VisualEffect
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetScale = Vector3.one * 1.2f;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private Ease ease = Ease.OutBack;

    private Vector3 _originalScale;

    private void Awake()
    {
        if (target == null) target = transform;
        _originalScale = target.localScale;
    }

    public override void PlayAnimation()
    {
        Tween?.Kill();
        Tween = target.DOScale(targetScale, duration).SetEase(ease);
    }

    public override void RevertAnimation()
    {
        Tween?.Kill();
        Tween = target.DOScale(_originalScale, duration).SetEase(ease);
    }

    public override void StopAnimation()
    {
        Tween?.Kill();
    }

    private void OnDestroy()
    {
        Tween?.Kill();
    }
}
