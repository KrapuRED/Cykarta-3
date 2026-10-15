using UnityEngine;
using DG.Tweening;

public enum SlideType
{
    X,
    Y,
    Z
}

public class VisualSlide : VisualEffect
{
    [SerializeField] private Transform target;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private SlideType slideType;
    [SerializeField] private float slideOffset = 1f;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private Ease ease = Ease.OutBack;
    [SerializeField] private bool useLocalSpace = true;
    [SerializeField] private bool ignoreTimeScale = false;

    [SerializeField] private float _originalPosition;
    private Tween _tween;
    
    private void Awake()
    {
        if (target == null) target = transform;
        _originalPosition = GetAxisValue();
    }
    
    public override void PlayAnimation()
    {
        _tween?.Kill();
     
        SetCanvasGroup(visible: true);

        _tween = SlideTo(slideOffset)
            .SetEase(ease)
            .SetUpdate(ignoreTimeScale)
            .SetLink(gameObject);
    }

    public override void RevertAnimation()
    {
        _tween?.Kill();
        _tween = SlideTo(_originalPosition)
            .SetEase(ease)
            .SetUpdate(ignoreTimeScale)
            .SetLink(gameObject)
            .OnComplete(() => SetCanvasGroup(false));
        
    }

    public override void StopAnimation()
    {
        _tween?.Kill();
    }
    
    private void SetCanvasGroup(bool visible)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }
    
    private Tween SlideTo(float value)
    {
        return slideType switch
        {
            SlideType.X => useLocalSpace ? target.DOLocalMoveX(value, duration) : target.DOMoveX(value, duration),
            SlideType.Y => useLocalSpace ? target.DOLocalMoveY(value, duration) : target.DOMoveY(value, duration),
            _           => useLocalSpace ? target.DOLocalMoveZ(value, duration) : target.DOMoveZ(value, duration),
        };
    }
    
    private float GetAxisValue()
    {
        Vector3 p = useLocalSpace ? target.localPosition : target.position;
        return slideType switch
        {
            SlideType.X => p.x,
            SlideType.Y => p.y,
            _           => p.z,
        };
    }
}
