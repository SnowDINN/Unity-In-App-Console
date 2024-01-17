using System.Linq;
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
		private bool isParameter;

		public void OnPointerExit(PointerEventData eventData)
		{
			uiButton.OnDeselect(eventData);
		}

		public void Setup(ApplicationDebugCommandSystem commandSystem)
		{
			this.commandSystem = commandSystem;
		}

		public void SetSupportType(bool isParameter)
		{
			this.isParameter = isParameter;
		}

		public void SetText(string text)
		{
			uiText.text = text;
		}

		public void OnClicked()
		{
			if (isParameter)
			{
				var split = commandSystem.UIInputCommand.text.Split(" ").ToList();
				split[^1] = uiText.text;

				var command = string.Empty;
				for (var i = 0; i < split.Count; i++)
					if (i == 0)
						command += $"{split[i]}";
					else
						command += $" {split[i]}";

				commandSystem.UIInputCommand.text = command;
			}
			else
			{
				commandSystem.UIInputCommand.text = uiText.text;
			}

			commandSystem.UIInputCommand.ActivateInputField();
			commandSystem.UIInputCommand.MoveTextEnd(false);

			ApplicationDebugCommandSystem.SupporterActive(false);
		}
	}
}