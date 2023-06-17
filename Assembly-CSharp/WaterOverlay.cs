using System;
using UnityStandardAssets.ImageEffects;

// Token: 0x020001D8 RID: 472
public class WaterOverlay : ImageEffectLayer, IClientComponent
{
	// Token: 0x04000C23 RID: 3107
	public static bool goggles;

	// Token: 0x04000C24 RID: 3108
	public WaterOverlay.EffectParams gogglesParams = WaterOverlay.EffectParams.DefaultGoggles;

	// Token: 0x04000C25 RID: 3109
	private WaterOverlay.EffectParams startParams;

	// Token: 0x04000C26 RID: 3110
	private BlurOptimized blur;

	// Token: 0x04000C27 RID: 3111
	private CC_Wiggle wiggle;

	// Token: 0x04000C28 RID: 3112
	private CC_DoubleVision doubleVision;

	// Token: 0x04000C29 RID: 3113
	private CC_PhotoFilter photoFilter;

	// Token: 0x06000F0E RID: 3854 RVA: 0x0000D7D2 File Offset: 0x0000B9D2
	public void Awake()
	{
		this.sortOrder = 2;
		this.InitializeControls();
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x0000D7E1 File Offset: 0x0000B9E1
	private void InitializeControls()
	{
		this.blur = base.GetComponent<BlurOptimized>();
		this.wiggle = base.GetComponent<CC_Wiggle>();
		this.doubleVision = base.GetComponent<CC_DoubleVision>();
		this.photoFilter = base.GetComponent<CC_PhotoFilter>();
		this.startParams = this.GetEffectParams();
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x00067794 File Offset: 0x00065994
	private WaterOverlay.EffectParams GetEffectParams()
	{
		return new WaterOverlay.EffectParams
		{
			blur = this.blur.enabled,
			blurDistance = this.blur.fadeToBlurDistance,
			wiggle = this.wiggle.enabled,
			doubleVisionAmount = this.doubleVision.amount,
			photoFilterDensity = this.photoFilter.density
		};
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x00067804 File Offset: 0x00065A04
	private void SetEffectParams(WaterOverlay.EffectParams param)
	{
		this.blur.enabled = param.blur;
		this.blur.fadeToBlurDistance = param.blurDistance;
		this.wiggle.enabled = param.wiggle;
		this.doubleVision.amount = param.doubleVisionAmount;
		this.photoFilter.density = param.photoFilterDensity;
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x00067868 File Offset: 0x00065A68
	private void UpdateGoggleState()
	{
		if (WaterSystem.Instance != null)
		{
			if (WaterOverlay.goggles)
			{
				WaterSystem.Instance.SetUnderwaterScatterCoefficientOverride(this.gogglesParams.scatterCoefficient);
				this.SetEffectParams(this.gogglesParams);
				return;
			}
			WaterSystem.Instance.ClearUnderwaterScatterCoefficientOverride();
			this.SetEffectParams(this.startParams);
		}
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x000678C4 File Offset: 0x00065AC4
	protected void Update()
	{
		bool layerEnabled = false;
		this.UpdateGoggleState();
		if (WaterSystem.Collision != null && MainCamera.mainCamera != null && MainCamera.isWaterVisible)
		{
			layerEnabled = WaterLevel.Test(MainCamera.mainCamera.transform.position);
		}
		base.layerEnabled = layerEnabled;
	}

	// Token: 0x020001D9 RID: 473
	[Serializable]
	public struct EffectParams
	{
		// Token: 0x04000C2A RID: 3114
		public float scatterCoefficient;

		// Token: 0x04000C2B RID: 3115
		public bool blur;

		// Token: 0x04000C2C RID: 3116
		public float blurDistance;

		// Token: 0x04000C2D RID: 3117
		public bool wiggle;

		// Token: 0x04000C2E RID: 3118
		public float doubleVisionAmount;

		// Token: 0x04000C2F RID: 3119
		public float photoFilterDensity;

		// Token: 0x04000C30 RID: 3120
		public static WaterOverlay.EffectParams DefaultGoggles = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.1f,
			blur = false,
			blurDistance = 10f,
			wiggle = false,
			doubleVisionAmount = 0.753f,
			photoFilterDensity = 1f
		};
	}
}
