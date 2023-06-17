using System;

// Token: 0x02000501 RID: 1281
public class GenerateClutterTopology : ProceduralComponent
{
	// Token: 0x06001DA4 RID: 7588 RVA: 0x000A1874 File Offset: 0x0009FA74
	public override void Process(uint seed)
	{
		int[] map = TerrainMeta.TopologyMap.dst;
		int res = TerrainMeta.TopologyMap.res;
		ImageProcessing.Dilate2D(map, res, res, 16777728, 3, delegate(int x, int y)
		{
			if ((map[x * res + y] & 512) == 0)
			{
				map[x * res + y] |= 16777216;
			}
		});
	}
}
