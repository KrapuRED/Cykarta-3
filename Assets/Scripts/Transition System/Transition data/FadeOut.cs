using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FadeOut : Transition
{
    [SerializeField] private float transitionDuration;
    [SerializeField] private CanvasGroup  canvasGroup;
    
    public override IEnumerator TransitionIn()
    {
        var tweener  = canvasGroup.DOFade(1,transitionDuration);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        
        yield return tweener.WaitForCompletion();
    }

    public override IEnumerator TransitionOut()
    {
        var tweener  = canvasGroup.DOFade(0,transitionDuration);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        
        yield return tweener.WaitForCompletion();
    }
}
