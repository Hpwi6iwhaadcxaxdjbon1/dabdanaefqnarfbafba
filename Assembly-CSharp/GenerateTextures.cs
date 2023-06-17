using System;

// Token: 0x02000522 RID: 1314
public class GenerateTextures : ProceduralComponent
{
	// Token: 0x06001DEE RID: 7662 RVA: 0x000A33F4 File Offset: 0x000A15F4
	public override void Process(uint seed)
	{
		if (!World.Cached)
		{
			World.AddMap("height", TerrainMeta.HeightMap.ToByteArray());
			World.AddMap("splat", TerrainMeta.SplatMap.ToByteArray());
			World.AddMap("biome", TerrainMeta.BiomeMap.ToByteArray());
			World.AddMap("topology", TerrainMeta.TopologyMap.ToByteArray());
			World.AddMap("alpha", TerrainMeta.AlphaMap.ToByteArray());
			World.AddMap("water", TerrainMeta.WaterMap.ToByteArray());
		}
		TerrainMeta.SplatMap.GenerateTextures();
		TerrainMeta.SplatMap.ApplyTextures();
		TerrainMeta.BiomeMap.GenerateTextures();
		TerrainMeta.BiomeMap.ApplyTextures();
		TerrainMeta.TopologyMap.GenerateTextures();
		TerrainMeta.TopologyMap.ApplyTextures();
		TerrainMeta.AlphaMap.GenerateTextures();
		TerrainMeta.AlphaMap.ApplyTextures();
		TerrainMeta.HeightMap.GenerateTextures(true, false);
		TerrainMeta.HeightMap.ApplyTextures();
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x06001DEF RID: 7663 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
