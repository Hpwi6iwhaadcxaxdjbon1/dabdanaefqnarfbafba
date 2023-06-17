using System;
using UnityEngine;

// Token: 0x02000535 RID: 1333
public class TerrainGenerator : SingletonComponent<TerrainGenerator>
{
	// Token: 0x04001A6C RID: 6764
	public TerrainConfig config;

	// Token: 0x04001A6D RID: 6765
	private const float HeightMapRes = 0.5f;

	// Token: 0x04001A6E RID: 6766
	private const float SplatMapRes = 0.5f;

	// Token: 0x04001A6F RID: 6767
	private const float BaseMapRes = 0.01f;

	// Token: 0x06001E15 RID: 7701 RVA: 0x000A4C68 File Offset: 0x000A2E68
	public GameObject CreateTerrain()
	{
		Terrain component = Terrain.CreateTerrainGameObject(new TerrainData
		{
			baseMapResolution = Mathf.NextPowerOfTwo((int)(World.Size * 0.01f)),
			heightmapResolution = Mathf.NextPowerOfTwo((int)(World.Size * 0.5f)) + 1,
			alphamapResolution = Mathf.NextPowerOfTwo((int)(World.Size * 0.5f)),
			size = new Vector3(World.Size, 1000f, World.Size)
		}).GetComponent<Terrain>();
		component.transform.position = base.transform.position + new Vector3((float)(-(float)((ulong)World.Size)) * 0.5f, 0f, (float)(-(float)((ulong)World.Size)) * 0.5f);
		component.castShadows = this.config.CastShadows;
		component.materialType = 3;
		component.materialTemplate = this.config.Material;
		component.gameObject.tag = base.gameObject.tag;
		component.gameObject.layer = base.gameObject.layer;
		component.gameObject.GetComponent<TerrainCollider>().sharedMaterial = this.config.GenericMaterial;
		TerrainMeta terrainMeta = component.gameObject.AddComponent<TerrainMeta>();
		component.gameObject.AddComponent<TerrainPhysics>();
		component.gameObject.AddComponent<TerrainColors>();
		component.gameObject.AddComponent<TerrainCollision>();
		component.gameObject.AddComponent<TerrainBiomeMap>();
		component.gameObject.AddComponent<TerrainAlphaMap>();
		component.gameObject.AddComponent<TerrainHeightMap>();
		component.gameObject.AddComponent<TerrainSplatMap>();
		component.gameObject.AddComponent<TerrainTopologyMap>();
		component.gameObject.AddComponent<TerrainWaterMap>();
		component.gameObject.AddComponent<TerrainPath>();
		component.gameObject.AddComponent<TerrainQuality>();
		component.gameObject.AddComponent<TerrainTexturing>();
		terrainMeta.terrain = component;
		terrainMeta.config = this.config;
		Object.DestroyImmediate(base.gameObject);
		return component.gameObject;
	}
}
