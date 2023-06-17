using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000E1 RID: 225
public class CounterConfig : UIDialog
{
	// Token: 0x04000707 RID: 1799
	[NonSerialized]
	private PowerCounter powerCounter;

	// Token: 0x04000708 RID: 1800
	public InputField input;

	// Token: 0x04000709 RID: 1801
	public int target;

	// Token: 0x06000AAB RID: 2731 RVA: 0x0000A780 File Offset: 0x00008980
	public override void OpenDialog()
	{
		this.input.text = this.target.ToString();
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x0000A798 File Offset: 0x00008998
	public void SetCounter(PowerCounter newSwitch)
	{
		this.powerCounter = newSwitch;
		this.target = Mathf.FloorToInt((float)this.powerCounter.GetTarget());
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x0000A7B8 File Offset: 0x000089B8
	public void Confirm()
	{
		if (this.isClosing)
		{
			return;
		}
		if (this.powerCounter != null)
		{
			this.powerCounter.SendNewTarget(this.GetIntValue());
		}
		this.CloseDialog();
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x0000A7E8 File Offset: 0x000089E8
	public void SelectTextField()
	{
		EventSystem.current.SetSelectedGameObject(this.input.gameObject, null);
		this.input.ActivateInputField();
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00055D1C File Offset: 0x00053F1C
	public int GetIntValue()
	{
		int num = 1;
		int.TryParse(this.input.text, ref num);
		num = Mathf.Clamp(num, 1, 100);
		return num;
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x00055D4C File Offset: 0x00053F4C
	public void ValueChanged()
	{
		string text = this.GetIntValue().ToString();
		if (text != this.input.text)
		{
			this.input.text = text;
		}
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0000A75E File Offset: 0x0000895E
	public void Cancel()
	{
		this.CloseDialog();
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x0000A80B File Offset: 0x00008A0B
	public void OnTextFieldEnd()
	{
		if (!Input.GetKey(KeyCode.Return))
		{
			return;
		}
		this.Confirm();
	}
}
