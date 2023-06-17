using System;
using UnityEngine;

// Token: 0x020004CB RID: 1227
public class TerrainBiomeMap : TerrainMap<byte>
{
	// Token: 0x04001916 RID: 6422
	public Texture2D BiomeTexture;

	// Token: 0x04001917 RID: 6423
	internal int num;

	// Token: 0x06001C28 RID: 7208 RVA: 0x0009AEBC File Offset: 0x000990BC
	public override void Setup()
	{
		if (!(this.BiomeTexture != null))
		{
			this.res = this.terrain.terrainData.alphamapResolution;
			this.num = 4;
			this.src = (this.dst = new byte[this.num * this.res * this.res]);
			return;
		}
		if (this.BiomeTexture.width == this.BiomeTexture.height)
		{
			this.res = this.BiomeTexture.width;
			this.num = 4;
			this.src = (this.dst = new byte[this.num * this.res * this.res]);
			Color32[] pixels = this.BiomeTexture.GetPixels32();
			int i = 0;
			int num = 0;
			while (i < this.res)
			{
				int j = 0;
				while (j < this.res)
				{
					Color32 color = pixels[num];
					byte[] dst = this.dst;
					int res = this.res;
					dst[(0 + i) * this.res + j] = color.r;
					this.dst[(this.res + i) * this.res + j] = color.g;
					this.dst[(2 * this.res + i) * this.res + j] = color.b;
					this.dst[(3 * this.res + i) * this.res + j] = color.a;
					j++;
					num++;
				}
				i++;
			}
			return;
		}
		Debug.LogError("Invalid biome texture: " + this.BiomeTexture.name);
	}

	// Token: 0x06001C29 RID: 7209 RVA: 0x0009B06C File Offset: 0x0009926C
	public void GenerateTextures()
	{
		this.BiomeTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.BiomeTexture.name = "BiomeTexture";
		this.BiomeTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte[] src = this.src;
				int res = this.res;
				byte r = src[(0 + z) * this.res + i];
				byte g = this.src[(this.res + z) * this.res + i];
				byte b = this.src[(2 * this.res + z) * this.res + i];
				byte a = this.src[(3 * this.res + z) * this.res + i];
				col[z * this.res + i] = new Color32(r, g, b, a);
			}
		});
		this.BiomeTexture.SetPixels32(col);
	}

	// Token: 0x06001C2A RID: 7210 RVA: 0x0001709B File Offset: 0x0001529B
	public void ApplyTextures()
	{
		this.BiomeTexture.Apply(true, false);
		this.BiomeTexture.Compress(false);
		this.BiomeTexture.Apply(false, true);
	}

	// Token: 0x06001C2B RID: 7211 RVA: 0x0009B100 File Offset: 0x00099300
	public float GetBiomeMax(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiomeMax(normX, normZ, mask);
	}

	// Token: 0x06001C2C RID: 7212 RVA: 0x0009B130 File Offset: 0x00099330
	public float GetBiomeMax(float normX, float normZ, int mask = -1)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBiomeMax(x, z, mask);
	}

	// Token: 0x06001C2D RID: 7213 RVA: 0x0009B158 File Offset: 0x00099358
	public float GetBiomeMax(int x, int z, int mask = -1)
	{
		byte b = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
				}
			}
		}
		return (float)b;
	}

	// Token: 0x06001C2E RID: 7214 RVA: 0x0009B1A8 File Offset: 0x000993A8
	public int GetBiomeMaxIndex(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiomeMaxIndex(normX, normZ, mask);
	}

	// Token: 0x06001C2F RID: 7215 RVA: 0x0009B1D8 File Offset: 0x000993D8
	public int GetBiomeMaxIndex(float normX, float normZ, int mask = -1)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBiomeMaxIndex(x, z, mask);
	}

	// Token: 0x06001C30 RID: 7216 RVA: 0x0009B200 File Offset: 0x00099400
	public int GetBiomeMaxIndex(int x, int z, int mask = -1)
	{
		byte b = 0;
		int result = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
					result = i;
				}
			}
		}
		return result;
	}

	// Token: 0x06001C31 RID: 7217 RVA: 0x000170C3 File Offset: 0x000152C3
	public int GetBiomeMaxType(Vector3 worldPos, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(worldPos, mask));
	}

	// Token: 0x06001C32 RID: 7218 RVA: 0x000170D2 File Offset: 0x000152D2
	public int GetBiomeMaxType(float normX, float normZ, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(normX, normZ, mask));
	}

	// Token: 0x06001C33 RID: 7219 RVA: 0x000170E2 File Offset: 0x000152E2
	public int GetBiomeMaxType(int x, int z, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(x, z, mask));
	}

	// Token: 0x06001C34 RID: 7220 RVA: 0x0009B250 File Offset: 0x00099450
	public float GetBiome(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiome(normX, normZ, mask);
	}

	// Token: 0x06001C35 RID: 7221 RVA: 0x0009B280 File Offset: 0x00099480
	public float GetBiome(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBiome(x, z, mask);
	}

	// Token: 0x06001C36 RID: 7222 RVA: 0x0009B2A8 File Offset: 0x000994A8
	public float GetBiome(int x, int z, int mask)
	{
		if (Mathf.IsPowerOfTwo(mask))
		{
			return BitUtility.Byte2Float((int)this.src[(TerrainBiome.TypeToIndex(mask) * this.res + z) * this.res + x]);
		}
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				num += (int)this.src[(i * this.res + z) * this.res + x];
			}
		}
		return Mathf.Clamp01(BitUtility.Byte2Float(num));
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x0009B328 File Offset: 0x00099528
	public void SetBiome(Vector3 worldPos, int id)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBiome(normX, normZ, id);
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x0009B358 File Offset: 0x00099558
	public void SetBiome(float normX, float normZ, int id)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetBiome(x, z, id);
	}

	// Token: 0x06001C39 RID: 7225 RVA: 0x0009B380 File Offset: 0x00099580
	public void SetBiome(int x, int z, int id)
	{
		int num = TerrainBiome.TypeToIndex(id);
		for (int i = 0; i < this.num; i++)
		{
			if (i == num)
			{
				this.dst[(i * this.res + z) * this.res + x] = byte.MaxValue;
			}
			else
			{
				this.dst[(i * this.res + z) * this.res + x] = 0;
			}
		}
	}

	// Token: 0x06001C3A RID: 7226 RVA: 0x0009B3E8 File Offset: 0x000995E8
	public void SetBiome(Vector3 worldPos, int id, float v)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBiome(normX, normZ, id, v);
	}

	// Token: 0x06001C3B RID: 7227 RVA: 0x0009B418 File Offset: 0x00099618
	public void SetBiome(float normX, float normZ, int id, float v)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetBiome(x, z, id, v);
	}

	// Token: 0x06001C3C RID: 7228 RVA: 0x000170F2 File Offset: 0x000152F2
	public void SetBiome(int x, int z, int id, float v)
	{
		this.SetBiome(x, z, id, this.GetBiome(x, z, id), v);
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x0009B440 File Offset: 0x00099640
	public void SetBiomeRaw(int x, int z, Vector4 v, float opacity)
	{
		if (opacity == 0f)
		{
			return;
		}
		float num = Mathf.Clamp01(v.x + v.y + v.z + v.w);
		if (num == 0f)
		{
			return;
		}
		float num2 = 1f - opacity * num;
		if (num2 == 0f && opacity == 1f)
		{
			byte[] dst = this.dst;
			int res = this.res;
			dst[(0 + z) * this.res + x] = BitUtility.Float2Byte(v.x);
			this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(v.y);
			this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v.z);
			this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v.w);
			return;
		}
		byte[] dst2 = this.dst;
		int res2 = this.res;
		int num3 = (0 + z) * this.res + x;
		byte[] src = this.src;
		int res3 = this.res;
		dst2[num3] = BitUtility.Float2Byte(BitUtility.Byte2Float(src[(0 + z) * this.res + x]) * num2 + v.x * opacity);
		this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(this.res + z) * this.res + x]) * num2 + v.y * opacity);
		this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(2 * this.res + z) * this.res + x]) * num2 + v.z * opacity);
		this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(3 * this.res + z) * this.res + x]) * num2 + v.w * opacity);
	}

	// Token: 0x06001C3E RID: 7230 RVA: 0x0009B658 File Offset: 0x00099858
	private void SetBiome(int x, int z, int id, float old_val, float new_val)
	{
		int num = TerrainBiome.TypeToIndex(id);
		if (old_val >= 1f)
		{
			return;
		}
		float num2 = (1f - new_val) / (1f - old_val);
		for (int i = 0; i < this.num; i++)
		{
			if (i == num)
			{
				this.dst[(i * this.res + z) * this.res + x] = BitUtility.Float2Byte(new_val);
			}
			else
			{
				this.dst[(i * this.res + z) * this.res + x] = BitUtility.Float2Byte(num2 * BitUtility.Byte2Float((int)this.dst[(i * this.res + z) * this.res + x]));
			}
		}
	}
}
