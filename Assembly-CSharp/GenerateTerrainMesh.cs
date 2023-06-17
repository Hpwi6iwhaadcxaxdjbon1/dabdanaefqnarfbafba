using System;

// Token: 0x02000521 RID: 1313
public class GenerateTerrainMesh : ProceduralComponent
{
	// Token: 0x06001DEB RID: 7659 RVA: 0x000A3398 File Offset: 0x000A1598
	public override void Process(uint seed)
	{
		if (!World.Cached)
		{
			World.AddMap("terrain", TerrainMeta.HeightMap.ToByteArray());
		}
		TerrainMeta.HeightMap.ApplyToTerrain();
		TerrainMeta.HeightMap.GenerateTextures(false, true);
		if (World.Cached)
		{
			TerrainMeta.HeightMap.FromByteArray(World.GetMap("height"));
		}
	}

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x06001DEC RID: 7660 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
