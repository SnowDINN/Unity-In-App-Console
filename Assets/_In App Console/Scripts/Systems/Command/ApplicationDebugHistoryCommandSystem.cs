using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Anonymous.Systems
{
	public class ApplicationDebugHistoryCommandSystem : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private UnityEvent OnClick;

		public void OnPointerClick(PointerEventData eventData)
		{
			OnClick?.Invoke();
		}
	}
}