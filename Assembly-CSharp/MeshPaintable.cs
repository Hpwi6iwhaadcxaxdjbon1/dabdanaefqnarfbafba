using System;
using Rust;
using UnityEngine;

// Token: 0x0200020D RID: 525
public class MeshPaintable : MonoBehaviour, IClientComponent
{
	// Token: 0x04000D06 RID: 3334
	public string replacementTextureName = "_MainTex";

	// Token: 0x04000D07 RID: 3335
	public int textureWidth = 256;

	// Token: 0x04000D08 RID: 3336
	public int textureHeight = 256;

	// Token: 0x04000D09 RID: 3337
	public Color clearColor = Color.clear;

	// Token: 0x04000D0A RID: 3338
	public Texture2D targetTexture;

	// Token: 0x04000D0B RID: 3339
	public bool hasChanges;

	// Token: 0x06001030 RID: 4144 RVA: 0x0000E508 File Offset: 0x0000C708
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.Shutdown();
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x0000E518 File Offset: 0x0000C718
	private void Shutdown()
	{
		if (this.targetTexture)
		{
			Object.Destroy(this.targetTexture);
			this.targetTexture = null;
		}
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x0006DAA4 File Offset: 0x0006BCA4
	public void Init()
	{
		if (this.targetTexture)
		{
			return;
		}
		Renderer component = base.GetComponent<Renderer>();
		if (component == null)
		{
			return;
		}
		this.targetTexture = new Texture2D(this.textureWidth, this.textureHeight, TextureFormat.RGBA32, true);
		this.targetTexture.name = "MeshPaintable_" + base.gameObject.name;
		this.Clear(this.clearColor);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetTexture(this.replacementTextureName, this.targetTexture);
		component.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x0000E539 File Offset: 0x0000C739
	public void Clear(Color color)
	{
		if (this.targetTexture == null)
		{
			return;
		}
		this.targetTexture.Clear(color);
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x0000E55B File Offset: 0x0000C75B
	public void Apply()
	{
		this.targetTexture.Apply(true, false);
		this.hasChanges = true;
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x0006DB34 File Offset: 0x0006BD34
	public void DrawTexture(Vector2 uv, float width, float height, Texture2D texture, Color textureColor)
	{
		this.Init();
		for (float num = 0f; num < width; num += 1f)
		{
			for (float num2 = 0f; num2 < height; num2 += 1f)
			{
				Color color = texture.GetPixel((int)Mathf.Lerp(0f, (float)texture.width, num / width), (int)Mathf.Lerp(0f, (float)texture.height, num2 / height));
				float num3 = textureColor.a * color.a;
				color *= textureColor;
				color.a = 1f;
				int num4 = (int)(num - width / 2f);
				int num5 = (int)(num2 - height / 2f);
				int num6 = (int)(uv.x * (float)this.targetTexture.width) + num4;
				int num7 = (int)(uv.y * (float)this.targetTexture.height) + num5;
				Color color2 = color;
				if (num3 < 1f)
				{
					Color color3 = this.targetTexture.GetPixel(num6, num7);
					float a = color3.a;
					if ((double)a < 0.01)
					{
						color3 = Color.Lerp(color3, color2, num3 + (1f - a));
						color3.a = a;
					}
					color2 = Color.Lerp(color3, color2, num3);
				}
				if (num6 >= 0 && num6 < this.targetTexture.width && num7 >= 0 && num7 < this.targetTexture.height)
				{
					this.targetTexture.SetPixel(num6, num7, color2);
				}
			}
		}
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x00002D44 File Offset: 0x00000F44
	public bool ShouldHit(RaycastHit info)
	{
		return true;
	}
}
