using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000622 RID: 1570
public class RenameSleepingBag : UIDialog
{
	// Token: 0x04001F39 RID: 7993
	public InputField input;

	// Token: 0x04001F3A RID: 7994
	public SleepingBag bag;

	// Token: 0x0600231D RID: 8989 RVA: 0x0001BD2C File Offset: 0x00019F2C
	public override void OpenDialog()
	{
		this.input.text = this.bag.niceName;
	}

	// Token: 0x0600231E RID: 8990 RVA: 0x0001BD44 File Offset: 0x00019F44
	public void SelectTextField()
	{
		EventSystem.current.SetSelectedGameObject(this.input.gameObject, null);
		this.input.ActivateInputField();
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x0001BD67 File Offset: 0x00019F67
	public void DoRename()
	{
		if (this.isClosing)
		{
			return;
		}
		if (this.bag)
		{
			this.bag.ClientRename(this.input.text);
		}
		this.CloseDialog();
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x0000A75E File Offset: 0x0000895E
	public void Cancel()
	{
		this.CloseDialog();
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x0001BD9B File Offset: 0x00019F9B
	public void OnTextFieldEnd()
	{
		if (!Input.GetKey(KeyCode.Return))
		{
			return;
		}
		this.DoRename();
	}
}
