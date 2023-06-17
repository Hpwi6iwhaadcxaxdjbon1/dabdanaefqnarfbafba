using System;
using UnityEngine;

// Token: 0x020004DC RID: 1244
public class TerrainSplatMap : TerrainMap<byte>
{
	// Token: 0x04001948 RID: 6472
	public Texture2D SplatTexture0;

	// Token: 0x04001949 RID: 6473
	public Texture2D SplatTexture1;

	// Token: 0x0400194A RID: 6474
	internal int num;

	// Token: 0x06001CAD RID: 7341 RVA: 0x0009D8FC File Offset: 0x0009BAFC
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.num = this.config.Splats.Length;
		this.src = (this.dst = new byte[this.num * this.res * this.res]);
		if (this.SplatTexture0 != null)
		{
			if (this.SplatTexture0.width == this.SplatTexture0.height && this.SplatTexture0.width == this.res)
			{
				Color32[] pixels = this.SplatTexture0.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						Color32 color = pixels[num];
						if (this.num > 0)
						{
							byte[] dst = this.dst;
							int res = this.res;
							dst[(0 + i) * this.res + j] = color.r;
						}
						if (this.num > 1)
						{
							this.dst[(this.res + i) * this.res + j] = color.g;
						}
						if (this.num > 2)
						{
							this.dst[(2 * this.res + i) * this.res + j] = color.b;
						}
						if (this.num > 3)
						{
							this.dst[(3 * this.res + i) * this.res + j] = color.a;
						}
						j++;
						num++;
					}
					i++;
				}
			}
			else
			{
				Debug.LogError("Invalid splat texture: " + this.SplatTexture0.name, this.SplatTexture0);
			}
		}
		if (this.SplatTexture1 != null)
		{
			if (this.SplatTexture1.width == this.SplatTexture1.height && this.SplatTexture1.width == this.res && this.num > 5)
			{
				Color32[] pixels2 = this.SplatTexture1.GetPixels32();
				int k = 0;
				int num2 = 0;
				while (k < this.res)
				{
					int l = 0;
					while (l < this.res)
					{
						Color32 color2 = pixels2[num2];
						if (this.num > 4)
						{
							this.dst[(4 * this.res + k) * this.res + l] = color2.r;
						}
						if (this.num > 5)
						{
							this.dst[(5 * this.res + k) * this.res + l] = color2.g;
						}
						if (this.num > 6)
						{
							this.dst[(6 * this.res + k) * this.res + l] = color2.b;
						}
						if (this.num > 7)
						{
							this.dst[(7 * this.res + k) * this.res + l] = color2.a;
						}
						l++;
						num2++;
					}
					k++;
				}
				return;
			}
			Debug.LogError("Invalid splat texture: " + this.SplatTexture1.name, this.SplatTexture1);
		}
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x0009DC2C File Offset: 0x0009BE2C
	public void GenerateTextures()
	{
		this.SplatTexture0 = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.SplatTexture0.name = "SplatTexture0";
		this.SplatTexture0.wrapMode = TextureWrapMode.Clamp;
		Color32[] cols2 = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b;
				if (this.num <= 0)
				{
					b = 0;
				}
				else
				{
					byte[] src = this.src;
					int res = this.res;
					b = src[(0 + z) * this.res + i];
				}
				byte r = b;
				byte g = (this.num > 1) ? this.src[(this.res + z) * this.res + i] : 0;
				byte b2 = (this.num > 2) ? this.src[(2 * this.res + z) * this.res + i] : 0;
				byte a = (this.num > 3) ? this.src[(3 * this.res + z) * this.res + i] : 0;
				cols2[z * this.res + i] = new Color32(r, g, b2, a);
			}
		});
		this.SplatTexture0.SetPixels32(cols2);
		this.SplatTexture1 = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.SplatTexture1.name = "SplatTexture1";
		this.SplatTexture1.wrapMode = TextureWrapMode.Clamp;
		Color32[] cols = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte r = (this.num > 4) ? this.src[(4 * this.res + z) * this.res + i] : 0;
				byte g = (this.num > 5) ? this.src[(5 * this.res + z) * this.res + i] : 0;
				byte b = (this.num > 6) ? this.src[(6 * this.res + z) * this.res + i] : 0;
				byte a = (this.num > 7) ? this.src[(7 * this.res + z) * this.res + i] : 0;
				cols[z * this.res + i] = new Color32(r, g, b, a);
			}
		});
		this.SplatTexture1.SetPixels32(cols);
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x000173D5 File Offset: 0x000155D5
	public void ApplyTextures()
	{
		this.SplatTexture0.Apply(true, true);
		this.SplatTexture1.Apply(true, true);
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x0009DD44 File Offset: 0x0009BF44
	public float GetSplatMax(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplatMax(normX, normZ, mask);
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x0009DD74 File Offset: 0x0009BF74
	public float GetSplatMax(float normX, float normZ, int mask = -1)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetSplatMax(x, z, mask);
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x0009DD9C File Offset: 0x0009BF9C
	public float GetSplatMax(int x, int z, int mask = -1)
	{
		byte b = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
				}
			}
		}
		return BitUtility.Byte2Float((int)b);
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x0009DDF0 File Offset: 0x0009BFF0
	public int GetSplatMaxIndex(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplatMaxIndex(normX, normZ, mask);
	}

	// Token: 0x06001CB4 RID: 7348 RVA: 0x0009DE20 File Offset: 0x0009C020
	public int GetSplatMaxIndex(float normX, float normZ, int mask = -1)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetSplatMaxIndex(x, z, mask);
	}

	// Token: 0x06001CB5 RID: 7349 RVA: 0x0009DE48 File Offset: 0x0009C048
	public int GetSplatMaxIndex(int x, int z, int mask = -1)
	{
		byte b = 0;
		int result = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
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

	// Token: 0x06001CB6 RID: 7350 RVA: 0x000173F1 File Offset: 0x000155F1
	public int GetSplatMaxType(Vector3 worldPos, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(worldPos, mask));
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x00017400 File Offset: 0x00015600
	public int GetSplatMaxType(float normX, float normZ, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(normX, normZ, mask));
	}

	// Token: 0x06001CB8 RID: 7352 RVA: 0x00017410 File Offset: 0x00015610
	public int GetSplatMaxType(int x, int z, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(x, z, mask));
	}

	// Token: 0x06001CB9 RID: 7353 RVA: 0x0009DE98 File Offset: 0x0009C098
	public float GetSplat(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplat(normX, normZ, mask);
	}

	// Token: 0x06001CBA RID: 7354 RVA: 0x0009DEC8 File Offset: 0x0009C0C8
	public float GetSplat(float normX, float normZ, int mask)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float a = Mathf.Lerp(this.GetSplat(num4, num5, mask), this.GetSplat(x, num5, mask), num2 - (float)num4);
		float b = Mathf.Lerp(this.GetSplat(num4, z, mask), this.GetSplat(x, z, mask), num2 - (float)num4);
		return Mathf.Lerp(a, b, num3 - (float)num5);
	}

	// Token: 0x06001CBB RID: 7355 RVA: 0x0009DF60 File Offset: 0x0009C160
	public float GetSplat(int x, int z, int mask)
	{
		if (Mathf.IsPowerOfTwo(mask))
		{
			return BitUtility.Byte2Float((int)this.src[(TerrainSplat.TypeToIndex(mask) * this.res + z) * this.res + x]);
		}
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				num += (int)this.src[(i * this.res + z) * this.res + x];
			}
		}
		return Mathf.Clamp01(BitUtility.Byte2Float(num));
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x0009DFE0 File Offset: 0x0009C1E0
	public void SetSplat(Vector3 worldPos, int id)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(normX, normZ, id);
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x0009E010 File Offset: 0x0009C210
	public void SetSplat(float normX, float normZ, int id)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetSplat(x, z, id);
	}

	// Token: 0x06001CBE RID: 7358 RVA: 0x0009E038 File Offset: 0x0009C238
	public void SetSplat(int x, int z, int id)
	{
		int num = TerrainSplat.TypeToIndex(id);
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

	// Token: 0x06001CBF RID: 7359 RVA: 0x0009E0A0 File Offset: 0x0009C2A0
	public void SetSplat(Vector3 worldPos, int id, float v)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(normX, normZ, id, v);
	}

	// Token: 0x06001CC0 RID: 7360 RVA: 0x0009E0D0 File Offset: 0x0009C2D0
	public void SetSplat(float normX, float normZ, int id, float v)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetSplat(x, z, id, v);
	}

	// Token: 0x06001CC1 RID: 7361 RVA: 0x00017420 File Offset: 0x00015620
	public void SetSplat(int x, int z, int id, float v)
	{
		this.SetSplat(x, z, id, this.GetSplat(x, z, id), v);
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x0009E0F8 File Offset: 0x0009C2F8
	public void SetSplatRaw(int x, int z, Vector4 v1, Vector4 v2, float opacity)
	{
		if (opacity == 0f)
		{
			return;
		}
		float num = Mathf.Clamp01(v1.x + v1.y + v1.z + v1.w + v2.x + v2.y + v2.z + v2.w);
		if (num == 0f)
		{
			return;
		}
		float num2 = 1f - opacity * num;
		if (num2 == 0f && opacity == 1f)
		{
			byte[] dst = this.dst;
			int res = this.res;
			dst[(0 + z) * this.res + x] = BitUtility.Float2Byte(v1.x);
			this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.y);
			this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.z);
			this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.w);
			this.dst[(4 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.x);
			this.dst[(5 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.y);
			this.dst[(6 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.z);
			this.dst[(7 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.w);
			return;
		}
		byte[] dst2 = this.dst;
		int res2 = this.res;
		int num3 = (0 + z) * this.res + x;
		byte[] src = this.src;
		int res3 = this.res;
		dst2[num3] = BitUtility.Float2Byte(BitUtility.Byte2Float(src[(0 + z) * this.res + x]) * num2 + v1.x * opacity);
		this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(this.res + z) * this.res + x]) * num2 + v1.y * opacity);
		this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(2 * this.res + z) * this.res + x]) * num2 + v1.z * opacity);
		this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(3 * this.res + z) * this.res + x]) * num2 + v1.w * opacity);
		this.dst[(4 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(4 * this.res + z) * this.res + x]) * num2 + v2.x * opacity);
		this.dst[(5 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(5 * this.res + z) * this.res + x]) * num2 + v2.y * opacity);
		this.dst[(6 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(6 * this.res + z) * this.res + x]) * num2 + v2.z * opacity);
		this.dst[(7 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(7 * this.res + z) * this.res + x]) * num2 + v2.w * opacity);
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x0009E4F0 File Offset: 0x0009C6F0
	public void SetSplat(Vector3 worldPos, int id, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(normX, normZ, id, opacity, radius, fade);
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x0009E524 File Offset: 0x0009C724
	public void SetSplat(float normX, float normZ, int id, float opacity, float radius, float fade = 0f)
	{
		int idx = TerrainSplat.TypeToIndex(id);
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				float num = (float)this.dst[(idx * this.res + z) * this.res + x];
				float new_val = Mathf.Lerp(num, 1f, lerp * opacity);
				this.SetSplat(x, z, id, num, new_val);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06001CC5 RID: 7365 RVA: 0x0009E578 File Offset: 0x0009C778
	public void AddSplat(Vector3 worldPos, int id, float delta, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddSplat(normX, normZ, id, delta, radius, fade);
	}

	// Token: 0x06001CC6 RID: 7366 RVA: 0x0009E5AC File Offset: 0x0009C7AC
	public void AddSplat(float normX, float normZ, int id, float delta, float radius, float fade = 0f)
	{
		int idx = TerrainSplat.TypeToIndex(id);
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				float num = (float)this.dst[(idx * this.res + z) * this.res + x];
				float new_val = Mathf.Clamp01(num + lerp * delta);
				this.SetSplat(x, z, id, num, new_val);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06001CC7 RID: 7367 RVA: 0x0009E600 File Offset: 0x0009C800
	private void SetSplat(int x, int z, int id, float old_val, float new_val)
	{
		int num = TerrainSplat.TypeToIndex(id);
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
