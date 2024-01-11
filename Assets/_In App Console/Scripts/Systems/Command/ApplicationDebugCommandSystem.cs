using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Anonymous.Systems
{
	public enum SupportCommandType
	{
		Command,
		Parameter
	}

	public class Command
	{
		private readonly List<List<string>> parameters = new();
		private Action<string> onExecute;
		public string Comment { get; private set; }

		public Command AddComment(string comment)
		{
			Comment = comment;
			return this;
		}

		public Command AddExecute(Action<string> onExecute)
		{
			this.onExecute = onExecute;
			return this;
		}
		
		public Command AddParameters(List<string> parameters)
		{
			this.parameters.Add(parameters);
			return this;
		}

		public void Execute(string command)
		{
			onExecute.Invoke(command);
		}

		public List<string> GetParameters(int index)
		{
			var parameterIndex = index - 2;
			return parameters[parameterIndex];
		}

		public bool canPrameter(int index)
		{
			var parameterIndex = index - 2;
			return parameterIndex >= 0 && parameters.Count > parameterIndex;
		}
	}

	public class ApplicationDebugCommandSystem : MonoBehaviour, IApplicationDebugSystems
	{
		private static readonly Dictionary<string, Command> commands = new(StringComparer.OrdinalIgnoreCase);
		private static readonly List<string> histories = new();

		private static int historyIndex = -1;

		private static GameObject supporterObject;

		[Header("Command Input")]
		public TMP_InputField UIInputCommand;

		[Header("Command Support")]
		[SerializeField] private GameObject CommandPrefabs;

		[SerializeField] private Transform CommandContents;

		[Header("Command Support Object")]
		[SerializeField] private GameObject CommandSupporterObject;

		private readonly char[] chr =
		{
			'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ',
			'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
		};

		private readonly int[] chrint =
		{
			44032, 44620, 45208, 45796, 46384, 46972, 47560, 48148, 48736, 49324, 49912,
			50500, 51088, 51676, 52264, 52852, 53440, 54028, 54616, 55204
		};

		private readonly string[] str =
		{
			"가", "까", "나", "다", "따", "라", "마", "바", "빠", "사", "싸",
			"아", "자", "짜", "차", "카", "타", "파", "하"
		};

		private float supporterHeight
		{
			get
			{
				var canvasRect = (RectTransform)GetComponentInParent<Canvas>().transform;
				return canvasRect.sizeDelta.y * 0.45f;
			}
		}

		private void Awake()
		{
			supporterObject = CommandSupporterObject;

			var rect = (RectTransform)supporterObject.transform;
			rect.sizeDelta = new Vector2(rect.sizeDelta.x, supporterHeight);
		}

		public void Setup()
		{
			AddCommand("Help").AddExecute(_ =>
			{
				foreach (var command in commands
					         .Where(command => !string.IsNullOrEmpty(command.Value.Comment)))
					Debug.Log(
						$"<color=orange>{command.Key}</color><br><color=white>{command.Value.Comment}<br></color>");
			});

			UIInputCommand.onSubmit.AddListener(value =>
			{
				var split = value.Split(' ');
				if (commands.ContainsKey(split[0]))
				{
					Debug.Log($"<color=#ffa500ff>[Command : {value}]</color>");
					commands[split[0]].Execute(value);
				}

				AddHistory(value);
				UIInputCommand.text = string.Empty;
			});

			UIInputCommand.onValueChanged.AddListener(command =>
			{
				UIInputCommand.text = Regex.Replace(UIInputCommand.text, @"\s+", " ");

				if (string.IsNullOrEmpty(command))
				{
					foreach (Transform transform in CommandContents)
						Destroy(transform.gameObject);
					SupporterActive(false);
					return;
				}

				if (commands == null || commands.Count == 0)
				{
					foreach (Transform transform in CommandContents)
						Destroy(transform.gameObject);
					SupporterActive(false);
					return;
				}

				var split = command.Split(" ");
				if (split.Length == 1)
				{
					var matched = RecommandCommand(split[0], commands.Keys.ToList());

					var useRecommand = true;
					foreach (var match in matched)
						if (match == split[0])
							useRecommand = false;

					if (useRecommand)
						AddSupporterCommand(SupportCommandType.Command, matched, matched.Any());
					else
						SupporterActive(false);
				}
				else if (commands[split[0]].canPrameter(split.Length))
				{
					var recommand = commands[split[0]].GetParameters(split.Length);
					var matched = RecommandCommand(split[^1], recommand);

					var useRecommand = true;
					foreach (var match in matched)
						if (match == split[^1])
							useRecommand = false;

					if (useRecommand)
						AddSupporterCommand(SupportCommandType.Parameter, matched, matched.Any());
					else
						SupporterActive(false);
				}
				else
				{
					SupporterActive(false);
				}
			});

			StartCoroutine(nameof(DetectingInputAsync));
			StartCoroutine(nameof(DetectingCloseAsync));
		}

		public void Dispose()
		{
			historyIndex = -1;
			StopCoroutine(nameof(DetectingInputAsync));
			StopCoroutine(nameof(DetectingCloseAsync));
		}

		public void OpenHistory()
		{
			if (!string.IsNullOrEmpty(UIInputCommand.text))
				return;

			if (supporterObject.activeSelf)
				return;

			if (histories.Count <= 0)
				return;

			AddSupporterCommand(SupportCommandType.Command, histories, true);
		}

		public static Command AddCommand(string key)
		{
			commands[key] = new Command();
			return commands[key];
		}

		public static void AddHistory(string command)
		{
			if (!string.IsNullOrEmpty(command))
				histories.Add(command);

			historyIndex = -1;
		}

		public static void RemoveCommand(string command)
		{
			if (commands.ContainsKey(command))
				commands.Remove(command);
		}

		public static void SupporterActive(bool isActive)
		{
			supporterObject.SetActive(isActive);
		}

		private void AddSupporterCommand(SupportCommandType type, IEnumerable<string> commands, bool isActive)
		{
			foreach (Transform transform in CommandContents)
				Destroy(transform.gameObject);

			var rect = ((RectTransform)CommandContents.parent).rect;
			foreach (var command in commands)
			{
				var prefab = Instantiate(CommandPrefabs, CommandContents);
				var item = prefab.GetComponent<ApplicationDebugCommandItemSystem>();
				item.Setup(this);
				item.SetText(command);
				item.SetWidth(rect.width);
				item.useParameter(type == SupportCommandType.Parameter);
			}

			SupporterActive(isActive);
		}

		private IEnumerable<string> RecommandCommand(string command, List<string> recommand)
		{
			var pattern = "";
			foreach (var search in command)
				switch (search)
				{
					case >= 'ㄱ' and <= 'ㅎ':
					{
						for (var j = 0; j < chr.Length; j++)
							if (search == chr[j])
								pattern += $"[{str[j]}-{(char)(chrint[j + 1] - 1)}]";

						break;
					}
					case >= '가':
					{
						var magic = (search - '가') % 588;
						if (magic == 0)
						{
							pattern += $"[{search}-{(char)(search + 27)}]";
						}
						else
						{
							magic = 27 - magic % 28;
							pattern += $"[{search}-{(char)(search + magic)}]";
						}

						break;
					}
					case >= 'A' and <= 'z':
					case >= '0' and <= '9':
						pattern += search;
						break;
				}

			recommand.Sort();
			return recommand.Where(key => Regex.IsMatch(key, pattern, RegexOptions.IgnoreCase));
		}

		private IEnumerator DetectingInputAsync()
		{
			while (true)
			{
				if (UIInputCommand.isFocused)
				{
					if (Input.GetKeyDown(KeyCode.UpArrow))
					{
						if (historyIndex <= 0)
							historyIndex = histories.Count;

						UIInputCommand.text = histories[historyIndex -= 1];
						UIInputCommand.caretPosition = UIInputCommand.text.Length;

						UIInputCommand.MoveTextEnd(false);
					}

					if (Input.GetKeyDown(KeyCode.DownArrow))
					{
						if (historyIndex >= histories.Count - 1)
							historyIndex = -1;

						UIInputCommand.text = histories[historyIndex += 1];
						UIInputCommand.caretPosition = UIInputCommand.text.Length;

						UIInputCommand.MoveTextEnd(false);
					}
				}

				yield return null;
			}
		}

		private IEnumerator DetectingCloseAsync()
		{
			while (true)
			{
#if UNITY_EDITOR
				if (Input.GetMouseButtonDown(0))
				{
					var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
					{
						position = Input.mousePosition
					};
					var results = new List<RaycastResult>();
					EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

					var isTarget = false;
					foreach (var dummy in results.Where(result =>
						         result.gameObject.GetInstanceID() == supporterObject.GetInstanceID() ||
						         result.gameObject.GetInstanceID() == UIInputCommand.gameObject.GetInstanceID()))
						isTarget = true;

					if (!isTarget)
					{
						historyIndex = -1;
						SupporterActive(false);
					}
				}
#endif

				if (Input.touchCount > 0)
				{
					var touch = Input.GetTouch(0);
					var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
					{
						position = touch.position
					};
					var results = new List<RaycastResult>();
					EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

					var isTarget = false;
					foreach (var dummy in results.Where(result =>
						         result.gameObject.GetInstanceID() == supporterObject.GetInstanceID() ||
						         result.gameObject.GetInstanceID() == UIInputCommand.gameObject.GetInstanceID()))
						isTarget = true;

					if (!isTarget)
					{
						historyIndex = -1;
						SupporterActive(false);
					}
				}

				yield return null;
			}
		}
	}
}