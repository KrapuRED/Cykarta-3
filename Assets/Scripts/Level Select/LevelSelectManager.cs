using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilTools;
using MoreMountains.Tools;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private InputActionReference clickLevel;
    [SerializeField] private InputActionReference mousePosRef;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform containerPos;
    [SerializeField] private List<Transform> levelObjects =  new ();

    [Header("Sound Effect and ambient")]
    [SerializeField] private AudioClip ambientSound;    
    [SerializeField] private float ambientSoundVolume;    
    
    private LevelSelect _currentHovered;
    
    private void Start()
    {
        if (containerPos == null) return;

        foreach (Transform child in containerPos)
        {
            levelObjects.Add(child);
        }
        
        lineRenderer.positionCount = levelObjects.Count;
        for (int i = 0; i < levelObjects.Count; i++)
        {
            lineRenderer.SetPosition(i, levelObjects[i].position);
            levelObjects[i].GetComponent<LevelSelect>().InitializeLevelSelect();
        }
        
        MMSoundManagerSoundPlayEvent.Trigger(ambientSound, MMSoundManager.MMSoundManagerTracks.Music, transform.position, volume:ambientSoundVolume);
    }

    private void OnEnable()
    {
        clickLevel.action.Enable();
        mousePosRef.action.Enable();
        
        clickLevel.action.performed += _ => OnClickLevel();
        mousePosRef.action.performed += _ => OnMouseMove();
    }

    private void OnDisable()
    {
        clickLevel.action.performed -= _ => OnClickLevel();
        mousePosRef.action.performed -= _ => OnMouseMove();
    }

    private LevelSelect GetLevelUnderMouse()
    {
        Vector2 mousePos = UtilsClass.GetMouseWorldPosition();
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        return hit.collider != null ? hit.collider.GetComponent<LevelSelect>() : null;
    }
    
    private void OnMouseMove()
    {
        if (TransitionManager.Instance.isTrasitioning)
        {
            ClearHover();
            return;
        }

        LevelSelect hovered = GetLevelUnderMouse();

        // Still on the same level (or still on nothing), do nothing
        if (hovered == _currentHovered || hovered?.LevelStatus == LevelStatus.Lock) return;

        // Left the previous one
        if (_currentHovered != null)
            _currentHovered.OnLevelHoverOut();

        _currentHovered = hovered;

        // Entered a new one
        if (_currentHovered != null)
            _currentHovered.OnLevelHoverIn();
    }
    
    private void OnClickLevel()
    {
        if (TransitionManager.Instance.isTrasitioning) return;

        LevelSelect level = GetLevelUnderMouse();
        if (level != null)
            level.OnLevelSelected();
    }
    
    private void ClearHover()
    {
        if (_currentHovered == null) return;
        _currentHovered.OnLevelHoverOut();
        _currentHovered = null;
    }
}
