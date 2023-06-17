using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ConVar;
using UnityEngine;

// Token: 0x0200049E RID: 1182
public static class World
{
	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06001B75 RID: 7029 RVA: 0x00016870 File Offset: 0x00014A70
	// (set) Token: 0x06001B76 RID: 7030 RVA: 0x00016877 File Offset: 0x00014A77
	public static uint Seed { get; set; }

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06001B77 RID: 7031 RVA: 0x0001687F File Offset: 0x00014A7F
	// (set) Token: 0x06001B78 RID: 7032 RVA: 0x00016886 File Offset: 0x00014A86
	public static uint Size { get; set; }

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x06001B79 RID: 7033 RVA: 0x0001688E File Offset: 0x00014A8E
	// (set) Token: 0x06001B7A RID: 7034 RVA: 0x00016895 File Offset: 0x00014A95
	public static string Checksum { get; set; }

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06001B7B RID: 7035 RVA: 0x0001689D File Offset: 0x00014A9D
	// (set) Token: 0x06001B7C RID: 7036 RVA: 0x000168A4 File Offset: 0x00014AA4
	public static string Url { get; set; }

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06001B7D RID: 7037 RVA: 0x000168AC File Offset: 0x00014AAC
	// (set) Token: 0x06001B7E RID: 7038 RVA: 0x000168B3 File Offset: 0x00014AB3
	public static bool Procedural { get; set; }

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06001B7F RID: 7039 RVA: 0x000168BB File Offset: 0x00014ABB
	// (set) Token: 0x06001B80 RID: 7040 RVA: 0x000168C2 File Offset: 0x00014AC2
	public static bool Cached { get; set; }

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06001B81 RID: 7041 RVA: 0x000168CA File Offset: 0x00014ACA
	// (set) Token: 0x06001B82 RID: 7042 RVA: 0x000168D1 File Offset: 0x00014AD1
	public static WorldSerialization Serialization { get; set; }

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x06001B83 RID: 7043 RVA: 0x000168D9 File Offset: 0x00014AD9
	public static string Name
	{
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return Path.GetFileNameWithoutExtension(WWW.UnEscapeURL(global::World.Url));
			}
			return Application.loadedLevelName;
		}
	}

	// Token: 0x06001B84 RID: 7044 RVA: 0x000168F7 File Offset: 0x00014AF7
	public static bool CanLoadFromUrl()
	{
		return !string.IsNullOrEmpty(global::World.Url);
	}

	// Token: 0x06001B85 RID: 7045 RVA: 0x00016906 File Offset: 0x00014B06
	public static bool CanLoadFromDisk()
	{
		return File.Exists(global::World.MapFolderName + "/" + global::World.MapFileName);
	}

	// Token: 0x06001B86 RID: 7046 RVA: 0x000987BC File Offset: 0x000969BC
	public static void CleanupOldFiles()
	{
		Regex regex1 = new Regex("proceduralmap\\.[0-9]+\\.[0-9]+\\.[0-9]+\\.map");
		Regex regex2 = new Regex("\\.[0-9]+\\.[0-9]+\\." + 176 + "\\.map");
		foreach (string text in Enumerable.Where<string>(Directory.GetFiles(global::World.MapFolderName, "*.map"), (string path) => regex1.IsMatch(path) && !regex2.IsMatch(path)))
		{
			try
			{
				File.Delete(text);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
			}
		}
	}

	// Token: 0x1700018D RID: 397
	// (get) Token: 0x06001B87 RID: 7047 RVA: 0x00098878 File Offset: 0x00096A78
	public static string MapFileName
	{
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return global::World.Name + ".map";
			}
			return string.Concat(new object[]
			{
				global::World.Name.Replace(" ", "").ToLower(),
				".",
				global::World.Size,
				".",
				global::World.Seed,
				".",
				176,
				".map"
			});
		}
	}

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x06001B88 RID: 7048 RVA: 0x00016921 File Offset: 0x00014B21
	public static string MapFolderName
	{
		get
		{
			return ConVar.Client.GetClientFolder("maps");
		}
	}

	// Token: 0x06001B89 RID: 7049 RVA: 0x0009890C File Offset: 0x00096B0C
	public static byte[] GetMap(string name)
	{
		WorldSerialization.MapData map = global::World.Serialization.GetMap(name);
		if (map == null)
		{
			return null;
		}
		return map.data;
	}

	// Token: 0x06001B8A RID: 7050 RVA: 0x0001692D File Offset: 0x00014B2D
	public static void AddMap(string name, byte[] data)
	{
		global::World.Serialization.AddMap(name, data);
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x0001693B File Offset: 0x00014B3B
	public static void AddPrefab(string category, uint id, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		global::World.Serialization.AddPrefab(category, id, position, rotation, scale);
		if (!global::World.Cached)
		{
			global::World.Spawn(category, id, position, rotation, scale);
		}
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x00098930 File Offset: 0x00096B30
	public static WorldSerialization.PathData PathListToPathData(PathList src)
	{
		WorldSerialization.PathData pathData = new WorldSerialization.PathData();
		pathData.name = src.Name;
		pathData.spline = src.Spline;
		pathData.start = src.Start;
		pathData.end = src.End;
		pathData.width = src.Width;
		pathData.innerPadding = src.InnerPadding;
		pathData.outerPadding = src.OuterPadding;
		pathData.innerFade = src.InnerFade;
		pathData.outerFade = src.OuterFade;
		pathData.randomScale = src.RandomScale;
		pathData.meshOffset = src.MeshOffset;
		pathData.terrainOffset = src.TerrainOffset;
		pathData.splat = src.Splat;
		pathData.topology = src.Topology;
		pathData.nodes = Array.ConvertAll<Vector3, WorldSerialization.VectorData>(src.Path.Points, (Vector3 item) => item);
		return pathData;
	}

	// Token: 0x06001B8D RID: 7053 RVA: 0x00098A20 File Offset: 0x00096C20
	public static PathList PathDataToPathList(WorldSerialization.PathData src)
	{
		PathList pathList = new PathList(src.name, Array.ConvertAll<WorldSerialization.VectorData, Vector3>(src.nodes, (WorldSerialization.VectorData item) => item));
		pathList.Spline = src.spline;
		pathList.Start = src.start;
		pathList.End = src.end;
		pathList.Width = src.width;
		pathList.InnerPadding = src.innerPadding;
		pathList.OuterPadding = src.outerPadding;
		pathList.InnerFade = src.innerFade;
		pathList.OuterFade = src.outerFade;
		pathList.RandomScale = src.randomScale;
		pathList.MeshOffset = src.meshOffset;
		pathList.TerrainOffset = src.terrainOffset;
		pathList.Splat = src.splat;
		pathList.Topology = src.topology;
		pathList.Path.RecalculateTangents();
		return pathList;
	}

	// Token: 0x06001B8E RID: 7054 RVA: 0x0001695F File Offset: 0x00014B5F
	public static IEnumerable<PathList> GetPaths(string name)
	{
		return Enumerable.Select<WorldSerialization.PathData, PathList>(global::World.Serialization.GetPaths(name), (WorldSerialization.PathData p) => global::World.PathDataToPathList(p));
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x00098B0C File Offset: 0x00096D0C
	public static void AddPaths(IEnumerable<PathList> paths)
	{
		foreach (PathList path in paths)
		{
			global::World.AddPath(path);
		}
	}

	// Token: 0x06001B90 RID: 7056 RVA: 0x00016990 File Offset: 0x00014B90
	public static void AddPath(PathList path)
	{
		global::World.Serialization.AddPath(global::World.PathListToPathData(path));
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x000169A2 File Offset: 0x00014BA2
	public static IEnumerator Spawn(float deltaTime, Action<string> statusFunction = null)
	{
		Stopwatch sw = Stopwatch.StartNew();
		int num;
		for (int i = 0; i < global::World.Serialization.world.prefabs.Count; i = num + 1)
		{
			if (sw.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == global::World.Serialization.world.prefabs.Count - 1)
			{
				global::World.Status(statusFunction, "Spawning World ({0}/{1})", i + 1, global::World.Serialization.world.prefabs.Count);
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			global::World.Spawn(global::World.Serialization.world.prefabs[i]);
			num = i;
		}
		yield break;
	}

	// Token: 0x06001B92 RID: 7058 RVA: 0x00098B54 File Offset: 0x00096D54
	public static void Spawn()
	{
		for (int i = 0; i < global::World.Serialization.world.prefabs.Count; i++)
		{
			global::World.Spawn(global::World.Serialization.world.prefabs[i]);
		}
	}

	// Token: 0x06001B93 RID: 7059 RVA: 0x000169B8 File Offset: 0x00014BB8
	private static void Spawn(WorldSerialization.PrefabData prefab)
	{
		global::World.Spawn(prefab.category, prefab.id, prefab.position, prefab.rotation, prefab.scale);
	}

	// Token: 0x06001B94 RID: 7060 RVA: 0x00098B9C File Offset: 0x00096D9C
	private static void Spawn(string category, uint id, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		GameObject gameObject = Prefab.DefaultManager.CreatePrefab(StringPool.Get(id), position, rotation, scale, true);
		if (gameObject)
		{
			gameObject.SetHierarchyGroup(category, true, false);
		}
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x000169EC File Offset: 0x00014BEC
	private static void Status(Action<string> statusFunction, string status, object obj1)
	{
		if (statusFunction != null)
		{
			statusFunction.Invoke(string.Format(status, obj1));
		}
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x000169FE File Offset: 0x00014BFE
	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2)
	{
		if (statusFunction != null)
		{
			statusFunction.Invoke(string.Format(status, obj1, obj2));
		}
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x00016A11 File Offset: 0x00014C11
	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2, object obj3)
	{
		if (statusFunction != null)
		{
			statusFunction.Invoke(string.Format(status, obj1, obj2, obj3));
		}
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x00016A26 File Offset: 0x00014C26
	private static void Status(Action<string> statusFunction, string status, params object[] objs)
	{
		if (statusFunction != null)
		{
			statusFunction.Invoke(string.Format(status, objs));
		}
	}
}
