using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200052C RID: 1324
public class PlaceMonumentsOffshore : ProceduralComponent
{
	// Token: 0x04001A55 RID: 6741
	public string ResourceFolder = string.Empty;

	// Token: 0x04001A56 RID: 6742
	public int MinDistanceFromTerrain = 100;

	// Token: 0x04001A57 RID: 6743
	public int MaxDistanceFromTerrain = 500;

	// Token: 0x04001A58 RID: 6744
	public int DistanceBetweenMonuments = 500;

	// Token: 0x04001A59 RID: 6745
	public int MinSize;

	// Token: 0x04001A5A RID: 6746
	private const int Candidates = 10;

	// Token: 0x04001A5B RID: 6747
	private const int Attempts = 10000;

	// Token: 0x06001E02 RID: 7682 RVA: 0x000A4114 File Offset: 0x000A2314
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
		float num = position.x - (float)this.MaxDistanceFromTerrain;
		float num2 = position.x - (float)this.MinDistanceFromTerrain;
		float num3 = position.x + size.x + (float)this.MinDistanceFromTerrain;
		float num4 = position.x + size.x + (float)this.MaxDistanceFromTerrain;
		float num5 = position.z - (float)this.MaxDistanceFromTerrain;
		int minDistanceFromTerrain = this.MinDistanceFromTerrain;
		float num6 = position.z + size.z + (float)this.MinDistanceFromTerrain;
		float num7 = position.z + size.z + (float)this.MaxDistanceFromTerrain;
		List<PlaceMonumentsOffshore.SpawnInfo> list = new List<PlaceMonumentsOffshore.SpawnInfo>();
		int num8 = 0;
		List<PlaceMonumentsOffshore.SpawnInfo> list2 = new List<PlaceMonumentsOffshore.SpawnInfo>();
		for (int i = 0; i < 10; i++)
		{
			int num9 = 0;
			list.Clear();
			foreach (Prefab<MonumentInfo> prefab in array)
			{
				int num10 = (int)(prefab.Parameters ? (prefab.Parameters.Priority + 1) : PrefabPriority.Low);
				int num11 = num10 * num10 * num10 * num10;
				for (int k = 0; k < 10000; k++)
				{
					float x = 0f;
					float z = 0f;
					switch (seed % 4U)
					{
					case 0U:
						x = SeedRandom.Range(ref seed, num, num2);
						z = SeedRandom.Range(ref seed, num5, num7);
						break;
					case 1U:
						x = SeedRandom.Range(ref seed, num3, num4);
						z = SeedRandom.Range(ref seed, num5, num7);
						break;
					case 2U:
						x = SeedRandom.Range(ref seed, num, num4);
						z = SeedRandom.Range(ref seed, num5, num5);
						break;
					case 3U:
						x = SeedRandom.Range(ref seed, num, num4);
						z = SeedRandom.Range(ref seed, num6, num7);
						break;
					}
					float normX = TerrainMeta.NormalizeX(x);
					float normZ = TerrainMeta.NormalizeZ(z);
					float height = heightMap.GetHeight(normX, normZ);
					Vector3 vector = new Vector3(x, height, z);
					Quaternion localRotation = prefab.Object.transform.localRotation;
					Vector3 localScale = prefab.Object.transform.localScale;
					if (!this.CheckRadius(list, vector, (float)this.DistanceBetweenMonuments))
					{
						prefab.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
						if ((!prefab.Component || prefab.Component.CheckPlacement(vector, localRotation, localScale)) && !prefab.CheckEnvironmentVolumes(vector, localRotation, localScale, EnvironmentType.Underground))
						{
							PlaceMonumentsOffshore.SpawnInfo spawnInfo = new PlaceMonumentsOffshore.SpawnInfo
							{
								prefab = prefab,
								position = vector,
								rotation = localRotation,
								scale = localScale
							};
							list.Add(spawnInfo);
							num9 += num11;
							break;
						}
					}
				}
			}
			if (num9 > num8)
			{
				num8 = num9;
				GenericsUtil.Swap<List<PlaceMonumentsOffshore.SpawnInfo>>(ref list, ref list2);
			}
		}
		foreach (PlaceMonumentsOffshore.SpawnInfo spawnInfo2 in list2)
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

	// Token: 0x06001E03 RID: 7683 RVA: 0x000A44B8 File Offset: 0x000A26B8
	private bool CheckRadius(List<PlaceMonumentsOffshore.SpawnInfo> spawns, Vector3 pos, float radius)
	{
		float num = radius * radius;
		using (List<PlaceMonumentsOffshore.SpawnInfo>.Enumerator enumerator = spawns.GetEnumerator())
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

	// Token: 0x0200052D RID: 1325
	private struct SpawnInfo
	{
		// Token: 0x04001A5C RID: 6748
		public Prefab prefab;

		// Token: 0x04001A5D RID: 6749
		public Vector3 position;

		// Token: 0x04001A5E RID: 6750
		public Quaternion rotation;

		// Token: 0x04001A5F RID: 6751
		public Vector3 scale;
	}
}
