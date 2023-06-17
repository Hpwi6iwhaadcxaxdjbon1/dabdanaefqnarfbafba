using System;
using System.Collections;
using System.Linq;
using Facepunch;
using Facepunch.Extend;
using Rust;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200060D RID: 1549
public class ConsoleUI : SingletonComponent<ConsoleUI>
{
	// Token: 0x04001F02 RID: 7938
	public Text text;

	// Token: 0x04001F03 RID: 7939
	public InputField outputField;

	// Token: 0x04001F04 RID: 7940
	public InputField inputField;

	// Token: 0x04001F05 RID: 7941
	public GameObject AutocompleteDropDown;

	// Token: 0x04001F06 RID: 7942
	public GameObject ItemTemplate;

	// Token: 0x04001F07 RID: 7943
	public Color errorColor;

	// Token: 0x04001F08 RID: 7944
	public Color warningColor;

	// Token: 0x04001F09 RID: 7945
	public Color inputColor;

	// Token: 0x04001F0A RID: 7946
	private TextBuffer buffer = new TextBuffer(512, 8000);

	// Token: 0x04001F0B RID: 7947
	private TextBuffer history = new TextBuffer(64, int.MaxValue);

	// Token: 0x04001F0C RID: 7948
	private string historyTemp = string.Empty;

	// Token: 0x04001F0D RID: 7949
	private string historyText = string.Empty;

	// Token: 0x04001F0E RID: 7950
	private int historyIndex = -1;

	// Token: 0x04001F0F RID: 7951
	private Button[] autocompleteButtons;

	// Token: 0x04001F10 RID: 7952
	private int autocompleteIndex;

	// Token: 0x060022C0 RID: 8896 RVA: 0x000B9BCC File Offset: 0x000B7DCC
	protected override void Awake()
	{
		base.Awake();
		this.buffer.Add(this.text.text);
		Output.Install();
		Output.OnMessage += new Action<string, string, LogType>(this.OutputHandler_OnMessage);
		for (int i = 0; i < 50; i++)
		{
			Object.Instantiate<GameObject>(this.ItemTemplate, this.ItemTemplate.transform.parent);
		}
		this.autocompleteButtons = this.ItemTemplate.transform.parent.GetComponentsInChildren<Button>();
		Button[] array = this.autocompleteButtons;
		for (int j = 0; j < array.Length; j++)
		{
			Button button = array[j];
			Button btn = button;
			button.onClick.AddListener(delegate()
			{
				this.AutocompleteButtonClicked(btn);
			});
		}
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x0001B8D1 File Offset: 0x00019AD1
	protected void OnEnable()
	{
		this.historyText = string.Empty;
		this.historyIndex = -1;
		this.Refresh();
	}

	// Token: 0x060022C2 RID: 8898 RVA: 0x0001B8EB File Offset: 0x00019AEB
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (EventSystem.current)
		{
			EventSystem.current.SetSelectedGameObject(null, null);
		}
	}

	// Token: 0x060022C3 RID: 8899 RVA: 0x000B9C98 File Offset: 0x000B7E98
	protected void Update()
	{
		if (EventSystem.current == null)
		{
			return;
		}
		if (this.AutocompleteDropDown.activeInHierarchy)
		{
			if (Input.GetKeyDown(KeyCode.Tab) && EventSystem.current.currentSelectedGameObject == this.inputField.gameObject && this.autocompleteButtons[0].IsActive())
			{
				this.inputField.text = this.autocompleteButtons[0].name;
				this.inputField.MoveTextEnd(false);
			}
			if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
			{
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					this.autocompleteIndex++;
				}
				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					this.autocompleteIndex--;
				}
				if (this.autocompleteIndex >= 0)
				{
					if (this.autocompleteButtons[this.autocompleteIndex].IsActive())
					{
						this.autocompleteButtons[this.autocompleteIndex].Select();
						this.inputField.text = this.autocompleteButtons[this.autocompleteIndex].name;
					}
					else
					{
						this.autocompleteIndex--;
					}
				}
				if (this.autocompleteIndex < 0)
				{
					this.SelectInputField(false);
					this.autocompleteIndex = -1;
				}
			}
			if (this.autocompleteIndex != -1)
			{
				return;
			}
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			this.LoadHistory(1);
			return;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			this.LoadHistory(-1);
		}
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x000B9E10 File Offset: 0x000B8010
	public void CloseAutocomplete()
	{
		this.AutocompleteDropDown.SetActive(false);
		this.autocompleteIndex = -1;
		Button[] array = this.autocompleteButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		this.SelectInputField(false);
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x000B9E5C File Offset: 0x000B805C
	public void OnTextTypes(string str)
	{
		if (EventSystem.current.currentSelectedGameObject != this.inputField.gameObject)
		{
			return;
		}
		this.CloseAutocomplete();
		if (!string.IsNullOrEmpty(str) && str != this.historyText)
		{
			this.historyText = string.Empty;
			ConsoleSystem.Command[] array = Enumerable.ToArray<ConsoleSystem.Command>(Enumerable.Take<ConsoleSystem.Command>(Enumerable.Where<ConsoleSystem.Command>(ConsoleSystem.Index.All, (ConsoleSystem.Command x) => x.Name.StartsWith(str, 3) || x.FullName.StartsWith(str, 3)), this.autocompleteButtons.Length));
			if (array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].FullName;
					if (array[i].GetOveride != null)
					{
						text = string.Format("{0} <color=#8BC12CFF>[{1}]</color>", text, array[i].GetOveride.Invoke());
					}
					if (!string.IsNullOrEmpty(array[i].Description))
					{
						text = string.Format("{0} - <color=#FFD272FF>{1}</color>", text, StringExtensions.Truncate(array[i].Description, 100, null));
					}
					this.autocompleteButtons[i].GetComponentInChildren<Text>().text = text;
					this.autocompleteButtons[i].name = array[i].FullName;
					this.autocompleteButtons[i].gameObject.SetActive(true);
				}
				this.AutocompleteDropDown.SetActive(true);
			}
		}
	}

	// Token: 0x060022C6 RID: 8902 RVA: 0x0001B90D File Offset: 0x00019B0D
	private void AutocompleteButtonClicked(Button button)
	{
		this.inputField.text = button.name;
		this.SelectInputField(true);
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x0001B927 File Offset: 0x00019B27
	private void OnOpenDevTools()
	{
		if (EventSystem.current)
		{
			EventSystem.current.SetSelectedGameObject(null, null);
		}
		this.SelectInputField(false);
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x000B9FAC File Offset: 0x000B81AC
	private void OutputHandler_OnMessage(string message, string stack, LogType type = LogType.Error)
	{
		if (type == LogType.Error || type == LogType.Assert)
		{
			message = string.Concat(new string[]
			{
				"<color=#",
				this.errorColor.ToHex(),
				">",
				message,
				"</color>"
			});
		}
		if (type == LogType.Warning)
		{
			message = string.Concat(new string[]
			{
				"<color=#",
				this.warningColor.ToHex(),
				">",
				message,
				"</color>"
			});
		}
		this.Log(message);
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x0001B948 File Offset: 0x00019B48
	private void Log(string message)
	{
		this.buffer.Add(message);
		if (base.gameObject != null && base.gameObject.activeInHierarchy)
		{
			this.Refresh();
		}
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x0001B977 File Offset: 0x00019B77
	private void Refresh()
	{
		this.text.text = this.buffer.ToString();
	}

	// Token: 0x060022CB RID: 8907 RVA: 0x000BA044 File Offset: 0x000B8244
	private void LoadHistory(int delta)
	{
		if (this.historyIndex < 0)
		{
			this.historyTemp = this.inputField.text;
		}
		this.historyIndex = Mathf.Clamp(this.historyIndex + delta, -1, this.history.Count - 1);
		if (this.historyIndex < 0)
		{
			this.inputField.text = (this.historyText = this.historyTemp);
		}
		else
		{
			this.inputField.text = (this.historyText = this.history.Get(this.historyIndex));
		}
		this.inputField.MoveTextEnd(false);
	}

	// Token: 0x060022CC RID: 8908 RVA: 0x000BA0E4 File Offset: 0x000B82E4
	public void SubmitCommand(string command)
	{
		this.historyText = string.Empty;
		this.historyIndex = -1;
		if (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			return;
		}
		if (string.IsNullOrEmpty(command))
		{
			return;
		}
		this.Log(string.Concat(new string[]
		{
			"> <color=#",
			this.inputColor.ToHex(),
			">",
			command,
			"</color>"
		}));
		this.history.Add(command);
		this.historyTemp = (this.inputField.text = "");
		EventSystem.current.SetSelectedGameObject(null, null);
		this.SelectInputField(false);
		ConsoleSystem.Run(ConsoleSystem.Option.Client, command, Array.Empty<object>());
	}

	// Token: 0x060022CD RID: 8909 RVA: 0x0001B98F File Offset: 0x00019B8F
	private void SelectInputField(bool updateAutoComplete = false)
	{
		base.StartCoroutine(this.SetSelected(updateAutoComplete));
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x0001B99F File Offset: 0x00019B9F
	private IEnumerator SetSelected(bool updateAutoComplete = false)
	{
		if (EventSystem.current == null)
		{
			yield break;
		}
		if (EventSystem.current.currentSelectedGameObject == this.inputField.gameObject)
		{
			yield break;
		}
		EventSystem.current.SetSelectedGameObject(this.inputField.gameObject, null);
		yield return null;
		this.inputField.MoveTextEnd(false);
		if (updateAutoComplete)
		{
			this.OnTextTypes(this.inputField.text);
		}
		yield break;
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x0001B9B5 File Offset: 0x00019BB5
	public void ClearContents()
	{
		this.buffer.Clear();
		this.Refresh();
	}

	// Token: 0x060022D0 RID: 8912 RVA: 0x000BA1AC File Offset: 0x000B83AC
	public void Copy()
	{
		GUIUtility.systemCopyBuffer = this.buffer.ToString().Replace("<color=#" + this.errorColor.ToHex() + ">", "").Replace("<color=#" + this.warningColor.ToHex() + ">", "").Replace("<color=#" + this.inputColor.ToHex() + ">", "").Replace("</color>", "");
	}
}
