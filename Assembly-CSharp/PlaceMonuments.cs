using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200052A RID: 1322
public class PlaceMonuments : ProceduralComponent
{
	// Token: 0x04001A4B RID: 6731
	public SpawnFilter Filter;

	// Token: 0x04001A4C RID: 6732
	public string ResourceFolder = string.Empty;

	// Token: 0x04001A4D RID: 6733
	public int Distance = 500;

	// Token: 0x04001A4E RID: 6734
	public int MinSize;

	// Token: 0x04001A4F RID: 6735
	private const int Candidates = 10;

	// Token: 0x04001A50 RID: 6736
	private const int Attempts = 10000;

	// Token: 0x06001DFF RID: 7679 RVA: 0x000A3D78 File Offset: 0x000A1F78
	public override void Process(uint seed)
	{
		if ((ulong)World.Size < (ulong)((long)this.MinSize))
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Prefab<MonumentInfo>[] array = Prefab.Load<MonumentInfo>("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, null, null, true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		array.Shuffle(seed);
		array.BubbleSort<Prefab<MonumentInfo>>();
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		List<PlaceMonuments.SpawnInfo> list = new List<PlaceMonuments.SpawnInfo>();
		int num3 = 0;
		List<PlaceMonuments.SpawnInfo> list2 = new List<PlaceMonuments.SpawnInfo>();
		for (int i = 0; i < 10; i++)
		{
			int num4 = 0;
			list.Clear();
			foreach (Prefab<MonumentInfo> prefab in array)
			{
				int num5 = (int)(prefab.Parameters ? (prefab.Parameters.Priority + 1) : PrefabPriority.Low);
				int num6 = num5 * num5 * num5 * num5;
				for (int k = 0; k < 10000; k++)
				{
					float x2 = SeedRandom.Range(ref seed, x, num);
					float z2 = SeedRandom.Range(ref seed, z, num2);
					float normX = TerrainMeta.NormalizeX(x2);
					float normZ = TerrainMeta.NormalizeZ(z2);
					float num7 = SeedRandom.Value(ref seed);
					float factor = this.Filter.GetFactor(normX, normZ);
					if (factor * factor >= num7)
					{
						float height = heightMap.GetHeight(normX, normZ);
						Vector3 vector = new Vector3(x2, height, z2);
						Quaternion localRotation = prefab.Object.transform.localRotation;
						Vector3 localScale = prefab.Object.transform.localScale;
						if (!this.CheckRadius(list, vector, (float)this.Distance))
						{
							prefab.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
							if ((!prefab.Component || prefab.Component.CheckPlacement(vector, localRotation, localScale)) && prefab.ApplyTerrainAnchors(ref vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainChecks(vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainFilters(vector, localRotation, localScale, null) && prefab.ApplyWaterChecks(vector, localRotation, localScale) && !prefab.CheckEnvironmentVolumes(vector, localRotation, localScale, EnvironmentType.Underground))
							{
								PlaceMonuments.SpawnInfo spawnInfo = new PlaceMonuments.SpawnInfo
								{
									prefab = prefab,
									position = vector,
									rotation = localRotation,
									scale = localScale
								};
								list.Add(spawnInfo);
								num4 += num6;
								break;
							}
						}
					}
				}
			}
			if (num4 > num3)
			{
				num3 = num4;
				GenericsUtil.Swap<List<PlaceMonuments.SpawnInfo>>(ref list, ref list2);
			}
		}
		foreach (PlaceMonuments.SpawnInfo spawnInfo2 in list2)
		{
			Prefab prefab2 = spawnInfo2.prefab;
			Vector3 position2 = spawnInfo2.position;
			Quaternion rotation = spawnInfo2.rotation;
			Vector3 scale = spawnInfo2.scale;
			prefab2.ApplyTerrainPlacements(position2, rotation, scale);
			prefab2.ApplyTerrainModifiers(position2, rotation, scale);
			World.AddPrefab("Monument", prefab2.ID, position2, rotation, scale);
		}
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x000A40AC File Offset: 0x000A22AC
	private bool CheckRadius(List<PlaceMonuments.SpawnInfo> spawns, Vector3 pos, float radius)
	{
		float num = radius * radius;
		using (List<PlaceMonuments.SpawnInfo>.Enumerator enumerator = spawns.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if ((enumerator.Current.position - pos).sqrMagnitude < num)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0200052B RID: 1323
	private struct SpawnInfo
	{
		// Token: 0x04001A51 RID: 6737
		public Prefab prefab;

		// Token: 0x04001A52 RID: 6738
		public Vector3 position;

		// Token: 0x04001A53 RID: 6739
		public Quaternion rotation;

		// Token: 0x04001A54 RID: 6740
		public Vector3 scale;
	}
}
