using System;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class ScreenBounce : BaseScreenShake
{
	// Token: 0x04000F2E RID: 3886
	public AnimationCurve bounceScale;

	// Token: 0x04000F2F RID: 3887
	public AnimationCurve bounceSpeed;

	// Token: 0x04000F30 RID: 3888
	public AnimationCurve bounceViewmodel;

	// Token: 0x04000F31 RID: 3889
	private float bounceTime;

	// Token: 0x04000F32 RID: 3890
	private Vector3 bounceVelocity = Vector3.zero;

	// Token: 0x0600128A RID: 4746 RVA: 0x0000FED6 File Offset: 0x0000E0D6
	public override void Setup()
	{
		this.bounceTime = Random.Range(0f, 1000f);
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x00079024 File Offset: 0x00077224
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		this.bounceTime += Time.deltaTime * this.bounceSpeed.Evaluate(delta);
		float num = this.bounceScale.Evaluate(delta) * 0.1f;
		this.bounceVelocity.x = Mathf.Sin(this.bounceTime * 20f) * num;
		this.bounceVelocity.y = Mathf.Cos(this.bounceTime * 25f) * num;
		this.bounceVelocity.z = 0f;
		Vector3 vector = Vector3.zero;
		vector += this.bounceVelocity.x * cam.right;
		vector += this.bounceVelocity.y * cam.up;
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
