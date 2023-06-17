using System;
using UnityEngine;

// Token: 0x0200054D RID: 1357
public abstract class TerrainPlacement : PrefabAttribute
{
	// Token: 0x04001AD4 RID: 6868
	[ReadOnly]
	public Vector3 size = Vector3.zero;

	// Token: 0x04001AD5 RID: 6869
	[ReadOnly]
	public Vector3 extents = Vector3.zero;

	// Token: 0x04001AD6 RID: 6870
	[ReadOnly]
	public Vector3 offset = Vector3.zero;

	// Token: 0x04001AD7 RID: 6871
	public bool HeightMap = true;

	// Token: 0x04001AD8 RID: 6872
	public bool AlphaMap = true;

	// Token: 0x04001AD9 RID: 6873
	public bool WaterMap;

	// Token: 0x04001ADA RID: 6874
	[InspectorFlags]
	public TerrainSplat.Enum SplatMask;

	// Token: 0x04001ADB RID: 6875
	[InspectorFlags]
	public TerrainBiome.Enum BiomeMask;

	// Token: 0x04001ADC RID: 6876
	[InspectorFlags]
	public TerrainTopology.Enum TopologyMask;

	// Token: 0x04001ADD RID: 6877
	[HideInInspector]
	public Texture2D heightmap;

	// Token: 0x04001ADE RID: 6878
	[HideInInspector]
	public Texture2D splatmap0;

	// Token: 0x04001ADF RID: 6879
	[HideInInspector]
	public Texture2D splatmap1;

	// Token: 0x04001AE0 RID: 6880
	[HideInInspector]
	public Texture2D alphamap;

	// Token: 0x04001AE1 RID: 6881
	[HideInInspector]
	public Texture2D biomemap;

	// Token: 0x04001AE2 RID: 6882
	[HideInInspector]
	public Texture2D topologymap;

	// Token: 0x04001AE3 RID: 6883
	[HideInInspector]
	public Texture2D watermap;

	// Token: 0x04001AE4 RID: 6884
	[HideInInspector]
	public Texture2D blendmap;

	// Token: 0x06001E5D RID: 7773 RVA: 0x000A77FC File Offset: 0x000A59FC
	[ContextMenu("Refresh Terrain Data")]
	public void RefreshTerrainData()
	{
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		TerrainHeightMap component = Terrain.activeTerrain.GetComponent<TerrainHeightMap>();
		if (component)
		{
			this.heightmap = component.HeightTexture;
		}
		TerrainSplatMap component2 = Terrain.activeTerrain.GetComponent<TerrainSplatMap>();
		if (component2)
		{
			this.splatmap0 = component2.SplatTexture0;
			this.splatmap1 = component2.SplatTexture1;
		}
		TerrainAlphaMap component3 = Terrain.activeTerrain.GetComponent<TerrainAlphaMap>();
		if (component3)
		{
			this.alphamap = component3.AlphaTexture;
		}
		TerrainBiomeMap component4 = Terrain.activeTerrain.GetComponent<TerrainBiomeMap>();
		if (component4)
		{
			this.biomemap = component4.BiomeTexture;
		}
		TerrainTopologyMap component5 = Terrain.activeTerrain.GetComponent<TerrainTopologyMap>();
		if (component5)
		{
			this.topologymap = component5.TopologyTexture;
		}
		TerrainWaterMap component6 = Terrain.activeTerrain.GetComponent<TerrainWaterMap>();
		if (component6)
		{
			this.watermap = component6.WaterTexture;
		}
		TerrainBlendMap component7 = Terrain.activeTerrain.GetComponent<TerrainBlendMap>();
		if (component7)
		{
			this.blendmap = component7.BlendTexture;
		}
		this.size = terrainData.size;
		this.extents = terrainData.size * 0.5f;
		this.offset = Terrain.activeTerrain.GetPosition() + Vector3Ex.XZ3D(terrainData.size) * 0.5f - base.transform.position;
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x000A7964 File Offset: 0x000A5B64
	public void Apply(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.ShouldHeight())
		{
			this.ApplyHeight(localToWorld, worldToLocal);
		}
		if (this.ShouldSplat(-1))
		{
			this.ApplySplat(localToWorld, worldToLocal);
		}
		if (this.ShouldAlpha())
		{
			this.ApplyAlpha(localToWorld, worldToLocal);
		}
		if (this.ShouldBiome(-1))
		{
			this.ApplyBiome(localToWorld, worldToLocal);
		}
		if (this.ShouldTopology(-1))
		{
			this.ApplyTopology(localToWorld, worldToLocal);
		}
		if (this.ShouldWater())
		{
			this.ApplyWater(localToWorld, worldToLocal);
		}
	}

	// Token: 0x06001E5F RID: 7775 RVA: 0x00018010 File Offset: 0x00016210
	protected bool ShouldHeight()
	{
		return this.heightmap != null && this.HeightMap;
	}

	// Token: 0x06001E60 RID: 7776 RVA: 0x00018028 File Offset: 0x00016228
	protected bool ShouldSplat(int id = -1)
	{
		return this.splatmap0 != null && this.splatmap1 != null && (this.SplatMask & id) > 0;
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x00018053 File Offset: 0x00016253
	protected bool ShouldAlpha()
	{
		return this.alphamap != null && this.AlphaMap;
	}

	// Token: 0x06001E62 RID: 7778 RVA: 0x0001806B File Offset: 0x0001626B
	protected bool ShouldBiome(int id = -1)
	{
		return this.biomemap != null && (this.BiomeMask & id) > 0;
	}

	// Token: 0x06001E63 RID: 7779 RVA: 0x00018088 File Offset: 0x00016288
	protected bool ShouldTopology(int id = -1)
	{
		return this.topologymap != null && (this.TopologyMask & id) > 0;
	}

	// Token: 0x06001E64 RID: 7780 RVA: 0x000180A5 File Offset: 0x000162A5
	protected bool ShouldWater()
	{
		return this.watermap != null && this.WaterMap;
	}

	// Token: 0x06001E65 RID: 7781
	protected abstract void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x06001E66 RID: 7782
	protected abstract void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x06001E67 RID: 7783
	protected abstract void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x06001E68 RID: 7784
	protected abstract void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x06001E69 RID: 7785
	protected abstract void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x06001E6A RID: 7786
	protected abstract void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x06001E6B RID: 7787 RVA: 0x000180BD File Offset: 0x000162BD
	protected override Type GetIndexedType()
	{
		return typeof(TerrainPlacement);
	}
}
