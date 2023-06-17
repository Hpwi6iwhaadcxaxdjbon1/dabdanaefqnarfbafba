using System;
using UnityEngine;

// Token: 0x0200072D RID: 1837
public class BlurTexture : ProcessedTexture
{
	// Token: 0x06002819 RID: 10265 RVA: 0x0001F410 File Offset: 0x0001D610
	public BlurTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/Rust/SeparableBlur");
		this.result = base.CreateRenderTexture("Blur Texture", width, height, linear);
	}

	// Token: 0x0600281A RID: 10266 RVA: 0x0001F43D File Offset: 0x0001D63D
	public void Blur(float radius)
	{
		this.Blur(this.result, radius);
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x000CED94 File Offset: 0x000CCF94
	public void Blur(Texture source, float radius)
	{
		RenderTexture renderTexture = base.CreateTemporary();
		this.material.SetVector("offsets", new Vector4(radius / (float)Screen.width, 0f, 0f, 0f));
		Graphics.Blit(source, renderTexture, this.material, 0);
		this.material.SetVector("offsets", new Vector4(0f, radius / (float)Screen.height, 0f, 0f));
		Graphics.Blit(renderTexture, this.result, this.material, 0);
		base.ReleaseTemporary(renderTexture);
	}
}
