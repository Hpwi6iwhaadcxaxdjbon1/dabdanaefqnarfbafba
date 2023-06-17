using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000390 RID: 912
[CreateAssetMenu(menuName = "Rust/Foliage Placement")]
public class FoliagePlacement : ScriptableObject
{
	// Token: 0x040013FA RID: 5114
	[Header("Placement")]
	public float Density = 2f;

	// Token: 0x040013FB RID: 5115
	[Header("Filter")]
	public SpawnFilter Filter;

	// Token: 0x040013FC RID: 5116
	[FormerlySerializedAs("Cutoff")]
	public float FilterCutoff = 0.5f;

	// Token: 0x040013FD RID: 5117
	public float FilterFade = 0.1f;

	// Token: 0x040013FE RID: 5118
	[FormerlySerializedAs("Scaling")]
	public float FilterScaling = 1f;

	// Token: 0x040013FF RID: 5119
	[Header("Randomization")]
	public float RandomScaling = 0.2f;

	// Token: 0x04001400 RID: 5120
	[MinMax(0f, 1f)]
	[Header("Placement Range")]
	public MinMax Range = new MinMax(0f, 1f);

	// Token: 0x04001401 RID: 5121
	public float RangeFade = 0.1f;

	// Token: 0x04001402 RID: 5122
	[Range(0f, 1f)]
	[Header("LOD")]
	public float DistanceDensity;

	// Token: 0x04001403 RID: 5123
	[Range(1f, 2f)]
	public float DistanceScaling = 2f;

	// Token: 0x04001404 RID: 5124
	[Header("Visuals")]
	public Material material;

	// Token: 0x04001405 RID: 5125
	public Mesh mesh;

	// Token: 0x04001406 RID: 5126
	public const int octaves = 1;

	// Token: 0x04001407 RID: 5127
	public const float frequency = 0.05f;

	// Token: 0x04001408 RID: 5128
	public const float amplitude = 0.5f;

	// Token: 0x04001409 RID: 5129
	public const float offset = 0.5f;

	// Token: 0x0600173F RID: 5951 RVA: 0x00013864 File Offset: 0x00011A64
	public void Init()
	{
		MeshCache.Get(this.mesh);
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x00089B0C File Offset: 0x00087D0C
	public bool CheckBatch(Vector3 pivot, float size)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainColors colors = TerrainMeta.Colors;
		Vector3 position = TerrainMeta.Position;
		Vector3 size2 = TerrainMeta.Size;
		float num = 2f;
		float num2 = size * 0.5f;
		float num3 = pivot.x - num2;
		float num4 = pivot.z - num2;
		float num5 = pivot.x + num2;
		float num6 = pivot.z + num2;
		for (float num7 = num4; num7 <= num6; num7 += num)
		{
			for (float num8 = num3; num8 <= num5; num8 += num)
			{
				float normX = (num8 - position.x) / size2.x;
				float normZ = (num7 - position.z) / size2.z;
				if (this.Filter.GetFactor(normX, normZ) >= this.FilterCutoff)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x00089BC8 File Offset: 0x00087DC8
	public void AddBatch(FoliageGroup batchGroup, float lod, uint seed)
	{
		MeshInstance instance = default(MeshInstance);
		instance.mesh = this.mesh;
		Vector3 position = batchGroup.Position;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainColors colors = TerrainMeta.Colors;
		Vector3 position2 = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float size2 = batchGroup.Size;
		float num = size2 * 0.5f;
		float num2 = position.x - num;
		float num3 = position.z - num;
		float num4 = position.x + num;
		float num5 = position.z + num;
		float num6 = Mathf.Lerp(1f, this.DistanceDensity, lod);
		float num7 = Mathf.Lerp(1f, this.DistanceScaling, lod);
		int num8 = Mathf.RoundToInt(size2 * size2 * this.Density);
		for (int i = 0; i < num8; i++)
		{
			float num9 = SeedRandom.Value(ref seed);
			float num10 = SeedRandom.Range(ref seed, this.FilterCutoff, this.FilterCutoff + this.FilterFade);
			float num11 = SeedRandom.Range(ref seed, 0f, this.RangeFade);
			float num12 = SeedRandom.Range(ref seed, num2, num4);
			float num13 = SeedRandom.Range(ref seed, num3, num5);
			float y = (float)SeedRandom.Range(ref seed, 0, 360);
			float num14 = SeedRandom.Range(ref seed, 1f - this.RandomScaling, 1f + this.RandomScaling);
			if (num6 >= num9)
			{
				float normX = (num12 - position2.x) / size.x;
				float normZ = (num13 - position2.z) / size.z;
				float factor = this.Filter.GetFactor(normX, normZ);
				if (factor >= num10)
				{
					float num15 = factor * Mathf.Clamp01(Noise.Turbulence(num12, num13, 1, 0.05f, 0.5f, 2f, 0.5f) + 0.5f);
					if (num15 >= this.Range.x - num11 && num15 <= this.Range.y + num11)
					{
						float height = heightMap.GetHeight(normX, normZ);
						Vector3 position3 = new Vector3(num12, height, num13);
						float num16 = Mathf.Lerp(this.FilterScaling, 1f, factor) * num14;
						Vector3 normal = heightMap.GetNormal(normX, normZ);
						Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal) * Quaternion.Euler(0f, y, 0f);
						Vector3 scale = new Vector3(num16 * num7, num16, num16 * num7);
						instance.position = position3;
						instance.rotation = rotation;
						instance.scale = scale;
						batchGroup.Add(instance);
					}
				}
			}
		}
	}
}
