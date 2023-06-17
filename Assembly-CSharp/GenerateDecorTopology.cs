using System;
using UnityEngine;

// Token: 0x02000503 RID: 1283
public class GenerateDecorTopology : ProceduralComponent
{
	// Token: 0x040019FA RID: 6650
	public bool KeepExisting = true;

	// Token: 0x06001DA8 RID: 7592 RVA: 0x000A18D0 File Offset: 0x0009FAD0
	public override void Process(uint seed)
	{
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int topores = topomap.res;
		Parallel.For(0, topores, delegate(int z)
		{
			for (int i = 0; i < topores; i++)
			{
				if (topomap.GetTopology(i, z, 4194306))
				{
					topomap.AddTopology(i, z, 512);
				}
				else if (!this.KeepExisting)
				{
					topomap.RemoveTopology(i, z, 512);
				}
			}
		});
	}
}
