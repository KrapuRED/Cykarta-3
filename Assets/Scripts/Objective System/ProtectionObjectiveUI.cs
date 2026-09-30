using UnityEngine;
using TMPro;

public class ProtectionObjectiveUI : ObjectiveUI<ProtectionObjectiveDataUI>
{
    [SerializeField] private TMP_Text enemyCounterText;

    protected override void Refresh(ProtectionObjectiveDataUI data)
    {
        enemyCounterText.text = $"{data.currentProtectionLevel}";
    }
}
