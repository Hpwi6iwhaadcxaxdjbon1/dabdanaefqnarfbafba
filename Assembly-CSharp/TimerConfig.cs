using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000E6 RID: 230
public class TimerConfig : UIDialog
{
	// Token: 0x04000728 RID: 1832
	[NonSerialized]
	private CustomTimerSwitch timerSwitch;

	// Token: 0x04000729 RID: 1833
	public InputField input;

	// Token: 0x0400072A RID: 1834
	public int seconds;

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0000A9A9 File Offset: 0x00008BA9
	public override void OpenDialog()
	{
		this.input.text = this.seconds.ToString();
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x0000A9C1 File Offset: 0x00008BC1
	public void SetTimerSwitch(CustomTimerSwitch newSwitch)
	{
		this.timerSwitch = newSwitch;
		this.seconds = Mathf.FloorToInt(newSwitch.timerLength);
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0000A9DB File Offset: 0x00008BDB
	public void Confirm()
	{
		if (this.isClosing)
		{
			return;
		}
		if (this.timerSwitch != null)
		{
			this.timerSwitch.SendNewTime(this.GetIntValue());
		}
		this.CloseDialog();
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0000AA0B File Offset: 0x00008C0B
	public void SelectTextField()
	{
		EventSystem.current.SetSelectedGameObject(this.input.gameObject, null);
		this.input.ActivateInputField();
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x000564A0 File Offset: 0x000546A0
	public int GetIntValue()
	{
		int num = 1;
		int.TryParse(this.input.text, ref num);
		num = Mathf.Clamp(num, 1, 1000000000);
		return num;
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x000564D0 File Offset: 0x000546D0
	public void ValueChanged()
	{
		string text = this.GetIntValue().ToString();
		if (text != this.input.text)
		{
			this.input.text = text;
		}
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x0000A75E File Offset: 0x0000895E
	public void Cancel()
	{
		this.CloseDialog();
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x0000AA2E File Offset: 0x00008C2E
	public void OnTextFieldEnd()
	{
		if (!Input.GetKey(KeyCode.Return))
		{
			return;
		}
		this.Confirm();
	}
}
