using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int startCurrency;
    
    public int CurrentCurrency { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ReceiveCurrency(startCurrency);
    }

    public void ReceiveCurrency(int currency)
    {
        CurrentCurrency += currency;
        GameEvents.OnUpdateVisualCurrency.Invoke(CurrentCurrency);
    }

    public void UseCurrency(int costObject)
    {
        CurrentCurrency -= costObject;
        GameEvents.OnUpdateVisualCurrency.Invoke(CurrentCurrency);
    }

    public bool IsCurrentCurrencyEnough(int cost)
    {
        return CurrentCurrency >= cost;
    }
}
