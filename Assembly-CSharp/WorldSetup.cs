using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000536 RID: 1334
public class WorldSetup : SingletonComponent<WorldSetup>
{
	// Token: 0x04001A70 RID: 6768
	public bool AutomaticallySetup;

	// Token: 0x04001A71 RID: 6769
	public GameObject terrain;

	// Token: 0x04001A72 RID: 6770
	public GameObject decorPrefab;

	// Token: 0x04001A73 RID: 6771
	public GameObject grassPrefab;

	// Token: 0x04001A74 RID: 6772
	public GameObject spawnPrefab;

	// Token: 0x04001A75 RID: 6773
	private TerrainMeta terrainMeta;

	// Token: 0x04001A76 RID: 6774
	public uint EditorSeed;

	// Token: 0x04001A77 RID: 6775
	public uint EditorSalt;

	// Token: 0x04001A78 RID: 6776
	public uint EditorSize;

	// Token: 0x04001A79 RID: 6777
	public string EditorUrl = string.Empty;

	// Token: 0x04001A7A RID: 6778
	internal List<ProceduralObject> ProceduralObjects = new List<ProceduralObject>();

	// Token: 0x06001E17 RID: 7703 RVA: 0x000A4E60 File Offset: 0x000A3060
	private void OnValidate()
	{
		if (this.terrain == null)
		{
			Terrain terrain = Object.FindObjectOfType<Terrain>();
			if (terrain != null)
			{
				this.terrain = terrain.gameObject;
			}
		}
	}

	// Token: 0x06001E18 RID: 7704 RVA: 0x000A4E98 File Offset: 0x000A3098
	protected override void Awake()
	{
		base.Awake();
		foreach (Prefab prefab in Prefab.Load("assets/bundled/prefabs/world", null, null, true))
		{
			if (!(prefab.Object.GetComponent<BaseEntity>() != null))
			{
				prefab.Spawn(Vector3.zero, Quaternion.identity);
			}
		}
		SingletonComponent[] array2 = Object.FindObjectsOfType<SingletonComponent>();
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Setup();
		}
		if (this.terrain)
		{
			if (this.terrain.GetComponent<TerrainGenerator>())
			{
				global::World.Procedural = true;
			}
			else
			{
				global::World.Procedural = false;
				this.terrainMeta = this.terrain.GetComponent<TerrainMeta>();
				this.terrainMeta.Init(null, null);
				this.terrainMeta.SetupComponents();
				this.CreateObject(this.decorPrefab);
				this.CreateObject(this.grassPrefab);
				this.CreateObject(this.spawnPrefab);
			}
		}
		global::World.Serialization = new WorldSerialization();
		global::World.Cached = false;
		global::World.CleanupOldFiles();
		if (this.AutomaticallySetup)
		{
			base.StartCoroutine(this.InitCoroutine());
		}
	}

	// Token: 0x06001E19 RID: 7705 RVA: 0x000A4FB0 File Offset: 0x000A31B0
	protected void CreateObject(GameObject prefab)
	{
		if (prefab == null)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(prefab);
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
	}

	// Token: 0x06001E1A RID: 7706 RVA: 0x00017E2E File Offset: 0x0001602E
	public IEnumerator InitCoroutine()
	{
		if (global::World.CanLoadFromUrl())
		{
			Debug.Log("Loading custom map from " + global::World.Url);
		}
		else
		{
			Debug.Log(string.Concat(new object[]
			{
				"Generating procedural map of size ",
				global::World.Size,
				" with seed ",
				global::World.Seed
			}));
		}
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			GameManager.Destroy(SingletonComponent<SpawnHandler>.Instance.gameObject, 0f);
		}
		ProceduralComponent[] components = base.GetComponentsInChildren<ProceduralComponent>(true);
		Timing downloadTimer = Timing.Start("Downloading World");
		if (global::World.Procedural && !global::World.CanLoadFromDisk() && global::World.CanLoadFromUrl())
		{
			LoadingScreen.Update("DOWNLOADING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			UnityWebRequest request = UnityWebRequest.Get(global::World.Url);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.Send();
			while (!request.isDone)
			{
				LoadingScreen.Update("DOWNLOADING WORLD " + (request.downloadProgress * 100f).ToString("0.0") + "%");
				yield return CoroutineEx.waitForEndOfFrame;
			}
			if (!request.isHttpError && !request.isNetworkError)
			{
				File.WriteAllBytes(global::World.MapFolderName + "/" + global::World.MapFileName, request.downloadHandler.data);
			}
			else
			{
				this.CancelSetup(string.Concat(new string[]
				{
					"Couldn't Download Level: ",
					global::World.Name,
					" (",
					request.error,
					")"
				}));
			}
			request = null;
		}
		downloadTimer.End();
		Timing loadTimer = Timing.Start("Loading World");
		if (global::World.Procedural && global::World.CanLoadFromDisk())
		{
			LoadingScreen.Update("LOADING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			global::World.Serialization.Load(global::World.MapFolderName + "/" + global::World.MapFileName);
			global::World.Cached = true;
		}
		loadTimer.End();
		if (global::World.Cached && 8U != global::World.Serialization.Version)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"World cache version mismatch: ",
				8U,
				" != ",
				global::World.Serialization.Version
			}));
			global::World.Serialization.Clear();
			global::World.Cached = false;
			if (global::World.CanLoadFromUrl())
			{
				this.CancelSetup("World File Outdated: " + global::World.Name);
			}
		}
		if (global::World.Cached && global::World.Checksum != global::World.Serialization.Checksum)
		{
			Debug.LogWarning("World cache checksum mismatch: " + global::World.Checksum + " != " + global::World.Serialization.Checksum);
			global::World.Serialization.Clear();
			global::World.Cached = false;
			if (global::World.CanLoadFromUrl())
			{
				this.CancelSetup("World File Mismatch: " + global::World.Name);
			}
		}
		if (this.terrain)
		{
			TerrainGenerator component2 = this.terrain.GetComponent<TerrainGenerator>();
			if (component2)
			{
				this.terrain = component2.CreateTerrain();
				this.terrainMeta = this.terrain.GetComponent<TerrainMeta>();
				this.terrainMeta.Init(null, null);
				this.terrainMeta.SetupComponents();
				this.CreateObject(this.decorPrefab);
				this.CreateObject(this.grassPrefab);
				this.CreateObject(this.spawnPrefab);
			}
		}
		Timing spawnTimer = Timing.Start("Spawning World");
		if (global::World.Cached)
		{
			LoadingScreen.Update("SPAWNING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			TerrainMeta.HeightMap.FromByteArray(global::World.GetMap("terrain"));
			TerrainMeta.SplatMap.FromByteArray(global::World.GetMap("splat"));
			TerrainMeta.BiomeMap.FromByteArray(global::World.GetMap("biome"));
			TerrainMeta.TopologyMap.FromByteArray(global::World.GetMap("topology"));
			TerrainMeta.AlphaMap.FromByteArray(global::World.GetMap("alpha"));
			TerrainMeta.WaterMap.FromByteArray(global::World.GetMap("water"));
			IEnumerator worldSpawn = global::World.Spawn(0.2f, delegate(string str)
			{
				LoadingScreen.Update(str);
			});
			while (worldSpawn.MoveNext())
			{
				object obj = worldSpawn.Current;
				yield return obj;
			}
			TerrainMeta.Path.Clear();
			TerrainMeta.Path.Roads.AddRange(global::World.GetPaths("Road"));
			TerrainMeta.Path.Rivers.AddRange(global::World.GetPaths("River"));
			TerrainMeta.Path.Powerlines.AddRange(global::World.GetPaths("Powerline"));
			worldSpawn = null;
		}
		spawnTimer.End();
		Timing procgenTimer = Timing.Start("Processing World");
		if (components.Length != 0)
		{
			int num;
			for (int i = 0; i < components.Length; i = num + 1)
			{
				ProceduralComponent component = components[i];
				if (component && component.ShouldRun())
				{
					uint seed = (uint)((ulong)global::World.Seed + (ulong)((long)i));
					LoadingScreen.Update(component.Description.ToUpper());
					yield return CoroutineEx.waitForEndOfFrame;
					yield return CoroutineEx.waitForEndOfFrame;
					yield return CoroutineEx.waitForEndOfFrame;
					if (component)
					{
						component.Process(seed);
					}
					component = null;
				}
				num = i;
			}
		}
		procgenTimer.End();
		Timing saveTimer = Timing.Start("Saving World");
		if (ConVar.World.cache && global::World.Procedural && !global::World.Cached)
		{
			LoadingScreen.Update("SAVING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			global::World.Serialization.world.size = global::World.Size;
			global::World.AddPaths(TerrainMeta.Path.Roads);
			global::World.AddPaths(TerrainMeta.Path.Rivers);
			global::World.AddPaths(TerrainMeta.Path.Powerlines);
			global::World.Serialization.Save(global::World.MapFolderName + "/" + global::World.MapFileName);
		}
		saveTimer.End();
		Timing checksumTimer = Timing.Start("Calculating Checksum");
		if (string.IsNullOrEmpty(global::World.Serialization.Checksum))
		{
			LoadingScreen.Update("CALCULATING CHECKSUM");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			global::World.Serialization.CalculateChecksum();
		}
		checksumTimer.End();
		if (global::World.Checksum != global::World.Serialization.Checksum)
		{
			ErrorLogger.CaptureLog("Checksum does not match", "", LogType.Error);
			Debug.Log("Checksum does not match\n\tClient=" + global::World.Serialization.Checksum + "\n\tServer=" + global::World.Checksum);
		}
		Timing finalizeTimer = Timing.Start("Finalizing World");
		LoadingScreen.Update("FINALIZING WORLD");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (this.terrainMeta)
		{
			this.terrainMeta.BindShaderProperties();
			this.terrainMeta.PostSetupComponents();
			TerrainMargin.Create();
		}
		global::World.Serialization.Clear();
		finalizeTimer.End();
		LoadingScreen.Update("DONE");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (this)
		{
			GameManager.Destroy(base.gameObject, 0f);
		}
		yield break;
	}

	// Token: 0x06001E1B RID: 7707 RVA: 0x00017E3D File Offset: 0x0001603D
	private void CancelSetup(string msg)
	{
		Net.cl.Disconnect(msg, true);
	}
}
