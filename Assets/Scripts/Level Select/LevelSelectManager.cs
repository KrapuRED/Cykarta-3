using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilTools;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private InputActionReference clickLevel;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform containerPos;
    [SerializeField] private List<Transform> levelObjects =  new ();

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
    }

    private void OnEnable()
    {
        clickLevel.action.Enable();
        clickLevel.action.performed += _ => OnClickLevel();
    }

    private void OnDisable()
    {
        clickLevel.action.performed -= _ => OnClickLevel();
    }
    
    private void OnClickLevel()
    {
        if (TransitionManager.Instance.isTrasitioning) return;
        
        Vector2 mousePos = UtilTools.UtilsClass.GetMouseWorldPosition();
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        
        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;
            LevelSelect levelSelect = clickedObject.GetComponent<LevelSelect>();
            
            Debug.Log("Clicked on 2D Object: " + clickedObject.name);
            levelSelect.OnLevelSelected();
        }
    }
}
