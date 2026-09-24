using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRecycleTrash
{
    public int MaxCapacity { get; set; }
    public int CurrentCapacity { get; set; }
    public float DurationRecycleTrash { get; set; }
    public List<Trash> AccumulatedTrashes { get; set; }
    public bool IsRecycleTrash { get; set; }
    
    public IEnumerator RecycleTrash();
}
