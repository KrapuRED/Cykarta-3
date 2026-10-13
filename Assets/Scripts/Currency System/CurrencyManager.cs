using System;
using UnityEngine;
using MoreMountains.Tools;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int startCurrency;
    
    [Header("Sound Effect and ambient")]
    [SerializeField] private AudioClip gainCurrencySound;  
    [SerializeField] private AudioClip lossCurrencySound;
    [SerializeField] private float volumeCurrencySound;
    
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
        MMSoundManagerSoundPlayEvent.Trigger(gainCurrencySound, MMSoundManager.MMSoundManagerTracks.Music, transform.position, volume:volumeCurrencySound);
        
        GameEvents.OnUpdateVisualCurrency.Invoke(CurrentCurrency);
    }

    public void UseCurrency(int costObject)
    {
        CurrentCurrency -= costObject;
        MMSoundManagerSoundPlayEvent.Trigger(lossCurrencySound, MMSoundManager.MMSoundManagerTracks.Music, transform.position, volume:volumeCurrencySound);
        
        GameEvents.OnUpdateVisualCurrency.Invoke(CurrentCurrency);
    }

    public bool IsCurrentCurrencyEnough(int cost)
    {
        return CurrentCurrency >= cost;
    }
}
