using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000359 RID: 857
public class FrequencyConfig : UIDialog
{
	// Token: 0x0400131C RID: 4892
	[NonSerialized]
	private IRFObject rfObject;

	// Token: 0x0400131D RID: 4893
	public InputField input;

	// Token: 0x0400131E RID: 4894
	public int target;

	// Token: 0x0600161D RID: 5661 RVA: 0x00012B00 File Offset: 0x00010D00
	public override void OpenDialog()
	{
		this.input.text = this.target.ToString();
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x00012B18 File Offset: 0x00010D18
	public void SetRFObj(IRFObject obj)
	{
		this.rfObject = obj;
		this.target = Mathf.FloorToInt((float)this.rfObject.GetFrequency());
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x00012B38 File Offset: 0x00010D38
	public void Confirm()
	{
		if (this.isClosing)
		{
			return;
		}
		if (this.rfObject != null)
		{
			this.rfObject.ClientSetFrequency(this.GetIntValue());
		}
		this.CloseDialog();
	}

	// Token: 0x06001620 RID: 5664 RVA: 0x00012B62 File Offset: 0x00010D62
	public void SelectTextField()
	{
		EventSystem.current.SetSelectedGameObject(this.input.gameObject, null);
		this.input.ActivateInputField();
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x000861F0 File Offset: 0x000843F0
	public int GetIntValue()
	{
		int num = 1;
		int.TryParse(this.input.text, ref num);
		num = Mathf.Clamp(num, RFManager.minFreq, RFManager.maxFreq);
		return num;
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x00086224 File Offset: 0x00084424
	public void ValueChanged()
	{
		string text = this.GetIntValue().ToString();
		if (text != this.input.text)
		{
			this.input.text = text;
		}
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x0000A75E File Offset: 0x0000895E
	public void Cancel()
	{
		this.CloseDialog();
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x00012B85 File Offset: 0x00010D85
	public void OnTextFieldEnd()
	{
		if (!Input.GetKey(KeyCode.Return))
		{
			return;
		}
		this.Confirm();
	}
}
