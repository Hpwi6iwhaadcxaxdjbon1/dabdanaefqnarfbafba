using System;
using UnityEngine;

// Token: 0x02000780 RID: 1920
public class ViewmodelBob : MonoBehaviour
{
	// Token: 0x040024D0 RID: 9424
	internal float bobSpeedWalk = 9f;

	// Token: 0x040024D1 RID: 9425
	internal float bobSpeedRun = 13f;

	// Token: 0x040024D2 RID: 9426
	internal float bobAmountWalk = 0.005f;

	// Token: 0x040024D3 RID: 9427
	internal float bobAmountRun = 0.02f;

	// Token: 0x040024D4 RID: 9428
	internal float leftOffsetRun = 0.04f;

	// Token: 0x040024D5 RID: 9429
	internal float bobAmountRotateYaw = 3f;

	// Token: 0x040024D6 RID: 9430
	internal float bobAmountRotateRoll = 4f;

	// Token: 0x040024D7 RID: 9431
	internal Vector3 lastPosition = Vector3.zero;

	// Token: 0x040024D8 RID: 9432
	internal Vector3 velocity = Vector3.zero;

	// Token: 0x040024D9 RID: 9433
	internal Vector3 bobOffset = Vector3.zero;

	// Token: 0x040024DA RID: 9434
	internal float bobRotateYaw;

	// Token: 0x040024DB RID: 9435
	internal float bobRotateRoll;

	// Token: 0x040024DC RID: 9436
	internal float bobCycle;

	// Token: 0x040024DD RID: 9437
	internal ViewmodelBob.BobSettings smoothedBob;

	// Token: 0x060029C7 RID: 10695 RVA: 0x000D44B8 File Offset: 0x000D26B8
	private ViewmodelBob.BobSettings WorkoutBobSettings(Vector3 vPosition, float fov, bool isRunning, bool OnGround, bool isAiming, bool isDucked, bool isMounted)
	{
		if (this.lastPosition == Vector3.zero)
		{
			this.lastPosition = vPosition;
		}
		this.velocity = Vector3.Lerp(this.velocity, this.lastPosition - vPosition, 0.9f);
		this.lastPosition = vPosition;
		if (isMounted)
		{
			this.velocity = Vector3.zero;
		}
		ViewmodelBob.BobSettings result = default(ViewmodelBob.BobSettings);
		result.effectAmount = Mathf.InverseLerp(30f, 70f, fov);
		result.offsetLateral = 0f;
		if (!OnGround)
		{
			result.bobSpeed = this.bobSpeedWalk;
			result.bobAmount = this.bobAmountWalk;
			result.bobRotate = 1f;
			result.effectAmount = 0f;
			return result;
		}
		if (this.velocity.magnitude <= 0.01f)
		{
			result.bobSpeed = this.bobSpeedWalk;
			result.bobAmount = this.bobAmountWalk;
			result.bobRotate = 1f;
			result.effectAmount = 0f;
			return result;
		}
		if (isRunning)
		{
			result.bobSpeed = this.bobSpeedRun;
			result.bobAmount = this.bobAmountRun;
			result.offsetLateral = this.leftOffsetRun;
			result.bobRotate = 1.25f;
			return result;
		}
		if (isAiming)
		{
			result.bobSpeed = this.bobSpeedWalk;
			result.bobAmount = this.bobAmountWalk;
			result.bobRotate = 1f;
			result.effectAmount = 0.05f;
			return result;
		}
		if (isDucked)
		{
			result.bobSpeed = this.bobSpeedWalk * 0.5f;
			result.bobAmount = this.bobAmountWalk * 0.5f;
			result.bobRotate = 0.5f;
			return result;
		}
		result.bobSpeed = this.bobSpeedWalk;
		result.bobAmount = this.bobAmountWalk;
		result.bobRotate = 1f;
		return result;
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x000D4690 File Offset: 0x000D2890
	public void Apply(ref CachedTransform<BaseViewModel> vm, float fov)
	{
		if (!base.enabled)
		{
			return;
		}
		bool isAiming = vm.component.ironSights && vm.component.ironSights.Enabled;
		bool isRunning = false;
		bool onGround = true;
		bool isDucked = false;
		bool isMounted = false;
		BasePlayer entity = LocalPlayer.Entity;
		if (entity != null)
		{
			isRunning = entity.IsRunning();
			onGround = entity.IsOnGround();
			isDucked = entity.IsDucked();
			isMounted = entity.isMounted;
		}
		Vector3 vector = vm.position;
		if (entity != null)
		{
			BaseEntity parentEntity = entity.GetParentEntity();
			if (parentEntity != null)
			{
				vector = parentEntity.transform.InverseTransformPoint(vector);
			}
		}
		ViewmodelBob.BobSettings bobSettings = this.WorkoutBobSettings(vector, fov, isRunning, onGround, isAiming, isDucked, isMounted);
		float num = 5f;
		this.smoothedBob.bobAmount = Mathf.Lerp(this.smoothedBob.bobAmount, bobSettings.bobAmount, Time.deltaTime * num);
		this.smoothedBob.bobRotate = Mathf.Lerp(this.smoothedBob.bobRotate, bobSettings.bobRotate, Time.deltaTime * num);
		this.smoothedBob.bobSpeed = Mathf.Lerp(this.smoothedBob.bobSpeed, bobSettings.bobSpeed, Time.deltaTime * num);
		this.smoothedBob.effectAmount = Mathf.Lerp(this.smoothedBob.effectAmount, bobSettings.effectAmount, Time.deltaTime * num);
		this.smoothedBob.offsetLateral = Mathf.Lerp(this.smoothedBob.offsetLateral, bobSettings.offsetLateral, Time.deltaTime * num);
		this.bobCycle += Time.deltaTime * this.smoothedBob.bobSpeed;
		this.ApplySettings(ref vm, this.smoothedBob);
	}

	// Token: 0x060029C9 RID: 10697 RVA: 0x000D4854 File Offset: 0x000D2A54
	private void ApplySettings(ref CachedTransform<BaseViewModel> vm, ViewmodelBob.BobSettings settings)
	{
		float num = Mathf.Sin(this.bobCycle * 2f) * settings.bobAmount;
		float d = num * 1.2f;
		float num2 = Mathf.Sin(this.bobCycle + 0.5f) * settings.bobAmount * 4f;
		num2 -= settings.offsetLateral;
		Vector3 b = base.transform.up * num + base.transform.right * num2 + base.transform.forward * d;
		this.bobOffset = Vector3.Lerp(this.bobOffset, b, Time.deltaTime * 3f);
		vm.position += this.bobOffset * settings.effectAmount;
		float num3 = Mathf.Sin(this.bobCycle + 0.5f) * this.bobAmountRotateYaw;
		this.bobRotateYaw = Mathf.Lerp(this.bobRotateYaw, num3 * settings.bobRotate, Time.deltaTime * 8f);
		vm.RotateAround(vm.position, base.transform.up, this.bobRotateYaw * settings.effectAmount);
		float num4 = Mathf.Sin(this.bobCycle + 0.6f) * this.bobAmountRotateRoll;
		this.bobRotateRoll = Mathf.Lerp(this.bobRotateRoll, num4 * settings.bobRotate, Time.deltaTime * 8f);
		vm.RotateAround(vm.position, base.transform.forward, this.bobRotateRoll * settings.effectAmount);
	}

	// Token: 0x02000781 RID: 1921
	public struct BobSettings
	{
		// Token: 0x040024DE RID: 9438
		public float bobSpeed;

		// Token: 0x040024DF RID: 9439
		public float bobAmount;

		// Token: 0x040024E0 RID: 9440
		public float offsetLateral;

		// Token: 0x040024E1 RID: 9441
		public float bobRotate;

		// Token: 0x040024E2 RID: 9442
		public float effectAmount;
	}
}
