using System;
using UnityEngine;

// Token: 0x020004D0 RID: 1232
public class TerrainHeightMap : TerrainMap<short>
{
	// Token: 0x04001920 RID: 6432
	public Texture2D HeightTexture;

	// Token: 0x04001921 RID: 6433
	public Texture2D NormalTexture;

	// Token: 0x04001922 RID: 6434
	private float normY;

	// Token: 0x06001C53 RID: 7251 RVA: 0x0009BBD4 File Offset: 0x00099DD4
	public override void Setup()
	{
		if (this.HeightTexture != null)
		{
			if (this.HeightTexture.width == this.HeightTexture.height)
			{
				this.res = this.HeightTexture.width;
				this.src = (this.dst = new short[this.res * this.res]);
				Color32[] pixels = this.HeightTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						Color32 color = pixels[num];
						this.dst[i * this.res + j] = BitUtility.DecodeShort(color);
						j++;
						num++;
					}
					i++;
				}
			}
			else
			{
				Debug.LogError("Invalid height texture: " + this.HeightTexture.name);
			}
		}
		else
		{
			this.res = this.terrain.terrainData.heightmapResolution;
			this.src = (this.dst = new short[this.res * this.res]);
		}
		this.normY = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)this.res;
	}

	// Token: 0x06001C54 RID: 7252 RVA: 0x0009BD14 File Offset: 0x00099F14
	public void ApplyToTerrain()
	{
		float[,] heights = this.terrain.terrainData.GetHeights(0, 0, this.res, this.res);
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				heights[z, i] = this.GetHeight01(i, z);
			}
		});
		this.terrain.terrainData.SetHeights(0, 0, heights);
		TerrainCollider component = this.terrain.GetComponent<TerrainCollider>();
		if (component)
		{
			component.enabled = false;
			component.enabled = true;
		}
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x0009BDA4 File Offset: 0x00099FA4
	public void GenerateTextures(bool heightTexture = true, bool normalTexture = true)
	{
		if (heightTexture)
		{
			Color32[] heights = new Color32[this.res * this.res];
			Parallel.For(0, this.res, delegate(int z)
			{
				for (int i = 0; i < this.res; i++)
				{
					heights[z * this.res + i] = BitUtility.EncodeShort(this.src[z * this.res + i]);
				}
			});
			this.HeightTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
			this.HeightTexture.name = "HeightTexture";
			this.HeightTexture.wrapMode = TextureWrapMode.Clamp;
			this.HeightTexture.SetPixels32(heights);
		}
		if (normalTexture)
		{
			int normalres = this.res - 1;
			Color32[] normals = new Color32[normalres * normalres];
			Parallel.For(0, normalres, delegate(int z)
			{
				float normZ = ((float)z + 0.5f) / (float)normalres;
				for (int i = 0; i < normalres; i++)
				{
					float normX = ((float)i + 0.5f) / (float)normalres;
					Vector3 normal = this.GetNormal(normX, normZ);
					normals[z * normalres + i] = BitUtility.EncodeNormal(normal);
				}
			});
			this.NormalTexture = new Texture2D(normalres, normalres, TextureFormat.RGBA32, true, true);
			this.NormalTexture.name = "NormalTexture";
			this.NormalTexture.wrapMode = TextureWrapMode.Clamp;
			this.NormalTexture.SetPixels32(normals);
		}
	}

	// Token: 0x06001C56 RID: 7254 RVA: 0x0009BED4 File Offset: 0x0009A0D4
	public void ApplyTextures()
	{
		this.HeightTexture.Apply(true, false);
		this.NormalTexture.Apply(true, false);
		this.NormalTexture.Compress(false);
		this.HeightTexture.Apply(false, true);
		this.NormalTexture.Apply(false, true);
	}

	// Token: 0x06001C57 RID: 7255 RVA: 0x00017172 File Offset: 0x00015372
	public float GetHeight(Vector3 worldPos)
	{
		return TerrainMeta.Position.y + this.GetHeight01(worldPos) * TerrainMeta.Size.y;
	}

	// Token: 0x06001C58 RID: 7256 RVA: 0x00017191 File Offset: 0x00015391
	public float GetHeight(float normX, float normZ)
	{
		return TerrainMeta.Position.y + this.GetHeight01(normX, normZ) * TerrainMeta.Size.y;
	}

	// Token: 0x06001C59 RID: 7257 RVA: 0x0009BF24 File Offset: 0x0009A124
	public float GetHeightFast(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = (int)num2;
		int num5 = (int)num3;
		float num6 = num2 - (float)num4;
		float num7 = num3 - (float)num5;
		num4 = ((num4 >= 0) ? num4 : 0);
		num5 = ((num5 >= 0) ? num5 : 0);
		num4 = ((num4 <= num) ? num4 : num);
		num5 = ((num5 <= num) ? num5 : num);
		int num8 = (num2 < (float)num) ? 1 : 0;
		int num9 = (num3 < (float)num) ? this.res : 0;
		int num10 = num5 * this.res + num4;
		int num11 = num10 + num8;
		int num12 = num10 + num9;
		int num13 = num12 + num8;
		float num14 = (float)this.src[num10] * 3.051944E-05f;
		float num15 = (float)this.src[num11] * 3.051944E-05f;
		float num16 = (float)this.src[num12] * 3.051944E-05f;
		float num17 = (float)this.src[num13] * 3.051944E-05f;
		float num18 = (num15 - num14) * num6 + num14;
		float num19 = ((num17 - num16) * num6 + num16 - num18) * num7 + num18;
		return TerrainMeta.Position.y + num19 * TerrainMeta.Size.y;
	}

	// Token: 0x06001C5A RID: 7258 RVA: 0x000171B1 File Offset: 0x000153B1
	public float GetHeight(int x, int z)
	{
		return TerrainMeta.Position.y + this.GetHeight01(x, z) * TerrainMeta.Size.y;
	}

	// Token: 0x06001C5B RID: 7259 RVA: 0x0009C034 File Offset: 0x0009A234
	public float GetHeight01(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetHeight01(normX, normZ);
	}

	// Token: 0x06001C5C RID: 7260 RVA: 0x0009C064 File Offset: 0x0009A264
	public float GetHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float height = this.GetHeight01(num4, num5);
		float height2 = this.GetHeight01(x, num5);
		float height3 = this.GetHeight01(num4, z);
		float height4 = this.GetHeight01(x, z);
		float t = num2 - (float)num4;
		float t2 = num3 - (float)num5;
		float a = Mathf.Lerp(height, height2, t);
		float b = Mathf.Lerp(height3, height4, t);
		return Mathf.Lerp(a, b, t2);
	}

	// Token: 0x06001C5D RID: 7261 RVA: 0x000171D1 File Offset: 0x000153D1
	public float GetHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06001C5E RID: 7262 RVA: 0x000171D1 File Offset: 0x000153D1
	private float GetSrcHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06001C5F RID: 7263 RVA: 0x000171E9 File Offset: 0x000153E9
	private float GetDstHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.dst[z * this.res + x]);
	}

	// Token: 0x06001C60 RID: 7264 RVA: 0x0009C108 File Offset: 0x0009A308
	public Vector3 GetNormal(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetNormal(normX, normZ);
	}

	// Token: 0x06001C61 RID: 7265 RVA: 0x0009C138 File Offset: 0x0009A338
	public Vector3 GetNormal(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		Vector3 normal = this.GetNormal(num4, num5);
		Vector3 normal2 = this.GetNormal(x, num5);
		Vector3 normal3 = this.GetNormal(num4, z);
		Vector3 normal4 = this.GetNormal(x, z);
		float t = num2 - (float)num4;
		float t2 = num3 - (float)num5;
		Vector3 a = Vector3.Lerp(normal, normal2, t);
		Vector3 b = Vector3.Lerp(normal3, normal4, t);
		return Vector3.Lerp(a, b, t2).normalized;
	}

	// Token: 0x06001C62 RID: 7266 RVA: 0x0009C1E8 File Offset: 0x0009A3E8
	public Vector3 GetNormal(int x, int z)
	{
		int max = this.res - 1;
		int x2 = Mathf.Clamp(x - 1, 0, max);
		int z2 = Mathf.Clamp(z - 1, 0, max);
		int x3 = Mathf.Clamp(x + 1, 0, max);
		int z3 = Mathf.Clamp(z + 1, 0, max);
		float num = (this.GetHeight01(x3, z2) - this.GetHeight01(x2, z2)) * 0.5f;
		float num2 = (this.GetHeight01(x2, z3) - this.GetHeight01(x2, z2)) * 0.5f;
		return new Vector3(-num, this.normY, -num2).normalized;
	}

	// Token: 0x06001C63 RID: 7267 RVA: 0x0009C274 File Offset: 0x0009A474
	private Vector3 GetNormalSobel(int x, int z)
	{
		int num = this.res - 1;
		Vector3 vector = new Vector3(TerrainMeta.Size.x / (float)num, TerrainMeta.Size.y, TerrainMeta.Size.z / (float)num);
		int x2 = Mathf.Clamp(x - 1, 0, num);
		int z2 = Mathf.Clamp(z - 1, 0, num);
		int x3 = Mathf.Clamp(x + 1, 0, num);
		int z3 = Mathf.Clamp(z + 1, 0, num);
		float num2 = this.GetHeight01(x2, z2) * -1f;
		num2 += this.GetHeight01(x2, z) * -2f;
		num2 += this.GetHeight01(x2, z3) * -1f;
		num2 += this.GetHeight01(x3, z2) * 1f;
		num2 += this.GetHeight01(x3, z) * 2f;
		num2 += this.GetHeight01(x3, z3) * 1f;
		num2 *= vector.y;
		num2 /= vector.x;
		float num3 = this.GetHeight01(x2, z2) * -1f;
		num3 += this.GetHeight01(x, z2) * -2f;
		num3 += this.GetHeight01(x3, z2) * -1f;
		num3 += this.GetHeight01(x2, z3) * 1f;
		num3 += this.GetHeight01(x, z3) * 2f;
		num3 += this.GetHeight01(x3, z3) * 1f;
		num3 *= vector.y;
		num3 /= vector.z;
		Vector3 vector2 = new Vector3(-num2, 8f, -num3);
		return vector2.normalized;
	}

	// Token: 0x06001C64 RID: 7268 RVA: 0x00017201 File Offset: 0x00015401
	public float GetSlope(Vector3 worldPos)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(worldPos));
	}

	// Token: 0x06001C65 RID: 7269 RVA: 0x00017214 File Offset: 0x00015414
	public float GetSlope(float normX, float normZ)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(normX, normZ));
	}

	// Token: 0x06001C66 RID: 7270 RVA: 0x00017228 File Offset: 0x00015428
	public float GetSlope(int x, int z)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(x, z));
	}

	// Token: 0x06001C67 RID: 7271 RVA: 0x0001723C File Offset: 0x0001543C
	public float GetSlope01(Vector3 worldPos)
	{
		return this.GetSlope(worldPos) * 0.011111111f;
	}

	// Token: 0x06001C68 RID: 7272 RVA: 0x0001724B File Offset: 0x0001544B
	public float GetSlope01(float normX, float normZ)
	{
		return this.GetSlope(normX, normZ) * 0.011111111f;
	}

	// Token: 0x06001C69 RID: 7273 RVA: 0x0001725B File Offset: 0x0001545B
	public float GetSlope01(int x, int z)
	{
		return this.GetSlope(x, z) * 0.011111111f;
	}

	// Token: 0x06001C6A RID: 7274 RVA: 0x0009C420 File Offset: 0x0009A620
	public void SetHeight(Vector3 worldPos, float height)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(normX, normZ, height);
	}

	// Token: 0x06001C6B RID: 7275 RVA: 0x0009C450 File Offset: 0x0009A650
	public void SetHeight(float normX, float normZ, float height)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetHeight(x, z, height);
	}

	// Token: 0x06001C6C RID: 7276 RVA: 0x0001726B File Offset: 0x0001546B
	public void SetHeight(int x, int z, float height)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Short(height);
	}

	// Token: 0x06001C6D RID: 7277 RVA: 0x0009C478 File Offset: 0x0009A678
	public void SetHeight(Vector3 worldPos, float height, float opacity)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(normX, normZ, height, opacity);
	}

	// Token: 0x06001C6E RID: 7278 RVA: 0x0009C4A8 File Offset: 0x0009A6A8
	public void SetHeight(float normX, float normZ, float height, float opacity)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetHeight(x, z, height, opacity);
	}

	// Token: 0x06001C6F RID: 7279 RVA: 0x0009C4D0 File Offset: 0x0009A6D0
	public void SetHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.SmoothStep(this.GetDstHeight01(x, z), height, opacity);
		this.SetHeight(x, z, height2);
	}

	// Token: 0x06001C70 RID: 7280 RVA: 0x0009C4F8 File Offset: 0x0009A6F8
	public void AddHeight(Vector3 worldPos, float delta)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddHeight(normX, normZ, delta);
	}

	// Token: 0x06001C71 RID: 7281 RVA: 0x0009C528 File Offset: 0x0009A728
	public void AddHeight(float normX, float normZ, float delta)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.AddHeight(x, z, delta);
	}

	// Token: 0x06001C72 RID: 7282 RVA: 0x0009C550 File Offset: 0x0009A750
	public void AddHeight(int x, int z, float delta)
	{
		float height = Mathf.Clamp01(this.GetDstHeight01(x, z) + delta);
		this.SetHeight(x, z, height);
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x0009C578 File Offset: 0x0009A778
	public void LowerHeight(Vector3 worldPos, float height, float opacity)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.LowerHeight(normX, normZ, height, opacity);
	}

	// Token: 0x06001C74 RID: 7284 RVA: 0x0009C5A8 File Offset: 0x0009A7A8
	public void LowerHeight(float normX, float normZ, float height, float opacity)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.LowerHeight(x, z, height, opacity);
	}

	// Token: 0x06001C75 RID: 7285 RVA: 0x0009C5D0 File Offset: 0x0009A7D0
	public void LowerHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.Min(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
		this.SetHeight(x, z, height2);
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x0009C604 File Offset: 0x0009A804
	public void RaiseHeight(Vector3 worldPos, float height, float opacity)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.RaiseHeight(normX, normZ, height, opacity);
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x0009C634 File Offset: 0x0009A834
	public void RaiseHeight(float normX, float normZ, float height, float opacity)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.RaiseHeight(x, z, height, opacity);
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x0009C65C File Offset: 0x0009A85C
	public void RaiseHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.Max(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
		this.SetHeight(x, z, height2);
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x0009C690 File Offset: 0x0009A890
	public void SetHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		this.SetHeight(normX, normZ, height, opacity, radius, fade);
	}

	// Token: 0x06001C7A RID: 7290 RVA: 0x0009C6D0 File Offset: 0x0009A8D0
	public void SetHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.SetHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06001C7B RID: 7291 RVA: 0x0009C714 File Offset: 0x0009A914
	public void LowerHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		this.LowerHeight(normX, normZ, height, opacity, radius, fade);
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x0009C754 File Offset: 0x0009A954
	public void LowerHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.LowerHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x0009C798 File Offset: 0x0009A998
	public void RaiseHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		this.RaiseHeight(normX, normZ, height, opacity, radius, fade);
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x0009C7D8 File Offset: 0x0009A9D8
	public void RaiseHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.RaiseHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x0009C81C File Offset: 0x0009AA1C
	public void AddHeight(Vector3 worldPos, float delta, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddHeight(normX, normZ, delta, radius, fade);
	}

	// Token: 0x06001C80 RID: 7296 RVA: 0x0009C850 File Offset: 0x0009AA50
	public void AddHeight(float normX, float normZ, float delta, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.AddHeight(x, z, lerp * delta);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}
}
