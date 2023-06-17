using System;
using Facepunch.Extend;
using UnityEngine;

// Token: 0x020004C7 RID: 1223
[RequireComponent(typeof(TerrainMeta))]
public abstract class TerrainExtension : MonoBehaviour
{
	// Token: 0x0400190D RID: 6413
	[NonSerialized]
	public bool isInitialized;

	// Token: 0x0400190E RID: 6414
	internal Terrain terrain;

	// Token: 0x0400190F RID: 6415
	internal TerrainConfig config;

	// Token: 0x06001C12 RID: 7186 RVA: 0x00016FCA File Offset: 0x000151CA
	public void Init(Terrain terrain, TerrainConfig config)
	{
		this.terrain = terrain;
		this.config = config;
	}

	// Token: 0x06001C13 RID: 7187 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void Setup()
	{
	}

	// Token: 0x06001C14 RID: 7188 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void PostSetup()
	{
	}

	// Token: 0x06001C15 RID: 7189 RVA: 0x00016FDA File Offset: 0x000151DA
	public void LogSize(object obj, ulong size)
	{
		Debug.Log(obj.GetType() + " allocated: " + NumberExtensions.FormatBytes<ulong>(size, false));
	}
}
