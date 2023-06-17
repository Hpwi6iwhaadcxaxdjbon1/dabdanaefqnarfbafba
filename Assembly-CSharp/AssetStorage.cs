using System;
using UnityEngine;

// Token: 0x02000735 RID: 1845
public static class AssetStorage
{
	// Token: 0x06002840 RID: 10304 RVA: 0x0001F63B File Offset: 0x0001D83B
	public static void Save<T>(ref T asset, string path) where T : Object
	{
		asset;
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x00002ECE File Offset: 0x000010CE
	public static void Save(ref Texture2D asset)
	{
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x0001F64E File Offset: 0x0001D84E
	public static void Save(ref Texture2D asset, string path, bool linear, bool compress)
	{
		asset;
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x00002ECE File Offset: 0x000010CE
	public static void Load<T>(ref T asset, string path) where T : Object
	{
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x0001F658 File Offset: 0x0001D858
	public static void Delete<T>(ref T asset) where T : Object
	{
		if (!asset)
		{
			return;
		}
		Object.Destroy(asset);
		asset = default(T);
	}
}
