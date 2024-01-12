using System;
using System.Collections;
using UnityEngine;

namespace Anonymous.Pooling.Scroller
{
	public class StretchConstraint : MonoBehaviour
	{
		[Flags]
		public enum StretchType
		{
			Horizontal = 2,
			Vertial = 4
		}

		[SerializeField] private StretchType type;

		private void Awake()
		{
			StartCoroutine(YieldEndFrame());
		}

		private IEnumerator YieldEndFrame()
		{
			yield return new WaitForEndOfFrame();

			var rect = ((RectTransform)transform.parent).rect;

			var size = ((RectTransform)transform).sizeDelta;
			if (type.HasFlag(StretchType.Horizontal))
				((RectTransform)transform).sizeDelta = new Vector2(rect.width, size.y);

			if (type.HasFlag(StretchType.Vertial))
				((RectTransform)transform).sizeDelta = new Vector2(size.x, rect.height);
		}
	}
}