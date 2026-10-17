using System;
using UnityEngine;
using MoreMountains.Tools;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int startCurrency;
    [SerializeField] private int passiveEarningCurrency;
    [SerializeField] protected float earningInterval = 1f;
    private  float _earningIntervalTimer;
    
    [Header("Sound Effect and ambient")]
    [SerializeField] private AudioClip gainCurrencySound;  
    [SerializeField] private AudioClip lossCurrencySound;
    [SerializeField] private float volumeCurrencySound;
    
    [SerializeField] private VisualEffect slideEffect;
    
    
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
        slideEffect.PlayAnimation();
    }

    private void Update()
    {
        PassiveEarning(Time.deltaTime);
    }

    private void PassiveEarning(float deltaTime)
    {
        if (earningInterval <= 0f) return;
  
        _earningIntervalTimer += deltaTime;
        if (_earningIntervalTimer < earningInterval)
        {
            return;
        }

        _earningIntervalTimer -= earningInterval;
        ReceiveCurrency(passiveEarningCurrency);
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
