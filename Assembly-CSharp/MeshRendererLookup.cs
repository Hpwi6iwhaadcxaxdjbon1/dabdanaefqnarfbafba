using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class MeshRendererLookup
{
	// Token: 0x04000C8B RID: 3211
	public MeshRendererLookup.LookupGroup src = new MeshRendererLookup.LookupGroup();

	// Token: 0x04000C8C RID: 3212
	public MeshRendererLookup.LookupGroup dst = new MeshRendererLookup.LookupGroup();

	// Token: 0x06000F96 RID: 3990 RVA: 0x0006AF74 File Offset: 0x00069174
	public void Apply()
	{
		MeshRendererLookup.LookupGroup lookupGroup = this.src;
		this.src = this.dst;
		this.dst = lookupGroup;
		this.dst.Clear();
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0000DDC5 File Offset: 0x0000BFC5
	public void Clear()
	{
		this.dst.Clear();
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x0000DDD2 File Offset: 0x0000BFD2
	public void Add(MeshRendererInstance instance)
	{
		this.dst.Add(instance);
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x0000DDE0 File Offset: 0x0000BFE0
	public MeshRendererLookup.LookupEntry Get(int index)
	{
		return this.src.Get(index);
	}

	// Token: 0x020001F1 RID: 497
	public class LookupGroup
	{
		// Token: 0x04000C8D RID: 3213
		public List<MeshRendererLookup.LookupEntry> data = new List<MeshRendererLookup.LookupEntry>();

		// Token: 0x06000F9B RID: 3995 RVA: 0x0000DE0C File Offset: 0x0000C00C
		public void Clear()
		{
			this.data.Clear();
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x0000DE19 File Offset: 0x0000C019
		public void Add(MeshRendererInstance instance)
		{
			this.data.Add(new MeshRendererLookup.LookupEntry(instance));
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x0000DE2C File Offset: 0x0000C02C
		public MeshRendererLookup.LookupEntry Get(int index)
		{
			return this.data[index];
		}
	}

	// Token: 0x020001F2 RID: 498
	public struct LookupEntry
	{
		// Token: 0x04000C8E RID: 3214
		public Renderer renderer;

		// Token: 0x06000F9F RID: 3999 RVA: 0x0000DE4D File Offset: 0x0000C04D
		public LookupEntry(MeshRendererInstance instance)
		{
			this.renderer = instance.renderer;
		}
	}
}
