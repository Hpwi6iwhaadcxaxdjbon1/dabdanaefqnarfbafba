using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x02000605 RID: 1541
public class BlurManager : ImageEffectLayer
{
	// Token: 0x04001EE0 RID: 7904
	public BlurOptimized blur;

	// Token: 0x04001EE1 RID: 7905
	public ColorCorrectionCurves color;

	// Token: 0x04001EE2 RID: 7906
	public float maxBlurScale;

	// Token: 0x04001EE3 RID: 7907
	internal float blurAmount = 1f;

	// Token: 0x04001EE4 RID: 7908
	internal float desaturationAmount = 0.6f;

	// Token: 0x06002294 RID: 8852 RVA: 0x000B923C File Offset: 0x000B743C
	private void Update()
	{
		if (this.blur == null)
		{
			return;
		}
		float blurArmount = this.GetBlurArmount();
		this.blurAmount = Mathf.Lerp(this.blurAmount, blurArmount, Time.deltaTime * 10f);
		if (this.blurAmount < 0.03f)
		{
			base.layerEnabled = false;
			return;
		}
		base.layerEnabled = true;
		this.blur.blurSize = this.blurAmount * this.maxBlurScale;
		this.color.saturation = 1f - this.blurAmount * this.desaturationAmount;
	}

	// Token: 0x06002295 RID: 8853 RVA: 0x0001B7B6 File Offset: 0x000199B6
	private float GetBlurArmount()
	{
		if (DeveloperTools.isOpen)
		{
			return 1f;
		}
		return UIBackgroundBlur.currentMax;
	}
}
