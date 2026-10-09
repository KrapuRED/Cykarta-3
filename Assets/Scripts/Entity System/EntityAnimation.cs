using UnityEngine;

public class EntityAnimation : MonoBehaviour
{
    [SerializeField] private Transform pivotTransform;
    [SerializeField] private Animator animator;

    private Vector2Int _currentDir = Vector2Int.zero;
    private Quaternion _targetRotation = Quaternion.identity;
    
    public void PlayWalkAnimation()
    {
        animator.SetBool("IsWalking", true);
    }

    public void PlayIdleAnimation()
    {
        
    }


    public void PlayThrowAnimation()
    {
        animator.SetTrigger("Throw");
    }

    public void StopWalkAnimation()
    {
        
    }

    public void RotateAnimation(Vector3 fromPosition, Vector3 nextPoint, float deltaTime)
    {
        Vector2 delta = nextPoint - fromPosition;
        
        if (delta.sqrMagnitude < 0.0001f) return;
        
        Vector2Int dir;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            dir = delta.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            dir = delta.y > 0 ? Vector2Int.up : Vector2Int.down;
        
        if (dir == _currentDir) return;
        _currentDir = dir;

        float angle = 0f;
        if (dir == Vector2Int.right) angle = -90f;
        else if (dir == Vector2Int.left) angle = 90f;
        else if (dir == Vector2Int.up) angle = 0f;
        else if (dir == Vector2Int.down) angle = 180f;
        
        Debug.Log($"[{transform.parent.parent.name}] angle = {angle}");
        
        _targetRotation = Quaternion.Euler(0, 0, angle);
        
        pivotTransform.rotation = _targetRotation;
    }
}
