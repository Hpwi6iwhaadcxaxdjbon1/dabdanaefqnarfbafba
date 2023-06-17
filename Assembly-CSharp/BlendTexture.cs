using System;
using UnityEngine;

// Token: 0x0200072C RID: 1836
public class BlendTexture : ProcessedTexture
{
	// Token: 0x06002816 RID: 10262 RVA: 0x0001F395 File Offset: 0x0001D595
	public BlendTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/BlitCopyAlpha");
		this.result = base.CreateRenderTexture("Blend Texture", width, height, linear);
	}

	// Token: 0x06002817 RID: 10263 RVA: 0x0001F3C2 File Offset: 0x0001D5C2
	public void Blend(Texture source, Texture target, float alpha)
	{
		this.material.SetTexture("_BlendTex", target);
		this.material.SetFloat("_Alpha", Mathf.Clamp01(alpha));
		Graphics.Blit(source, this.result, this.material);
	}

	// Token: 0x06002818 RID: 10264 RVA: 0x0001F3FD File Offset: 0x0001D5FD
	public void CopyTo(BlendTexture target)
	{
		Graphics.Blit(this.result, target.result);
	}
}
