using System;
using UnityEngine;

// Token: 0x020004C6 RID: 1222
public class TerrainColors : TerrainExtension
{
	// Token: 0x0400190B RID: 6411
	private TerrainSplatMap splatMap;

	// Token: 0x0400190C RID: 6412
	private TerrainBiomeMap biomeMap;

	// Token: 0x06001C0E RID: 7182 RVA: 0x00016FA6 File Offset: 0x000151A6
	public override void Setup()
	{
		this.splatMap = this.terrain.GetComponent<TerrainSplatMap>();
		this.biomeMap = this.terrain.GetComponent<TerrainBiomeMap>();
	}

	// Token: 0x06001C0F RID: 7183 RVA: 0x0009A9F8 File Offset: 0x00098BF8
	public Color GetColor(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetColor(normX, normZ, mask);
	}

	// Token: 0x06001C10 RID: 7184 RVA: 0x0009AA28 File Offset: 0x00098C28
	public Color GetColor(float normX, float normZ, int mask = -1)
	{
		float biome = this.biomeMap.GetBiome(normX, normZ, 1);
		float biome2 = this.biomeMap.GetBiome(normX, normZ, 2);
		float biome3 = this.biomeMap.GetBiome(normX, normZ, 4);
		float biome4 = this.biomeMap.GetBiome(normX, normZ, 8);
		int num = TerrainSplat.TypeToIndex(this.splatMap.GetSplatMaxType(normX, normZ, mask));
		TerrainConfig.SplatType splatType = this.config.Splats[num];
		return biome * splatType.AridColor + biome2 * splatType.TemperateColor + biome3 * splatType.TundraColor + biome4 * splatType.ArcticColor;
	}
}
