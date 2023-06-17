using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

// Token: 0x02000293 RID: 659
public class FileSystem_Warmup : MonoBehaviour
{
	// Token: 0x04000F45 RID: 3909
	private static bool run = true;

	// Token: 0x04000F46 RID: 3910
	private static bool running = false;

	// Token: 0x04000F47 RID: 3911
	private static string[] excludeFilter = new string[]
	{
		"/bundled/prefabs/autospawn/monument",
		"/bundled/prefabs/autospawn/mountain",
		"/bundled/prefabs/autospawn/canyon",
		"/bundled/prefabs/autospawn/decor",
		"/bundled/prefabs/navmesh",
		"/content/ui/",
		"/prefabs/ui/",
		"/prefabs/world/",
		"/prefabs/system/",
		"/standard assets/",
		"/third party/"
	};

	// Token: 0x060012A0 RID: 4768 RVA: 0x00079524 File Offset: 0x00077724
	public static void Run()
	{
		if (!FileSystem_Warmup.run || FileSystem_Warmup.running)
		{
			return;
		}
		FileSystem_Warmup.running = true;
		string[] assetList = FileSystem_Warmup.GetAssetList();
		for (int i = 0; i < assetList.Length; i++)
		{
			FileSystem_Warmup.PrefabWarmup(assetList[i]);
		}
		FileSystem_Warmup.running = (FileSystem_Warmup.run = false);
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x0000FFEF File Offset: 0x0000E1EF
	public static IEnumerator Run(float deltaTime, Action<string> statusFunction = null, string format = null)
	{
		if (!FileSystem_Warmup.run || FileSystem_Warmup.running)
		{
			yield break;
		}
		FileSystem_Warmup.running = true;
		string[] prewarmAssets = FileSystem_Warmup.GetAssetList();
		Stopwatch sw = Stopwatch.StartNew();
		int num;
		for (int i = 0; i < prewarmAssets.Length; i = num + 1)
		{
			if (sw.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == prewarmAssets.Length - 1)
			{
				if (statusFunction != null)
				{
					statusFunction.Invoke(string.Format((format != null) ? format : "{0}/{1}", i + 1, prewarmAssets.Length));
				}
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			FileSystem_Warmup.PrefabWarmup(prewarmAssets[i]);
			num = i;
		}
		FileSystem_Warmup.running = (FileSystem_Warmup.run = false);
		yield break;
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x00079570 File Offset: 0x00077770
	private static bool ShouldIgnore(string path)
	{
		for (int i = 0; i < FileSystem_Warmup.excludeFilter.Length; i++)
		{
			if (StringEx.Contains(path, FileSystem_Warmup.excludeFilter[i], 1))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x000795A4 File Offset: 0x000777A4
	private static string[] GetAssetList()
	{
		return Enumerable.ToArray<string>(Enumerable.Where<string>(Enumerable.Select<GameManifest.PrefabProperties, string>(GameManifest.Current.prefabProperties, (GameManifest.PrefabProperties x) => x.name), (string x) => !FileSystem_Warmup.ShouldIgnore(x)));
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x0001000C File Offset: 0x0000E20C
	private static void PrefabWarmup(string path)
	{
		GameManager.client.FindPrefab(path);
	}
}
