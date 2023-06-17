using System;
using UnityEngine;

// Token: 0x020004CD RID: 1229
public class TerrainBlendMap : TerrainMap<byte>
{
	// Token: 0x0400191A RID: 6426
	public Texture2D BlendTexture;

	// Token: 0x06001C42 RID: 7234 RVA: 0x0009B7F4 File Offset: 0x000999F4
	public override void Setup()
	{
		if (!(this.BlendTexture != null))
		{
			this.res = this.terrain.terrainData.alphamapResolution;
			this.src = (this.dst = new byte[this.res * this.res]);
			for (int i = 0; i < this.res; i++)
			{
				for (int j = 0; j < this.res; j++)
				{
					this.dst[i * this.res + j] = 0;
				}
			}
			return;
		}
		if (this.BlendTexture.width == this.BlendTexture.height)
		{
			this.res = this.BlendTexture.width;
			this.src = (this.dst = new byte[this.res * this.res]);
			Color32[] pixels = this.BlendTexture.GetPixels32();
			int k = 0;
			int num = 0;
			while (k < this.res)
			{
				int l = 0;
				while (l < this.res)
				{
					this.dst[k * this.res + l] = pixels[num].a;
					l++;
					num++;
				}
				k++;
			}
			return;
		}
		Debug.LogError("Invalid alpha texture: " + this.BlendTexture.name);
	}

	// Token: 0x06001C43 RID: 7235 RVA: 0x0009B948 File Offset: 0x00099B48
	public void GenerateTextures()
	{
		this.BlendTexture = new Texture2D(this.res, this.res, TextureFormat.Alpha8, true, true);
		this.BlendTexture.name = "BlendTexture";
		this.BlendTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b = this.src[z * this.res + i];
				col[z * this.res + i] = new Color32(b, b, b, b);
			}
		});
		this.BlendTexture.SetPixels32(col);
	}

	// Token: 0x06001C44 RID: 7236 RVA: 0x00017108 File Offset: 0x00015308
	public void ApplyTextures()
	{
		this.BlendTexture.Apply(true, false);
		this.BlendTexture.Compress(false);
		this.BlendTexture.Apply(false, true);
	}

	// Token: 0x06001C45 RID: 7237 RVA: 0x0009B9DC File Offset: 0x00099BDC
	public float GetAlpha(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetAlpha(normX, normZ);
	}

	// Token: 0x06001C46 RID: 7238 RVA: 0x0009BA0C File Offset: 0x00099C0C
	public float GetAlpha(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float a = Mathf.Lerp(this.GetAlpha(num4, num5), this.GetAlpha(x, num5), num2 - (float)num4);
		float b = Mathf.Lerp(this.GetAlpha(num4, z), this.GetAlpha(x, z), num2 - (float)num4);
		return Mathf.Lerp(a, b, num3 - (float)num5);
	}

	// Token: 0x06001C47 RID: 7239 RVA: 0x00017020 File Offset: 0x00015220
	public float GetAlpha(int x, int z)
	{
		return BitUtility.Byte2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06001C48 RID: 7240 RVA: 0x0009BAA0 File Offset: 0x00099CA0
	public void SetAlpha(Vector3 worldPos, float a)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a);
	}

	// Token: 0x06001C49 RID: 7241 RVA: 0x0009BAD0 File Offset: 0x00099CD0
	public void SetAlpha(float normX, float normZ, float a)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetAlpha(x, z, a);
	}

	// Token: 0x06001C4A RID: 7242 RVA: 0x00017038 File Offset: 0x00015238
	public void SetAlpha(int x, int z, float a)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Byte(a);
	}

	// Token: 0x06001C4B RID: 7243 RVA: 0x00017130 File Offset: 0x00015330
	public void SetAlpha(int x, int z, float a, float opacity)
	{
		this.SetAlpha(x, z, Mathf.Lerp(this.GetAlpha(x, z), a, opacity));
	}

	// Token: 0x06001C4C RID: 7244 RVA: 0x0009BAF8 File Offset: 0x00099CF8
	public void SetAlpha(Vector3 worldPos, float a, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a, opacity, radius, fade);
	}

	// Token: 0x06001C4D RID: 7245 RVA: 0x0009BB2C File Offset: 0x00099D2C
	public void SetAlpha(float normX, float normZ, float a, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			lerp *= opacity;
			if (lerp > 0f)
			{
				this.SetAlpha(x, z, a, lerp);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}
}
