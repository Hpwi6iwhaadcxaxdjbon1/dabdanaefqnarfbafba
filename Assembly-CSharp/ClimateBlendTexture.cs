using System;
using UnityEngine;

// Token: 0x0200072F RID: 1839
public class ClimateBlendTexture : ProcessedTexture
{
	// Token: 0x0600281F RID: 10271 RVA: 0x0001F44C File Offset: 0x0001D64C
	public ClimateBlendTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/ClimateBlendLUTs");
		this.result = base.CreateRenderTexture("Climate Blend Texture", width, height, linear);
		this.result.wrapMode = TextureWrapMode.Clamp;
	}

	// Token: 0x06002820 RID: 10272 RVA: 0x0001F485 File Offset: 0x0001D685
	public bool CheckLostData()
	{
		if (!this.result.IsCreated())
		{
			this.result.Create();
			return true;
		}
		return false;
	}

	// Token: 0x06002821 RID: 10273 RVA: 0x000CF0CC File Offset: 0x000CD2CC
	public void Blend(Texture srcLut1, Texture dstLut1, float lerpLut1, Texture srcLut2, Texture dstLut2, float lerpLut2, float lerp, ClimateBlendTexture prevLut, float time)
	{
		this.material.SetTexture("_srcLut1", srcLut1);
		this.material.SetTexture("_dstLut1", dstLut1);
		this.material.SetTexture("_srcLut2", srcLut2);
		this.material.SetTexture("_dstLut2", dstLut2);
		this.material.SetTexture("_prevLut", prevLut);
		this.material.SetFloat("_lerpLut1", lerpLut1);
		this.material.SetFloat("_lerpLut2", lerpLut2);
		this.material.SetFloat("_lerp", lerp);
		this.material.SetFloat("_time", time);
		Graphics.Blit(null, this.result, this.material);
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x000CF190 File Offset: 0x000CD390
	public static void Swap(ref ClimateBlendTexture a, ref ClimateBlendTexture b)
	{
		ClimateBlendTexture climateBlendTexture = a;
		a = b;
		b = climateBlendTexture;
	}
}
