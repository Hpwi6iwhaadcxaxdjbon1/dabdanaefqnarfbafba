using System;
using UnityEngine;

// Token: 0x0200053A RID: 1338
public class TerrainCarve : TerrainModifier
{
	// Token: 0x06001E28 RID: 7720 RVA: 0x00017E94 File Offset: 0x00016094
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.AlphaMap)
		{
			return;
		}
		TerrainMeta.AlphaMap.SetAlpha(position, 0f, opacity, radius, fade);
	}
}
