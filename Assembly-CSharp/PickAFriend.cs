using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000621 RID: 1569
public class PickAFriend : UIDialog
{
	// Token: 0x04001F37 RID: 7991
	public InputField input;

	// Token: 0x04001F38 RID: 7992
	public Action<ulong> onSelected;

	// Token: 0x06002318 RID: 8984 RVA: 0x0001BCDB File Offset: 0x00019EDB
	public override void OpenDialog()
	{
		this.input.text = "";
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x0001BCED File Offset: 0x00019EED
	public void SelectTextField()
	{
		EventSystem.current.SetSelectedGameObject(this.input.gameObject, null);
		this.input.ActivateInputField();
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x0001BD10 File Offset: 0x00019F10
	public void DoAssign(ulong steamid)
	{
		if (this.onSelected != null)
		{
			this.onSelected.Invoke(steamid);
		}
		this.CloseDialog();
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x0000A75E File Offset: 0x0000895E
	public void Cancel()
	{
		this.CloseDialog();
	}
}
