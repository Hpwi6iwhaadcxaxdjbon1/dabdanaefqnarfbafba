using System;
using UnityEngine;

// Token: 0x02000541 RID: 1345
public class TerrainTopologySet : TerrainModifier
{
	// Token: 0x04001A93 RID: 6803
	[InspectorFlags]
	public TerrainTopology.Enum TopologyType = 512;

	// Token: 0x06001E39 RID: 7737 RVA: 0x00017FB4 File Offset: 0x000161B4
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		TerrainMeta.TopologyMap.SetTopology(position, this.TopologyType, radius, fade);
	}
}
