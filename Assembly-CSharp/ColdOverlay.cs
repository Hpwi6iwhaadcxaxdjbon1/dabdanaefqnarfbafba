using System;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class ColdOverlay : ImageEffectLayer
{
	// Token: 0x04000C07 RID: 3079
	internal bool isPlaying;

	// Token: 0x04000C08 RID: 3080
	public ScreenOverlayEx screenOverlay;

	// Token: 0x04000C09 RID: 3081
	public CC_Frost frost;

	// Token: 0x04000C0A RID: 3082
	public LensDirtiness lensDirtyness;

	// Token: 0x04000C0B RID: 3083
	private float smoothCurrent;

	// Token: 0x04000C0C RID: 3084
	private float smoothVelocity;

	// Token: 0x06000EEC RID: 3820 RVA: 0x0000D6E8 File Offset: 0x0000B8E8
	public void Awake()
	{
		this.sortOrder = 2;
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0000D6F1 File Offset: 0x0000B8F1
	public override void Start()
	{
		base.Start();
		base.layerEnabled = false;
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x00067174 File Offset: 0x00065374
	protected void Update()
	{
		float intensity = this.GetIntensity();
		if (intensity <= 0f)
		{
			base.layerEnabled = false;
			return;
		}
		base.layerEnabled = true;
		if (this.screenOverlay)
		{
			this.screenOverlay.intensity = intensity * 0.01f;
		}
		if (this.frost)
		{
			this.frost.scale = intensity * 1f;
		}
		if (this.lensDirtyness)
		{
			this.lensDirtyness.gain = intensity * 3f;
		}
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x000671FC File Offset: 0x000653FC
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
		float target = Mathf.InverseLerp(0f, -20f, entity.metabolism.temperature.value);
		this.smoothCurrent = Mathf.SmoothDamp(this.smoothCurrent, target, ref this.smoothVelocity, Time.smoothDeltaTime);
		return this.smoothCurrent;
	}
}
