using System;
using System.Collections.Generic;

// Token: 0x0200051D RID: 1309
public class GenerateRoadTopology : ProceduralComponent
{
	// Token: 0x06001DE3 RID: 7651 RVA: 0x000A312C File Offset: 0x000A132C
	public override void Process(uint seed)
	{
		GenerateRoadTopology.<>c__DisplayClass0_0 CS$<>8__locals1 = new GenerateRoadTopology.<>c__DisplayClass0_0();
		List<PathList> roads = TerrainMeta.Path.Roads;
		CS$<>8__locals1.heightmap = TerrainMeta.HeightMap;
		CS$<>8__locals1.topomap = TerrainMeta.TopologyMap;
		foreach (PathList pathList in roads)
		{
			pathList.Path.RecalculateTangents();
		}
		CS$<>8__locals1.heightmap.Push();
		foreach (PathList pathList2 in roads)
		{
			pathList2.AdjustTerrainHeight();
			pathList2.AdjustTerrainTexture();
			pathList2.AdjustTerrainTopology();
		}
		CS$<>8__locals1.heightmap.Pop();
		int[] map = CS$<>8__locals1.topomap.dst;
		int res = CS$<>8__locals1.topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 6144, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 4096;
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
