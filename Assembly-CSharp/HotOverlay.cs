using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x020001D3 RID: 467
public class HotOverlay : ImageEffectLayer
{
	// Token: 0x04000C0E RID: 3086
	public LensDirtiness lensDirtyness;

	// Token: 0x04000C0F RID: 3087
	public VignetteAndChromaticAberration vingette;

	// Token: 0x04000C10 RID: 3088
	private float smoothCurrent;

	// Token: 0x04000C11 RID: 3089
	private float smoothVelocity;

	// Token: 0x06000EF5 RID: 3829 RVA: 0x0000D6F1 File Offset: 0x0000B8F1
	public override void Start()
	{
		base.Start();
		base.layerEnabled = false;
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x00067280 File Offset: 0x00065480
	protected void Update()
	{
		float intensity = this.GetIntensity();
		if (intensity <= 0f)
		{
			base.layerEnabled = false;
			return;
		}
		base.layerEnabled = true;
		if (this.lensDirtyness)
		{
			this.lensDirtyness.gain = intensity * 3f;
		}
		if (this.vingette)
		{
			this.vingette.blur = 1f * intensity;
			this.vingette.blurDistance = 0.5f * intensity;
		}
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x000672FC File Offset: 0x000654FC
	private float GetIntensity()
	{
		BasePlayer entity = LocalPlayer.Entity;
		if (entity == null)
		{
			return 0f;
		}
		if (entity.IsDead())
		{
			return 0f;
		}
		if (entity.IsSpectating())
		{
			return 0f;
		}
		float target = Mathf.InverseLerp(30f, 50f, entity.metabolism.temperature.value);
		this.smoothCurrent = Mathf.SmoothDamp(this.smoothCurrent, target, ref this.smoothVelocity, Time.smoothDeltaTime);
		return this.smoothCurrent;
	}
}
