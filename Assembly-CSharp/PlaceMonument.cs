using System;
using UnityEngine;

// Token: 0x02000528 RID: 1320
public class PlaceMonument : ProceduralComponent
{
	// Token: 0x04001A44 RID: 6724
	public SpawnFilter Filter;

	// Token: 0x04001A45 RID: 6725
	public GameObjectRef Monument;

	// Token: 0x04001A46 RID: 6726
	private const int Attempts = 10000;

	// Token: 0x06001DFD RID: 7677 RVA: 0x000A3B30 File Offset: 0x000A1D30
	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		PlaceMonument.SpawnInfo spawnInfo = default(PlaceMonument.SpawnInfo);
		int num3 = int.MinValue;
		Prefab<MonumentInfo> prefab = Prefab.Load<MonumentInfo>(this.Monument.resourceID, null, null);
		for (int i = 0; i < 10000; i++)
		{
			float x2 = SeedRandom.Range(ref seed, x, num);
			float z2 = SeedRandom.Range(ref seed, z, num2);
			float normX = TerrainMeta.NormalizeX(x2);
			float normZ = TerrainMeta.NormalizeZ(z2);
			float num4 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(normX, normZ);
			if (factor * factor >= num4)
			{
				float height = heightMap.GetHeight(normX, normZ);
				Vector3 vector = new Vector3(x2, height, z2);
				Quaternion localRotation = prefab.Object.transform.localRotation;
				Vector3 localScale = prefab.Object.transform.localScale;
				prefab.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
				if ((!prefab.Component || prefab.Component.CheckPlacement(vector, localRotation, localScale)) && prefab.ApplyTerrainAnchors(ref vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainChecks(vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainFilters(vector, localRotation, localScale, null) && prefab.ApplyWaterChecks(vector, localRotation, localScale) && !prefab.CheckEnvironmentVolumes(vector, localRotation, localScale, EnvironmentType.Underground))
				{
					PlaceMonument.SpawnInfo spawnInfo2 = default(PlaceMonument.SpawnInfo);
					spawnInfo2.prefab = prefab;
					spawnInfo2.position = vector;
					spawnInfo2.rotation = localRotation;
					spawnInfo2.scale = localScale;
					int num5 = -Mathf.RoundToInt(Vector3Ex.Magnitude2D(vector));
					if (num5 > num3)
					{
						num3 = num5;
						spawnInfo = spawnInfo2;
					}
				}
			}
		}
		if (num3 != -2147483648)
		{
			Prefab prefab2 = spawnInfo.prefab;
			Vector3 position2 = spawnInfo.position;
			Quaternion rotation = spawnInfo.rotation;
			Vector3 scale = spawnInfo.scale;
			prefab2.ApplyTerrainPlacements(position2, rotation, scale);
			prefab2.ApplyTerrainModifiers(position2, rotation, scale);
			World.AddPrefab("Monument", prefab2.ID, position2, rotation, scale);
		}
	}

	// Token: 0x02000529 RID: 1321
	private struct SpawnInfo
	{
		// Token: 0x04001A47 RID: 6727
		public Prefab prefab;

		// Token: 0x04001A48 RID: 6728
		public Vector3 position;

		// Token: 0x04001A49 RID: 6729
		public Quaternion rotation;

		// Token: 0x04001A4A RID: 6730
		public Vector3 scale;
	}
}
