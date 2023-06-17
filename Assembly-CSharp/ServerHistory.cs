using System;
using Facepunch.Steamworks;
using Rust;
using UnityEngine;

// Token: 0x0200069D RID: 1693
public class ServerHistory : MonoBehaviour
{
	// Token: 0x040021CB RID: 8651
	public ServerHistoryItem prefab;

	// Token: 0x040021CC RID: 8652
	public GameObject panelList;

	// Token: 0x040021CD RID: 8653
	internal ServerList.Request Request;

	// Token: 0x060025C4 RID: 9668 RVA: 0x0001D7EF File Offset: 0x0001B9EF
	private void Start()
	{
		this.Refresh();
	}

	// Token: 0x060025C5 RID: 9669 RVA: 0x000C6C14 File Offset: 0x000C4E14
	public void Refresh()
	{
		if (global::Client.Steam == null)
		{
			return;
		}
		ServerList.Filter filter = new ServerList.Filter();
		filter.Add("appid", global::Client.Steam.AppId.ToString());
		this.Request = global::Client.Steam.ServerList.History(filter);
		this.panelList.transform.DestroyAllChildren(false);
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x0001D7F7 File Offset: 0x0001B9F7
	public void Update()
	{
		if (this.Request == null)
		{
			return;
		}
		this.Request.Responded.ForEach(new Action<ServerList.Server>(this.OnServer));
		this.Request.Responded.Clear();
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x000C6C74 File Offset: 0x000C4E74
	private void OnServer(ServerList.Server server)
	{
		if (server.AppId != Defines.appID)
		{
			return;
		}
		ServerHistoryItem component = Object.Instantiate<GameObject>(this.prefab.gameObject).GetComponent<ServerHistoryItem>();
		component.transform.SetParent(this.panelList.transform, false);
		component.Setup(server);
		this.panelList.transform.OrderChildren((Transform x) => x.GetComponent<ServerHistoryItem>().order);
	}
}
