using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000E0 RID: 224
public class BranchConfig : UIDialog
{
	// Token: 0x04000704 RID: 1796
	[NonSerialized]
	private ElectricalBranch branch;

	// Token: 0x04000705 RID: 1797
	public InputField input;

	// Token: 0x04000706 RID: 1798
	public int target;

	// Token: 0x06000AA2 RID: 2722 RVA: 0x0000A6D3 File Offset: 0x000088D3
	public override void OpenDialog()
	{
		this.input.text = this.target.ToString();
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x0000A6EB File Offset: 0x000088EB
	public void SetBranch(ElectricalBranch newSwitch)
	{
		this.branch = newSwitch;
		this.target = Mathf.FloorToInt((float)this.branch.branchAmount);
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x0000A70B File Offset: 0x0000890B
	public void Confirm()
	{
		if (this.isClosing)
		{
			return;
		}
		if (this.branch != null)
		{
			this.branch.ClientChangePower(this.GetIntValue());
		}
		this.CloseDialog();
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x0000A73B File Offset: 0x0000893B
	public void SelectTextField()
	{
		EventSystem.current.SetSelectedGameObject(this.input.gameObject, null);
		this.input.ActivateInputField();
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00055CB0 File Offset: 0x00053EB0
	public int GetIntValue()
	{
		int num = 1;
		int.TryParse(this.input.text, ref num);
		num = Mathf.Clamp(num, 1, 10000000);
		return num;
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00055CE0 File Offset: 0x00053EE0
	public void ValueChanged()
	{
		string text = this.GetIntValue().ToString();
		if (text != this.input.text)
		{
			this.input.text = text;
		}
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x0000A75E File Offset: 0x0000895E
	public void Cancel()
	{
		this.CloseDialog();
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x0000A766 File Offset: 0x00008966
	public void OnTextFieldEnd()
	{
		if (!Input.GetKey(KeyCode.Return))
		{
			return;
		}
		this.Confirm();
	}
}
