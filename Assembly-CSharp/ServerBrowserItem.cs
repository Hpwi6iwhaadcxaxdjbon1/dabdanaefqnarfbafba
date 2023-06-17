using System;
using System.Collections.Generic;
using ConVar;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000696 RID: 1686
public class ServerBrowserItem : MonoBehaviour
{
	// Token: 0x04002197 RID: 8599
	public Text serverName;

	// Token: 0x04002198 RID: 8600
	public Text mapName;

	// Token: 0x04002199 RID: 8601
	public Text playerCount;

	// Token: 0x0400219A RID: 8602
	public Text ping;

	// Token: 0x0400219B RID: 8603
	public Toggle favourited;

	// Token: 0x0400219C RID: 8604
	private ServerList.Server serverInfo;

	// Token: 0x0400219D RID: 8605
	internal bool preventFavouriteToggle;

	// Token: 0x0400219E RID: 8606
	public static List<string> steamFavourites;

	// Token: 0x0600259A RID: 9626 RVA: 0x000C605C File Offset: 0x000C425C
	public void Init(ServerList.Server s)
	{
		this.serverInfo = s;
		this.serverName.text = s.Name;
		if (Global.streamermode)
		{
			this.serverName.text = "Streamer Mode is Active.";
		}
		this.playerCount.text = string.Format("{0}/{1}", s.Players, s.MaxPlayers);
		this.mapName.text = s.Map;
		this.ping.text = s.Ping.ToString();
		this.CheckFavouriteStatus();
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x000C60F4 File Offset: 0x000C42F4
	public void OnClicked()
	{
		if (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift))
		{
			Debug.Log(this.serverInfo.Name);
			Debug.Log(this.serverInfo.Address);
			Debug.Log("QueryPort: " + this.serverInfo.QueryPort);
			Debug.Log("ConnectionPort: " + this.serverInfo.ConnectionPort);
			GUIUtility.systemCopyBuffer = this.serverInfo.Address + ":" + this.serverInfo.QueryPort;
			return;
		}
		SingletonComponent<ServerBrowserInfo>.Instance.Open(this.serverInfo);
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x0001D63B File Offset: 0x0001B83B
	public void CheckFavouriteStatus()
	{
		this.preventFavouriteToggle = true;
		this.favourited.isOn = this.isFavourited;
		this.preventFavouriteToggle = false;
	}

	// Token: 0x0600259D RID: 9629 RVA: 0x0001D65C File Offset: 0x0001B85C
	public void OnFavouriteToggle(bool favourite)
	{
		if (this.preventFavouriteToggle)
		{
			return;
		}
		if (favourite)
		{
			this.serverInfo.AddToFavourites();
		}
		else
		{
			this.serverInfo.RemoveFromFavourites();
		}
		ServerBrowserItem.steamFavourites = null;
	}

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x0600259E RID: 9630 RVA: 0x0001D688 File Offset: 0x0001B888
	public bool isFavourited
	{
		get
		{
			return this.serverInfo.Favourite;
		}
	}
}
