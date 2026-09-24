using System.Collections.Generic;
using UnityEngine;

public class BlockScan : TowerScanArea
{
    [SerializeField] private float widthScanArea;
    [SerializeField] private float heightScanArea;
    [SerializeField] private Transform scannerPositon;

    private Quaternion _rotation;
    private Vector2 _sizeBox;

    private void Start()
    {
        _sizeBox = new Vector2(widthScanArea, heightScanArea);
    }

    protected override Transform PickTarget(int count)
    {
        var blockerOwner = towerOwner as BlockerTower;
        
        for (int i = 0; i < count; i++)
        {
            if (Hits[i].TryGetComponent<Entity>(out var entityData))
            {
               if (Hits[i].TryGetComponent<Trash>(out var trash) &&
                   blockerOwner != null && blockerOwner.IsAlreadyColleted(trash))
                   continue;
               
               return Hits[i].transform;
            }
        }
        
        return null;
    }

    protected override void ScanArea()
    {
        float angle = transform.eulerAngles.z;
        int count = Physics2D.OverlapBox(scannerPositon.position, _sizeBox, angle, Filter, Hits);

        CurrentTarget = count > 0 ? PickTarget(count) : null;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        _rotation = Quaternion.Euler(0, 0, towerOwner.CurrentRotation);
        
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(scannerPositon.position, _rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(widthScanArea, heightScanArea, 0f)); // hardcoded test size
    }
}
