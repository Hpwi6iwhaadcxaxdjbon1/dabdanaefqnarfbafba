using System;
using UnityEngine;

// Token: 0x02000691 RID: 1681
public class ServerBrowser : MonoBehaviour
{
	// Token: 0x0400217B RID: 8571
	public string orderBy = "pingDesc";

	// Token: 0x0400217C RID: 8572
	private string searchText = "";

	// Token: 0x0400217D RID: 8573
	private string mapSearchText = "";

	// Token: 0x0400217E RID: 8574
	private bool showFull = true;

	// Token: 0x0400217F RID: 8575
	private bool showEmpty = true;

	// Token: 0x06002579 RID: 9593 RVA: 0x0001D53E File Offset: 0x0001B73E
	public void Awake()
	{
		this.RefreshAll();
	}

	// Token: 0x0600257A RID: 9594 RVA: 0x000C5958 File Offset: 0x000C3B58
	public void RefreshAll()
	{
		ServerBrowserList[] componentsInChildren = base.GetComponentsInChildren<ServerBrowserList>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Refresh();
		}
	}

	// Token: 0x0600257B RID: 9595 RVA: 0x000C5984 File Offset: 0x000C3B84
	public void SetOrder(string strBy)
	{
		if (this.orderBy == strBy)
		{
			strBy += "Desc";
		}
		this.orderBy = strBy;
		ServerBrowserList[] componentsInChildren = base.GetComponentsInChildren<ServerBrowserList>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].OrderBy(this.orderBy);
		}
	}

	// Token: 0x0600257C RID: 9596 RVA: 0x000C59D8 File Offset: 0x000C3BD8
	public void SetShowFull(bool showFull)
	{
		this.showFull = showFull;
		ServerBrowserList[] componentsInChildren = base.GetComponentsInChildren<ServerBrowserList>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SearchFilter(this.searchText, this.mapSearchText, this.showFull, this.showEmpty);
		}
	}

	// Token: 0x0600257D RID: 9597 RVA: 0x000C5A24 File Offset: 0x000C3C24
	public void SetShowEmpty(bool showFull)
	{
		this.showEmpty = showFull;
		ServerBrowserList[] componentsInChildren = base.GetComponentsInChildren<ServerBrowserList>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SearchFilter(this.searchText, this.mapSearchText, this.showFull, this.showEmpty);
		}
	}

	// Token: 0x0600257E RID: 9598 RVA: 0x000C5A70 File Offset: 0x000C3C70
	public void SetSearchFilter(string txt)
	{
		this.searchText = txt;
		ServerBrowserList[] componentsInChildren = base.GetComponentsInChildren<ServerBrowserList>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SearchFilter(this.searchText, this.mapSearchText, this.showFull, this.showEmpty);
		}
	}

	// Token: 0x0600257F RID: 9599 RVA: 0x000C5ABC File Offset: 0x000C3CBC
	public void SetMapFilter(string txt)
	{
		this.mapSearchText = txt;
		ServerBrowserList[] componentsInChildren = base.GetComponentsInChildren<ServerBrowserList>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SearchFilter(this.searchText, this.mapSearchText, this.showFull, this.showEmpty);
		}
	}
}
