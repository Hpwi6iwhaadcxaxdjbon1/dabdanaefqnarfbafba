using System;
using UnityEngine;

// Token: 0x020004E1 RID: 1249
public class TerrainTopologyMap : TerrainMap<int>
{
	// Token: 0x04001957 RID: 6487
	public Texture2D TopologyTexture;

	// Token: 0x06001CD1 RID: 7377 RVA: 0x0009E9F8 File Offset: 0x0009CBF8
	public override void Setup()
	{
		if (!(this.TopologyTexture != null))
		{
			this.res = this.terrain.terrainData.alphamapResolution;
			this.src = (this.dst = new int[this.res * this.res]);
			return;
		}
		if (this.TopologyTexture.width == this.TopologyTexture.height)
		{
			this.res = this.TopologyTexture.width;
			this.src = (this.dst = new int[this.res * this.res]);
			Color32[] pixels = this.TopologyTexture.GetPixels32();
			int i = 0;
			int num = 0;
			while (i < this.res)
			{
				int j = 0;
				while (j < this.res)
				{
					this.dst[i * this.res + j] = BitUtility.DecodeInt(pixels[num]);
					j++;
					num++;
				}
				i++;
			}
			return;
		}
		Debug.LogError("Invalid topology texture: " + this.TopologyTexture.name);
	}

	// Token: 0x06001CD2 RID: 7378 RVA: 0x0009EB0C File Offset: 0x0009CD0C
	public void GenerateTextures()
	{
		this.TopologyTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, false, true);
		this.TopologyTexture.name = "TopologyTexture";
		this.TopologyTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				col[z * this.res + i] = BitUtility.EncodeInt(this.src[z * this.res + i]);
			}
		});
		this.TopologyTexture.SetPixels32(col);
	}

	// Token: 0x06001CD3 RID: 7379 RVA: 0x00017436 File Offset: 0x00015636
	public void ApplyTextures()
	{
		this.TopologyTexture.Apply(false, true);
	}

	// Token: 0x06001CD4 RID: 7380 RVA: 0x0009EBA0 File Offset: 0x0009CDA0
	public bool GetTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(normX, normZ, mask);
	}

	// Token: 0x06001CD5 RID: 7381 RVA: 0x0009EBD0 File Offset: 0x0009CDD0
	public bool GetTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetTopology(x, z, mask);
	}

	// Token: 0x06001CD6 RID: 7382 RVA: 0x00017445 File Offset: 0x00015645
	public bool GetTopology(int x, int z, int mask)
	{
		return (this.src[z * this.res + x] & mask) != 0;
	}

	// Token: 0x06001CD7 RID: 7383 RVA: 0x0009EBF8 File Offset: 0x0009CDF8
	public int GetTopology(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(normX, normZ);
	}

	// Token: 0x06001CD8 RID: 7384 RVA: 0x0009EC28 File Offset: 0x0009CE28
	public int GetTopology(float normX, float normZ)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetTopology(x, z);
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x0009EC50 File Offset: 0x0009CE50
	public int GetTopologyFast(float normX, float normZ)
	{
		int num = this.res - 1;
		int num2 = (int)(normX * (float)this.res);
		int num3 = (int)(normZ * (float)this.res);
		num2 = ((num2 >= 0) ? num2 : 0);
		num3 = ((num3 >= 0) ? num3 : 0);
		num2 = ((num2 <= num) ? num2 : num);
		num3 = ((num3 <= num) ? num3 : num);
		return this.src[num3 * this.res + num2];
	}

	// Token: 0x06001CDA RID: 7386 RVA: 0x0001745D File Offset: 0x0001565D
	public int GetTopology(int x, int z)
	{
		return this.src[z * this.res + x];
	}

	// Token: 0x06001CDB RID: 7387 RVA: 0x0009ECB4 File Offset: 0x0009CEB4
	public void SetTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetTopology(normX, normZ, mask);
	}

	// Token: 0x06001CDC RID: 7388 RVA: 0x0009ECE4 File Offset: 0x0009CEE4
	public void SetTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetTopology(x, z, mask);
	}

	// Token: 0x06001CDD RID: 7389 RVA: 0x00017470 File Offset: 0x00015670
	public void SetTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] = mask;
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x0009ED0C File Offset: 0x0009CF0C
	public void AddTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddTopology(normX, normZ, mask);
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x0009ED3C File Offset: 0x0009CF3C
	public void AddTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.AddTopology(x, z, mask);
	}

	// Token: 0x06001CE0 RID: 7392 RVA: 0x00017484 File Offset: 0x00015684
	public void AddTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] |= mask;
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x0009ED64 File Offset: 0x0009CF64
	public void RemoveTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.RemoveTopology(normX, normZ, mask);
	}

	// Token: 0x06001CE2 RID: 7394 RVA: 0x0009ED94 File Offset: 0x0009CF94
	public void RemoveTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.RemoveTopology(x, z, mask);
	}

	// Token: 0x06001CE3 RID: 7395 RVA: 0x000174A0 File Offset: 0x000156A0
	public void RemoveTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] &= ~mask;
	}

	// Token: 0x06001CE4 RID: 7396 RVA: 0x0009EDBC File Offset: 0x0009CFBC
	public int GetTopology(Vector3 worldPos, float radius)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(normX, normZ, radius);
	}

	// Token: 0x06001CE5 RID: 7397 RVA: 0x0009EDEC File Offset: 0x0009CFEC
	public int GetTopology(float normX, float normZ, float radius)
	{
		int num = 0;
		float num2 = TerrainMeta.OneOverSize.x * radius;
		int num3 = base.Index(normX - num2);
		int num4 = base.Index(normX + num2);
		int num5 = base.Index(normZ - num2);
		int num6 = base.Index(normZ + num2);
		for (int i = num5; i <= num6; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				num |= this.src[i * this.res + j];
			}
		}
		return num;
	}

	// Token: 0x06001CE6 RID: 7398 RVA: 0x0009EE68 File Offset: 0x0009D068
	public void SetTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetTopology(normX, normZ, mask, radius, fade);
	}

	// Token: 0x06001CE7 RID: 7399 RVA: 0x0009EE9C File Offset: 0x0009D09C
	public void SetTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] = mask;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06001CE8 RID: 7400 RVA: 0x0009EED8 File Offset: 0x0009D0D8
	public void AddTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddTopology(normX, normZ, mask, radius, fade);
	}

	// Token: 0x06001CE9 RID: 7401 RVA: 0x0009EF0C File Offset: 0x0009D10C
	public void AddTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] |= mask;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}
}
