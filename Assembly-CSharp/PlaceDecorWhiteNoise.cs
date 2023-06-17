using System;
using UnityEngine;

// Token: 0x02000527 RID: 1319
public class PlaceDecorWhiteNoise : ProceduralComponent
{
	// Token: 0x04001A41 RID: 6721
	public SpawnFilter Filter;

	// Token: 0x04001A42 RID: 6722
	public string ResourceFolder = string.Empty;

	// Token: 0x04001A43 RID: 6723
	public float ObjectDensity = 100f;

	// Token: 0x06001DFB RID: 7675 RVA: 0x000A397C File Offset: 0x000A1B7C
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
		for (int i = 0; i < num; i++)
		{
			float x2 = SeedRandom.Range(ref seed, x, num2);
			float z2 = SeedRandom.Range(ref seed, z, num3);
			float normX = TerrainMeta.NormalizeX(x2);
			float normZ = TerrainMeta.NormalizeZ(z2);
			float num4 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(normX, normZ);
			Prefab random = array.GetRandom(ref seed);
			if (factor * factor >= num4)
			{
				float height = heightMap.GetHeight(normX, normZ);
				Vector3 vector = new Vector3(x2, height, z2);
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
