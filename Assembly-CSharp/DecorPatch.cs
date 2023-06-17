using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020004AC RID: 1196
public class DecorPatch
{
	// Token: 0x04001894 RID: 6292
	private bool initialized;

	// Token: 0x04001895 RID: 6293
	private float LOD;

	// Token: 0x04001896 RID: 6294
	private float shift;

	// Token: 0x04001897 RID: 6295
	private float extent;

	// Token: 0x04001898 RID: 6296
	private Vector3 offset;

	// Token: 0x04001899 RID: 6297
	private Vector3 position;

	// Token: 0x0400189A RID: 6298
	private DecorSpawn decorSpawn;

	// Token: 0x0400189B RID: 6299
	private List<GameObject> spawns;

	// Token: 0x06001BBA RID: 7098 RVA: 0x000992C8 File Offset: 0x000974C8
	public void DestroyInstances()
	{
		for (int i = 0; i < this.spawns.Count; i++)
		{
			Prefab.DefaultManager.Retire(this.spawns[i]);
		}
		this.spawns.Clear();
	}

	// Token: 0x06001BBB RID: 7099 RVA: 0x0009930C File Offset: 0x0009750C
	public DecorPatch(DecorSpawn decorSpawn, int i, int j)
	{
		int patchSize = decorSpawn.PatchSize;
		int patchCount = decorSpawn.PatchCount;
		float num = (float)(patchSize - patchCount * patchSize) / 2f;
		this.decorSpawn = decorSpawn;
		this.shift = (float)(patchCount * patchSize);
		this.extent = (float)patchSize * 0.5f;
		this.offset = new Vector3(num + (float)(i * patchSize), 0f, num + (float)(j * patchSize));
		this.spawns = new List<GameObject>(decorSpawn.ObjectsPerPatch);
		this.position = Vector3.zero;
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x00099394 File Offset: 0x00097594
	public bool Shift()
	{
		Vector3 vector = this.decorSpawn.Anchor.position - this.offset;
		float x = (float)Mathf.RoundToInt(vector.x / this.shift) * this.shift;
		float z = (float)Mathf.RoundToInt(vector.z / this.shift) * this.shift;
		Vector3 rhs = this.position;
		Vector3 lhs = this.position = this.offset + new Vector3(x, 0f, z);
		float lod = this.LOD;
		float num = this.LOD = (this.decorSpawn.LOD ? Mathf.Clamp01(Decor.quality01) : 1f);
		if (!this.initialized)
		{
			return this.initialized = true;
		}
		return lod != num || lhs != rhs;
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x00099474 File Offset: 0x00097674
	public void Spawn()
	{
		if (this.decorSpawn.Prefabs == null || this.decorSpawn.Prefabs.Length == 0)
		{
			return;
		}
		this.DestroyInstances();
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 vector = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		int clusterSizeMin = this.decorSpawn.ClusterSizeMin;
		int num = this.decorSpawn.ClusterSizeMax + 1;
		float clusterRadius = this.decorSpawn.ClusterRadius;
		int i = this.decorSpawn.ObjectsPerPatch;
		float objectCutoff = this.decorSpawn.ObjectCutoff;
		float objectTapering = this.decorSpawn.ObjectTapering;
		float num2 = this.position.x - this.extent;
		float num3 = this.position.z - this.extent;
		float num4 = this.position.x + this.extent;
		float num5 = this.position.z + this.extent;
		uint num6 = SeedEx.Seed(this.position, this.decorSpawn.Seed);
		while (i > 0)
		{
			float num7 = SeedRandom.Range(ref num6, num2, num4);
			float num8 = SeedRandom.Range(ref num6, num3, num5);
			int num9 = SeedRandom.Range(ref num6, clusterSizeMin, num);
			for (int j = 0; j < num9; j++)
			{
				float num10 = SeedRandom.Value(ref num6);
				float num11 = SeedRandom.Range(ref num6, objectCutoff, objectTapering);
				float num12 = num7 + SeedRandom.Range(ref num6, -clusterRadius, clusterRadius);
				float num13 = num8 + SeedRandom.Range(ref num6, -clusterRadius, clusterRadius);
				Prefab random = this.decorSpawn.Prefabs.GetRandom(ref num6);
				if (this.LOD >= num10 && num12 >= vector.x && num12 <= vector.x + size.x && num13 >= vector.z && num13 <= vector.z + size.z)
				{
					float normX = (num12 - vector.x) / size.x;
					float normZ = (num13 - vector.z) / size.z;
					float factor = this.decorSpawn.Filter.GetFactor(normX, normZ);
					if (factor * factor >= num11)
					{
						float height = heightMap.GetHeight(normX, normZ);
						Vector3 pos = new Vector3(num12, height, num13);
						Quaternion rotation = random.Object.transform.rotation;
						Vector3 localScale = random.Object.transform.localScale;
						random.ApplyDecorComponents(ref pos, ref rotation, ref localScale);
						if (random.ApplyTerrainAnchors(ref pos, rotation, localScale, this.decorSpawn.Filter) && random.ApplyTerrainChecks(pos, rotation, localScale, this.decorSpawn.Filter) && random.ApplyTerrainFilters(pos, rotation, localScale, null) && random.ApplyWaterChecks(pos, rotation, localScale))
						{
							GameObject gameObject = random.Spawn(pos, rotation, localScale);
							if (gameObject)
							{
								SceneManager.MoveGameObjectToScene(gameObject, Rust.Client.DecorScene);
								this.spawns.Add(gameObject);
							}
						}
					}
				}
			}
			i -= num9;
		}
	}
}
