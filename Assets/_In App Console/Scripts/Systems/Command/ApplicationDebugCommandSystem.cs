using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Anonymous.Systems
{
	public class ApplicationDebugCommandSystem : MonoBehaviour, IApplicationDebugSystems
	{
		private static readonly Dictionary<string, Action<string>> Command = new(StringComparer.OrdinalIgnoreCase);
		private static readonly Dictionary<string, string> Comment = new();

		public TMP_InputField UIInputCommand;

		[Header("Command Settings")]
		public GameObject CommandPrefabs;

		public GameObject CommandSupporterObject;
		public Transform CommandContents;

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

		public void Setup()
		{
			AddCommand("Help", _ =>
			{
				foreach (var command in Comment)
					Debug.Log($"<color=orange>{command.Key}</color><br><color=white>{command.Value}<br></color>");
			});

			UIInputCommand.onSubmit.AddListener(value =>
			{
				var split = value.Split(' ');
				if (Command.ContainsKey(split[0]))
				{
					Debug.Log($"<color=#ffa500ff>[Command : {value}]</color>");
					Command[split[0]].Invoke(value);
				}

				UIInputCommand.text = string.Empty;
				UIInputCommand.ActivateInputField();
			});

			UIInputCommand.onValueChanged.AddListener(command =>
			{
				if (string.IsNullOrEmpty(command))
				{
					foreach (Transform transform in CommandContents)
						Destroy(transform.gameObject);
					CommandSupporterObject.SetActive(false);
					return;
				}

				if (Command == null || Command.Count == 0)
				{
					foreach (Transform transform in CommandContents)
						Destroy(transform.gameObject);
					CommandSupporterObject.SetActive(false);
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

				var keys = Command.Keys.ToList();
				keys.Sort();

				try
				{
					var matched = keys.Where(key => Regex.IsMatch(key, pattern, RegexOptions.IgnoreCase));
					foreach (Transform transform in CommandContents)
						Destroy(transform.gameObject);
					matched.ToList().ForEach(search =>
					{
						var prefab = Instantiate(CommandPrefabs, CommandContents);
						var item = prefab.GetComponent<ApplicationDebugCommandItemSystem>();
						item.UIText.text = search;
						item.Setup(this);
					});

					CommandSupporterObject.SetActive(matched.Any());
				}
				catch (Exception ex)
				{
				}
			});
		}

		public void Dispose()
		{
		}

		public static void AddCommand(string command, Action<string> action)
		{
			Command[command] = action;
		}

		public static void RemoveCommand(string command)
		{
			if (Command.ContainsKey(command))
				Command.Remove(command);
		}

		public static void AddComment(string command, string comment)
		{
			Comment[command] = comment;
		}
	}
}