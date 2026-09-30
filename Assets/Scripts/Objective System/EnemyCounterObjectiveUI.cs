using System;
using TMPro;
using UnityEngine;

public class EnemyCounterObjectiveUI : ObjectiveUI<EnemyCounterObjectiveDataUI>
{
    [SerializeField] private TMP_Text enemyCounterText;
    
    protected override void Refresh(EnemyCounterObjectiveDataUI data)
    {
        enemyCounterText.text = $"{data.currentEnemyCounter}/{data.maxEnemyCounter}";
    }
}
