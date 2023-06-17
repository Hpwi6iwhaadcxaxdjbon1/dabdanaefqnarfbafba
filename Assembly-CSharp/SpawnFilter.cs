using System;
using UnityEngine;

// Token: 0x02000499 RID: 1177
[Serializable]
public class SpawnFilter
{
	// Token: 0x04001845 RID: 6213
	[InspectorFlags]
	public TerrainSplat.Enum SplatType = -1;

	// Token: 0x04001846 RID: 6214
	[InspectorFlags]
	public TerrainBiome.Enum BiomeType = -1;

	// Token: 0x04001847 RID: 6215
	[InspectorFlags]
	public TerrainTopology.Enum TopologyAny = -1;

	// Token: 0x04001848 RID: 6216
	[InspectorFlags]
	public TerrainTopology.Enum TopologyAll;

	// Token: 0x04001849 RID: 6217
	[InspectorFlags]
	public TerrainTopology.Enum TopologyNot;

	// Token: 0x06001B4C RID: 6988 RVA: 0x0001673A File Offset: 0x0001493A
	public bool Test(Vector3 worldPos)
	{
		return this.GetFactor(worldPos) > 0.5f;
	}

	// Token: 0x06001B4D RID: 6989 RVA: 0x0001674A File Offset: 0x0001494A
	public bool Test(float normX, float normZ)
	{
		return this.GetFactor(normX, normZ) > 0.5f;
	}

	// Token: 0x06001B4E RID: 6990 RVA: 0x00097A44 File Offset: 0x00095C44
	public float GetFactor(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetFactor(normX, normZ);
	}

	// Token: 0x06001B4F RID: 6991 RVA: 0x00097A74 File Offset: 0x00095C74
	public float GetFactor(float normX, float normZ)
	{
		if (TerrainMeta.TopologyMap == null)
		{
			return 0f;
		}
		int splatType = this.SplatType;
		int biomeType = this.BiomeType;
		int topologyAny = this.TopologyAny;
		int topologyAll = this.TopologyAll;
		int topologyNot = this.TopologyNot;
		if (topologyAny == 0)
		{
			Debug.LogError("Empty topology filter is invalid.");
		}
		else if (topologyAny != -1 || topologyAll != 0 || topologyNot != 0)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(normX, normZ);
			if (topologyAny != -1 && (topology & topologyAny) == 0)
			{
				return 0f;
			}
			if (topologyNot != 0 && (topology & topologyNot) != 0)
			{
				return 0f;
			}
			if (topologyAll != 0 && (topology & topologyAll) != topologyAll)
			{
				return 0f;
			}
		}
		if (biomeType == 0)
		{
			Debug.LogError("Empty biome filter is invalid.");
		}
		else if (biomeType != -1 && (TerrainMeta.BiomeMap.GetBiomeMaxType(normX, normZ, -1) & biomeType) == 0)
		{
			return 0f;
		}
		if (splatType == 0)
		{
			Debug.LogError("Empty splat filter is invalid.");
		}
		else if (splatType != -1)
		{
			return TerrainMeta.SplatMap.GetSplat(normX, normZ, splatType);
		}
		return 1f;
	}
}
