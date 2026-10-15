using UnityEngine;
using DG.Tweening;

public class VisualEffectFade : VisualEffect
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease = Ease.OutBack;
    
    public override void PlayAnimation()
    {
        Tween?.Kill();
        Tween = canvasGroup.DOFade(1, duration)
                .SetEase(ease)
                .OnComplete(() => SetCanvasGroup(true));;
        
    }

    public override void RevertAnimation()
    {
        Tween?.Kill();
        Tween = canvasGroup.DOFade(1, duration)
                .SetEase(ease)
                .OnComplete(() => SetCanvasGroup(false));
    }

    public override void StopAnimation()
    {
        Tween?.Kill();
    }
    
    private void SetCanvasGroup(bool visible)
    {
        if (canvasGroup == null) return;
        
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }
}
