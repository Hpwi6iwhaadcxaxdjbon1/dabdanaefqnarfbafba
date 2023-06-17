using System;
using UnityEngine;

// Token: 0x0200076D RID: 1901
[Serializable]
public struct LayerSelect
{
	// Token: 0x04002476 RID: 9334
	[SerializeField]
	private int layer;

	// Token: 0x06002965 RID: 10597 RVA: 0x00020351 File Offset: 0x0001E551
	public LayerSelect(int layer)
	{
		this.layer = layer;
	}

	// Token: 0x06002966 RID: 10598 RVA: 0x0002035A File Offset: 0x0001E55A
	public static implicit operator int(LayerSelect layer)
	{
		return layer.layer;
	}

	// Token: 0x06002967 RID: 10599 RVA: 0x00020362 File Offset: 0x0001E562
	public static implicit operator LayerSelect(int layer)
	{
		return new LayerSelect(layer);
	}

	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06002968 RID: 10600 RVA: 0x0002036A File Offset: 0x0001E56A
	public int Mask
	{
		get
		{
			return 1 << this.layer;
		}
	}

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x06002969 RID: 10601 RVA: 0x00020377 File Offset: 0x0001E577
	public string Name
	{
		get
		{
			return LayerMask.LayerToName(this.layer);
		}
	}
}
