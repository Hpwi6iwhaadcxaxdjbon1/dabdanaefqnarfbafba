using System;
using ConVar;
using Facepunch.Extend;
using Facepunch.Math;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200069F RID: 1695
public class ServerHistoryItem : MonoBehaviour
{
	// Token: 0x040021D0 RID: 8656
	private ServerList.Server serverInfo;

	// Token: 0x040021D1 RID: 8657
	public Text serverName;

	// Token: 0x040021D2 RID: 8658
	public Text players;

	// Token: 0x040021D3 RID: 8659
	public Text lastJoinDate;

	// Token: 0x040021D4 RID: 8660
	public uint order;

	// Token: 0x060025CC RID: 9676 RVA: 0x000C6CF0 File Offset: 0x000C4EF0
	internal void Setup(ServerList.Server s)
	{
		this.serverInfo = s;
		foreach (string text in this.serverInfo.Tags)
		{
			if (text.StartsWith("cp"))
			{
				this.serverInfo.Players = StringExtensions.ToInt(text.Substring(2), 0);
			}
			if (text.StartsWith("mp"))
			{
				this.serverInfo.MaxPlayers = StringExtensions.ToInt(text.Substring(2), 0);
			}
		}
		this.serverName.text = StringExtensions.Truncate(s.Name, 32, null);
		if (Global.streamermode)
		{
			this.serverName.text = "STREAMER MODE ACTIVE";
		}
		this.players.text = string.Format("{0}/{1}", s.Players, s.MaxPlayers);
		this.lastJoinDate.text = "last join date unavailable";
		this.order = uint.MaxValue;
		if (s.LastTimePlayed > 0U)
		{
			this.order = uint.MaxValue - s.LastTimePlayed;
			long num = (long)Epoch.Current - (long)((ulong)s.LastTimePlayed);
			this.lastJoinDate.text = "last joined " + NumberExtensions.FormatSecondsLong(num) + " ago";
		}
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x0001D84C File Offset: 0x0001BA4C
	public void Open()
	{
		SingletonComponent<ServerBrowserInfo>.Instance.Open(this.serverInfo);
	}
}
