using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Steamworks;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Rust.UI.Debug;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020001DC RID: 476
public class Client : SingletonComponent<global::Client>, IClientCallback
{
	// Token: 0x04000C3A RID: 3130
	private Auth.Ticket authTicket;

	// Token: 0x04000C3B RID: 3131
	private IEnumerator currentCoroutine;

	// Token: 0x04000C3C RID: 3132
	private static EventSystem _eventsystem;

	// Token: 0x04000C3D RID: 3133
	private float LastConfigSaveTime;

	// Token: 0x04000C3E RID: 3134
	private Stopwatch ngTimer = Stopwatch.StartNew();

	// Token: 0x04000C3F RID: 3135
	private static UnityEngine.Mesh _cubeMesh;

	// Token: 0x04000C41 RID: 3137
	public static ulong DemoLocalClient;

	// Token: 0x04000C42 RID: 3138
	private BenchmarkData.Result LoadTime;

	// Token: 0x04000C43 RID: 3139
	public static BaseEntity CurrentEntity;

	// Token: 0x04000C44 RID: 3140
	public HashSet<uint> subscriptions = new HashSet<uint>();

	// Token: 0x04000C45 RID: 3141
	private const long EntityPositionPacketSize = 36L;

	// Token: 0x04000C46 RID: 3142
	private const long EntityFlagsPacketSize = 8L;

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000F22 RID: 3874 RVA: 0x0000D933 File Offset: 0x0000BB33
	private bool StatsEnabled
	{
		get
		{
			return Netgraph.enabled;
		}
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0000D93A File Offset: 0x0000BB3A
	private void RegisterIncoming(string type, long bytes)
	{
		if (!string.IsNullOrEmpty(Netgraph.typefilter) && !StringEx.Contains(type, Netgraph.typefilter, 1))
		{
			return;
		}
		Net.cl.IncomingStats.Add(type, bytes);
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x00067D78 File Offset: 0x00065F78
	private void RegisterIncoming(string type, string entity, long bytes)
	{
		if (!string.IsNullOrEmpty(Netgraph.typefilter) && !StringEx.Contains(type, Netgraph.typefilter, 1))
		{
			return;
		}
		if (!string.IsNullOrEmpty(Netgraph.entityfilter) && !StringEx.Contains(entity, Netgraph.entityfilter, 1))
		{
			return;
		}
		Net.cl.IncomingStats.Add(type, entity, bytes);
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x00067DD0 File Offset: 0x00065FD0
	public void OnNetworkMessage(Message packet)
	{
		switch (packet.type)
		{
		case 3:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("Approved", packet.read.Length);
			}
			this.OnApproved(packet);
			return;
		case 5:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("Entities", packet.read.Length);
			}
			this.OnEntities(packet);
			return;
		case 6:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("EntityDestroy", packet.read.Length);
			}
			this.OnEntityDestroy(packet);
			return;
		case 7:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("GroupChange", packet.read.Length);
			}
			this.OnGroupChange(packet);
			return;
		case 8:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("GroupDestroy", packet.read.Length);
			}
			this.OnGroupDestroy(packet);
			return;
		case 9:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("RPCMessage", packet.read.Length);
			}
			this.OnRPCMessage(packet);
			return;
		case 10:
			this.OnEntityPosition(packet);
			return;
		case 11:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("ConsoleMessage", packet.read.Length);
			}
			ConsoleNetwork.OnConsoleMessageFromServer(packet);
			return;
		case 12:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("ConsoleCommand", packet.read.Length);
			}
			ConsoleNetwork.OnConsoleCommandfromServer(packet);
			return;
		case 13:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("Effect", packet.read.Length);
			}
			EffectNetwork.OnReceivedEffect(packet);
			return;
		case 14:
		{
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("Disconnect", packet.read.Length);
			}
			string text = packet.read.String();
			Debug.Log("Disconnect Reason: " + text);
			Network.Client.disconnectReason = text;
			return;
		}
		case 15:
			if (!global::Client.IsPlayingDemo)
			{
				return;
			}
			this.DemoPlayerTick(PlayerTick.Deserialize(packet.read));
			return;
		case 16:
			this.OnMessage(packet);
			return;
		case 17:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("RequestUserInformation", packet.read.Length);
			}
			this.OnRequestUserInformation(packet);
			return;
		case 19:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("GroupEnter", packet.read.Length);
			}
			this.OnGroupEnter(packet);
			return;
		case 20:
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("GroupLeave", packet.read.Length);
			}
			this.OnGroupLeave(packet);
			return;
		case 21:
		{
			uint uid = packet.read.UInt32();
			BaseEntity baseEntity = BaseNetworkable.clientEntities.Find(uid) as BaseEntity;
			if (baseEntity != null)
			{
				baseEntity.OnVoiceData(packet.read.BytesWithSize());
			}
			return;
		}
		case 22:
			EAC.OnMessageReceived(packet);
			return;
		case 23:
			this.OnEntityFlags(packet);
			return;
		}
		Debug.LogWarning("[CLIENT][UNHANDLED] " + packet.type);
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x0000D968 File Offset: 0x0000BB68
	private void OnMessage(Message packet)
	{
		LoadingScreen.Show();
		if (SingletonComponent<LoadingScreen>.Instance)
		{
			SingletonComponent<LoadingScreen>.Instance.UpdateFromServer(packet.read.String(), packet.read.String());
		}
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x000680D8 File Offset: 0x000662D8
	private void OnRequestUserInformation(Message packet)
	{
		LoadingScreen.Show();
		if (SingletonComponent<LoadingScreen>.Instance)
		{
			SingletonComponent<LoadingScreen>.Instance.UpdateFromServer("CONNECTING", "NEGOTIATING CONNECTION");
		}
		if (packet.peer.write.Start())
		{
			packet.peer.write.PacketID(18);
			packet.peer.write.UInt8(228);
			packet.peer.write.UInt64(global::Client.Steam.SteamId);
			packet.peer.write.UInt32(2153U);
			packet.peer.write.String(this.GetOSName());
			packet.peer.write.String(global::Client.Steam.Username);
			packet.peer.write.String(SteamUtil.betaBranch);
			Auth.Ticket ticket = this.GetAuthTicket();
			if (ticket == null)
			{
				Debug.LogWarning("No Token Data!");
				Net.cl.Disconnect("No Token Data", true);
				return;
			}
			packet.peer.write.BytesWithSize(ticket.Data);
			packet.peer.write.Send(new SendInfo(packet.connection));
		}
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x00068210 File Offset: 0x00066410
	private void OnApproved(Message packet)
	{
		ConnectionScreen.SetStatus("connected");
		if (SingletonComponent<LoadingScreen>.Instance)
		{
			SingletonComponent<LoadingScreen>.Instance.UpdateFromServer("LOADING", "Loading Level");
		}
		Approval msg = Approval.Deserialize(packet.read);
		base.StartCoroutine(this.DoClientConnected(msg));
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x00068264 File Offset: 0x00066464
	private void OnRPCMessage(Message packet)
	{
		uint uid = packet.read.UInt32();
		uint num = packet.read.UInt32();
		ulong sourceConnection = packet.read.UInt64();
		BaseEntity baseEntity = BaseNetworkable.clientEntities.Find(uid) as BaseEntity;
		if (baseEntity == null)
		{
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("RPCMessage", "Unknown", packet.read.Length);
			}
			return;
		}
		if (this.StatsEnabled)
		{
			this.RegisterIncoming(StringPool.Get(num), baseEntity.ToString(), packet.read.Length);
		}
		global::Client.CurrentEntity = baseEntity;
		baseEntity.CL_RPCMessage(num, sourceConnection, packet);
		global::Client.CurrentEntity = null;
	}

	// Token: 0x06000F2A RID: 3882 RVA: 0x0000D99B File Offset: 0x0000BB9B
	private string GetOSName()
	{
		return "windows";
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x0000D9A2 File Offset: 0x0000BBA2
	public void CancelAuthTicket()
	{
		if (this.authTicket != null)
		{
			this.authTicket.Cancel();
			this.authTicket = null;
		}
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x0000D9BE File Offset: 0x0000BBBE
	private Auth.Ticket GetAuthTicket()
	{
		this.CancelAuthTicket();
		this.authTicket = global::Client.Steam.Auth.GetAuthSessionTicket();
		return this.authTicket;
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x0000D9E1 File Offset: 0x0000BBE1
	private void CancelLoading()
	{
		if (this.currentCoroutine != null)
		{
			base.StopCoroutine(this.currentCoroutine);
			this.currentCoroutine = null;
		}
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x0000D9FE File Offset: 0x0000BBFE
	private Coroutine StartLoading(IEnumerator coroutine)
	{
		this.currentCoroutine = coroutine;
		return base.StartCoroutine(coroutine);
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x00068310 File Offset: 0x00066510
	public void Connect(string strAddress, int port)
	{
		if (global::Client.IsPlayingDemo)
		{
			this.StopPlayingDemo(false);
		}
		if (global::Client.IsRecordingDemo)
		{
			this.StopRecordingDemo();
		}
		EAC.OnJoinServer();
		if (!Net.cl.Connect(strAddress, port))
		{
			Debug.Log("Connect failed!");
			return;
		}
		this.NetworkInit();
		Analytics.RecordAdd("game", "pressed_start_game", 1.0);
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x00068374 File Offset: 0x00066574
	private void DestroyNetworkables()
	{
		BaseEntity[] array = Object.FindObjectsOfType<BaseEntity>();
		BaseEntity[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GameManager.Destroy(array2[i].gameObject, 0f);
		}
		Debug.Log("Destroyed " + array.Length + " map networkables");
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x0000DA0E File Offset: 0x0000BC0E
	private IEnumerator DoClientConnected(Approval msg)
	{
		Net.cl.Connection.encryptionLevel = msg.encryption;
		Net.cl.Connection.decryptIncoming = true;
		this.CancelLoading();
		UnityEngine.Time.timeScale = 1f;
		Application.isLoading = true;
		ConVar.Client.prediction = true;
		Culling.debug = 0;
		ConVar.Graphics.showtexeldensity = 0;
		CommunityEntity.DestroyServerCreatedUI();
		LoadingScreen.Show();
		MainMenuSystem.Hide();
		GameObjectUtil.GlobalBroadcast("ClientConnected", null);
		if (SingletonComponent<LoadingScreen>.Instance)
		{
			SingletonComponent<LoadingScreen>.Instance.UpdateFromServer("LOADING", "Connection accepted");
		}
		yield return CoroutineEx.waitForEndOfFrame;
		Net.cl.ServerName = msg.hostname;
		Net.cl.IsOfficialServer = msg.official;
		ExceptionReporter.Disabled = !msg.official;
		LoadingScreen.Update("Skinnable Warmup");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		GameManifest.LoadAssets();
		yield return this.StartLoading(FileSystem_Warmup.Run(0.2f, new Action<string>(LoadingScreen.Update), "Asset Warmup ({0}/{1})"));
		LoadingScreen.Update("Preload Complete");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (!LevelManager.IsValid(msg.level))
		{
			LoadingScreen.Update("Couldn't Load Scene " + msg.level.ToUpper());
			yield return CoroutineEx.waitForSeconds(2f);
			Net.cl.Disconnect("Couldn't Load Scene " + msg.level.ToUpper(), true);
			Application.isLoading = false;
			yield break;
		}
		LoadingScreen.Update("Loading Scene");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		global::World.Url = msg.levelUrl;
		global::World.Size = msg.levelSize;
		global::World.Seed = msg.levelSeed;
		global::World.Checksum = msg.checksum;
		LevelManager.LoadLevel(msg.level, true);
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (SingletonComponent<WorldSetup>.Instance)
		{
			yield return this.StartLoading(SingletonComponent<WorldSetup>.Instance.InitCoroutine());
		}
		LoadingScreen.Update("Client Ready");
		if (!Net.cl.IsConnected())
		{
			Application.isLoading = false;
			yield break;
		}
		this.DestroyNetworkables();
		yield return this.StartLoading(PrefabPoolWarmup.Run(0.2f, new Action<string>(LoadingScreen.Update), "Prefab Warmup ({0}/{1})"));
		if (Net.cl.write.Start())
		{
			using (ClientReady clientReady = Pool.Get<ClientReady>())
			{
				clientReady.clientInfo = Pool.GetList<ClientReady.ClientInfo>();
				foreach (ConsoleSystem.Command command in Enumerable.Where<ConsoleSystem.Command>(ConsoleSystem.Index.All, (ConsoleSystem.Command x) => x.ClientInfo))
				{
					ClientReady.ClientInfo clientInfo = Pool.Get<ClientReady.ClientInfo>();
					clientInfo.name = command.FullName;
					clientInfo.value = command.String;
					clientReady.clientInfo.Add(clientInfo);
				}
				Net.cl.write.PacketID(4);
				clientReady.WriteToStream(Net.cl.write);
				Net.cl.write.Send(new SendInfo(Net.cl.Connection));
				Net.cl.Connection.encryptOutgoing = true;
			}
		}
		uiPlayerPreview.Create();
		Application.isLoading = false;
		yield break;
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x000683C8 File Offset: 0x000665C8
	public void OnClientDisconnected(string strReason)
	{
		this.CancelLoading();
		this.OnLeaveServer();
		this.CancelAuthTicket();
		if (Application.isQuitting)
		{
			return;
		}
		LoadingScreen.Hide();
		GameObjectUtil.GlobalBroadcast("ClientDisconnect", null);
		Debug.Log("Disconnected (" + strReason + ") - returning to main menu");
		LevelManager.UnloadLevel();
		ConnectionScreen.DisconnectReason(strReason);
		CommunityEntity.DestroyServerCreatedUI();
		GameManager.client.Reset();
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000F33 RID: 3891 RVA: 0x0000DA24 File Offset: 0x0000BC24
	public static Facepunch.Steamworks.Client Steam
	{
		get
		{
			return Global.SteamClient;
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000F34 RID: 3892 RVA: 0x0000DA2B File Offset: 0x0000BC2B
	public static EventSystem EventSystem
	{
		get
		{
			if (global::Client._eventsystem == null)
			{
				global::Client._eventsystem = SingletonComponent<global::Client>.Instance.GetComponentInChildren<EventSystem>();
			}
			return global::Client._eventsystem;
		}
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0000DA4E File Offset: 0x0000BC4E
	private void ClearVisibility()
	{
		BasePlayer.ClearVisibility();
		BaseNpc.ClearVisibility();
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x0000DA5A File Offset: 0x0000BC5A
	private void NetworkInit()
	{
		Net.cl.callbackHandler = this;
		Net.cl.cryptography = new NetworkCryptographyClient();
		Net.cl.visibility = new Manager(null);
		BuildingManager.client.Clear();
		this.ClearVisibility();
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x00068430 File Offset: 0x00066630
	private void Update()
	{
		using (TimeWarning.New("Net.cl.Cycle", 0.1f))
		{
			Net.cl.Cycle();
		}
		this.DemoFrame();
		if (Net.cl.IsConnected())
		{
			if (MainCamera.isValid)
			{
				PositionLerp.DebugDraw = (ConVar.Vis.lerp && ((LocalPlayer.Entity != null && LocalPlayer.Entity.IsDeveloper) || global::Client.IsPlayingDemo));
				using (TimeWarning.New("PositionLerp.Cycle", 0.1f))
				{
					PositionLerp.Cycle();
				}
				using (TimeWarning.New("BasePlayer.ClientCycle", 0.1f))
				{
					BasePlayer.ClientCycle(UnityEngine.Time.deltaTime);
				}
				using (TimeWarning.New("BaseNpc.ClientCycle", 0.1f))
				{
					BaseNpc.ClientCycle(UnityEngine.Time.deltaTime);
				}
				using (TimeWarning.New("HTNAnimal.ClientCycle", 0.1f))
				{
					HTNAnimal.ClientCycle(UnityEngine.Time.deltaTime);
				}
			}
			BuildingManager.client.Cycle();
		}
		this.SaveConfigs();
		this.RunDebugCommands();
		EAC.DoUpdate();
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x000685A0 File Offset: 0x000667A0
	private void LateUpdate()
	{
		this.DemoLateUpdate();
		if (MainCamera.isValid)
		{
			using (TimeWarning.New("BasePlayer.ClientCycle", 0.1f))
			{
				BasePlayer.LateClientCycle();
			}
			using (TimeWarning.New("HTNAnimal.ClientCycle", 0.1f))
			{
				HTNAnimal.LateClientCycle();
			}
		}
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x0000DA96 File Offset: 0x0000BC96
	private void Disconnect()
	{
		Net.cl.Disconnect("closing", true);
		Net.cl.callbackHandler = null;
		EAC.DoShutdown();
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x0000DAB8 File Offset: 0x0000BCB8
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.Disconnect();
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x0000DAC8 File Offset: 0x0000BCC8
	private void OnApplicationQuit()
	{
		Application.isQuitting = true;
		this.Disconnect();
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x00068618 File Offset: 0x00066818
	private void OnLeaveServer()
	{
		this.ClearVisibility();
		LODComponent.ClearOccludees();
		WindZoneExManager.Clear();
		WaterSystem.Clear();
		AtmosphereVolumeRenderer.Clear();
		ImpostorRenderer.Clear();
		if (Net.cl.visibility != null)
		{
			Net.cl.visibility.Dispose();
			Net.cl.visibility = null;
		}
		EAC.OnLeaveServer();
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x00068670 File Offset: 0x00066870
	private void SaveConfigs()
	{
		if (!ConsoleSystem.HasChanges)
		{
			return;
		}
		if (this.LastConfigSaveTime > UnityEngine.Time.realtimeSinceStartup)
		{
			return;
		}
		this.LastConfigSaveTime = UnityEngine.Time.realtimeSinceStartup + 5f;
		ConsoleSystem.HasChanges = false;
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "writecfg", Array.Empty<object>());
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x000686C0 File Offset: 0x000668C0
	private void RunDebugCommands()
	{
		this.UpdateNetgraph();
		if (!LocalPlayer.Entity)
		{
			return;
		}
		if (!LocalPlayer.Entity.IsAdmin && !LocalPlayer.Entity.IsDeveloper && !global::Client.IsPlayingDemo)
		{
			return;
		}
		if (Debugging.drawcolliders)
		{
			this.DrawColliders();
		}
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x00068710 File Offset: 0x00066910
	private void DrawColliders()
	{
		Material material = FileSystem.Load<Material>("Assets/Content/materials/collider_mesh.mat", true);
		Material material2 = FileSystem.Load<Material>("Assets/Content/materials/collider_mesh_convex.mat", true);
		Material material3 = FileSystem.Load<Material>("Assets/Content/materials/collider_trigger.mat", true);
		if (global::Client._cubeMesh == null)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			global::Client._cubeMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			GameManager.DestroyImmediate(gameObject, false);
		}
		List<Collider> list = Pool.GetList<Collider>();
		global::Vis.Colliders<Collider>(MainCamera.position, 10f, list, -1, 2);
		foreach (Collider collider in list)
		{
			Material material4 = material;
			if (collider is MeshCollider)
			{
				MeshCollider meshCollider = collider as MeshCollider;
				if (meshCollider.convex)
				{
					material4 = material2;
				}
				if (collider.isTrigger)
				{
					material4 = material3;
				}
				UnityEngine.Graphics.DrawMesh(meshCollider.sharedMesh, meshCollider.transform.localToWorldMatrix, material4, 0);
			}
			if (collider is BoxCollider)
			{
				BoxCollider boxCollider = collider as BoxCollider;
				material4 = material2;
				if (collider.isTrigger)
				{
					material4 = material3;
				}
				Matrix4x4 rhs = Matrix4x4.TRS(boxCollider.center, Quaternion.identity, boxCollider.size);
				UnityEngine.Graphics.DrawMesh(global::Client._cubeMesh, boxCollider.transform.localToWorldMatrix * rhs, material4, 0);
			}
		}
		Pool.FreeList<Collider>(ref list);
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x00068870 File Offset: 0x00066A70
	private void UpdateNetgraph()
	{
		NetGraph instance = SingletonComponent<NetGraph>.Instance;
		if (instance == null)
		{
			return;
		}
		Stats currentNetworkStats = this.GetCurrentNetworkStats();
		if (currentNetworkStats == null)
		{
			instance.Enabled = false;
			return;
		}
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		bool flag = LocalPlayer.Entity.IsDeveloper || LocalPlayer.Entity.IsAdmin;
		currentNetworkStats.Enabled = (Netgraph.enabled && flag);
		if (!currentNetworkStats.Enabled)
		{
			instance.Enabled = false;
			return;
		}
		instance.Enabled = true;
		instance.UpdateFrom(currentNetworkStats);
		if (this.ngTimer.Elapsed.TotalSeconds <= (double)Netgraph.updatespeed)
		{
			return;
		}
		this.ngTimer.Reset();
		this.ngTimer.Start();
		Net.cl.IncomingStats.Flip();
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x0000DAD6 File Offset: 0x0000BCD6
	private Stats GetCurrentNetworkStats()
	{
		if (Net.cl == null)
		{
			return null;
		}
		if (Net.cl.IncomingStats == null)
		{
			return null;
		}
		return Net.cl.IncomingStats;
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000F42 RID: 3906 RVA: 0x0000DAF9 File Offset: 0x0000BCF9
	public static bool IsPlayingDemo
	{
		get
		{
			return Net.cl.IsPlaying;
		}
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000F43 RID: 3907 RVA: 0x0000DB05 File Offset: 0x0000BD05
	public static bool IsRecordingDemo
	{
		get
		{
			return Net.cl.IsRecording;
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000F44 RID: 3908 RVA: 0x0000DB11 File Offset: 0x0000BD11
	// (set) Token: 0x06000F45 RID: 3909 RVA: 0x0000DB18 File Offset: 0x0000BD18
	public static bool IsBenchmarkDemo { get; set; }

	// Token: 0x06000F46 RID: 3910 RVA: 0x0000DB20 File Offset: 0x0000BD20
	public void StartPlayingDemo(string name, DemoHeader msg)
	{
		if (name.Contains("benchmark.dem"))
		{
			global::Client.IsBenchmarkDemo = true;
		}
		base.StartCoroutine(this.StartPlayingDemoInteral(msg));
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x0000DB43 File Offset: 0x0000BD43
	private IEnumerator StartPlayingDemoInteral(DemoHeader msg)
	{
		Application.isLoading = true;
		Stopwatch timer = Stopwatch.StartNew();
		GameManifest.LoadAssets();
		this.NetworkInit();
		MainMenuSystem.Hide();
		DeveloperTools.Close();
		LoadingScreen.Show();
		LoadingScreen.Update("Loading Scene");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		global::World.Url = msg.levelUrl;
		global::World.Size = msg.levelSize;
		global::World.Seed = msg.levelSeed;
		global::World.Checksum = msg.checksum;
		LevelManager.LoadLevel(msg.level, true);
		global::Client.DemoLocalClient = msg.localclient;
		MainCamera.position = msg.position;
		MainCamera.forward = msg.rotation;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (SingletonComponent<WorldSetup>.Instance)
		{
			yield return this.StartLoading(SingletonComponent<WorldSetup>.Instance.InitCoroutine());
		}
		yield return CoroutineEx.waitForEndOfFrame;
		LoadingScreen.Update("Refreshing Decor");
		DecorSpawn.RefreshAll(false);
		FoliageGrid.RefreshAll(false);
		LoadBalancer.ProcessAll();
		LODGrid.RefreshAll();
		RendererGrid.RefreshAll();
		LoadingScreen.Update("Ready");
		LoadingScreen.Hide();
		MainMenuSystem.Hide();
		MapInterface.SetOpen(false);
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		this.LoadTime = new BenchmarkData.Result
		{
			Name = "Load Time",
			Seconds = (float)timer.Elapsed.TotalSeconds
		};
		MainCamera.position = msg.position;
		MainCamera.forward = msg.rotation;
		Application.isLoading = false;
		ConVar.Client.prediction = false;
		if (global::Client.DemoLocalClient == 0UL)
		{
			global::Client.DemoLocalClient = 76561197960279927UL;
		}
		yield break;
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x0000DB59 File Offset: 0x0000BD59
	public void StopRecordingDemo()
	{
		if (!global::Client.IsRecordingDemo)
		{
			return;
		}
		Net.cl.StopRecording();
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x00068934 File Offset: 0x00066B34
	public void StopPlayingDemo(bool finished = false)
	{
		if (!global::Client.IsPlayingDemo)
		{
			return;
		}
		Net.cl.StopPlayback();
		LevelManager.UnloadLevel();
		Debug.LogFormat("Demo Length:   {0:0.00}", new object[]
		{
			Net.cl.PlaybackStats.DemoLength.TotalSeconds
		});
		Debug.LogFormat("Time Taken:    {0:0.00}", new object[]
		{
			Net.cl.PlaybackStats.TotalTime.TotalSeconds
		});
		Debug.LogFormat("Packets:       {0}", new object[]
		{
			Net.cl.PlaybackStats.Packets
		});
		Debug.LogFormat("Frames:        {0}", new object[]
		{
			Net.cl.PlaybackStats.Frames
		});
		Debug.LogFormat("Framerate:     {0:0.00}", new object[]
		{
			(double)Net.cl.PlaybackStats.Frames / Net.cl.PlaybackStats.TotalTime.TotalSeconds
		});
		Debug.LogFormat("Avg Ms:        {0:0.00}", new object[]
		{
			1000.0 / ((double)Net.cl.PlaybackStats.Frames / Net.cl.PlaybackStats.TotalTime.TotalSeconds)
		});
		if (global::Client.IsBenchmarkDemo && finished)
		{
			Debug.Log("Report Demo Info to " + Application.Manifest.BenchmarkUrl);
			BenchmarkData benchmarkData = BenchmarkData.New();
			BenchmarkData.Result result = new BenchmarkData.Result
			{
				Name = "Playback",
				FrameCount = Net.cl.PlaybackStats.Frames,
				Avg = (float)(1000.0 / ((double)Net.cl.PlaybackStats.Frames / Net.cl.PlaybackStats.TotalTime.TotalSeconds)),
				Seconds = (float)Net.cl.PlaybackStats.TotalTime.TotalSeconds
			};
			benchmarkData.Results = new BenchmarkData.Result[]
			{
				this.LoadTime,
				result
			};
			string text = benchmarkData.Upload();
			Debug.Log("Returned: " + text);
			if (!CommandLine.HasSwitch("+autobench"))
			{
				if (text.StartsWith("http"))
				{
					Application.OpenURL(text);
				}
			}
			else
			{
				Application.Quit();
			}
		}
		UnityEngine.Time.timeScale = 1f;
		Net.cl.StopPlayback();
		global::Client.IsBenchmarkDemo = false;
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x00068B94 File Offset: 0x00066D94
	private void DemoFrame()
	{
		if (!global::Client.IsPlayingDemo)
		{
			return;
		}
		if (Net.cl.PlayingFinished)
		{
			this.StopPlayingDemo(true);
			return;
		}
		if (Application.isLoading)
		{
			return;
		}
		UnityEngine.Time.timeScale = Demo.timescale;
		long num = (long)((double)UnityEngine.Time.deltaTime * 1000.0);
		long num2 = num;
		if (Demo.IsTimeDemo)
		{
			num = 100L;
			num2 = 100L;
		}
		Net.cl.UpdatePlayback(num, num2);
		Facepunch.Input.Frame();
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x00068C04 File Offset: 0x00066E04
	private void DemoLateUpdate()
	{
		if (!global::Client.IsPlayingDemo)
		{
			return;
		}
		BasePlayer basePlayer = BasePlayer.FindByID_Clientside(global::Client.DemoLocalClient);
		if (basePlayer == null)
		{
			return;
		}
		if (!CameraMan.Active)
		{
			FirstPersonSpectatorMode.Apply(MainCamera.mainCamera, basePlayer);
		}
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x00068C40 File Offset: 0x00066E40
	private void DemoPlayerTick(PlayerTick playerTick)
	{
		if (!CameraMan.Active && MainCamera.Distance(playerTick.position) > 50f)
		{
			MainCamera.mainCamera.transform.position = playerTick.position;
			MainCamera.mainCamera.transform.rotation = Quaternion.Euler(playerTick.inputState.aimAngles);
		}
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x00068C9C File Offset: 0x00066E9C
	private void OnGroupEnter(Message msg)
	{
		uint num = msg.read.GroupID();
		if (!this.subscriptions.Add(num) && Global.developer > 0)
		{
			Debug.LogWarning("Trying to enter group we are already subscribed to: " + num);
		}
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x00068CE0 File Offset: 0x00066EE0
	private void OnGroupLeave(Message msg)
	{
		uint num = msg.read.GroupID();
		if (!this.subscriptions.Remove(num) && Global.developer > 0)
		{
			Debug.LogWarning("Trying to leave group we are not subscribed to: " + num);
		}
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x00068D24 File Offset: 0x00066F24
	private void OnEntityDestroy(Message msg)
	{
		uint uid = msg.read.UInt32();
		BaseNetworkable.DestroyMode mode = (BaseNetworkable.DestroyMode)msg.read.UInt8();
		BaseEntity baseEntity = BaseNetworkable.clientEntities.Find(uid) as BaseEntity;
		if (baseEntity == null)
		{
			return;
		}
		global::Client.CurrentEntity = baseEntity;
		baseEntity.DoDestroyEffects(mode, msg);
		baseEntity.NetworkDestroy(true);
		global::Client.CurrentEntity = null;
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x00068D80 File Offset: 0x00066F80
	private void OnGroupDestroy(Message msg)
	{
		uint uid = msg.read.GroupID();
		List<BaseNetworkable> list = Pool.GetList<BaseNetworkable>();
		BaseNetworkable.clientEntities.FindInGroup(uid, list);
		for (int i = 0; i < list.Count; i++)
		{
			BaseNetworkable baseNetworkable = list[i];
			if (!(baseNetworkable == null) && !(baseNetworkable.gameObject == null))
			{
				if (!baseNetworkable.ShouldDestroyWithGroup())
				{
					baseNetworkable.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 1, "skipping OnGroupDestroy - ShouldDestroyWithGroup == false");
				}
				else
				{
					baseNetworkable.NetworkDestroy(baseNetworkable.ShouldDestroyImmediately());
				}
			}
		}
		Pool.FreeList<BaseNetworkable>(ref list);
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x00068E08 File Offset: 0x00067008
	private void OnGroupChange(Message msg)
	{
		uint num = msg.read.EntityID();
		uint num2 = msg.read.GroupID();
		BaseNetworkable baseNetworkable = BaseNetworkable.clientEntities.Find(num);
		Group group = BaseNetworkable.clientEntities.FindGroup(num2);
		if (baseNetworkable != null)
		{
			if (Global.developer > 0 && !this.subscriptions.Contains(num2))
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Trying to move entity to group we are not subscribed to (entity ",
					num,
					" from group ",
					baseNetworkable.net.group.ID,
					" to group ",
					num2,
					")"
				}), baseNetworkable);
			}
			baseNetworkable.net.SwitchGroup(group);
			return;
		}
		if (Global.developer > 0)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Trying to update group on entity that does not exist (entity ",
				num,
				" to group ",
				num2,
				")"
			}));
		}
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x00068F10 File Offset: 0x00067110
	private void OnEntities(Message packet)
	{
		uint num = packet.read.UInt32();
		using (Entity entity = Entity.Deserialize(packet.read))
		{
			this.CreateOrUpdateEntity(entity, packet.read.Length);
			if (!global::Client.IsPlayingDemo)
			{
				Connection connection = packet.connection;
				connection.validate.entityUpdates = connection.validate.entityUpdates + 1U;
				if (packet.connection.validate.entityUpdates != num)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"Entities Out Of Order.. Got ",
						num,
						" expected ",
						packet.connection.validate.entityUpdates
					}));
					Debug.LogError("Entities Out Of Order");
					Net.cl.Disconnect("Entities Out Of Order", true);
					packet.connection.validate.entityUpdates = num;
				}
			}
		}
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x00069004 File Offset: 0x00067204
	private BaseEntity CreateOrUpdateEntity(Entity info, long size)
	{
		uint group = info.baseNetworkable.group;
		uint uid = info.baseNetworkable.uid;
		BaseEntity baseEntity = BaseNetworkable.clientEntities.Find(uid) as BaseEntity;
		if (baseEntity)
		{
			using (TimeWarning.New("UpdateEntity", 0.1f))
			{
				using (TimeWarning.New(baseEntity.PrefabName, 0.1f))
				{
					if (Global.developer > 0 && !this.subscriptions.Contains(group))
					{
						Debug.LogWarning(string.Concat(new object[]
						{
							"Trying to update entity in group we are not subscribed to (entity ",
							uid,
							" in group ",
							group,
							")"
						}), baseEntity);
					}
					if (this.StatsEnabled)
					{
						this.RegisterIncoming("EntityUpdate", baseEntity.ToString(), size);
					}
					using (TimeWarning.New("OnNetworkUpdate", 0.1f))
					{
						baseEntity.OnNetworkUpdate(info);
						return baseEntity;
					}
				}
			}
		}
		string text = StringPool.Get(info.baseNetworkable.prefabID);
		using (TimeWarning.New("CreateEntity", 0.1f))
		{
			if (Global.developer > 0 && !this.subscriptions.Contains(group))
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Trying to create entity in group we are not subscribed to (entity ",
					uid,
					" in group ",
					group,
					")"
				}));
			}
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Couldn't create network entity (\"",
					text,
					"\"/",
					info.baseNetworkable.prefabID,
					")"
				}));
				return null;
			}
			baseEntity = GameManager.client.CreateEntity(text, info.baseEntity.pos, Quaternion.Euler(info.baseEntity.rot), false);
			if (baseEntity == null)
			{
				Debug.LogWarning("Couldn't create network entity! (" + text + ")");
				return null;
			}
		}
		using (TimeWarning.New("SpawnEntity", 0.1f))
		{
			using (TimeWarning.New(text, 0.1f))
			{
				using (TimeWarning.New("ClientSpawn", 0.1f))
				{
					baseEntity.ClientSpawn(info);
				}
				using (TimeWarning.New("NetworkingStats", 0.1f))
				{
					if (this.StatsEnabled)
					{
						this.RegisterIncoming("EntityCreate", baseEntity.ToString(), size);
					}
				}
				using (TimeWarning.New("AwakeFromInstantiate", 0.1f))
				{
					baseEntity.gameObject.AwakeFromInstantiate();
				}
				using (TimeWarning.New("ClientOnEnable", 0.1f))
				{
					baseEntity.ClientOnEnable();
				}
			}
		}
		return baseEntity;
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x00069394 File Offset: 0x00067594
	private void OnEntityPosition(Message packet)
	{
		uint entID = packet.read.EntityID();
		Vector3 pos = packet.read.Vector3();
		Vector3 rot = packet.read.Vector3();
		float time = packet.read.Float();
		uint parentID = (packet.read.unread > 0) ? packet.read.EntityID() : 0U;
		this.UpdateEntityPosition(entID, pos, rot, time, parentID);
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x000693FC File Offset: 0x000675FC
	private void UpdateEntityPosition(uint entID, Vector3 pos, Vector3 rot, float time, uint parentID)
	{
		BaseEntity baseEntity = BaseNetworkable.clientEntities.Find(entID) as BaseEntity;
		if (baseEntity == null)
		{
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("EntityPosition", "Unknown", 36L);
			}
			return;
		}
		if (this.StatsEnabled)
		{
			this.RegisterIncoming("EntityPosition", baseEntity.ToString(), 36L);
		}
		if (baseEntity.parentEntity.uid == parentID)
		{
			baseEntity.OnPositionalFromNetwork(pos, rot, time);
		}
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x00069474 File Offset: 0x00067674
	private void OnEntityFlags(Message packet)
	{
		uint entID = packet.read.EntityID();
		int flags = packet.read.Int32();
		this.UpdateEntityFlags(entID, flags);
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x000694A4 File Offset: 0x000676A4
	private void UpdateEntityFlags(uint entID, int flags)
	{
		BaseEntity baseEntity = BaseNetworkable.clientEntities.Find(entID) as BaseEntity;
		if (baseEntity == null)
		{
			if (this.StatsEnabled)
			{
				this.RegisterIncoming("EntityFlags", "Unknown", 8L);
			}
			return;
		}
		if (this.StatsEnabled)
		{
			this.RegisterIncoming("EntityFlags", baseEntity.ToString(), 8L);
		}
		BaseEntity.Flags flags2 = baseEntity.flags;
		baseEntity.PreNetworkUpdate();
		baseEntity.flags = (BaseEntity.Flags)flags;
		baseEntity.OnFlagsChanged(flags2, (BaseEntity.Flags)flags);
		baseEntity.PostNetworkUpdate();
		baseEntity.gameObject.BroadcastOnPostNetworkUpdate(baseEntity);
	}
}
