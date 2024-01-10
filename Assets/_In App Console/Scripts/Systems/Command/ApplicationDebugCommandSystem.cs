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
	public class ApplicationDebugCommandSystem : MonoBehaviour, IApplicationDebugSystems
	{
		private static readonly Dictionary<string, Action<string>> command = new(StringComparer.OrdinalIgnoreCase);
		private static readonly Dictionary<string, string> comment = new();
		private static readonly List<string> history = new();

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

		private void Awake()
		{
			supporterObject = CommandSupporterObject;
		}

		public void Setup()
		{
			AddCommand("Help", _ =>
			{
				foreach (var command in comment)
					Debug.Log($"<color=orange>{command.Key}</color><br><color=white>{command.Value}<br></color>");
			});

			UIInputCommand.onSubmit.AddListener(value =>
			{
				var split = value.Split(' ');
				if (command.ContainsKey(split[0]))
				{
					Debug.Log($"<color=#ffa500ff>[Command : {value}]</color>");
					command[split[0]].Invoke(value);
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

				if (ApplicationDebugCommandSystem.command == null || ApplicationDebugCommandSystem.command.Count == 0)
				{
					foreach (Transform transform in CommandContents)
						Destroy(transform.gameObject);
					SupporterActive(false);
					return;
				}

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

				var keys = ApplicationDebugCommandSystem.command.Keys.ToList();
				keys.Sort();

				var matched = keys.Where(key => Regex.IsMatch(key, pattern, RegexOptions.IgnoreCase));
				AddSupporterCommand(matched, matched.Any());
			});

			StartCoroutine(nameof(DetectingInputAsync));
		}

		public void Dispose()
		{
			historyIndex = -1;
			StopCoroutine(nameof(DetectingInputAsync));
		}

		public void OnSelectHistory()
		{
			if (!string.IsNullOrEmpty(UIInputCommand.text))
				return;

			if (supporterObject.activeSelf)
				return;

			if (history.Count <= 0)
				return;

			AddSupporterCommand(history, true);
		}

		public void OnDeselectHistory()
		{
			var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
			{
				position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
			};
			var results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

			var isTarget = false;
			foreach (var dummy in results.Where(result =>
				         result.gameObject.GetInstanceID() == supporterObject.GetInstanceID()))
				isTarget = true;

			if (!isTarget)
			{
				historyIndex = -1;
				SupporterActive(false);
			}
		}

		public static void AddCommand(string command, Action<string> action)
		{
			ApplicationDebugCommandSystem.command[command] = action;
		}

		public static void AddComment(string command, string comment)
		{
			ApplicationDebugCommandSystem.comment[command] = comment;
		}

		public static void AddHistory(string command)
		{
			if (!string.IsNullOrEmpty(command))
				history.Add(command);

			historyIndex = -1;
		}

		public static void RemoveCommand(string command)
		{
			if (ApplicationDebugCommandSystem.command.ContainsKey(command))
				ApplicationDebugCommandSystem.command.Remove(command);
		}

		public static void SupporterActive(bool isActive)
		{
			supporterObject.SetActive(isActive);
		}

		private void AddSupporterCommand(IEnumerable<string> commands, bool isActive)
		{
			foreach (Transform transform in CommandContents)
				Destroy(transform.gameObject);

			foreach (var command in commands)
			{
				var prefab = Instantiate(CommandPrefabs, CommandContents);
				var item = prefab.GetComponent<ApplicationDebugCommandItemSystem>();
				item.UIText.text = command;
				item.Setup(this);
			}

			SupporterActive(isActive);
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
							historyIndex = history.Count;

						UIInputCommand.text = history[historyIndex -= 1];
						UIInputCommand.caretPosition = UIInputCommand.text.Length;

						UIInputCommand.MoveTextEnd(false);
					}

					if (Input.GetKeyDown(KeyCode.DownArrow))
					{
						if (historyIndex >= history.Count - 1)
							historyIndex = -1;

						UIInputCommand.text = history[historyIndex += 1];
						UIInputCommand.caretPosition = UIInputCommand.text.Length;

						UIInputCommand.MoveTextEnd(false);
					}
				}

				yield return null;
			}
		}
	}
}