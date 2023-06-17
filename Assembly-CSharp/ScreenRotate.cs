using System;
using UnityEngine;

// Token: 0x02000290 RID: 656
public class ScreenRotate : BaseScreenShake
{
	// Token: 0x04000F3D RID: 3901
	public AnimationCurve Pitch;

	// Token: 0x04000F3E RID: 3902
	public AnimationCurve Yaw;

	// Token: 0x04000F3F RID: 3903
	public AnimationCurve Roll;

	// Token: 0x04000F40 RID: 3904
	public AnimationCurve ViewmodelEffect;

	// Token: 0x04000F41 RID: 3905
	public bool useViewModelEffect = true;

	// Token: 0x06001293 RID: 4755 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void Setup()
	{
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x000792E8 File Offset: 0x000774E8
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		Vector3 zero = Vector3.zero;
		zero.x = this.Pitch.Evaluate(delta);
		zero.y = this.Yaw.Evaluate(delta);
		zero.z = this.Roll.Evaluate(delta);
		if (cam)
		{
			cam.rotation *= Quaternion.Euler(zero);
		}
		if (vm && this.useViewModelEffect)
		{
			vm.rotation *= Quaternion.Euler(zero * -1f * (1f - this.ViewmodelEffect.Evaluate(delta)));
		}
	}
}
