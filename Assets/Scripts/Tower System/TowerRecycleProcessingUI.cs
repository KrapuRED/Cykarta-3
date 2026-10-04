using UnityEngine;
using UnityEngine.UI;

public class TowerRecycleProcessingUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider progressBar;
    
    private float _currentProgress;
    
    public void ShowProcessingUI(float progress)
    {
        progressBar.maxValue = progress;
        progressBar.value = progress;
        _currentProgress = progress;
        
        canvasGroup.alpha = 1;
    }

    public void UpdateProgress(float deltaTime)
    {
        float progress = _currentProgress - deltaTime;
        progressBar.value = progress;
    }
    
    public void HideProcessingUI()
    {
        
    }
}
