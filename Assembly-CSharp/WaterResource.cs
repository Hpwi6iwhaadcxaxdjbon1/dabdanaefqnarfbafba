using System;
using UnityEngine;

// Token: 0x02000413 RID: 1043
public class WaterResource
{
	// Token: 0x06001963 RID: 6499 RVA: 0x00015043 File Offset: 0x00013243
	public static ItemDefinition GetAtPoint(Vector3 pos)
	{
		return ItemManager.FindItemDefinition(WaterResource.IsFreshWater(pos) ? "water" : "water.salt");
	}

	// Token: 0x06001964 RID: 6500 RVA: 0x0001505E File Offset: 0x0001325E
	public static bool IsFreshWater(Vector3 pos)
	{
		return !(TerrainMeta.TopologyMap == null) && TerrainMeta.TopologyMap.GetTopology(pos, 245760);
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x0008FF74 File Offset: 0x0008E174
	public static ItemDefinition Merge(ItemDefinition first, ItemDefinition second)
	{
		if (first == second)
		{
			return first;
		}
		if (first.shortname == "water.salt" || second.shortname == "water.salt")
		{
			return ItemManager.FindItemDefinition("water.salt");
		}
		return ItemManager.FindItemDefinition("water");
	}
}
