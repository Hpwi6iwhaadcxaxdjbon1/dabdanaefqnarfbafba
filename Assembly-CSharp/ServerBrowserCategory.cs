using System;
using System.Linq;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000692 RID: 1682
public class ServerBrowserCategory : MonoBehaviour
{
	// Token: 0x04002180 RID: 8576
	public Text serverCountText;

	// Token: 0x04002181 RID: 8577
	[NonSerialized]
	public ServerBrowserList browserList;

	// Token: 0x04002182 RID: 8578
	[NonSerialized]
	public bool isDirty;

	// Token: 0x06002581 RID: 9601 RVA: 0x0001D57D File Offset: 0x0001B77D
	public void OnEnable()
	{
		this.Dirty();
	}

	// Token: 0x06002582 RID: 9602 RVA: 0x0001D585 File Offset: 0x0001B785
	public void Update()
	{
		if (this.isDirty)
		{
			this.isDirty = false;
			this.UpdateCounts();
		}
	}

	// Token: 0x06002583 RID: 9603 RVA: 0x0001D59C File Offset: 0x0001B79C
	public void Dirty()
	{
		this.isDirty = true;
	}

	// Token: 0x06002584 RID: 9604 RVA: 0x000C5B08 File Offset: 0x000C3D08
	public void UpdateCounts()
	{
		if (this.browserList == null)
		{
			return;
		}
		IOrderedEnumerable<ServerList.Server> sortedServers = this.browserList.GetSortedServers();
		this.serverCountText.text = string.Format("{0:N0} players\n{1:N0} servers", Enumerable.Sum<ServerList.Server>(sortedServers, (ServerList.Server x) => x.Players), Enumerable.Count<ServerList.Server>(sortedServers));
		this.isDirty = false;
	}
}
