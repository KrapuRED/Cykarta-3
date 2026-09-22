using System;
using TMPro;
using UnityEngine;

public class CurrecnyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currentCurrency;

    private void OnEnable()
    {
        GameEvents.OnUpdateVisualCurrency.AddListener(UpdateCurrentCurrencyUI);
    }

    private void OnDisable()
    {
        GameEvents.OnUpdateVisualCurrency.RemoveListener(UpdateCurrentCurrencyUI);
    }

    private void UpdateCurrentCurrencyUI(int currency)
    {
        currentCurrency.text = $"${currency}";
    }
}
