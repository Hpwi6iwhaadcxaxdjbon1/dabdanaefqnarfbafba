using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000620 RID: 1568
public class KeyCodeEntry : UIDialog
{
	// Token: 0x04001F30 RID: 7984
	public Text textDisplay;

	// Token: 0x04001F31 RID: 7985
	public Action<string> onCodeEntered;

	// Token: 0x04001F32 RID: 7986
	public Text typeDisplay;

	// Token: 0x04001F33 RID: 7987
	public Translate.Phrase masterCodePhrase;

	// Token: 0x04001F34 RID: 7988
	public Translate.Phrase guestCodePhrase;

	// Token: 0x04001F35 RID: 7989
	private string textEntered;

	// Token: 0x04001F36 RID: 7990
	private int lastEnteredFrame;

	// Token: 0x06002310 RID: 8976 RVA: 0x0001BC51 File Offset: 0x00019E51
	public void SetUsingGuestCode(bool should)
	{
		this.typeDisplay.text = (should ? this.guestCodePhrase.translated : this.masterCodePhrase.translated);
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x0001BC79 File Offset: 0x00019E79
	protected override void OnEnable()
	{
		base.OnEnable();
		this.textEntered = "";
		this.textDisplay.text = this.textEntered;
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x0000A75E File Offset: 0x0000895E
	public void Cancel()
	{
		this.CloseDialog();
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x0001BC9D File Offset: 0x00019E9D
	public void ClearCode()
	{
		this.textEntered = "";
		this.textDisplay.text = "";
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x000BABD8 File Offset: 0x000B8DD8
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
		{
			this.EnterNumber(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
		{
			this.EnterNumber(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
		{
			this.EnterNumber(2);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
		{
			this.EnterNumber(3);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
		{
			this.EnterNumber(4);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
		{
			this.EnterNumber(5);
		}
		if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
		{
			this.EnterNumber(6);
		}
		if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
		{
			this.EnterNumber(7);
		}
		if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
		{
			this.EnterNumber(8);
		}
		if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
		{
			this.EnterNumber(9);
		}
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x000BAD00 File Offset: 0x000B8F00
	public void EnterNumber(int i)
	{
		if (this.textEntered.Length == 4)
		{
			return;
		}
		if (this.lastEnteredFrame == Time.frameCount)
		{
			return;
		}
		this.lastEnteredFrame = Time.frameCount;
		this.textEntered += i.ToString();
		Text text = this.textDisplay;
		text.text += "* ";
		if (this.textEntered.Length == 4)
		{
			base.Invoke(new Action(this.Finished), 0.3f);
		}
	}

	// Token: 0x06002316 RID: 8982 RVA: 0x0001BCBA File Offset: 0x00019EBA
	public void Finished()
	{
		if (this.onCodeEntered != null)
		{
			this.onCodeEntered.Invoke(this.textEntered);
		}
		this.CloseDialog();
	}
}
