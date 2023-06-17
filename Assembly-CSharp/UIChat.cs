using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000608 RID: 1544
public class UIChat : SingletonComponent<UIChat>
{
	// Token: 0x04001EEE RID: 7918
	public GameObject inputArea;

	// Token: 0x04001EEF RID: 7919
	public GameObject chatArea;

	// Token: 0x04001EF0 RID: 7920
	public InputField inputField;

	// Token: 0x04001EF1 RID: 7921
	public ScrollRect scrollRect;

	// Token: 0x04001EF2 RID: 7922
	public CanvasGroup canvasGroup;

	// Token: 0x04001EF3 RID: 7923
	public GameObjectRef chatItemPlayer;

	// Token: 0x04001EF4 RID: 7924
	public static bool isOpen;

	// Token: 0x0600229E RID: 8862 RVA: 0x000B9458 File Offset: 0x000B7658
	public static void Open()
	{
		if (NeedsKeyboard.AnyActive())
		{
			return;
		}
		if (Cursor.visible)
		{
			return;
		}
		SingletonComponent<UIChat>.Instance.inputArea.SetActive(true);
		SingletonComponent<UIChat>.Instance.inputArea.GetComponent<NeedsKeyboard>().enabled = true;
		SingletonComponent<UIChat>.Instance.ClearText();
		EventSystem.current.SetSelectedGameObject(SingletonComponent<UIChat>.Instance.inputField.gameObject, null);
		SingletonComponent<UIChat>.Instance.inputField.ActivateInputField();
		UIChat.isOpen = true;
		for (int i = 0; i < SingletonComponent<UIChat>.Instance.chatArea.transform.childCount; i++)
		{
			SingletonComponent<UIChat>.Instance.chatArea.transform.GetChild(i).gameObject.SetActive(true);
		}
		LeanTween.cancel(SingletonComponent<UIChat>.Instance.inputArea);
		LeanTween.alphaCanvas(SingletonComponent<UIChat>.Instance.inputArea.GetComponent<CanvasGroup>(), 1f, 0.2f);
		LeanTween.scale(SingletonComponent<UIChat>.Instance.inputArea, Vector3.one, 0.2f).setEase(27);
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x0001B816 File Offset: 0x00019A16
	public void OnEnable()
	{
		this.Cancel();
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x0001B81E File Offset: 0x00019A1E
	private IEnumerator TestRoutine()
	{
		yield return CoroutineEx.waitForSeconds(5f);
		for (;;)
		{
			yield return CoroutineEx.waitForSeconds(2f);
			UIChat.Add((ulong)(76561190000000000L + (long)Random.Range(0, int.MaxValue)), "HELLO this is Text <color=red>with coloured</color> sections in it. It's <color=red>long</color> on purpose for testing.", 1f);
		}
		yield break;
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000B9564 File Offset: 0x000B7764
	public void SubmitText()
	{
		if (Input.GetButtonDown("Submit"))
		{
			PlayerInput.IgnoreCurrentKeys();
			if (this.inputField.text.Length > 0)
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Client.Quiet(), "chat.say", new object[]
				{
					this.inputField.text
				});
			}
			base.Invoke(new Action(this.Cancel), 0.1f);
		}
		if (Input.GetButtonDown("Cancel"))
		{
			PlayerInput.IgnoreCurrentKeys();
			base.Invoke(new Action(this.Cancel), 0.1f);
		}
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x000B9600 File Offset: 0x000B7800
	public void Cancel()
	{
		this.ClearText();
		this.inputArea.GetComponent<NeedsKeyboard>().enabled = false;
		LeanTween.alphaCanvas(this.inputArea.GetComponent<CanvasGroup>(), 0f, 0.2f);
		LeanTween.scale(this.inputArea, Vector3.one * 0.2f, 0.2f).setEase(26).setOnComplete(delegate()
		{
			this.inputArea.SetActive(false);
		});
		if (this.inputField.isFocused && EventSystem.current)
		{
			EventSystem.current.SetSelectedGameObject(null, null);
		}
		UIChat.isOpen = false;
		this.scrollRect.verticalNormalizedPosition = 0f;
	}

	// Token: 0x060022A3 RID: 8867 RVA: 0x000B96B4 File Offset: 0x000B78B4
	private void TrimMessages()
	{
		if (this.chatArea.transform.childCount > 16)
		{
			GameObject gameObject = this.chatArea.transform.GetChild(0).gameObject;
			GameManager.client.Retire(gameObject);
		}
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x000B96F8 File Offset: 0x000B78F8
	private void ClearText()
	{
		try
		{
			this.inputField.text = " ";
		}
		catch (Exception ex)
		{
			Debug.Log("Unity UI Exception! " + ex);
		}
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x0001B826 File Offset: 0x00019A26
	public void Update()
	{
		this.TrimMessages();
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x000B973C File Offset: 0x000B793C
	public static void Add(ulong steamid, string text, float volume)
	{
		if (SingletonComponent<UIChat>.Instance == null)
		{
			return;
		}
		GameObject gameObject = GameManager.client.CreatePrefab(SingletonComponent<UIChat>.Instance.chatItemPlayer.resourcePath, Vector3.zero, Quaternion.identity, true);
		gameObject.transform.SetParent(SingletonComponent<UIChat>.Instance.chatArea.transform, false);
		gameObject.transform.SetAsLastSibling();
		gameObject.GetComponent<ChatEntry>().Setup(steamid, text, volume);
		if (steamid > 0UL)
		{
			SteamFriendsList.JustSeen(steamid);
		}
		SingletonComponent<UIChat>.Instance.TrimMessages();
	}
}
