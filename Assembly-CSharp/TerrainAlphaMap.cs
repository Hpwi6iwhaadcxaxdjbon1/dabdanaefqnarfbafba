using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004C8 RID: 1224
public class TerrainAlphaMap : TerrainMap<byte>
{
	// Token: 0x04001910 RID: 6416
	[FormerlySerializedAs("ColorTexture")]
	public Texture2D AlphaTexture;

	// Token: 0x06001C17 RID: 7191 RVA: 0x0009AAD8 File Offset: 0x00098CD8
	public override void Setup()
	{
		if (!(this.AlphaTexture != null))
		{
			this.res = this.terrain.terrainData.alphamapResolution;
			this.src = (this.dst = new byte[this.res * this.res]);
			for (int i = 0; i < this.res; i++)
			{
				for (int j = 0; j < this.res; j++)
				{
					this.dst[i * this.res + j] = byte.MaxValue;
				}
			}
			return;
		}
		if (this.AlphaTexture.width == this.AlphaTexture.height)
		{
			this.res = this.AlphaTexture.width;
			this.src = (this.dst = new byte[this.res * this.res]);
			Color32[] pixels = this.AlphaTexture.GetPixels32();
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
		Debug.LogError("Invalid alpha texture: " + this.AlphaTexture.name);
	}

	// Token: 0x06001C18 RID: 7192 RVA: 0x0009AC30 File Offset: 0x00098E30
	public void GenerateTextures()
	{
		this.AlphaTexture = new Texture2D(this.res, this.res, TextureFormat.Alpha8, false, true);
		this.AlphaTexture.name = "AlphaTexture";
		this.AlphaTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b = this.src[z * this.res + i];
				col[z * this.res + i] = new Color32(b, b, b, b);
			}
		});
		this.AlphaTexture.SetPixels32(col);
	}

	// Token: 0x06001C19 RID: 7193 RVA: 0x00016FF8 File Offset: 0x000151F8
	public void ApplyTextures()
	{
		this.AlphaTexture.Apply(true, false);
		this.AlphaTexture.Compress(false);
		this.AlphaTexture.Apply(false, true);
	}

	// Token: 0x06001C1A RID: 7194 RVA: 0x0009ACC4 File Offset: 0x00098EC4
	public float GetAlpha(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetAlpha(normX, normZ);
	}

	// Token: 0x06001C1B RID: 7195 RVA: 0x0009ACF4 File Offset: 0x00098EF4
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

	// Token: 0x06001C1C RID: 7196 RVA: 0x00017020 File Offset: 0x00015220
	public float GetAlpha(int x, int z)
	{
		return BitUtility.Byte2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06001C1D RID: 7197 RVA: 0x0009AD88 File Offset: 0x00098F88
	public void SetAlpha(Vector3 worldPos, float a)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a);
	}

	// Token: 0x06001C1E RID: 7198 RVA: 0x0009ADB8 File Offset: 0x00098FB8
	public void SetAlpha(float normX, float normZ, float a)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetAlpha(x, z, a);
	}

	// Token: 0x06001C1F RID: 7199 RVA: 0x00017038 File Offset: 0x00015238
	public void SetAlpha(int x, int z, float a)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Byte(a);
	}

	// Token: 0x06001C20 RID: 7200 RVA: 0x00017051 File Offset: 0x00015251
	public void SetAlpha(int x, int z, float a, float opacity)
	{
		this.SetAlpha(x, z, Mathf.Lerp(this.GetAlpha(x, z), a, opacity));
	}

	// Token: 0x06001C21 RID: 7201 RVA: 0x0009ADE0 File Offset: 0x00098FE0
	public void SetAlpha(Vector3 worldPos, float a, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a, opacity, radius, fade);
	}

	// Token: 0x06001C22 RID: 7202 RVA: 0x0009AE14 File Offset: 0x00099014
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
