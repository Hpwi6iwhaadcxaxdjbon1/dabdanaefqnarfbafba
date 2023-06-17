using System;
using UnityEngine;

// Token: 0x0200028E RID: 654
public class ScreenBounceFade : BaseScreenShake
{
	// Token: 0x04000F33 RID: 3891
	public AnimationCurve bounceScale;

	// Token: 0x04000F34 RID: 3892
	public AnimationCurve bounceSpeed;

	// Token: 0x04000F35 RID: 3893
	public AnimationCurve bounceViewmodel;

	// Token: 0x04000F36 RID: 3894
	public AnimationCurve distanceFalloff;

	// Token: 0x04000F37 RID: 3895
	public AnimationCurve timeFalloff;

	// Token: 0x04000F38 RID: 3896
	private float bounceTime;

	// Token: 0x04000F39 RID: 3897
	private Vector3 bounceVelocity = Vector3.zero;

	// Token: 0x04000F3A RID: 3898
	public float maxDistance = 10f;

	// Token: 0x04000F3B RID: 3899
	public float scale = 1f;

	// Token: 0x0600128D RID: 4749 RVA: 0x0000FF00 File Offset: 0x0000E100
	public override void Setup()
	{
		this.bounceTime = Random.Range(0f, 1000f);
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00079154 File Offset: 0x00077354
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		float value = Vector3.Distance(cam.position, base.transform.position);
		float num = 1f - Mathf.InverseLerp(0f, this.maxDistance, value);
		this.bounceTime += Time.deltaTime * this.bounceSpeed.Evaluate(delta);
		float num2 = this.distanceFalloff.Evaluate(num);
		float num3 = this.bounceScale.Evaluate(delta) * 0.1f * num2 * this.scale * this.timeFalloff.Evaluate(delta);
		this.bounceVelocity.x = Mathf.Sin(this.bounceTime * 20f) * num3;
		this.bounceVelocity.y = Mathf.Cos(this.bounceTime * 25f) * num3;
		this.bounceVelocity.z = 0f;
		Vector3 vector = Vector3.zero;
		vector += this.bounceVelocity.x * cam.right;
		vector += this.bounceVelocity.y * cam.up;
		vector *= num;
		if (cam)
		{
			cam.position += vector;
		}
		if (vm)
		{
			vm.position += vector * -1f * this.bounceViewmodel.Evaluate(delta);
		}
	}
}
