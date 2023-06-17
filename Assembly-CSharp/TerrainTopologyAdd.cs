using System;
using UnityEngine;

// Token: 0x02000540 RID: 1344
public class TerrainTopologyAdd : TerrainModifier
{
	// Token: 0x04001A92 RID: 6802
	[InspectorFlags]
	public TerrainTopology.Enum TopologyType = 512;

	// Token: 0x06001E37 RID: 7735 RVA: 0x00017F7E File Offset: 0x0001617E
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		TerrainMeta.TopologyMap.AddTopology(position, this.TopologyType, radius, fade);
	}
}
