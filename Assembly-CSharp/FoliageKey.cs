using System;
using UnityEngine;

// Token: 0x0200038F RID: 911
public struct FoliageKey : IEquatable<FoliageKey>
{
	// Token: 0x040013F9 RID: 5113
	public Material material;

	// Token: 0x0600173A RID: 5946 RVA: 0x00013815 File Offset: 0x00011A15
	public FoliageKey(Material material)
	{
		this.material = material;
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x0001381E File Offset: 0x00011A1E
	public FoliageKey(FoliageRenderer renderer)
	{
		this.material = renderer.material;
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x0001382C File Offset: 0x00011A2C
	public override int GetHashCode()
	{
		return this.material.GetHashCode();
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x00013839 File Offset: 0x00011A39
	public override bool Equals(object other)
	{
		return other is FoliageKey && this.Equals((FoliageKey)other);
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x00013851 File Offset: 0x00011A51
	public bool Equals(FoliageKey other)
	{
		return this.material == other.material;
	}
}
