using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x020005CB RID: 1483
public class GameManifest : ScriptableObject
{
	// Token: 0x04001DCC RID: 7628
	public GameManifest.PooledString[] pooledStrings;

	// Token: 0x04001DCD RID: 7629
	public GameManifest.MeshColliderInfo[] meshColliders;

	// Token: 0x04001DCE RID: 7630
	public GameManifest.PrefabProperties[] prefabProperties;

	// Token: 0x04001DCF RID: 7631
	public GameManifest.EffectCategory[] effectCategories;

	// Token: 0x04001DD0 RID: 7632
	public string[] skinnables;

	// Token: 0x04001DD1 RID: 7633
	public string[] entities;

	// Token: 0x04001DD2 RID: 7634
	internal static GameManifest loadedManifest;

	// Token: 0x04001DD3 RID: 7635
	internal static Dictionary<string, string> guidToPath = new Dictionary<string, string>();

	// Token: 0x04001DD4 RID: 7636
	internal static Dictionary<string, Object> guidToObject = new Dictionary<string, Object>();

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x060021E2 RID: 8674 RVA: 0x0001AF03 File Offset: 0x00019103
	public static GameManifest Current
	{
		get
		{
			if (GameManifest.loadedManifest != null)
			{
				return GameManifest.loadedManifest;
			}
			GameManifest.Load();
			return GameManifest.loadedManifest;
		}
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x000B6F44 File Offset: 0x000B5144
	public static void Load()
	{
		if (GameManifest.loadedManifest != null)
		{
			return;
		}
		GameManifest.loadedManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
		foreach (GameManifest.PrefabProperties prefabProperties in GameManifest.loadedManifest.prefabProperties)
		{
			GameManifest.guidToPath.Add(prefabProperties.guid, prefabProperties.name);
		}
		DebugEx.Log(GameManifest.GetMetadataStatus(), StackTraceLogType.None);
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x0001AF22 File Offset: 0x00019122
	public static void LoadAssets()
	{
		if (Skinnable.All != null)
		{
			return;
		}
		Skinnable.All = GameManifest.LoadSkinnableAssets();
		DebugEx.Log(GameManifest.GetAssetStatus(), StackTraceLogType.None);
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x000B6FB0 File Offset: 0x000B51B0
	private static Skinnable[] LoadSkinnableAssets()
	{
		string[] array = GameManifest.loadedManifest.skinnables;
		Skinnable[] array2 = new Skinnable[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = FileSystem.Load<Skinnable>(array[i], true);
		}
		return array2;
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x000B6FEC File Offset: 0x000B51EC
	internal static Dictionary<string, string[]> LoadEffectDictionary()
	{
		GameManifest.EffectCategory[] array = GameManifest.loadedManifest.effectCategories;
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		foreach (GameManifest.EffectCategory effectCategory in array)
		{
			dictionary.Add(effectCategory.folder, effectCategory.prefabs.ToArray());
		}
		return dictionary;
	}

	// Token: 0x060021E7 RID: 8679 RVA: 0x000B7034 File Offset: 0x000B5234
	internal static string GUIDToPath(string guid)
	{
		if (string.IsNullOrEmpty(guid))
		{
			Debug.LogError("GUIDToPath: guid is empty");
			return string.Empty;
		}
		GameManifest.Load();
		string result;
		if (GameManifest.guidToPath.TryGetValue(guid, ref result))
		{
			return result;
		}
		Debug.LogWarning("GUIDToPath: no path found for guid " + guid);
		return string.Empty;
	}

	// Token: 0x060021E8 RID: 8680 RVA: 0x000B7084 File Offset: 0x000B5284
	internal static Object GUIDToObject(string guid)
	{
		Object result = null;
		if (GameManifest.guidToObject.TryGetValue(guid, ref result))
		{
			return result;
		}
		string text = GameManifest.GUIDToPath(guid);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogWarning("Missing file for guid " + guid);
			GameManifest.guidToObject.Add(guid, null);
			return null;
		}
		Object @object = FileSystem.Load<Object>(text, true);
		GameManifest.guidToObject.Add(guid, @object);
		return @object;
	}

	// Token: 0x060021E9 RID: 8681 RVA: 0x000B70E8 File Offset: 0x000B52E8
	private static string GetMetadataStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManifest.loadedManifest != null)
		{
			stringBuilder.Append("Manifest Metadata Loaded");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.pooledStrings.Length.ToString());
			stringBuilder.Append(" pooled strings");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.meshColliders.Length.ToString());
			stringBuilder.Append(" mesh colliders");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.prefabProperties.Length.ToString());
			stringBuilder.Append(" prefab properties");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.effectCategories.Length.ToString());
			stringBuilder.Append(" effect categories");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.entities.Length.ToString());
			stringBuilder.Append(" entity names");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.skinnables.Length.ToString());
			stringBuilder.Append(" skinnable names");
		}
		else
		{
			stringBuilder.Append("Manifest Metadata Missing");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060021EA RID: 8682 RVA: 0x000B7288 File Offset: 0x000B5488
	private static string GetAssetStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManifest.loadedManifest != null)
		{
			stringBuilder.Append("Manifest Assets Loaded");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append((Skinnable.All != null) ? Skinnable.All.Length.ToString() : "0");
			stringBuilder.Append(" skinnable objects");
		}
		else
		{
			stringBuilder.Append("Manifest Assets Missing");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x020005CC RID: 1484
	[Serializable]
	public struct PooledString
	{
		// Token: 0x04001DD5 RID: 7637
		[HideInInspector]
		public string str;

		// Token: 0x04001DD6 RID: 7638
		public uint hash;
	}

	// Token: 0x020005CD RID: 1485
	[Serializable]
	public class MeshColliderInfo
	{
		// Token: 0x04001DD7 RID: 7639
		[HideInInspector]
		public string name;

		// Token: 0x04001DD8 RID: 7640
		public uint hash;

		// Token: 0x04001DD9 RID: 7641
		public PhysicMaterial physicMaterial;
	}

	// Token: 0x020005CE RID: 1486
	[Serializable]
	public class PrefabProperties
	{
		// Token: 0x04001DDA RID: 7642
		[HideInInspector]
		public string name;

		// Token: 0x04001DDB RID: 7643
		public string guid;

		// Token: 0x04001DDC RID: 7644
		public uint hash;

		// Token: 0x04001DDD RID: 7645
		public bool pool;
	}

	// Token: 0x020005CF RID: 1487
	[Serializable]
	public class EffectCategory
	{
		// Token: 0x04001DDE RID: 7646
		[HideInInspector]
		public string folder;

		// Token: 0x04001DDF RID: 7647
		public List<string> prefabs;
	}
}
