using System;
using UnityEngine;

// Token: 0x020004FD RID: 1277
public class GenerateCliffSplat : ProceduralComponent
{
	// Token: 0x040019F0 RID: 6640
	private const int filter = 8389632;

	// Token: 0x06001D98 RID: 7576 RVA: 0x000A15DC File Offset: 0x0009F7DC
	public static void Process(int x, int z)
	{
		TerrainSplatMap splatMap = TerrainMeta.SplatMap;
		float normZ = splatMap.Coordinate(z);
		float normX = splatMap.Coordinate(x);
		if ((TerrainMeta.TopologyMap.GetTopology(normX, normZ) & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
			if (slope > 30f)
			{
				splatMap.SetSplat(x, z, 8, Mathf.InverseLerp(30f, 50f, slope));
			}
		}
	}

	// Token: 0x06001D99 RID: 7577 RVA: 0x000A1644 File Offset: 0x0009F844
	public override void Process(uint seed)
	{
		TerrainSplatMap splatMap = TerrainMeta.SplatMap;
		int splatres = splatMap.res;
		Parallel.For(0, splatres, delegate(int z)
		{
			for (int i = 0; i < splatres; i++)
			{
				GenerateCliffSplat.Process(i, z);
			}
		});
	}
}
