using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006A8 RID: 1704
public class UIPaintableImage : MonoBehaviour
{
	// Token: 0x040021F0 RID: 8688
	public RawImage image;

	// Token: 0x040021F1 RID: 8689
	public int texSize = 64;

	// Token: 0x040021F2 RID: 8690
	public Color clearColor = Color.clear;

	// Token: 0x040021F3 RID: 8691
	public FilterMode filterMode = FilterMode.Bilinear;

	// Token: 0x040021F4 RID: 8692
	public bool mipmaps;

	// Token: 0x040021F5 RID: 8693
	[NonSerialized]
	public int imageNumber;

	// Token: 0x040021F6 RID: 8694
	[NonSerialized]
	public uint imageHash;

	// Token: 0x040021F7 RID: 8695
	[NonSerialized]
	public bool isLocked = true;

	// Token: 0x040021F8 RID: 8696
	[NonSerialized]
	public bool isLoading;

	// Token: 0x040021F9 RID: 8697
	[NonSerialized]
	public bool isBlank = true;

	// Token: 0x040021FA RID: 8698
	[NonSerialized]
	public bool needsApply;

	// Token: 0x040021FB RID: 8699
	internal Texture2D texture;

	// Token: 0x17000278 RID: 632
	// (get) Token: 0x060025FA RID: 9722 RVA: 0x0000B1D8 File Offset: 0x000093D8
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x060025FB RID: 9723 RVA: 0x0001D9D6 File Offset: 0x0001BBD6
	private void Start()
	{
		this.SetupTexture();
		this.imageHash = 0U;
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x0001D9E5 File Offset: 0x0001BBE5
	private void Update()
	{
		if (this.needsApply && this.texture != null)
		{
			this.texture.Apply();
			this.needsApply = false;
		}
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x000C7AD8 File Offset: 0x000C5CD8
	public void SetupTexture()
	{
		if (this.texture != null)
		{
			return;
		}
		this.texture = new Texture2D(this.texSize, this.texSize, TextureFormat.ARGB32, this.mipmaps);
		this.texture.wrapMode = TextureWrapMode.Clamp;
		this.texture.filterMode = this.filterMode;
		this.texture.name = base.transform.GetRecursiveName("") + "/UIPaintableImage";
		this.texture.Clear(this.clearColor);
		this.image.texture = this.texture;
	}

	// Token: 0x060025FE RID: 9726 RVA: 0x0001DA0F File Offset: 0x0001BC0F
	public void Clear()
	{
		this.imageHash = 0U;
		this.isLocked = true;
		this.isLoading = false;
	}

	// Token: 0x060025FF RID: 9727 RVA: 0x0001DA26 File Offset: 0x0001BC26
	private void OnDestroy()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (this.texture)
		{
			Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x0001DA4F File Offset: 0x0001BC4F
	public void ClearTexture()
	{
		this.SetupTexture();
		this.texture.Clear(this.clearColor);
	}

	// Token: 0x06002601 RID: 9729 RVA: 0x0001DA6D File Offset: 0x0001BC6D
	public void FromData(byte[] data)
	{
		this.SetupTexture();
		ImageConversion.LoadImage(this.texture, data);
		this.isBlank = false;
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x000C7B7C File Offset: 0x000C5D7C
	public byte[] ToPng(BaseEntity ent)
	{
		this.SetupTexture();
		byte[] array = ImageConversion.EncodeToPNG(this.texture);
		this.imageHash = FileStorage.client.Store(array, FileStorage.Type.png, ent.net.ID, 0U);
		return array;
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x000C7BBC File Offset: 0x000C5DBC
	public byte[] ToJpg(BaseEntity ent, int quality)
	{
		this.SetupTexture();
		byte[] array = ImageConversion.EncodeToJPG(this.texture, quality);
		this.imageHash = FileStorage.client.Store(array, FileStorage.Type.jpg, ent.net.ID, 0U);
		return array;
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x000C7BFC File Offset: 0x000C5DFC
	public void DrawTexture(Vector2 pos, Vector2 size, Texture2D brush, Color textureColor, UIPaintableImage.DrawMode mode)
	{
		if (this.isLocked)
		{
			return;
		}
		this.SetupTexture();
		Rect rect = this.image.rectTransform.rect;
		pos.x *= (float)this.texture.width / rect.width;
		pos.y *= (float)this.texture.height / rect.height;
		size.x *= (float)this.texture.width / rect.width;
		size.y *= (float)this.texture.height / rect.height;
		if (pos.x + size.x * 0.5f < -1f)
		{
			return;
		}
		if (pos.x - size.x * 0.5f >= (float)(this.texture.width + 1))
		{
			return;
		}
		if (pos.y + size.y * 0.5f < -1f)
		{
			return;
		}
		if (pos.y - size.y * 0.5f >= (float)(this.texture.height + 1))
		{
			return;
		}
		bool flag = false;
		Color color = Color.white;
		int width = this.texture.width;
		int height = this.texture.height;
		int num = brush ? brush.width : 0;
		int num2 = brush ? brush.height : 0;
		for (float num3 = 0f; num3 < size.x; num3 += 1f)
		{
			for (float num4 = 0f; num4 < size.y; num4 += 1f)
			{
				if (brush)
				{
					color = brush.GetPixel((int)Mathf.Lerp(0f, (float)num, num3 / size.x), (int)Mathf.Lerp(0f, (float)num2, num4 / size.y));
				}
				float num5 = textureColor.a * color.a;
				if ((double)num5 > 0.001 || mode == UIPaintableImage.DrawMode.Erase)
				{
					float num6 = num3 - size.x * 0.5f;
					float num7 = num4 - size.y * 0.5f;
					float num8 = pos.x + num6;
					float num9 = (float)this.texture.height - (pos.y + num7);
					if (num8 >= 0f && num8 < (float)width && num9 >= 0f && num9 < (float)height)
					{
						Color color2 = color;
						Color pixel = this.texture.GetPixel(Mathf.FloorToInt(num8), Mathf.FloorToInt(num9));
						switch (mode)
						{
						case UIPaintableImage.DrawMode.AlphaBlended:
						{
							color2 *= textureColor;
							Color color3 = Color.Lerp(pixel, color2, (1f - pixel.a) * (1f - num5));
							color2.r = Mathf.Lerp(color3.r, color2.r, num5);
							color2.g = Mathf.Lerp(color3.g, color2.g, num5);
							color2.b = Mathf.Lerp(color3.b, color2.b, num5);
							color2.a = pixel.a + color2.a;
							break;
						}
						case UIPaintableImage.DrawMode.Additive:
							color2.r = pixel.r + color.r * num5;
							color2.g = pixel.g + color.g * num5;
							color2.b = pixel.b + color.b * num5;
							break;
						case UIPaintableImage.DrawMode.Lighten:
							color2.r = Mathf.Max(pixel.r, color.r);
							color2.g = Mathf.Max(pixel.g, color.g);
							color2.b = Mathf.Max(pixel.b, color.b);
							break;
						case UIPaintableImage.DrawMode.Erase:
							color2 = Color.Lerp(pixel, textureColor, color.a);
							break;
						}
						if (!(pixel == color2))
						{
							this.texture.SetPixel(Mathf.FloorToInt(num8), Mathf.FloorToInt(num9), color2);
							flag = true;
						}
					}
				}
			}
		}
		if (flag)
		{
			this.needsApply = true;
			this.imageHash = 0U;
			this.isBlank = false;
		}
	}

	// Token: 0x020006A9 RID: 1705
	public enum DrawMode
	{
		// Token: 0x040021FD RID: 8701
		AlphaBlended,
		// Token: 0x040021FE RID: 8702
		Additive,
		// Token: 0x040021FF RID: 8703
		Lighten,
		// Token: 0x04002200 RID: 8704
		Erase
	}
}
