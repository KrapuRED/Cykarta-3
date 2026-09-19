using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ReceiveCurrency(int currency)
    {
        
    }

    public bool UseCurrency(int currency)
    {
        return false;
    }
}
