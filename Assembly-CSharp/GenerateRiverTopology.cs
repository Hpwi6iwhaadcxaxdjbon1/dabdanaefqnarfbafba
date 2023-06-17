using System;
using System.Collections.Generic;

// Token: 0x02000511 RID: 1297
public class GenerateRiverTopology : ProceduralComponent
{
	// Token: 0x06001DC9 RID: 7625 RVA: 0x000A2584 File Offset: 0x000A0784
	public override void Process(uint seed)
	{
		GenerateRiverTopology.<>c__DisplayClass0_0 CS$<>8__locals1 = new GenerateRiverTopology.<>c__DisplayClass0_0();
		List<PathList> rivers = TerrainMeta.Path.Rivers;
		CS$<>8__locals1.heightmap = TerrainMeta.HeightMap;
		CS$<>8__locals1.topomap = TerrainMeta.TopologyMap;
		foreach (PathList pathList in rivers)
		{
			pathList.Path.RecalculateTangents();
		}
		CS$<>8__locals1.heightmap.Push();
		foreach (PathList pathList2 in rivers)
		{
			pathList2.AdjustTerrainHeight();
			pathList2.AdjustTerrainTexture();
			pathList2.AdjustTerrainTopology();
		}
		CS$<>8__locals1.heightmap.Pop();
		int[] map = CS$<>8__locals1.topomap.dst;
		int res = CS$<>8__locals1.topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 49152, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 32768;
			}
			float normX = CS$<>8__locals1.topomap.Coordinate(x);
			float normZ = CS$<>8__locals1.topomap.Coordinate(y);
			if (CS$<>8__locals1.heightmap.GetSlope(normX, normZ) > 40f)
			{
				map[x * res + y] |= 2;
			}
		});
	}
}
