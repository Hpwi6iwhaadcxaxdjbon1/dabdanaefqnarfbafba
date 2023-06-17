using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ConVar;
using Facepunch;
using Facepunch.Steamworks;
using Rust;
using UnityEngine;

// Token: 0x020005F9 RID: 1529
public class SteamClient : SingletonComponent<SteamClient>
{
	// Token: 0x04001EBB RID: 7867
	public static ulong localSteamID = 0UL;

	// Token: 0x04001EBC RID: 7868
	public static string localName = "localName";

	// Token: 0x04001EBD RID: 7869
	public static string currentVersion = "unset";

	// Token: 0x04001EBE RID: 7870
	public static string localCountry = "US";

	// Token: 0x06002250 RID: 8784 RVA: 0x0001B43A File Offset: 0x0001963A
	public Texture GetAvatarTexture(ulong steamid)
	{
		return SteamClient.SteamAvatarCache.FindTexture(steamid);
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x0001B442 File Offset: 0x00019642
	public string GetUserName(ulong steamid)
	{
		if (Global.streamermode)
		{
			return RandomUsernames.Get(steamid);
		}
		return global::Client.Steam.Friends.GetName(steamid);
	}

	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06002252 RID: 8786 RVA: 0x000B8148 File Offset: 0x000B6348
	public static string availableVersion
	{
		get
		{
			if (global::Client.Steam == null)
			{
				return "";
			}
			return global::Client.Steam.BuildId.ToString() + SteamUtil.betaName;
		}
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x0001B462 File Offset: 0x00019662
	public static void Init()
	{
		if (SingletonComponent<SteamClient>.Instance == null)
		{
			Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/steam.prefab", true));
		}
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x000B8180 File Offset: 0x000B6380
	public static bool InitSteamClient()
	{
		if (Global.SteamClient != null)
		{
			return true;
		}
		Config.ForUnity(Application.platform.ToString());
		Global.SteamClient = new Facepunch.Steamworks.Client(Defines.appID);
		if (!Global.SteamClient.IsValid)
		{
			Debug.Log("SteamClient not initialized properly");
			Global.SteamClient.Dispose();
			Global.SteamClient = null;
			return false;
		}
		Global.SteamClient.Inventory.OnDefinitionsUpdated += new Action(SteamClient.OnInventoryDefinitionsUpdated);
		if (CommandLine.HasSwitch("-debugsteamcallbacks"))
		{
			Facepunch.Steamworks.Client steamClient = Global.SteamClient;
			steamClient.OnAnyCallback = (Action<object>)Delegate.Combine(steamClient.OnAnyCallback, new Action<object>(SteamClient.DebugPrintSteamCallback));
		}
		Facepunch.Steamworks.Client.Instance.Achievements.Refresh();
		return true;
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x0001B486 File Offset: 0x00019686
	private static void OnInventoryDefinitionsUpdated()
	{
		ItemManager.InvalidateWorkshopSkinCache();
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x000B8244 File Offset: 0x000B6444
	private static void DebugPrintSteamCallback(object obj)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<color=#88dd88>" + obj.GetType().Name + "</color>");
		foreach (FieldInfo fieldInfo in obj.GetType().GetFields(52))
		{
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				"  <color=#aaaaaa>",
				fieldInfo.Name,
				":</color>\t <color=#fffff>",
				fieldInfo.GetValue(obj).ToString(),
				"</color>"
			}));
		}
		Debug.Log(stringBuilder.ToString());
	}

	// Token: 0x06002257 RID: 8791 RVA: 0x000B82E8 File Offset: 0x000B64E8
	protected override void Awake()
	{
		if (CommandLine.HasSwitch("-nosteam"))
		{
			return;
		}
		base.Awake();
		SteamClient.InitSteamClient();
		if (!global::Client.Steam.IsValid)
		{
			return;
		}
		SteamClient.currentVersion = SteamClient.availableVersion;
		global::Client.Steam.Inventory.OnUpdate += new Action(this.OnInventoryUpdated);
		SteamClient.localSteamID = global::Client.Steam.SteamId;
		SteamClient.localName = global::Client.Steam.Username;
		SteamClient.localCountry = global::Client.Steam.CurrentCountry;
		DebugEx.Log(string.Concat(new object[]
		{
			"SteamID: ",
			SteamClient.localSteamID,
			" (",
			SteamClient.localName,
			")"
		}), StackTraceLogType.None);
		global::Client.Steam.Stats.UpdateStats();
		global::Client.Steam.Stats.UpdateGlobalStats(2);
		base.StartCoroutine(this.InventoryDrop());
		global::Client.Steam.Inventory.Refresh();
	}

	// Token: 0x06002258 RID: 8792 RVA: 0x000B83E8 File Offset: 0x000B65E8
	private void Update()
	{
		if (global::Client.Steam == null)
		{
			return;
		}
		if (global::Client.Steam.IsValid)
		{
			global::Client.Steam.RunCallbacks();
			global::Client.Steam.Voice.Update();
			global::Client.Steam.Inventory.Update();
			global::Client.Steam.Networking.Update();
			global::Client.Steam.RunUpdateCallbacks();
		}
		SteamClient.SteamAvatarCache.Cycle();
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x0001B48D File Offset: 0x0001968D
	private void OnDisable()
	{
		if (Global.SteamClient != null)
		{
			Global.SteamClient.Stats.StoreStats();
			DebugEx.Log("Steam Client Shutdown", StackTraceLogType.None);
			Global.SteamClient.Dispose();
			Global.SteamClient = null;
		}
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x0001B4C1 File Offset: 0x000196C1
	private void OnInventoryUpdated()
	{
		ItemManager.UpdateUnlockedSkins();
		if (LocalPlayer.Entity)
		{
			LocalPlayer.Entity.GetComponent<SteamInventory>().ClientUpdate();
		}
		if (SingletonComponent<SteamInventoryInfo>.Instance)
		{
			SingletonComponent<SteamInventoryInfo>.Instance.Refresh();
		}
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x0001B4F9 File Offset: 0x000196F9
	public void SendUpdatedInventory()
	{
		if (global::Client.Steam == null)
		{
			return;
		}
		global::Client.Steam.Inventory.Refresh();
		this.OnInventoryUpdated();
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x0001B518 File Offset: 0x00019718
	public IEnumerator InventoryDrop()
	{
		yield return CoroutineEx.waitForSeconds(Random.Range(20f, 120f));
		global::Client.Steam.Inventory.GrantAllPromoItems();
		for (;;)
		{
			global::Client.Steam.Inventory.TriggerItemDrop(10);
			yield return CoroutineEx.waitForSeconds(300f);
		}
		yield break;
	}

	// Token: 0x020005FA RID: 1530
	private class SteamAvatarCache
	{
		// Token: 0x04001EBF RID: 7871
		public static List<SteamClient.SteamAvatarCache> avatarsLoading = new List<SteamClient.SteamAvatarCache>();

		// Token: 0x04001EC0 RID: 7872
		public static Dictionary<ulong, SteamClient.SteamAvatarCache> avatars = new Dictionary<ulong, SteamClient.SteamAvatarCache>();

		// Token: 0x04001EC1 RID: 7873
		public Texture2D texture;

		// Token: 0x04001EC2 RID: 7874
		public ulong steamid;

		// Token: 0x04001EC3 RID: 7875
		public Image image;

		// Token: 0x0600225F RID: 8799 RVA: 0x000B8450 File Offset: 0x000B6650
		public static Texture FindTexture(ulong steamid)
		{
			SteamClient.SteamAvatarCache steamAvatarCache;
			if (SteamClient.SteamAvatarCache.avatars.TryGetValue(steamid, ref steamAvatarCache))
			{
				return steamAvatarCache.texture;
			}
			if (global::Client.Steam == null)
			{
				return null;
			}
			Image cachedAvatar = global::Client.Steam.Friends.GetCachedAvatar(1, steamid);
			if (cachedAvatar == null)
			{
				global::Client.Steam.Friends.UpdateInformation(steamid);
				return null;
			}
			SteamClient.SteamAvatarCache steamAvatarCache2 = new SteamClient.SteamAvatarCache
			{
				texture = new Texture2D(64, 64, TextureFormat.ARGB32, true),
				steamid = steamid,
				image = cachedAvatar
			};
			steamAvatarCache2.texture.name = "SteamAvatar_" + steamid;
			steamAvatarCache2.texture.filterMode = FilterMode.Trilinear;
			steamAvatarCache2.texture.wrapMode = TextureWrapMode.Clamp;
			steamAvatarCache2.texture.anisoLevel = 8;
			for (int i = 0; i < steamAvatarCache2.texture.width; i++)
			{
				for (int j = 0; j < steamAvatarCache2.texture.height; j++)
				{
					steamAvatarCache2.texture.SetPixel(i, j, new Color32(0, 0, 0, 20));
				}
			}
			steamAvatarCache2.texture.Apply(true);
			SteamClient.SteamAvatarCache.avatars.Add(steamid, steamAvatarCache2);
			SteamClient.SteamAvatarCache.avatarsLoading.Add(steamAvatarCache2);
			steamAvatarCache2.Load();
			return steamAvatarCache2.texture;
		}

		// Token: 0x06002260 RID: 8800 RVA: 0x000B8584 File Offset: 0x000B6784
		public void Load()
		{
			if (this.image == null || this.image.IsError)
			{
				SteamClient.SteamAvatarCache.avatarsLoading.Remove(this);
				return;
			}
			if (!this.image.IsLoaded)
			{
				return;
			}
			for (int i = 0; i < this.image.Width; i++)
			{
				for (int j = 0; j < this.image.Height; j++)
				{
					Color pixel = this.image.GetPixel(i, j);
					this.texture.SetPixel(i, this.image.Height - 1 - j, new Color32(pixel.r, pixel.g, pixel.b, pixel.a));
				}
			}
			this.texture.Apply();
			SteamClient.SteamAvatarCache.avatarsLoading.Remove(this);
		}

		// Token: 0x06002261 RID: 8801 RVA: 0x000B8650 File Offset: 0x000B6850
		public static void Cycle()
		{
			if (SteamClient.SteamAvatarCache.avatarsLoading.Count == 0)
			{
				return;
			}
			SteamClient.SteamAvatarCache[] array = SteamClient.SteamAvatarCache.avatarsLoading.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Load();
			}
		}
	}
}
