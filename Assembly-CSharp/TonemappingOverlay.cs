using System;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;

// Token: 0x020001D7 RID: 471
public class TonemappingOverlay : ImageEffectLayer
{
	// Token: 0x04000C20 RID: 3104
	public TonemappingColorGrading tonemapping;

	// Token: 0x04000C21 RID: 3105
	private int screenWidth;

	// Token: 0x04000C22 RID: 3106
	private int screenHeight;

	// Token: 0x06000F09 RID: 3849 RVA: 0x0000D789 File Offset: 0x0000B989
	public void Awake()
	{
		this.sortOrder = -3;
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x0000D793 File Offset: 0x0000B993
	public override void Start()
	{
		base.Start();
		base.layerEnabled = true;
		this.screenWidth = Screen.width;
		this.screenHeight = Screen.height;
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x0000D7B8 File Offset: 0x0000B9B8
	private void ResetColorGrading()
	{
		this.tonemapping.enabled = false;
		this.tonemapping.enabled = true;
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x000676E8 File Offset: 0x000658E8
	protected void Update()
	{
		bool enabled = this.tonemapping.lut.enabled;
		bool flag = true;
		if (enabled && (this.screenWidth != Screen.width || this.screenHeight != Screen.height))
		{
			base.Invoke(new Action(this.ResetColorGrading), 0.5f);
			this.screenWidth = Screen.width;
			this.screenHeight = Screen.height;
		}
		if ((flag && !enabled) || (!flag && enabled))
		{
			TonemappingColorGrading.LUTSettings lut = this.tonemapping.lut;
			lut.enabled = flag;
			lut.texture = (flag ? lut.texture : null);
			this.tonemapping.lut = lut;
		}
	}
}
