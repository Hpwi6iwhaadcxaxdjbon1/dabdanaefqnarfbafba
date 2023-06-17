using System;
using UnityEngine;

// Token: 0x0200028F RID: 655
public class ScreenFov : BaseScreenShake
{
	// Token: 0x04000F3C RID: 3900
	public AnimationCurve FovAdjustment;

	// Token: 0x06001290 RID: 4752 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void Setup()
	{
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x0000FF40 File Offset: 0x0000E140
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (cam)
		{
			cam.component.fieldOfView += this.FovAdjustment.Evaluate(delta);
		}
	}
}
