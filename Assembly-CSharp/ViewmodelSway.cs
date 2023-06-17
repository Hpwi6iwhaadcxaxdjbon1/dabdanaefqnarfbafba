using System;
using UnityEngine;

// Token: 0x02000785 RID: 1925
public class ViewmodelSway : MonoBehaviour
{
	// Token: 0x040024EA RID: 9450
	public float positionalSwaySpeed = 1f;

	// Token: 0x040024EB RID: 9451
	public float positionalSwayAmount = 1f;

	// Token: 0x040024EC RID: 9452
	public float rotationSwaySpeed = 1f;

	// Token: 0x040024ED RID: 9453
	public float rotationSwayAmount = 1f;

	// Token: 0x040024EE RID: 9454
	internal Vector3 lastPosition = Vector3.zero;

	// Token: 0x040024EF RID: 9455
	internal Vector3 smoothedVelocity = Vector3.zero;

	// Token: 0x040024F0 RID: 9456
	internal Vector3 lastRotation = Vector3.zero;

	// Token: 0x040024F1 RID: 9457
	internal Vector3 smoothedRotation = Vector3.zero;

	// Token: 0x060029DA RID: 10714 RVA: 0x000D4F8C File Offset: 0x000D318C
	public void Apply(ref CachedTransform<BaseViewModel> vm)
	{
		bool aiming = vm.component.ironSights && vm.component.ironSights.Enabled;
		this.ApplyPositionalSway(ref vm, aiming);
		this.ApplyRotationSway(ref vm, aiming);
	}

	// Token: 0x060029DB RID: 10715 RVA: 0x000D4FD0 File Offset: 0x000D31D0
	private void ApplyPositionalSway(ref CachedTransform<BaseViewModel> vm, bool aiming)
	{
		if (this.lastPosition == Vector3.zero)
		{
			this.lastPosition = vm.position;
		}
		Vector3 a = this.smoothedVelocity;
		Matrix4x4 matrix4x = vm.worldToLocalMatrix;
		this.smoothedVelocity = a + matrix4x.MultiplyVector(this.lastPosition - vm.position);
		this.lastPosition = vm.position;
		this.smoothedVelocity = Vector3.Lerp(this.smoothedVelocity, Vector3.zero, Time.deltaTime * 4f * this.positionalSwaySpeed);
		Vector3 position = vm.position;
		matrix4x = vm.localToWorldMatrix;
		vm.position = position + matrix4x.MultiplyVector(this.smoothedVelocity * 0.01f * this.positionalSwayAmount * (aiming ? 0.05f : 0.5f));
	}

	// Token: 0x060029DC RID: 10716 RVA: 0x000D50B4 File Offset: 0x000D32B4
	private void ApplyRotationSway(ref CachedTransform<BaseViewModel> vm, bool aiming)
	{
		if (this.lastRotation == Vector3.zero)
		{
			this.lastRotation = vm.forward;
		}
		Vector3 a = this.smoothedRotation;
		Matrix4x4 matrix4x = vm.worldToLocalMatrix;
		this.smoothedRotation = a + matrix4x.MultiplyVector(vm.forward - this.lastRotation);
		this.lastRotation = vm.forward;
		this.smoothedRotation = Vector3.Lerp(this.smoothedRotation, Vector3.zero, Time.deltaTime * 15f * this.rotationSwaySpeed);
		Vector3 forward = vm.forward;
		matrix4x = vm.localToWorldMatrix;
		Vector3 forward2 = forward + matrix4x.MultiplyVector(this.smoothedRotation * 0.5f * this.rotationSwayAmount * (aiming ? 0.15f : 0.5f));
		Vector3 up = vm.up;
		vm.rotation = Quaternion.LookRotation(forward2, up);
	}
}
