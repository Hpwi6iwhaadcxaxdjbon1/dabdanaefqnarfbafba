using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200058F RID: 1423
public class SubsurfaceProfileTexture
{
	// Token: 0x04001C79 RID: 7289
	public const int SUBSURFACE_RADIUS_SCALE = 1024;

	// Token: 0x04001C7A RID: 7290
	public const int SUBSURFACE_KERNEL_SIZE = 3;

	// Token: 0x04001C7B RID: 7291
	private List<SubsurfaceProfileTexture.SubsurfaceProfileEntry> entries = new List<SubsurfaceProfileTexture.SubsurfaceProfileEntry>(16);

	// Token: 0x04001C7C RID: 7292
	private Texture2D texture;

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x06002082 RID: 8322 RVA: 0x00019BB2 File Offset: 0x00017DB2
	public Texture2D Texture
	{
		get
		{
			if (!(this.texture == null))
			{
				return this.texture;
			}
			return this.CreateTexture();
		}
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x00019BCF File Offset: 0x00017DCF
	public SubsurfaceProfileTexture()
	{
		this.AddProfile(SubsurfaceProfileData.Default, null);
	}

	// Token: 0x06002084 RID: 8324 RVA: 0x000B0E70 File Offset: 0x000AF070
	public int FindEntryIndex(SubsurfaceProfile profile)
	{
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (this.entries[i].profile == profile)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002085 RID: 8325 RVA: 0x000B0EB0 File Offset: 0x000AF0B0
	public int AddProfile(SubsurfaceProfileData data, SubsurfaceProfile profile)
	{
		int num = -1;
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (this.entries[i].profile == profile)
			{
				num = i;
				this.entries[num] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, profile);
				break;
			}
		}
		if (num < 0)
		{
			num = this.entries.Count;
			this.entries.Add(new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, profile));
		}
		this.ReleaseTexture();
		return num;
	}

	// Token: 0x06002086 RID: 8326 RVA: 0x00019BF1 File Offset: 0x00017DF1
	public void UpdateProfile(int id, SubsurfaceProfileData data)
	{
		if (id >= 0)
		{
			this.entries[id] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, this.entries[id].profile);
			this.ReleaseTexture();
		}
	}

	// Token: 0x06002087 RID: 8327 RVA: 0x00019C20 File Offset: 0x00017E20
	public void RemoveProfile(int id)
	{
		if (id >= 0)
		{
			this.entries[id] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(SubsurfaceProfileData.Invalid, null);
			this.CheckReleaseTexture();
		}
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x000B0F30 File Offset: 0x000AF130
	public static Color ColorClamp(Color color, float min = 0f, float max = 1f)
	{
		Color result;
		result.r = Mathf.Clamp(color.r, min, max);
		result.g = Mathf.Clamp(color.g, min, max);
		result.b = Mathf.Clamp(color.b, min, max);
		result.a = Mathf.Clamp(color.a, min, max);
		return result;
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x000B0F90 File Offset: 0x000AF190
	private Texture2D CreateTexture()
	{
		if (this.entries.Count > 0)
		{
			int num = 32;
			int num2 = Mathf.Max(this.entries.Count, 64);
			this.ReleaseTexture();
			this.texture = new Texture2D(num, num2, TextureFormat.RGBAHalf, false, true);
			this.texture.name = "SubsurfaceProfiles";
			this.texture.wrapMode = TextureWrapMode.Clamp;
			this.texture.filterMode = FilterMode.Bilinear;
			Color[] pixels = this.texture.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = Color.clear;
			}
			Color[] array = new Color[num];
			for (int j = 0; j < this.entries.Count; j++)
			{
				SubsurfaceProfileData data = this.entries[j].data;
				data.SubsurfaceColor = SubsurfaceProfileTexture.ColorClamp(data.SubsurfaceColor, 0f, 1f);
				data.FalloffColor = SubsurfaceProfileTexture.ColorClamp(data.FalloffColor, 0.009f, 1f);
				array[0] = data.SubsurfaceColor;
				array[0].a = 0f;
				SeparableSSS.CalculateKernel(array, 1, 13, data.SubsurfaceColor, data.FalloffColor);
				SeparableSSS.CalculateKernel(array, 14, 9, data.SubsurfaceColor, data.FalloffColor);
				SeparableSSS.CalculateKernel(array, 23, 6, data.SubsurfaceColor, data.FalloffColor);
				int num3 = num * (num2 - j - 1);
				for (int k = 0; k < 29; k++)
				{
					Color color = array[k] * new Color(1f, 1f, 1f, 0.33333334f);
					color.a *= data.ScatterRadius / 1024f;
					pixels[num3 + k] = color;
				}
			}
			this.texture.SetPixels(pixels, 0);
			this.texture.Apply(false, false);
			return this.texture;
		}
		return null;
	}

	// Token: 0x0600208A RID: 8330 RVA: 0x000B1194 File Offset: 0x000AF394
	private void CheckReleaseTexture()
	{
		int num = 0;
		for (int i = 0; i < this.entries.Count; i++)
		{
			num += ((this.entries[i].profile == null) ? 1 : 0);
		}
		if (this.entries.Count == num)
		{
			this.ReleaseTexture();
		}
	}

	// Token: 0x0600208B RID: 8331 RVA: 0x00019C43 File Offset: 0x00017E43
	private void ReleaseTexture()
	{
		if (this.texture != null)
		{
			Object.DestroyImmediate(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x02000590 RID: 1424
	private struct SubsurfaceProfileEntry
	{
		// Token: 0x04001C7D RID: 7293
		public SubsurfaceProfileData data;

		// Token: 0x04001C7E RID: 7294
		public SubsurfaceProfile profile;

		// Token: 0x0600208C RID: 8332 RVA: 0x00019C65 File Offset: 0x00017E65
		public SubsurfaceProfileEntry(SubsurfaceProfileData data, SubsurfaceProfile profile)
		{
			this.data = data;
			this.profile = profile;
		}
	}
}
