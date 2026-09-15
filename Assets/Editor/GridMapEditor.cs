using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridMap))]
public class GridMapEditor : Editor
{
   private static readonly Dictionary<GridZone, Color> ZoneColors = new Dictionary<GridZone, Color>
   {
       { GridZone.None, new Color(1f, 1f, 1f, 0f) },
      { GridZone.Build, Color.green },
      { GridZone.Pedestrian, new Color(1f, 0.55f, 0f) }, // Color.orange doesn't exist in Unity's Color struct
      { GridZone.Vehicle, Color.red },
      { GridZone.Water, Color.blue }
   };
   
   private GridZone _brushZone = GridZone.Pedestrian;
   private int _selectedMapIndex = 0;
   private bool _paintingEnabledGrid = true;

   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();
      
      GridMap map = (GridMap)target;
      List<GridMapData> mapData = map.GetGridMapDataList();
      
      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Zone Painter", EditorStyles.boldLabel);
      _paintingEnabledGrid = EditorGUILayout.Toggle("Painting Enabled", _paintingEnabledGrid);

      if (mapData == null || mapData.Count == 0)
      {
         EditorGUILayout.HelpBox("Add at least one Grid Map Data entry above to start painting.", MessageType.Info);
         return;
      }
      
      string[] names = new string[mapData.Count];
      for (int i = 0; i < mapData.Count; i++)
      {
         names[i] = string.IsNullOrEmpty(mapData[i].nameGridMap) ? $"Map {i}" : mapData[i].nameGridMap;
      }
      
      _selectedMapIndex = Mathf.Clamp(_selectedMapIndex, 0, mapData.Count - 1);
      _selectedMapIndex = EditorGUILayout.Popup("Active Map",_selectedMapIndex, names);
      
      EditorGUILayout.LabelField("Brush Zone");
      EditorGUILayout.BeginHorizontal();
      foreach (GridZone zone in System.Enum.GetValues(typeof(GridZone)))
      {
         Color prevColor = GUI.backgroundColor;
         if (ZoneColors.TryGetValue(zone, out Color color)) GUI.backgroundColor = color;
         if (GUILayout.Toggle(_brushZone == zone, zone.ToString(), "Button")) 
            _brushZone = zone;
         
         GUI.backgroundColor = prevColor;
      }
      EditorGUILayout.EndHorizontal();
      
      
      EditorGUILayout.HelpBox(
         "Click / drag in the Scene view to paint the selected zone. Hold Shift to erase (set to None).",
         MessageType.Info);
 
      if (mapData[_selectedMapIndex].origin == null)
         EditorGUILayout.HelpBox("This map has no Origin transform assigned yet — painting is disabled until it does.", MessageType.Warning);
 
      if (GUI.changed) SceneView.RepaintAll();
   }
   
   private void OnSceneGUI()
   {
      if (!_paintingEnabledGrid) return;
 
      GridMap gridMap = (GridMap)target;
      List<GridMapData> maps = gridMap.GetGridMapDataList();
      if (maps == null || _selectedMapIndex >= maps.Count) return;
 
      GridMapData mapData = maps[_selectedMapIndex];
      if (mapData.origin == null || mapData.widthCell <= 0 || mapData.heightCell <= 0) return;
 
      mapData.EnsureArraySize();
      DrawGridCells(mapData);
      HandlePaintInput(mapData);
 
      // Block default scene tools (move/rotate) from stealing our clicks while painting.
      HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
   }
   
    private void DrawGridCells(GridMapData mapData)
    {
        Vector3 origin = mapData.origin.position;
        float size = mapData.cellSize;
 
        for (int x = 0; x < mapData.widthCell; x++)
        {
            for (int y = 0; y < mapData.heightCell; y++)
            {
                GridZone zone = mapData.GetZone(x, y);
                Color fill = ZoneColors.TryGetValue(zone, out Color c) ? c : Color.clear;
 
                Vector3 bl = origin + new Vector3(x, y) * size;
                Vector3 br = bl + new Vector3(size, 0, 0);
                Vector3 tr = bl + new Vector3(size, size, 0);
                Vector3 tl = bl + new Vector3(0, size, 0);
 
                Handles.DrawSolidRectangleWithOutline(new[] { bl, br, tr, tl }, fill, new Color(1, 1, 1, 0.25f));
            }
        }
    }
 
    private void HandlePaintInput(GridMapData mapData)
    {
        Event e = Event.current;
        bool isPaintEvent = (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0;
        if (!isPaintEvent) return;
 
        // Grid sits on the XY plane at the map's origin Z (matches Grid<T>.GetWorldPosition).
        Plane plane = new Plane(Vector3.forward, mapData.origin.position);
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
 
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hit = ray.GetPoint(distance);
            Vector3 local = hit - mapData.origin.position;
            int x = Mathf.FloorToInt(local.x / mapData.cellSize);
            int y = Mathf.FloorToInt(local.y / mapData.cellSize);
 
            if (x >= 0 && y >= 0 && x < mapData.widthCell && y < mapData.heightCell)
            {
                Undo.RecordObject(target, "Paint Grid Zone");
                GridZone zoneToApply = e.shift ? GridZone.None : _brushZone;
                mapData.SetZone(x, y, zoneToApply);
                EditorUtility.SetDirty(target);
                HandleUtility.Repaint();
            }
            e.Use();
        }
    }
}