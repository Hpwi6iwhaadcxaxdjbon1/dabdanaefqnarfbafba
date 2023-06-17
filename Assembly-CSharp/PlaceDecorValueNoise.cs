using System;
using UnityEngine;

// Token: 0x02000526 RID: 1318
public class PlaceDecorValueNoise : ProceduralComponent
{
	// Token: 0x04001A3D RID: 6717
	public SpawnFilter Filter;

	// Token: 0x04001A3E RID: 6718
	public string ResourceFolder = string.Empty;

	// Token: 0x04001A3F RID: 6719
	public NoiseParameters Cluster = new NoiseParameters(2, 0.5f, 1f, 0f);

	// Token: 0x04001A40 RID: 6720
	public float ObjectDensity = 100f;

	// Token: 0x06001DF9 RID: 7673 RVA: 0x000A3734 File Offset: 0x000A1934
	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, null, null, true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		int num = Mathf.RoundToInt(this.ObjectDensity * size.x * size.z * 1E-06f);
		float x = position.x;
		float z = position.z;
		float num2 = position.x + size.x;
		float num3 = position.z + size.z;
		float num4 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		float num5 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		int octaves = this.Cluster.Octaves;
		float offset = this.Cluster.Offset;
		float frequency = this.Cluster.Frequency * 0.01f;
		float amplitude = this.Cluster.Amplitude;
		for (int i = 0; i < num; i++)
		{
			float num6 = SeedRandom.Range(ref seed, x, num2);
			float num7 = SeedRandom.Range(ref seed, z, num3);
			float normX = TerrainMeta.NormalizeX(num6);
			float normZ = TerrainMeta.NormalizeZ(num7);
			float num8 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(normX, normZ);
			Prefab random = array.GetRandom(ref seed);
			if (factor > 0f && (offset + Noise.Turbulence(num4 + num6, num5 + num7, octaves, frequency, amplitude, 2f, 0.5f)) * factor * factor >= num8)
			{
				float height = heightMap.GetHeight(normX, normZ);
				Vector3 vector = new Vector3(num6, height, num7);
				Quaternion localRotation = random.Object.transform.localRotation;
				Vector3 localScale = random.Object.transform.localScale;
				random.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
				if (random.ApplyTerrainAnchors(ref vector, localRotation, localScale, this.Filter) && random.ApplyTerrainChecks(vector, localRotation, localScale, this.Filter) && random.ApplyTerrainFilters(vector, localRotation, localScale, null) && random.ApplyWaterChecks(vector, localRotation, localScale))
				{
					random.ApplyTerrainModifiers(vector, localRotation, localScale);
					World.AddPrefab("Decor", random.ID, vector, localRotation, localScale);
				}
			}
		}
	}
}
