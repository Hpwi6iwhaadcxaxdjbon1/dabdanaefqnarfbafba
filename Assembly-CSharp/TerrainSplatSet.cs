using System;
using UnityEngine;

// Token: 0x0200053F RID: 1343
public class TerrainSplatSet : TerrainModifier
{
	// Token: 0x04001A91 RID: 6801
	public TerrainSplat.Enum SplatType;

	// Token: 0x06001E35 RID: 7733 RVA: 0x00017F5A File Offset: 0x0001615A
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.SplatMap)
		{
			return;
		}
		TerrainMeta.SplatMap.SetSplat(position, this.SplatType, opacity, radius, fade);
	}
}
