using UnityEngine;

namespace Anonymous.Tools
{
    public class ApplicationDebugSafeArea : MonoBehaviour
    {
        private void Awake()
        {
            var rectTransform = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;

            var minAnchor = safeArea.position;
            var maxAnchor = minAnchor + safeArea.size;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}