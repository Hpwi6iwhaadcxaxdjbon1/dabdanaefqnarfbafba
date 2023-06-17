using System;
using UnityEngine;

// Token: 0x020004E5 RID: 1253
public class TerrainWaterMap : TerrainMap<short>
{
	// Token: 0x0400195E RID: 6494
	public Texture2D WaterTexture;

	// Token: 0x0400195F RID: 6495
	private float normY;

	// Token: 0x06001CF1 RID: 7409 RVA: 0x0009EFA8 File Offset: 0x0009D1A8
	public override void Setup()
	{
		if (this.WaterTexture != null)
		{
			if (this.WaterTexture.width == this.WaterTexture.height)
			{
				this.res = this.WaterTexture.width;
				this.src = (this.dst = new short[this.res * this.res]);
				Color32[] pixels = this.WaterTexture.GetPixels32();
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
				Debug.LogError("Invalid water texture: " + this.WaterTexture.name);
			}
		}
		else
		{
			this.res = this.terrain.terrainData.heightmapResolution;
			this.src = (this.dst = new short[this.res * this.res]);
		}
		this.normY = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)this.res;
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x0009F0E8 File Offset: 0x0009D2E8
	public void GenerateTextures()
	{
		Color32[] heights = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				heights[z * this.res + i] = BitUtility.EncodeShort(this.src[z * this.res + i]);
			}
		});
		this.WaterTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.WaterTexture.name = "WaterTexture";
		this.WaterTexture.wrapMode = TextureWrapMode.Clamp;
		this.WaterTexture.SetPixels32(heights);
	}

	// Token: 0x06001CF3 RID: 7411 RVA: 0x0001752D File Offset: 0x0001572D
	public void ApplyTextures()
	{
		this.WaterTexture.Apply(true, true);
	}

	// Token: 0x06001CF4 RID: 7412 RVA: 0x0001753C File Offset: 0x0001573C
	public float GetHeight(Vector3 worldPos)
	{
		return TerrainMeta.Position.y + this.GetHeight01(worldPos) * TerrainMeta.Size.y;
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x0001755B File Offset: 0x0001575B
	public float GetHeight(float normX, float normZ)
	{
		return TerrainMeta.Position.y + this.GetHeight01(normX, normZ) * TerrainMeta.Size.y;
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x0009BF24 File Offset: 0x0009A124
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

	// Token: 0x06001CF7 RID: 7415 RVA: 0x0001757B File Offset: 0x0001577B
	public float GetHeight(int x, int z)
	{
		return TerrainMeta.Position.y + this.GetHeight01(x, z) * TerrainMeta.Size.y;
	}

	// Token: 0x06001CF8 RID: 7416 RVA: 0x0009F17C File Offset: 0x0009D37C
	public float GetHeight01(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetHeight01(normX, normZ);
	}

	// Token: 0x06001CF9 RID: 7417 RVA: 0x0009F1AC File Offset: 0x0009D3AC
	public float GetHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float a = Mathf.Lerp(this.GetHeight01(num4, num5), this.GetHeight01(x, num5), num2 - (float)num4);
		float b = Mathf.Lerp(this.GetHeight01(num4, z), this.GetHeight01(x, z), num2 - (float)num4);
		return Mathf.Lerp(a, b, num3 - (float)num5);
	}

	// Token: 0x06001CFA RID: 7418 RVA: 0x000171D1 File Offset: 0x000153D1
	public float GetHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06001CFB RID: 7419 RVA: 0x0009F240 File Offset: 0x0009D440
	public Vector3 GetNormal(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetNormal(normX, normZ);
	}

	// Token: 0x06001CFC RID: 7420 RVA: 0x0009F270 File Offset: 0x0009D470
	public Vector3 GetNormal(float normX, float normZ)
	{
		int num = this.res - 1;
		int num2 = (int)(normX * (float)num);
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp(num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float num6 = this.GetHeight01(x, num5) - this.GetHeight01(num4, num5);
		float num7 = this.GetHeight01(num4, z) - this.GetHeight01(num4, num5);
		return new Vector3(-num6, this.normY, -num7).normalized;
	}

	// Token: 0x06001CFD RID: 7421 RVA: 0x0009F2F8 File Offset: 0x0009D4F8
	public Vector3 GetNormalFast(float normX, float normZ)
	{
		int num = this.res - 1;
		int num2 = (int)(normX * (float)num);
		int num3 = (int)(normZ * (float)num);
		num2 = ((num2 >= 0) ? num2 : 0);
		num3 = ((num3 >= 0) ? num3 : 0);
		num2 = ((num2 <= num) ? num2 : num);
		num3 = ((num3 <= num) ? num3 : num);
		int num4 = (num2 < num) ? 1 : 0;
		int num5 = (num3 < num) ? this.res : 0;
		int num6 = num3 * this.res + num2;
		int num7 = num6 + num4;
		int num8 = num6 + num5;
		short num9 = this.src[num6];
		float num10 = (float)this.src[num7];
		short num11 = this.src[num8];
		float num12 = (num10 - (float)num9) * 3.051944E-05f;
		float num13 = (float)(num11 - num9) * 3.051944E-05f;
		return new Vector3(-num12, this.normY, -num13);
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x0009F3B8 File Offset: 0x0009D5B8
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

	// Token: 0x06001CFF RID: 7423 RVA: 0x0001759B File Offset: 0x0001579B
	public float GetSlope(Vector3 worldPos)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(worldPos));
	}

	// Token: 0x06001D00 RID: 7424 RVA: 0x000175AE File Offset: 0x000157AE
	public float GetSlope(float normX, float normZ)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(normX, normZ));
	}

	// Token: 0x06001D01 RID: 7425 RVA: 0x000175C2 File Offset: 0x000157C2
	public float GetSlope(int x, int z)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(x, z));
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x000175D6 File Offset: 0x000157D6
	public float GetSlope01(Vector3 worldPos)
	{
		return this.GetSlope(worldPos) * 0.011111111f;
	}

	// Token: 0x06001D03 RID: 7427 RVA: 0x000175E5 File Offset: 0x000157E5
	public float GetSlope01(float normX, float normZ)
	{
		return this.GetSlope(normX, normZ) * 0.011111111f;
	}

	// Token: 0x06001D04 RID: 7428 RVA: 0x000175F5 File Offset: 0x000157F5
	public float GetSlope01(int x, int z)
	{
		return this.GetSlope(x, z) * 0.011111111f;
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x00017605 File Offset: 0x00015805
	public float GetDepth(Vector3 worldPos)
	{
		return this.GetHeight(worldPos) - TerrainMeta.HeightMap.GetHeight(worldPos);
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x0001761A File Offset: 0x0001581A
	public float GetDepth(float normX, float normZ)
	{
		return this.GetHeight(normX, normZ) - TerrainMeta.HeightMap.GetHeight(normX, normZ);
	}

	// Token: 0x06001D07 RID: 7431 RVA: 0x0009F444 File Offset: 0x0009D644
	public void SetHeight(Vector3 worldPos, float height)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(normX, normZ, height);
	}

	// Token: 0x06001D08 RID: 7432 RVA: 0x0009F474 File Offset: 0x0009D674
	public void SetHeight(float normX, float normZ, float height)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetHeight(x, z, height);
	}

	// Token: 0x06001D09 RID: 7433 RVA: 0x0001726B File Offset: 0x0001546B
	public void SetHeight(int x, int z, float height)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Short(height);
	}
}
