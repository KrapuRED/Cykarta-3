using UnityEngine;
using TMPro;

public class WaveCounterObjectiveUI : ObjectiveUI<WaveCounterObjectiveDataUI>
{
    [SerializeField] private TMP_Text enemyCounterText;

    protected override void Refresh(WaveCounterObjectiveDataUI data)
    {
        enemyCounterText.text = $"WAVE {data.currentWaveCounter} / {data.maxWaveCounter}";
    }
}
