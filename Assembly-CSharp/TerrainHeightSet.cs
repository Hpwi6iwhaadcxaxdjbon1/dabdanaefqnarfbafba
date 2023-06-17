using System;
using UnityEngine;

// Token: 0x0200053C RID: 1340
public class TerrainHeightSet : TerrainModifier
{
	// Token: 0x06001E2C RID: 7724 RVA: 0x00017F02 File Offset: 0x00016102
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.HeightMap)
		{
			return;
		}
		TerrainMeta.HeightMap.SetHeight(position, opacity, radius, fade);
	}
}
