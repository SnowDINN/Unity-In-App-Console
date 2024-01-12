using UnityEngine;

namespace Anonymous.Tools
{
	[ExecuteInEditMode]
	public class MobileConstraint : MonoBehaviour
	{
		private Vector2 resolution = new(Screen.width, Screen.height);

		private void Awake()
		{
			Constraint();
		}

		private void Constraint()
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

#if UNITY_EDITOR
		private void Update()
		{
			OnChangeCanvas();
		}

		private void OnChangeCanvas()
		{
			var current = new Vector2(Screen.width, Screen.height);
			if (current == resolution)
				return;

			resolution = current;
			Constraint();
		}
#endif
	}
}