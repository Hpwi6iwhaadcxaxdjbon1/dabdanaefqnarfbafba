using System;
using System.Collections.Generic;
using System.Linq;
using EasyAntiCheat.Client.Hydra;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Models;
using Facepunch.Steamworks;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000697 RID: 1687
public class ServerBrowserList : BaseMonoBehaviour
{
	// Token: 0x0400219F RID: 8607
	public ServerBrowserCategory categoryButton;

	// Token: 0x040021A0 RID: 8608
	public bool startActive;

	// Token: 0x040021A1 RID: 8609
	public ServerBrowserItem itemTemplate;

	// Token: 0x040021A2 RID: 8610
	public int refreshOrder;

	// Token: 0x040021A3 RID: 8611
	public bool UseOfficialServers;

	// Token: 0x040021A4 RID: 8612
	public ServerBrowserItem[] items;

	// Token: 0x040021A5 RID: 8613
	public ServerBrowserList.Rules[] rules;

	// Token: 0x040021A6 RID: 8614
	internal bool isDirty;

	// Token: 0x040021A7 RID: 8615
	private string searchFilter = "";

	// Token: 0x040021A8 RID: 8616
	private string mapFilter = "";

	// Token: 0x040021A9 RID: 8617
	private bool showFull = true;

	// Token: 0x040021AA RID: 8618
	private bool showEmpty = true;

	// Token: 0x040021AB RID: 8619
	private List<ServerList.Server> serverList = new List<ServerList.Server>();

	// Token: 0x040021AC RID: 8620
	internal string sortOrder = "pingDesc";

	// Token: 0x040021AD RID: 8621
	private float nextRebuild;

	// Token: 0x040021AE RID: 8622
	public ServerBrowserList.QueryType queryType;

	// Token: 0x040021AF RID: 8623
	public static string VersionTag = "v" + 2153;

	// Token: 0x040021B0 RID: 8624
	public ServerBrowserList.ServerKeyvalues[] keyValues = new ServerBrowserList.ServerKeyvalues[0];

	// Token: 0x040021B1 RID: 8625
	private ServerList.Request Request;

	// Token: 0x040021B2 RID: 8626
	internal int servers;

	// Token: 0x040021B3 RID: 8627
	internal int players;

	// Token: 0x040021B4 RID: 8628
	[NonSerialized]
	public bool shouldShowSecureServers;

	// Token: 0x060025A1 RID: 9633 RVA: 0x000C61B8 File Offset: 0x000C43B8
	public void Awake()
	{
		this.SteamInit();
		if (this.categoryButton)
		{
			this.categoryButton.GetComponent<Button>().onClick.AddListener(delegate()
			{
				this.SetActiveList();
			});
			this.categoryButton.browserList = this;
		}
		this.itemTemplate.gameObject.SetActive(false);
		for (int i = 0; i < 256; i++)
		{
			Object.Instantiate<GameObject>(this.itemTemplate.gameObject, this.itemTemplate.transform.parent);
		}
		this.items = this.itemTemplate.transform.parent.GetComponentsInChildren<ServerBrowserItem>(true);
		for (int j = 0; j < this.items.Length; j++)
		{
			this.items[j].gameObject.name = "Server " + j.ToString();
		}
		base.gameObject.SetActive(false);
		if (this.startActive)
		{
			base.Invoke(new Action(this.SetActiveList), 0.2f);
		}
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x0001D695 File Offset: 0x0001B895
	public void OnEnable()
	{
		this.RemoveServers();
		this.Rebuild();
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x0001D6A3 File Offset: 0x0001B8A3
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.RemoveServers();
	}

	// Token: 0x060025A4 RID: 9636 RVA: 0x000C62C4 File Offset: 0x000C44C4
	public void Clear()
	{
		if (this.Request != null)
		{
			this.Request.Dispose();
			this.Request = null;
		}
		this.RemoveServers();
		this.serverList.Clear();
		this.servers = 0;
		this.players = 0;
		this.isDirty = true;
	}

	// Token: 0x060025A5 RID: 9637 RVA: 0x000C6314 File Offset: 0x000C4514
	public void AddServer(ServerList.Server server)
	{
		if (server == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(server.Name))
		{
			return;
		}
		if (server.Address == null)
		{
			return;
		}
		if (Application.Manifest != null && Application.Manifest.Servers.IsBannedServer(server.Address.ToString()))
		{
			return;
		}
		this.isDirty = true;
		this.serverList.Add(server);
		this.servers++;
		this.players += server.Players;
		if (this.categoryButton)
		{
			this.categoryButton.Dirty();
		}
	}

	// Token: 0x060025A6 RID: 9638 RVA: 0x000C63AC File Offset: 0x000C45AC
	public void RemoveServers()
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			this.items[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x060025A7 RID: 9639 RVA: 0x000C63E0 File Offset: 0x000C45E0
	public void Rebuild()
	{
		this.isDirty = false;
		int num = 0;
		using (IEnumerator<ServerList.Server> enumerator = Enumerable.Take<ServerList.Server>(this.GetSortedServers(), this.items.Length).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ServerList.Server s = enumerator.Current;
				ServerBrowserItem serverBrowserItem = this.items[num];
				serverBrowserItem.transform.SetParent(this.itemTemplate.transform.parent, false);
				serverBrowserItem.Init(s);
				serverBrowserItem.gameObject.SetActive(true);
				num++;
			}
			goto IL_90;
		}
		IL_79:
		this.items[num].gameObject.SetActive(false);
		num++;
		IL_90:
		if (num >= this.items.Length)
		{
			return;
		}
		goto IL_79;
	}

	// Token: 0x060025A8 RID: 9640 RVA: 0x000C6498 File Offset: 0x000C4698
	public IOrderedEnumerable<ServerList.Server> GetSortedServers()
	{
		IEnumerable<ServerList.Server> enumerable = Enumerable.AsEnumerable<ServerList.Server>(this.serverList);
		if (!string.IsNullOrEmpty(this.searchFilter))
		{
			enumerable = Enumerable.Where<ServerList.Server>(enumerable, (ServerList.Server x) => StringEx.Contains(x.Name, this.searchFilter, 1));
		}
		if (!this.showFull)
		{
			enumerable = Enumerable.Where<ServerList.Server>(enumerable, (ServerList.Server x) => x.Players < x.MaxPlayers);
		}
		if (!this.showEmpty)
		{
			enumerable = Enumerable.Where<ServerList.Server>(enumerable, (ServerList.Server x) => x.Players > 0);
		}
		if (!string.IsNullOrEmpty(this.mapFilter))
		{
			enumerable = Enumerable.Where<ServerList.Server>(enumerable, (ServerList.Server x) => StringEx.Contains(x.Map, this.mapFilter, 1));
		}
		if (this.sortOrder == "ping")
		{
			return Enumerable.OrderByDescending<ServerList.Server, int>(enumerable, (ServerList.Server x) => x.Ping);
		}
		if (this.sortOrder == "servername")
		{
			return Enumerable.OrderBy<ServerList.Server, string>(enumerable, (ServerList.Server x) => x.Name);
		}
		if (this.sortOrder == "servernameDesc")
		{
			return Enumerable.OrderByDescending<ServerList.Server, string>(enumerable, (ServerList.Server x) => x.Name);
		}
		if (this.sortOrder == "players")
		{
			return Enumerable.OrderByDescending<ServerList.Server, int>(enumerable, (ServerList.Server x) => x.Players);
		}
		if (this.sortOrder == "playersDesc")
		{
			return Enumerable.OrderBy<ServerList.Server, int>(enumerable, (ServerList.Server x) => x.Players);
		}
		return Enumerable.OrderBy<ServerList.Server, int>(enumerable, (ServerList.Server x) => x.Ping);
	}

	// Token: 0x060025A9 RID: 9641 RVA: 0x0001D6B3 File Offset: 0x0001B8B3
	public void Update()
	{
		if (this.isDirty && this.nextRebuild < Time.realtimeSinceStartup)
		{
			this.Rebuild();
			this.nextRebuild = Time.realtimeSinceStartup + 1f;
		}
	}

	// Token: 0x060025AA RID: 9642 RVA: 0x000C668C File Offset: 0x000C488C
	public void SetActiveList()
	{
		foreach (ServerBrowserList serverBrowserList in base.transform.GetSiblings(false))
		{
			serverBrowserList.gameObject.SetActive(false);
		}
		foreach (Button button in this.categoryButton.transform.GetSiblings(false))
		{
			button.interactable = true;
		}
		this.categoryButton.GetComponent<Button>().interactable = false;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060025AB RID: 9643 RVA: 0x0001D6E1 File Offset: 0x0001B8E1
	public void SearchFilter(string searchtext, string mapsearchtext, bool showFull, bool showEmpty)
	{
		this.isDirty = true;
		this.searchFilter = searchtext;
		this.mapFilter = mapsearchtext;
		this.showFull = showFull;
		this.showEmpty = showEmpty;
		this.categoryButton.Dirty();
	}

	// Token: 0x060025AC RID: 9644 RVA: 0x0001D712 File Offset: 0x0001B912
	public void OrderBy(string strBy)
	{
		this.isDirty = true;
		this.sortOrder = strBy;
		this.nextRebuild = 0f;
	}

	// Token: 0x060025AD RID: 9645 RVA: 0x0001D72D File Offset: 0x0001B92D
	public bool IsCheater()
	{
		return Runtime.IsActive() && Runtime.Initialized;
	}

	// Token: 0x060025AE RID: 9646 RVA: 0x000C6750 File Offset: 0x000C4950
	public void Refresh()
	{
		this.SteamInit();
		this.Clear();
		if (this.categoryButton)
		{
			this.categoryButton.Dirty();
		}
		if (this.queryType == ServerBrowserList.QueryType.None)
		{
			return;
		}
		this.shouldShowSecureServers = true;
		if (!this.shouldShowSecureServers)
		{
			Debug.Log("Showing only insecure servers because you are VAC banned or don't have EAC enabled");
		}
		if (!this.UseOfficialServers)
		{
			ServerList.Filter filter = new ServerList.Filter();
			filter.Add("appid", global::Client.Steam.AppId.ToString());
			filter.Add("gamedir", "rust");
			if (this.queryType == ServerBrowserList.QueryType.RegularInternet)
			{
				if (this.shouldShowSecureServers)
				{
					filter.Add("secure", "1");
				}
				filter.Add("and", (1 + this.keyValues.Length).ToString());
				filter.Add("gametype", ServerBrowserList.VersionTag);
				foreach (ServerBrowserList.ServerKeyvalues serverKeyvalues in this.keyValues)
				{
					filter.Add(serverKeyvalues.key, serverKeyvalues.value);
				}
			}
			if (this.queryType == ServerBrowserList.QueryType.RegularInternet)
			{
				this.Request = global::Client.Steam.ServerList.Internet(filter);
			}
			else if (this.queryType == ServerBrowserList.QueryType.Friends)
			{
				this.Request = global::Client.Steam.ServerList.Friends(filter);
			}
			else if (this.queryType == ServerBrowserList.QueryType.Favourites)
			{
				this.Request = global::Client.Steam.ServerList.Favourites(filter);
			}
			else if (this.queryType == ServerBrowserList.QueryType.LAN)
			{
				this.Request = global::Client.Steam.ServerList.Local(filter);
			}
			else if (this.queryType == ServerBrowserList.QueryType.History)
			{
				this.Request = global::Client.Steam.ServerList.History(filter);
			}
			if (this.Request != null)
			{
				this.Request.OnUpdate = new Action(this.OnServersUpdated);
			}
			return;
		}
		if (Application.Manifest == null)
		{
			Debug.LogWarning("Couldn't get official servers (manifest missing)");
			return;
		}
		if (Application.Manifest.Servers == null)
		{
			Debug.LogWarning("Couldn't get official servers (servers missing)");
			return;
		}
		if (Application.Manifest.Servers.Official == null)
		{
			Debug.LogWarning("Couldn't get official servers (official missing)");
			return;
		}
		string[] array2 = Enumerable.ToArray<string>(Enumerable.Select<Manifest.ServerDesc, string>(Application.Manifest.Servers.Official, (Manifest.ServerDesc x) => string.Format("{0}:{1}", x.Address, x.Port)));
		this.Request = global::Client.Steam.ServerList.Custom(array2);
		this.Request.OnUpdate = new Action(this.OnServersUpdated);
	}

	// Token: 0x060025AF RID: 9647 RVA: 0x00002ECE File Offset: 0x000010CE
	public void SteamInit()
	{
	}

	// Token: 0x060025B0 RID: 9648 RVA: 0x000C69D4 File Offset: 0x000C4BD4
	private void OnServersUpdated()
	{
		if (this.Request == null)
		{
			return;
		}
		if (this.Request.Responded.Count == 0)
		{
			return;
		}
		foreach (ServerList.Server server in this.Request.Responded)
		{
			this.ServerResponded(server);
		}
		this.Request.Responded.Clear();
	}

	// Token: 0x060025B1 RID: 9649 RVA: 0x000C6A58 File Offset: 0x000C4C58
	private void ServerResponded(ServerList.Server server)
	{
		using (TimeWarning.New("ServerResponded", 0.1f))
		{
			if (server.Secure == this.shouldShowSecureServers)
			{
				if (server.AppId == Defines.appID)
				{
					if (server.Tags == null)
					{
						Debug.Log("Server No Tags: " + server.Name);
					}
					else if (Enumerable.Contains<string>(server.Tags, ServerBrowserList.VersionTag))
					{
						foreach (string text in server.Tags)
						{
							if (text.StartsWith("cp"))
							{
								server.Players = StringExtensions.ToInt(text.Substring(2), 0);
							}
							if (text.StartsWith("mp"))
							{
								server.MaxPlayers = StringExtensions.ToInt(text.Substring(2), 0);
							}
						}
						ServerBrowserList.Rules[] array = this.rules;
						for (int i = 0; i < array.Length; i++)
						{
							ServerBrowserList.Rules rule = array[i];
							if (Enumerable.Any<string>(server.Tags, (string x) => x == rule.tag))
							{
								rule.serverList.AddServer(server);
								return;
							}
						}
						this.AddServer(server);
					}
				}
			}
		}
	}

	// Token: 0x02000698 RID: 1688
	[Serializable]
	public struct Rules
	{
		// Token: 0x040021B5 RID: 8629
		public string tag;

		// Token: 0x040021B6 RID: 8630
		public ServerBrowserList serverList;
	}

	// Token: 0x02000699 RID: 1689
	public enum QueryType
	{
		// Token: 0x040021B8 RID: 8632
		RegularInternet,
		// Token: 0x040021B9 RID: 8633
		Friends,
		// Token: 0x040021BA RID: 8634
		History,
		// Token: 0x040021BB RID: 8635
		LAN,
		// Token: 0x040021BC RID: 8636
		Favourites,
		// Token: 0x040021BD RID: 8637
		None
	}

	// Token: 0x0200069A RID: 1690
	[Serializable]
	public struct ServerKeyvalues
	{
		// Token: 0x040021BE RID: 8638
		public string key;

		// Token: 0x040021BF RID: 8639
		public string value;
	}
}
