using UnityEngine;
using UnityEngine.UI;

public class TowerRecycleProcessingUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressFillBar;
    
    public void ShowProcessingUI()
    {
        //Full
        progressFillBar.fillAmount = 1f;
        
        canvasGroup.alpha = 1;
    }

    public void UpdateProgress(float progress)
    {
        //Decreased
        progressFillBar.fillAmount = 1f - Mathf.Clamp01(progress);
    }
    
    public void HideProcessingUI()
    {
        canvasGroup.alpha = 0;
    }
}
