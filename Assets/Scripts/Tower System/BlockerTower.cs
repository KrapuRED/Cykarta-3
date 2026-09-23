using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockerTower : Tower
{
    [SerializeField] private int maxCapacity;
    [SerializeField] private List<Trash> accumulatedTrashes = new ();
    [SerializeField] private TrashState catchTrashState;
    
    private int _currentCapacity;

    private void Start()
    {
        maxCapacity =  TowerRunTimeData.towerCapacity;
    }
    
    public override void OnDetectingArea(float deltaTime)
    {
        if (!IsBeenPlace)
        {
            Debug.LogWarning($"[{name}] OnDetectingArea Is Been Place {IsBeenPlace}");
            return;
        }
        
        if (_currentCapacity > maxCapacity)
            return;
        
        TowerScanArea.OnDetecting(deltaTime, out Transform targetGetBlock);
        if (_currentCapacity <= maxCapacity &&  targetGetBlock != null)
        {
            targetGetBlock.TryGetComponent<Trash>(out var trash);
            if (trash != null && !accumulatedTrashes.Contains(trash) && trash.TrashData.trashState == catchTrashState)
            {
                if (_currentCapacity + trash.TrashData.trashWeight <= maxCapacity)
                {
                    accumulatedTrashes.Add(trash);
                    trash.HoldMovement();
                    _currentCapacity += trash.TrashData.trashWeight;
                }
            }
        }
    }

    protected override void SetVisualDetectRange()
    {
        
    }

    protected override void ReleaseObject()
    {
        foreach (var trash in accumulatedTrashes)
        {
            if (trash == null)
                continue;
            
            trash.UnholdMovement();
        }
    }

    public bool IsAlreadyColleted(Trash trash)
    {
        return accumulatedTrashes.Contains(trash);
    }
}
