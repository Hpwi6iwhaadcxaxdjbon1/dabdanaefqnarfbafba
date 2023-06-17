using System;
using System.Collections;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000394 RID: 916
public class GameSetup : MonoBehaviour
{
	// Token: 0x04001415 RID: 5141
	public static bool RunOnce;

	// Token: 0x04001416 RID: 5142
	public bool startServer = true;

	// Token: 0x04001417 RID: 5143
	public string clientConnectCommand = "client.connect 127.0.0.1:28015";

	// Token: 0x04001418 RID: 5144
	public bool loadMenu = true;

	// Token: 0x04001419 RID: 5145
	public bool loadLevel;

	// Token: 0x0400141A RID: 5146
	public string loadLevelScene = "";

	// Token: 0x0400141B RID: 5147
	public bool loadSave;

	// Token: 0x0400141C RID: 5148
	public string loadSaveFile = "";

	// Token: 0x0600175D RID: 5981 RVA: 0x0008A284 File Offset: 0x00088484
	protected void Awake()
	{
		if (GameSetup.RunOnce)
		{
			GameManager.Destroy(base.gameObject, 0f);
			return;
		}
		GameManifest.Load();
		GameManifest.LoadAssets();
		GameSetup.RunOnce = true;
		if (Bootstrap.needsSetup)
		{
			Bootstrap.Init_Tier0();
			Bootstrap.Init_Systems();
			Bootstrap.Init_Config();
		}
		base.StartCoroutine(this.DoGameSetup());
	}

	// Token: 0x0600175E RID: 5982 RVA: 0x00013A0B File Offset: 0x00011C0B
	private IEnumerator DoGameSetup()
	{
		Application.isLoading = true;
		TerrainMeta.InitNoTerrain();
		ItemManager.Initialize();
		LevelManager.CurrentLevelName = SceneManager.GetActiveScene().name;
		if (!Object.FindObjectOfType<CursorManager>())
		{
			Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/manager_cursor.prefab", true));
		}
		if (!Object.FindObjectOfType<Performance>())
		{
			Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/performance.prefab", true));
		}
		if (!Object.FindObjectOfType<MusicManager>())
		{
			Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/music.prefab", true));
		}
		if (!Object.FindObjectOfType<SoundManager>())
		{
			Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/sound_manager.prefab", true));
		}
		yield return base.StartCoroutine(Bootstrap.LoadUI(this.loadMenu));
		LoadingScreen.Show();
		LoadingScreen.Update("LOADING GAME");
		this.ClientJoin();
		yield return null;
		LoadingScreen.Hide();
		yield return null;
		Application.isLoading = false;
		yield break;
	}

	// Token: 0x0600175F RID: 5983 RVA: 0x0008A2DC File Offset: 0x000884DC
	private void ClientJoin()
	{
		if (!Object.FindObjectOfType<global::Client>())
		{
			Object.DontDestroyOnLoad(GameManager.client.CreatePrefab("assets/bundled/prefabs/system/client.prefab", true));
		}
		if (this.clientConnectCommand.Length > 0)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, this.clientConnectCommand, Array.Empty<object>());
		}
	}
}
