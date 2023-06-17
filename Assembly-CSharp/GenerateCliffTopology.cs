using System;
using UnityEngine;

// Token: 0x020004FF RID: 1279
public class GenerateCliffTopology : ProceduralComponent
{
	// Token: 0x040019F2 RID: 6642
	public bool KeepExisting = true;

	// Token: 0x040019F3 RID: 6643
	private const int filter = 8389632;

	// Token: 0x040019F4 RID: 6644
	private const int filter_del = 55296;

	// Token: 0x06001D9D RID: 7581 RVA: 0x000A16AC File Offset: 0x0009F8AC
	public static void Process(int x, int z)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float normZ = topologyMap.Coordinate(z);
		float normX = topologyMap.Coordinate(x);
		if ((topologyMap.GetTopology(x, z) & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
			float splat = TerrainMeta.SplatMap.GetSplat(normX, normZ, 8);
			if (slope > 40f || splat > 0.4f)
			{
				topologyMap.AddTopology(x, z, 2);
				return;
			}
			if (slope < 20f && splat < 0.2f)
			{
				topologyMap.RemoveTopology(x, z, 2);
			}
		}
	}

	// Token: 0x06001D9E RID: 7582 RVA: 0x000A1730 File Offset: 0x0009F930
	private static void Process(int x, int z, bool keepExisting)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float normZ = topologyMap.Coordinate(z);
		float normX = topologyMap.Coordinate(x);
		int topology = topologyMap.GetTopology(x, z);
		if ((topology & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
			float splat = TerrainMeta.SplatMap.GetSplat(normX, normZ, 8);
			if (slope > 40f || splat > 0.4f)
			{
				topologyMap.AddTopology(x, z, 2);
				return;
			}
			if (!keepExisting && slope < 20f && splat < 0.2f && (topology & 55296) != 0)
			{
				topologyMap.RemoveTopology(x, z, 2);
			}
		}
	}

	// Token: 0x06001D9F RID: 7583 RVA: 0x000A17C8 File Offset: 0x0009F9C8
	public override void Process(uint seed)
	{
		int[] map = TerrainMeta.TopologyMap.dst;
		int res = TerrainMeta.TopologyMap.res;
		Parallel.For(0, res, delegate(int z)
		{
			for (int i = 0; i < res; i++)
			{
				GenerateCliffTopology.Process(i, z, this.KeepExisting);
			}
		});
		ImageProcessing.Dilate2D(map, res, res, 4194306, 1, delegate(int x, int y)
		{
			if ((map[x * res + y] & 2) == 0)
			{
				map[x * res + y] |= 4194304;
			}
		});
	}
}
