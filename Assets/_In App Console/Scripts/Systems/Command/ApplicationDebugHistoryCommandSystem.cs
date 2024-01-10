using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Anonymous.Systems
{
	public class ApplicationDebugHistoryCommandSystem : MonoBehaviour, IPointerClickHandler, IDeselectHandler
	{
		[SerializeField] private UnityEvent OnSelectClick;
		[Space(10)]
		[SerializeField] private UnityEvent OnDeselectClick;

		public void OnPointerClick(PointerEventData eventData)
		{
			OnSelectClick?.Invoke();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			OnDeselectClick?.Invoke();
		}
	}
}