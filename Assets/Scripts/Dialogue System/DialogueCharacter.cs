using UnityEngine;
using  DG.Tweening;

public enum CharacterDialogueState
{
    Show,
    Dim,
    Hide
}

public class DialogueCharacter : MonoBehaviour
{
    [Header("Show/Hide Character")]
    [SerializeField] private float fadeDuration;
    [SerializeField] private Color hideColor;
    [SerializeField] private Color dimColor;
    [SerializeField] private Color showColor;
    [SerializeField] private bool isFliped;
    
    [Header("Move Character")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Ease moveEase = Ease.InOutQuad;
    
    [SerializeField] private Animator characterAnimator;
    
    private SpriteRenderer[] _spriteRenderers;
    private Sequence _transition;
    private Tween _moveTween;

    public bool IsInitialized {get; private set; }
    public string CharacterName { get; private set; }
    public CharacterDialogueState CharacterDialogueState;

    private void Awake()
    {
        CharacterName = gameObject.name;
        
        InitDialogueCharacter();
    }
    
    private void InitDialogueCharacter()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        if (_spriteRenderers == null || _spriteRenderers.Length == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] No SpriteRenderers found on Character or its children");
            return;
        }

        IsInitialized = true;

        FullHideCharacter();
    }

    private void PlayAnimationFade(Color targetColor)
    {
        _transition?.Kill();
        
        Sequence sequence = DOTween.Sequence();
        _transition = sequence;
        
        foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
        {
            sequence.Join(spriteRenderer.DOColor(targetColor, fadeDuration));
        }
        sequence.OnComplete(() =>
        {
            _transition = null;
        });
    }

    private void RotationCharacter(Transform newPosition)
    {
        if (!isFliped)
        {
            if (newPosition.localPosition.x > 0)
            {
                //Rotate character to left
                Quaternion newRotation = Quaternion.Euler(0, 0, 0);
                transform.rotation = newRotation;
            }
            else
            {
                //Rotate character to right
                Quaternion newRotation = Quaternion.Euler(0, 180f, 0);
                transform.rotation = newRotation;
            }
        }
        else
        {
            if (newPosition.localPosition.x > 0)
            {
                //Rotate character to left
                Quaternion newRotation = Quaternion.Euler(0, 180, 0);
                transform.rotation = newRotation;
            }
            else
            {
                //Rotate character to right
                Quaternion newRotation = Quaternion.Euler(0, 0, 0);
                transform.rotation = newRotation;
            }
        }
    }
    
    public void MovePointPosition(Transform newPosition)
    {
        if (!IsInitialized)
        {
            Debug.LogWarning($"[{name}] No SpriteRenderers found on Character");
            return;
        }
        
        if (newPosition == null)
        {
            Debug.LogWarning($"[{name}] newPosition is null!");
            return;
        }
        
        Debug.Log($"[{name}] MovePointPosition: {newPosition.position}");


        RotationCharacter(newPosition);
        
        _moveTween?.Kill();
        _moveTween = transform.DOMove(newPosition.position, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() => _moveTween = null);
    }
    
    public void ShowCharacter()
    {
        if (!IsInitialized)
        {
            Debug.LogWarning($"[{name}] No SpriteRenderers found on Character");
            return;
        }
        
        CharacterDialogueState = CharacterDialogueState.Show;
        PlayAnimationFade(showColor);
        characterAnimator.SetTrigger("InitTalk");
    }

    public void DimCharacter()
    {
        if (!IsInitialized)
        {
            Debug.LogWarning($"[{name}] No SpriteRenderers found on Character");
            return;
        }
        
        CharacterDialogueState = CharacterDialogueState.Dim;
        PlayAnimationFade(dimColor);
    }
    
    public void FullHideCharacter()
    {
        if (!IsInitialized)
        {
            Debug.LogWarning($"[{name}] No SpriteRenderers found on Character");
            return;
        }
        
        CharacterDialogueState = CharacterDialogueState.Hide;
        PlayAnimationFade(hideColor);
    }
}
