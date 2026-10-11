using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPreviewUpgradeUI : MonoBehaviour
{
    private enum UpgradeValueState { Improved, Same, Worse }

    [SerializeField] private CanvasGroup  canvasGroup;
    [SerializeField] private TMP_Text  towerNameText;
    [SerializeField] private TMP_Text  towerLevelText;
    [SerializeField] private TMP_Text  previewUpgradeText;
    [SerializeField] private Image towerIcon;
    
    [Header("Change Colors")]
    [SerializeField] private Color improvedColor = Color.green;
    [SerializeField] private Color sameColor = Color.black;
    [SerializeField] private Color worseColor = Color.red;
    
    public void ShowPreview(TowerRunTimeData currentData, UpgradeTowerData nextUpgrade, Sprite iconTower)
    {
        Debug.Log($"[{name}] ShowPreview");
        towerNameText.text = currentData.towerName;
        towerLevelText.text = $"level {currentData.towerLevel + 1}";
        towerIcon.sprite = iconTower;
        
        string bs = string.Empty;
        
        foreach (var s in  nextUpgrade.upgradeStatuses)
        {
            float currentValue = currentData.GetCurrentStatusValue(s.status);
            float nextValue = currentValue + s.valueStatus;
            
            UpgradeValueState state = UpgradeValueChanges(currentValue, nextValue, s.isLowerValueBetter);
            string colorHex = ColorUtility.ToHtmlStringRGB(GetColorForState(state));
            
            Debug.Log($"{s.status} : {s.valueStatus} {state}");
            bs += $"{s.status} : <color=#{colorHex}>{nextValue:0.##}</color>\n";
        }

        if (string.IsNullOrEmpty(bs))
            Debug.LogWarning($"[{name}] ShowPreview Failed");
        
        previewUpgradeText.text = bs;
        
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        
    }

    public void HidePreview()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private UpgradeValueState UpgradeValueChanges(float prevValue, float upgradeValue, bool isLowerValueBetter)
    {
        if (Mathf.Approximately(prevValue, upgradeValue))
            return UpgradeValueState.Same;
 
        bool wentUp = upgradeValue > prevValue;
        bool isImprovement = isLowerValueBetter ? !wentUp : wentUp;
 
        return isImprovement ? UpgradeValueState.Improved : UpgradeValueState.Same;
    }
    
    private Color GetColorForState(UpgradeValueState state)
    {
        switch (state)
        {
            case UpgradeValueState.Improved: return improvedColor;
            case UpgradeValueState.Worse: return worseColor;
            default: return sameColor;
        }
    }
}
