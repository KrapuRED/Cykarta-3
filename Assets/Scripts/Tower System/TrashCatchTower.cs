using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCatchTower : Tower, IRecycleTrash
{
    [SerializeField] private TrashState catchTrashState;
    [SerializeField] private float durationRecycle = 3f;
    
    public int MaxCapacity { get; set; }
    public int CurrentCapacity { get; set; }
    public float DurationRecycleTrash { get; set; }
    public List<Trash> AccumulatedTrashes { get; set; }
    public bool IsRecycleTrash { get; set; }

    private void Start()
    {
        MaxCapacity = TowerRunTimeData.towerCapacity;
        AccumulatedTrashes = new List<Trash>();
    }
    
    public override void OnDetectingArea(float deltaTime)
    {
        if (!IsBeenPlace || IsRecycleTrash) return;

        TowerScanArea.OnDetecting(deltaTime, out Transform target);
        
        if (target != null && target.TryGetComponent<Trash>(out Trash trash)
            && !AccumulatedTrashes.Contains(trash)
            && trash.TrashData.trashState == catchTrashState
            && IsEnoughSpace(trash.TrashData.trashWeight))
        {
            AccumulatedTrashes.Add(trash);
            trash.HoldMovement();
            CurrentCapacity += trash.TrashData.trashWeight;
            Debug.Log($"[{name}] Current Capacity: {CurrentCapacity} Max Capacity: {MaxCapacity} Accumulated Trashes: {AccumulatedTrashes.Count}");
        }
        
        if (CurrentCapacity >= MaxCapacity)
        {
            IsRecycleTrash = true;
            StartCoroutine(RecycleTrash());
        }
    }

    protected override void SetVisualDetectRange()
    {
        
    }
    
    public IEnumerator RecycleTrash()
    {
        Debug.Log($"[{name}] Start Recycle Trash");
        var trashes = new List<Trash>(AccumulatedTrashes);
        AccumulatedTrashes.Clear();
        
        yield return new WaitForSeconds(durationRecycle);

        foreach (var trash in trashes)
        {
            trash.DestroyEntity();
        }

        Debug.Log($"[{name}] Done Recycle Trash");
        CurrentCapacity = 0;
        
        yield return null;
        IsRecycleTrash = false;
    }

    public bool IsAlreadyColleted(Trash trash)
    {
        return AccumulatedTrashes.Contains(trash);
    }
    
    public bool IsEnoughSpace(int weight)
    {
        return CurrentCapacity + weight <= MaxCapacity;
    }
}
