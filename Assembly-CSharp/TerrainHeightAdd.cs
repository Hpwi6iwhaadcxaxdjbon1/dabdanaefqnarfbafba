using System;
using UnityEngine;

// Token: 0x0200053B RID: 1339
public class TerrainHeightAdd : TerrainModifier
{
	// Token: 0x04001A8D RID: 6797
	public float Delta = 1f;

	// Token: 0x06001E2A RID: 7722 RVA: 0x00017EBF File Offset: 0x000160BF
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.HeightMap)
		{
			return;
		}
		TerrainMeta.HeightMap.AddHeight(position, opacity * this.Delta * TerrainMeta.OneOverSize.y, radius, fade);
	}
}
