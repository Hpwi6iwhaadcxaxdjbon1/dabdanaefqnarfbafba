using System;
using ConVar;
using Facepunch;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006CC RID: 1740
public class KeyBindUI : MonoBehaviour
{
	// Token: 0x040022B3 RID: 8883
	public GameObject blockingCanvas;

	// Token: 0x040022B4 RID: 8884
	public Button btnA;

	// Token: 0x040022B5 RID: 8885
	public Button btnB;

	// Token: 0x040022B6 RID: 8886
	public string bindString;

	// Token: 0x040022B7 RID: 8887
	private Button binding;

	// Token: 0x040022B8 RID: 8888
	public bool noButtonsDown;

	// Token: 0x0600269B RID: 9883 RVA: 0x0001E133 File Offset: 0x0001C333
	private void OnEnable()
	{
		this.UpdateBinding();
	}

	// Token: 0x0600269C RID: 9884 RVA: 0x000CADC8 File Offset: 0x000C8FC8
	private void UpdateBinding()
	{
		string[] buttonsWithBind = Facepunch.Input.GetButtonsWithBind(this.bindString);
		this.btnA.GetComponentInChildren<Text>().text = "";
		this.btnB.GetComponentInChildren<Text>().text = "";
		if (buttonsWithBind.Length != 0)
		{
			this.btnA.GetComponentInChildren<Text>().text = buttonsWithBind[0];
		}
		if (buttonsWithBind.Length > 1)
		{
			this.btnB.GetComponentInChildren<Text>().text = buttonsWithBind[1];
		}
	}

	// Token: 0x0600269D RID: 9885 RVA: 0x0001E13B File Offset: 0x0001C33B
	public void StartBinding(Button button)
	{
		this.binding = button;
		this.binding.interactable = false;
		this.noButtonsDown = false;
		this.blockingCanvas.SetActive(true);
	}

	// Token: 0x0600269E RID: 9886 RVA: 0x000CAE3C File Offset: 0x000C903C
	public void ResetToDefault()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "exec keys_default.cfg", Array.Empty<object>());
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "writecfg", Array.Empty<object>());
		base.transform.root.BroadcastMessage("UpdateBinding");
	}

	// Token: 0x0600269F RID: 9887 RVA: 0x000CAE88 File Offset: 0x000C9088
	public void Update()
	{
		if (this.binding != null)
		{
			string[] pressedButtons = Facepunch.Input.GetPressedButtons();
			if (UnityEngine.Input.GetKey(KeyCode.Escape))
			{
				Facepunch.Input.SetBind(this.binding.GetComponentInChildren<Text>().text, null);
				this.binding.GetComponentInChildren<Text>().text = "";
				this.binding.interactable = true;
				this.binding = null;
				this.blockingCanvas.SetActive(false);
				base.transform.root.BroadcastMessage("UpdateBinding");
				return;
			}
			if (!this.noButtonsDown)
			{
				this.noButtonsDown = (pressedButtons.Length == 0);
				if (!this.noButtonsDown)
				{
					return;
				}
			}
			if (pressedButtons.Length != 0)
			{
				Facepunch.Input.SetBind(this.binding.GetComponentInChildren<Text>().text, null);
				Facepunch.Input.SetBind(pressedButtons[0], this.bindString);
				this.binding.GetComponentInChildren<Text>().text = string.Join(";", pressedButtons);
				this.binding.interactable = true;
				this.binding = null;
				this.blockingCanvas.SetActive(false);
				base.transform.root.BroadcastMessage("UpdateBinding");
				Global.writecfg();
			}
		}
	}
}
