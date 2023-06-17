using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading;
using Facepunch;
using Facepunch.Crypt;
using Facepunch.Network.Raknet;
using GameAnalyticsSDK;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000360 RID: 864
public class Bootstrap : SingletonComponent<Bootstrap>
{
	// Token: 0x04001328 RID: 4904
	internal static bool bootstrapInitRun = false;

	// Token: 0x04001329 RID: 4905
	public static bool isErrored = false;

	// Token: 0x0400132A RID: 4906
	public string messageString = "Loading...";

	// Token: 0x0400132B RID: 4907
	public GameObject errorPanel;

	// Token: 0x0400132C RID: 4908
	public Text errorText;

	// Token: 0x0400132D RID: 4909
	public Text statusText;

	// Token: 0x0400132E RID: 4910
	private TimeSince timeSinceVisible;

	// Token: 0x0400132F RID: 4911
	private static string[] ClientScenes = new string[]
	{
		"UIScene",
		"UIMainMenu",
		"UIIngameHUD",
		"UIIngameSleeping",
		"UIIngameDeath",
		"UIInventoryMenu",
		"UICraftingMenu",
		"UIIngameOverlay",
		"UIPieMenu",
		"UIProgressBar",
		"Netgraph"
	};

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x0600162F RID: 5679 RVA: 0x00012BF5 File Offset: 0x00010DF5
	public static bool needsSetup
	{
		get
		{
			return !Bootstrap.bootstrapInitRun;
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06001630 RID: 5680 RVA: 0x00012BFF File Offset: 0x00010DFF
	public static bool isPresent
	{
		get
		{
			return Bootstrap.bootstrapInitRun || Enumerable.Count<GameSetup>(Object.FindObjectsOfType<GameSetup>()) > 0;
		}
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x00086260 File Offset: 0x00084460
	public static void RunDefaults()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		Application.targetFrameRate = 100;
		Texture.SetGlobalAnisotropicFilteringLimits(1, 16);
		Shader.EnableKeyword("TERRAIN_PARALLAX_OFFSET");
		Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
		AudioConfiguration configuration = AudioSettings.GetConfiguration();
		if (configuration.speakerMode == 1)
		{
			configuration.speakerMode = 2;
			AudioSettings.Reset(configuration);
		}
		Time.fixedDeltaTime = 0.03125f;
		Time.maximumDeltaTime = 0.5f;
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x000862E0 File Offset: 0x000844E0
	public static void Init_Tier0()
	{
		Bootstrap.RunDefaults();
		GameSetup.RunOnce = true;
		Bootstrap.bootstrapInitRun = true;
		ConsoleSystem.Index.Initialize(ConsoleGen.All);
		UnityButtons.Register();
		Output.Install();
		Pool.ResizeBuffer<Networkable>(65536);
		Pool.ResizeBuffer<EntityLink>(65536);
		Pool.ResizeBuffer<RendererGroup>(4096);
		Pool.FillBuffer<Networkable>(int.MaxValue);
		Pool.FillBuffer<EntityLink>(int.MaxValue);
		Pool.FillBuffer<RendererGroup>(int.MaxValue);
		Bootstrap.NetworkInit();
		string text = CommandLine.Full.Replace(CommandLine.GetSwitch("-rcon.password", CommandLine.GetSwitch("+rcon.password", "RCONPASSWORD")), "******");
		Bootstrap.WriteToLog("Command Line: " + text);
		Global.FindPrefab = ((string str) => GameManager.client.FindPrefab(str));
		Global.CreatePrefab = ((string str) => GameManager.client.CreatePrefab(str, false));
		Global.OpenMainMenu = delegate()
		{
			LevelManager.UnloadLevel();
		};
		ConsoleSystem.ClientCanRunAdminCommands = (() => !Net.cl.IsConnected() || Net.cl.IsPlaying || (!(LocalPlayer.Entity == null) && LocalPlayer.Entity.IsAdmin));
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x0008641C File Offset: 0x0008461C
	public static void Init_Systems()
	{
		SteamClient.Init();
		Translate.Init();
		if (SteamClient.localSteamID > 0UL)
		{
			GameAnalytics.SetCustomId(Md5.Calculate(SteamClient.localSteamID.ToString()));
		}
		GameAnalytics.Initialize();
		Application.Initialize(new Integration());
		PerformanceUI.SpawnPrefab();
		Performance.GetMemoryUsage = (() => SystemInfoEx.systemMemoryUsed);
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x00012C1A File Offset: 0x00010E1A
	public static void Init_Config()
	{
		ConsoleNetwork.Init();
		ConsoleSystem.UpdateValuesFromCommandLine();
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "readcfg", Array.Empty<object>());
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x00012C3B File Offset: 0x00010E3B
	public static void NetworkInit()
	{
		Net.cl = new Facepunch.Network.Raknet.Client();
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x00012C47 File Offset: 0x00010E47
	private IEnumerator Start()
	{
		Bootstrap.WriteToLog("Bootstrap Startup");
		this.timeSinceVisible = 0f;
		Bootstrap.WriteToLog(SystemInfoGeneralText.currentInfo);
		Texture.SetGlobalAnisotropicFilteringLimits(1, 16);
		ErrorLogger.Install();
		if (!CommandLine.HasSwitch("-nosteam") && !SteamClient.InitSteamClient())
		{
			this.ThrowError("Steam Load Error - is Steam open?\nPlease exit and try again with Steam open.");
			yield break;
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Loading Bundles"));
		FileSystem.Backend = new AssetBundleBackend("Bundles/Bundles");
		if (FileSystem.Backend.isError)
		{
			this.ThrowError(FileSystem.Backend.loadingError);
		}
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Loading Game Manifest"));
		GameManifest.Load();
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("DONE!"));
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Running Self Check"));
		SelfCheck.Run();
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Tier0"));
		Bootstrap.Init_Tier0();
		ConsoleSystem.UpdateValuesFromCommandLine();
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Systems"));
		Bootstrap.Init_Systems();
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Config"));
		Bootstrap.Init_Config();
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Loading Items"));
		ItemManager.Initialize();
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return base.StartCoroutine(this.ClientStartup());
		GameManager.Destroy(base.gameObject, 0f);
		yield break;
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x00012C56 File Offset: 0x00010E56
	public static IEnumerator LoadUI(bool LoadMenu = true)
	{
		foreach (string text in Bootstrap.ClientScenes)
		{
			if ((LoadMenu || !(text == "UIMainMenu")) && !SceneManager.GetSceneByName(text).isLoaded)
			{
				SceneManager.LoadScene(text, LoadSceneMode.Additive);
			}
		}
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		foreach (string text2 in Bootstrap.ClientScenes)
		{
			if (LoadMenu || !(text2 == "UIMainMenu"))
			{
				Scene sceneByName = SceneManager.GetSceneByName(text2);
				if (!sceneByName.isLoaded)
				{
					Debug.LogWarning("Scene didn't load!? " + text2);
				}
				GameObject[] rootGameObjects = sceneByName.GetRootGameObjects();
				for (int j = 0; j < rootGameObjects.Length; j++)
				{
					Object.DontDestroyOnLoad(rootGameObjects[j]);
				}
				SceneManager.UnloadSceneAsync(sceneByName);
			}
		}
		yield break;
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x00012C65 File Offset: 0x00010E65
	private IEnumerator ClientStartup()
	{
		this.messageString = "Loading Default Settings..";
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/manager_cursor.prefab", true));
		this.StartupShared();
		Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/client.prefab", true));
		if (!Object.FindObjectOfType<Performance>())
		{
			Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/performance.prefab", true));
		}
		if (!Object.FindObjectOfType<SoundManager>())
		{
			Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/sound_manager.prefab", true));
		}
		if (!Object.FindObjectOfType<MusicManager>())
		{
			Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/music.prefab", true));
		}
		yield return base.StartCoroutine(EAC.DoStartup());
		this.messageString = "...";
		while (this.timeSinceVisible < 10f && !Input.anyKeyDown)
		{
			yield return null;
		}
		this.messageString = "Loading Main Menu..";
		SceneManager.LoadScene("Empty", LoadSceneMode.Single);
		yield return base.StartCoroutine(Bootstrap.LoadUI(true));
		LevelManager.UnloadLevel();
		LoadingScreen.Hide();
		if (CommandLine.HasSwitch("+connect"))
		{
			ConnectionScreen.SetStatus("connecting");
			ConnectionScreen.Show();
			string @switch = CommandLine.GetSwitch("+connect", "missing");
			Bootstrap.WriteToLog("Command Line Connect To: " + @switch);
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "client.connect", new object[]
			{
				@switch
			});
		}
		if (CommandLine.HasSwitch("+autobench"))
		{
			yield return CoroutineEx.waitForSeconds(1f);
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "timedemo", new object[]
			{
				"benchmark"
			});
		}
		yield break;
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x00012C74 File Offset: 0x00010E74
	protected void Update()
	{
		if (this.statusText)
		{
			this.statusText.text = this.messageString;
		}
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x00012C94 File Offset: 0x00010E94
	private void StartupShared()
	{
		ItemManager.Initialize();
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x00012C9B File Offset: 0x00010E9B
	public void ThrowError(string error)
	{
		Debug.Log("ThrowError: " + error);
		this.errorPanel.SetActive(true);
		this.errorText.text = error;
		Bootstrap.isErrored = true;
	}

	// Token: 0x0600163C RID: 5692 RVA: 0x00012CCB File Offset: 0x00010ECB
	public void ExitGame()
	{
		Debug.Log("Exiting due to Exit Game button on bootstrap error panel");
		Application.Quit();
	}

	// Token: 0x0600163D RID: 5693 RVA: 0x00012CDC File Offset: 0x00010EDC
	public static IEnumerator LoadingUpdate(string str)
	{
		if (!SingletonComponent<Bootstrap>.Instance)
		{
			yield break;
		}
		SingletonComponent<Bootstrap>.Instance.messageString = str;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield break;
	}

	// Token: 0x0600163E RID: 5694 RVA: 0x00012CEB File Offset: 0x00010EEB
	public static void WriteToLog(string str)
	{
		DebugEx.Log(str, StackTraceLogType.None);
	}
}
