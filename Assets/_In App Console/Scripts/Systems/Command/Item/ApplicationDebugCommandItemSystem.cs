using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Anonymous.Systems
{
	public class ApplicationDebugCommandItemSystem : MonoBehaviour, IPointerExitHandler
	{
		[Header("Command")]
		[SerializeField] private Button uiButton;
		[SerializeField] private TextMeshProUGUI uiText;

		[Header("Layout")]
		[SerializeField] private LayoutElement layout;
		
		private ApplicationDebugCommandSystem commandSystem;

		public void Setup(ApplicationDebugCommandSystem commandSystem)
		{
			this.commandSystem = commandSystem;
		}
		
		public void OnPointerExit(PointerEventData eventData)
		{
			uiButton.OnDeselect(eventData);
		}

		public void SetWidth(float width)
		{
			layout.preferredWidth = width;
		}

		public void SetText(string text)
		{
			uiText.text = text;
		}

		public void OnClicked()
		{
			commandSystem.UIInputCommand.text = uiText.text;
			commandSystem.UIInputCommand.ActivateInputField();
			commandSystem.UIInputCommand.MoveTextEnd(false);

			ApplicationDebugCommandSystem.SupporterActive(false);
		}
	}
}