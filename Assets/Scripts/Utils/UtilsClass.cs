using TMPro;
using UnityEngine;
using System.Collections.Generic;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace UtilTools
{
    public static class UtilsClass
    {
        #region  CreateWorldText
        public static TextMeshPro CreateWorldText(string text, Transform parent = null,
            Vector3 localPosition = default(Vector3), int fontSize = 24, Color color = default(Color),
            TextAnchor textAnchor = TextAnchor.MiddleCenter, TextAlignmentOptions  textAlignment = TextAlignmentOptions.Midline,
            int sortingOrder = 0, Vector2 boxSize = default(Vector2), string customeName = "World_Text")
        {
            if (color.Equals(default(Color))) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, color, textAnchor, textAlignment, sortingOrder, boxSize, customeName);
        }
    
        public static TextMeshPro CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color,  TextAnchor textAnchor, TextAlignmentOptions textAlignment, int sortingOrder, Vector2 boxSize, string customeName)
        {
            string nameGameObject = string.IsNullOrEmpty(customeName) ? "World_Text" : customeName;
            GameObject gameObject = new GameObject(nameGameObject, typeof(TextMeshPro));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            
            TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();
            textMeshPro.alignment = textAlignment;
            textMeshPro.fontSize = fontSize;
            textMeshPro.text = text;
            textMeshPro.color = color;
            textMeshPro.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            if (!boxSize.Equals(default(Vector2)))
            {
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = boxSize;
                textMeshPro.overflowMode = TextOverflowModes.Truncate;
            }
            
            return textMeshPro;
        }
        #endregion
        
        #region String Manipulation

        public static string AddStringBelow(string originalString, string newString)
        {
            if (string.IsNullOrEmpty(originalString))
            {
                return newString;
            }
            return $"{originalString}\n{newString}";
        }
        
        public static List<string> ConvertStringToList(string str)
        {
            return null;
        }
        
        #endregion
        
        //Get Mouse Position in World with Z = 0f
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 worldPosition = GetMouseWorldPositionWithZ(Camera.main);
            worldPosition.z = 0f;
            return worldPosition;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            Vector3 screenPosition = GetMouseScreenPosition();
            return GetMouseWorldPositionWithZ(screenPosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            return worldCamera.ScreenToWorldPoint(screenPosition);
        }

        // Reads the raw mouse screen position, whichever input system is active
        private static Vector3 GetMouseScreenPosition()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current == null) return Vector3.zero;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            return new Vector3(mousePos.x, mousePos.y, 0f);
#else
            return Input.mousePosition;
#endif
        }
    }
}

