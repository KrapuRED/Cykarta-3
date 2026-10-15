using UnityEngine;
using DG.Tweening;

public class VisualScaleButton : VisualEffect
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetScale = Vector3.one * 1.2f;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private Ease ease = Ease.OutBack;

    private Vector3 _originalScale;
    private Tween _tween;

    private void Awake()
    {
        if (target == null) target = transform;
        _originalScale = target.localScale;
    }

    public override void PlayAnimation()
    {
        _tween?.Kill();
        _tween = target.DOScale(targetScale, duration).SetEase(ease);
    }

    public override void RevertAnimation()
    {
        _tween?.Kill();
        _tween = target.DOScale(_originalScale, duration).SetEase(ease);
    }

    public override void StopAnimation()
    {
        _tween?.Kill();
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }
}
