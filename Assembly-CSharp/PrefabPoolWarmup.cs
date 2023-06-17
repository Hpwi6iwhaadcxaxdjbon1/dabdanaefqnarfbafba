using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x0200076A RID: 1898
public class PrefabPoolWarmup
{
	// Token: 0x06002956 RID: 10582 RVA: 0x000D2F0C File Offset: 0x000D110C
	public static void Run()
	{
		if (Application.isLoadingPrefabs)
		{
			return;
		}
		Application.isLoadingPrefabs = true;
		string[] assetList = PrefabPoolWarmup.GetAssetList();
		for (int i = 0; i < assetList.Length; i++)
		{
			PrefabPoolWarmup.PrefabWarmup(assetList[i]);
		}
		Application.isLoadingPrefabs = false;
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x00020309 File Offset: 0x0001E509
	public static IEnumerator Run(float deltaTime, Action<string> statusFunction = null, string format = null)
	{
		if (Application.isLoadingPrefabs)
		{
			yield break;
		}
		Application.isLoadingPrefabs = true;
		string[] prewarmAssets = PrefabPoolWarmup.GetAssetList();
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
			PrefabPoolWarmup.PrefabWarmup(prewarmAssets[i]);
			num = i;
		}
		Application.isLoadingPrefabs = false;
		yield break;
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x000D2F4C File Offset: 0x000D114C
	private static string[] GetAssetList()
	{
		return Enumerable.ToArray<string>(Enumerable.Select<GameManifest.PrefabProperties, string>(Enumerable.Where<GameManifest.PrefabProperties>(GameManifest.Current.prefabProperties, (GameManifest.PrefabProperties x) => x.pool), (GameManifest.PrefabProperties x) => x.name));
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x000D2FB0 File Offset: 0x000D11B0
	private static void PrefabWarmup(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			GameObject gameObject = GameManager.client.FindPrefab(path);
			if (gameObject != null && gameObject.SupportsPooling())
			{
				int clientCount = gameObject.GetComponent<Poolable>().ClientCount;
				List<GameObject> list = new List<GameObject>();
				for (int i = 0; i < clientCount; i++)
				{
					list.Add(GameManager.client.CreatePrefab(path, true));
				}
				for (int j = 0; j < clientCount; j++)
				{
					GameManager.client.Retire(list[j]);
				}
			}
		}
	}
}
