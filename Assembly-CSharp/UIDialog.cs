using System;

// Token: 0x02000623 RID: 1571
public class UIDialog : ListComponent<UIDialog>
{
	// Token: 0x04001F3B RID: 7995
	[NonSerialized]
	public bool isClosing;

	// Token: 0x17000251 RID: 593
	// (get) Token: 0x06002323 RID: 8995 RVA: 0x0001BDAD File Offset: 0x00019FAD
	public static bool isOpen
	{
		get
		{
			return ListComponent<UIDialog>.InstanceList.Count > 0;
		}
	}

	// Token: 0x06002324 RID: 8996 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OpenDialog()
	{
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x0001BDBC File Offset: 0x00019FBC
	public virtual void CloseDialog()
	{
		PlayerInput.IgnoreCurrentKeys();
		this.isClosing = true;
		GameManager.Destroy(base.gameObject, 0f);
	}
}
