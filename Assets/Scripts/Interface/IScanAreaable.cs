using UnityEngine;

public interface IScanAreaable
{
    public float detectionInterval { get; set; }
    public int maxTarget { get; set; }

    public void ScanArea()
    {
        
    }
}
