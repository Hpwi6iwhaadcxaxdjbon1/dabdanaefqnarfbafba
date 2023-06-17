using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005F7 RID: 1527
public class StringPool
{
	// Token: 0x04001EB6 RID: 7862
	private static Dictionary<uint, string> toString;

	// Token: 0x04001EB7 RID: 7863
	private static Dictionary<string, uint> toNumber;

	// Token: 0x04001EB8 RID: 7864
	private static bool initialized;

	// Token: 0x04001EB9 RID: 7865
	public static uint closest;

	// Token: 0x06002247 RID: 8775 RVA: 0x000B7F58 File Offset: 0x000B6158
	private static void Init()
	{
		if (StringPool.initialized)
		{
			return;
		}
		StringPool.toString = new Dictionary<uint, string>();
		StringPool.toNumber = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
		GameManifest gameManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
		uint num = 0U;
		while ((ulong)num < (ulong)((long)gameManifest.pooledStrings.Length))
		{
			StringPool.toString.Add(gameManifest.pooledStrings[(int)num].hash, gameManifest.pooledStrings[(int)num].str);
			StringPool.toNumber.Add(gameManifest.pooledStrings[(int)num].str, gameManifest.pooledStrings[(int)num].hash);
			num += 1U;
		}
		StringPool.initialized = true;
		StringPool.closest = StringPool.Get("closest");
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x000B8014 File Offset: 0x000B6214
	public static string Get(uint i)
	{
		if (i == 0U)
		{
			return string.Empty;
		}
		StringPool.Init();
		string result;
		if (StringPool.toString.TryGetValue(i, ref result))
		{
			return result;
		}
		Debug.LogWarning("StringPool.GetString - no string for ID" + i);
		return "";
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000B805C File Offset: 0x000B625C
	public static uint Get(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0U;
		}
		StringPool.Init();
		uint result;
		if (StringPool.toNumber.TryGetValue(str, ref result))
		{
			return result;
		}
		Debug.LogWarning("StringPool.GetNumber - no number for string " + str);
		return 0U;
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x000B809C File Offset: 0x000B629C
	public static uint Add(string str)
	{
		uint num = 0U;
		if (!StringPool.toNumber.TryGetValue(str, ref num))
		{
			num = StringEx.ManifestHash(str);
			StringPool.toString.Add(num, str);
			StringPool.toNumber.Add(str, num);
		}
		return num;
	}
}
