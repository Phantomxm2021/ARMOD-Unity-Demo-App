using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityARMODApp.Runtime
{
    /// <summary>
    /// Adapt to straight bangs.
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        RectTransform panel;
        Rect lastSafeArea = new Rect(0, 0, 0, 0);

        void Awake()
        {
            panel = GetComponent<RectTransform>();
            Refresh();
        }

        void Update()
        {
            Refresh();
        }

        void Refresh()
        {
            Rect tmp_SafeArea = GetSafeArea();

            if (tmp_SafeArea != lastSafeArea)
                ApplySafeArea(tmp_SafeArea);
        }

        Rect GetSafeArea()
        {
            return Screen.safeArea;
        }

        void ApplySafeArea(Rect _rect)
        {
            lastSafeArea = _rect;

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 tmp_AnchorMin = _rect.position;
            Vector2 tmp_AnchorMax = _rect.position + _rect.size;
            tmp_AnchorMin.x /= Screen.width;
            tmp_AnchorMin.y /= Screen.height;
            tmp_AnchorMax.x /= Screen.width;
            tmp_AnchorMax.y /= Screen.height;
            panel.anchorMin = tmp_AnchorMin;
            panel.anchorMax = tmp_AnchorMax;
#if UNITY_EDITOR
            Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, _rect.x, _rect.y, _rect.width, _rect.height, Screen.width, Screen.height);
#endif
        }
    }
}