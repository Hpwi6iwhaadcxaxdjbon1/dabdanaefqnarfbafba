using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x020001D5 RID: 469
public class RadiationOverlay : ImageEffectLayer
{
	// Token: 0x04000C17 RID: 3095
	public SoundDefinition[] geigerSounds;

	// Token: 0x04000C18 RID: 3096
	private Sound sound;

	// Token: 0x04000C19 RID: 3097
	private ColorCorrectionCurves colourCorrection;

	// Token: 0x04000C1A RID: 3098
	private NoiseAndGrain noiseAndGrain;

	// Token: 0x04000C1B RID: 3099
	private float smoothRadiationCurrent;

	// Token: 0x04000C1C RID: 3100
	private float smoothRadiationVelocity;

	// Token: 0x04000C1D RID: 3101
	private float smoothRadiationExposureCurrent;

	// Token: 0x04000C1E RID: 3102
	private float smoothRadiationExposureVelocity;

	// Token: 0x06000EFE RID: 3838 RVA: 0x0000D6E8 File Offset: 0x0000B8E8
	public void Awake()
	{
		this.sortOrder = 2;
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x0000D722 File Offset: 0x0000B922
	public override void Start()
	{
		base.Start();
		base.layerEnabled = false;
		this.colourCorrection = base.GetComponent<ColorCorrectionCurves>();
		this.noiseAndGrain = base.GetComponent<NoiseAndGrain>();
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x000674E0 File Offset: 0x000656E0
	protected void Update()
	{
		float radationExposureFraction = this.GetRadationExposureFraction();
		float radationFraction = this.GetRadationFraction();
		if (radationFraction > 0f)
		{
			base.layerEnabled = true;
			this.colourCorrection.saturation = 1f - radationFraction * 0.8f;
			this.noiseAndGrain.generalIntensity = radationFraction;
			this.noiseAndGrain.intensityMultiplier = radationFraction * 2f;
		}
		else
		{
			base.layerEnabled = false;
		}
		if (radationExposureFraction > 0f)
		{
			this.UpdateSound(radationExposureFraction);
			return;
		}
		if (this.sound)
		{
			this.sound.FadeOutAndRecycle(0.5f);
			this.sound = null;
		}
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x00067580 File Offset: 0x00065780
	private void UpdateSound(float exposure)
	{
		int num = (int)(exposure * (float)this.geigerSounds.Length);
		if (num >= this.geigerSounds.Length)
		{
			return;
		}
		SoundDefinition soundDefinition = this.geigerSounds[num];
		if (!this.sound || soundDefinition != this.sound.definition)
		{
			if (this.sound)
			{
				this.sound.FadeOutAndRecycle(0.5f);
			}
			this.sound = SoundManager.RequestSoundInstance(soundDefinition, base.gameObject, Vector3.zero, true);
			this.sound.FadeInAndPlay(0.5f);
		}
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x00067614 File Offset: 0x00065814
	private float GetRadationFraction()
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
		this.smoothRadiationCurrent = Mathf.SmoothDamp(this.smoothRadiationCurrent, Mathf.Clamp01(entity.metabolism.radiation_poison.value / 100f), ref this.smoothRadiationVelocity, Time.smoothDeltaTime);
		return this.smoothRadiationCurrent;
	}

	// Token: 0x06000F03 RID: 3843 RVA: 0x00067684 File Offset: 0x00065884
	private float GetRadationExposureFraction()
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
		this.smoothRadiationExposureCurrent = Mathf.SmoothDamp(this.smoothRadiationExposureCurrent, entity.metabolism.radiation_level.Fraction(), ref this.smoothRadiationExposureVelocity, Time.smoothDeltaTime);
		return this.smoothRadiationExposureCurrent;
	}
}
