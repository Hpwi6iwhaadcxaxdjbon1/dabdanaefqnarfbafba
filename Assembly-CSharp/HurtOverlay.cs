using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x020001D4 RID: 468
public class HurtOverlay : ImageEffectLayer
{
	// Token: 0x04000C12 RID: 3090
	public ScreenOverlayEx bloodOverlay;

	// Token: 0x04000C13 RID: 3091
	public VignetteAndChromaticAberration vignetting;

	// Token: 0x04000C14 RID: 3092
	public CC_Grayscale grayScale;

	// Token: 0x04000C15 RID: 3093
	private float smoothCurrent;

	// Token: 0x04000C16 RID: 3094
	private float smoothVelocity;

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0000D6E8 File Offset: 0x0000B8E8
	public void Awake()
	{
		this.sortOrder = 2;
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x0000D6F1 File Offset: 0x0000B8F1
	public override void Start()
	{
		base.Start();
		base.layerEnabled = false;
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x00067380 File Offset: 0x00065580
	protected void Update()
	{
		float intensity = this.GetIntensity();
		if (intensity <= 0f)
		{
			base.layerEnabled = false;
			return;
		}
		base.layerEnabled = true;
		if (this.bloodOverlay)
		{
			this.bloodOverlay.intensity = intensity * 1f;
		}
		if (this.vignetting)
		{
			this.vignetting.intensity = intensity * 1f;
			this.vignetting.chromaticAberration = intensity * 2f;
			this.vignetting.blurDistance = intensity * 0.5f;
			this.vignetting.blurSpread = intensity * 0.5f;
		}
		if (this.grayScale)
		{
			this.grayScale.amount = intensity * 0.5f;
		}
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x00067440 File Offset: 0x00065640
	private float GetIntensity()
	{
		BasePlayer entity = LocalPlayer.Entity;
		if (entity == null)
		{
			return 0f;
		}
		if (entity.IsSpectating())
		{
			return 0f;
		}
		if (entity.IsDead())
		{
			return 1f;
		}
		if (entity.IsWounded())
		{
			return 1f;
		}
		float num = entity.metabolism.bleeding.Fraction();
		float target = Mathf.Clamp01(Mathf.InverseLerp(50f, 5f, entity.health) + num);
		this.smoothCurrent = Mathf.SmoothDamp(this.smoothCurrent, target, ref this.smoothVelocity, Time.smoothDeltaTime);
		return this.smoothCurrent;
	}
}
