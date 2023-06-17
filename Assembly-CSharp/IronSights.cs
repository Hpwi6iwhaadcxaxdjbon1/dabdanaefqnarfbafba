using System;
using ConVar;
using UnityEngine;

// Token: 0x02000776 RID: 1910
public class IronSights : MonoBehaviour
{
	// Token: 0x040024A0 RID: 9376
	public bool Enabled;

	// Token: 0x040024A1 RID: 9377
	[Header("View Setup")]
	public IronsightAimPoint aimPoint;

	// Token: 0x040024A2 RID: 9378
	public float fieldOfViewOffset = -20f;

	// Token: 0x040024A3 RID: 9379
	[Header("Animation")]
	public float introSpeed = 1f;

	// Token: 0x040024A4 RID: 9380
	public AnimationCurve introCurve = new AnimationCurve();

	// Token: 0x040024A5 RID: 9381
	public float outroSpeed = 1f;

	// Token: 0x040024A6 RID: 9382
	public AnimationCurve outroCurve = new AnimationCurve();

	// Token: 0x040024A7 RID: 9383
	[Header("Sounds")]
	public SoundDefinition upSound;

	// Token: 0x040024A8 RID: 9384
	public SoundDefinition downSound;

	// Token: 0x040024A9 RID: 9385
	[Header("Info")]
	public IronSightOverride ironsightsOverride;

	// Token: 0x040024AA RID: 9386
	private Animator animator;

	// Token: 0x040024AB RID: 9387
	private int param_ironsightstrength = Animator.StringToHash("ironsightstrength");

	// Token: 0x040024AC RID: 9388
	private int param_ironsightsEnabled = Animator.StringToHash("ironsightsEnabled");

	// Token: 0x040024AD RID: 9389
	private float delta;

	// Token: 0x040024AE RID: 9390
	private float rawDelta;

	// Token: 0x040024AF RID: 9391
	private float maxDelta = 1f;

	// Token: 0x040024B0 RID: 9392
	private Vector3 positionOffset = Vector3.zero;

	// Token: 0x040024B1 RID: 9393
	private Quaternion rotationOffset = Quaternion.identity;

	// Token: 0x040024B2 RID: 9394
	private AnimationCurve currentCurve;

	// Token: 0x0600299E RID: 10654 RVA: 0x000205FB File Offset: 0x0001E7FB
	public void OnEnable()
	{
		this.animator = base.GetComponentInChildren<Animator>();
		this.currentCurve = this.introCurve;
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x000D3A28 File Offset: 0x000D1C28
	public void Update()
	{
		if (this.Enabled)
		{
			this.rawDelta = Mathf.MoveTowards(this.rawDelta, 1f, UnityEngine.Time.deltaTime * this.introSpeed * 2f);
			this.delta = this.currentCurve.Evaluate(this.rawDelta);
			this.animator.SetFloat(this.param_ironsightsEnabled, 1f, 0.1f, UnityEngine.Time.smoothDeltaTime);
		}
		else
		{
			this.rawDelta = Mathf.MoveTowards(this.rawDelta, 0f, UnityEngine.Time.deltaTime * this.outroSpeed * 2f);
			this.delta = this.currentCurve.Evaluate(this.rawDelta);
			this.animator.SetFloat(this.param_ironsightsEnabled, 0f, 0.05f, UnityEngine.Time.smoothDeltaTime);
		}
		if (this.rawDelta == 1f)
		{
			this.currentCurve = this.outroCurve;
		}
		if (this.rawDelta == 0f)
		{
			this.currentCurve = this.introCurve;
		}
		if (this.animator)
		{
			this.maxDelta = this.animator.GetFloat(this.param_ironsightstrength);
			this.delta *= this.maxDelta;
		}
	}

	// Token: 0x060029A0 RID: 10656 RVA: 0x000D3B68 File Offset: 0x000D1D68
	public void UpdateIronsights(ref CachedTransform<BaseViewModel> vm, Camera cam)
	{
		cam.fieldOfView = this.GetFOVOffset(cam.fieldOfView);
		vm.localScale = new Vector3(1f, 1f, 0.7f);
		Matrix4x4 inverse = Matrix4x4.TRS(vm.component.transform.position, vm.component.transform.rotation, Vector3.one).inverse;
		IronsightAimPoint ironsightAimPoint = this.ironsightsOverride ? this.ironsightsOverride.aimPoint : this.aimPoint;
		Vector3 upwards = inverse.MultiplyVector(ironsightAimPoint.transform.up);
		Vector3 b = inverse.MultiplyPoint3x4(ironsightAimPoint.transform.position);
		Quaternion b2 = Quaternion.LookRotation((inverse.MultiplyPoint3x4(ironsightAimPoint.targetPoint.position) - b).normalized, upwards);
		if (this.delta != this.maxDelta)
		{
			this.rotationOffset = b2;
			this.positionOffset = b;
		}
		else
		{
			this.rotationOffset = Quaternion.Slerp(this.rotationOffset, b2, UnityEngine.Time.smoothDeltaTime * 5f);
			this.positionOffset = Vector3.Lerp(this.positionOffset, b, UnityEngine.Time.deltaTime * 5f);
		}
		vm.rotation *= Quaternion.Inverse(Quaternion.Slerp(Quaternion.identity, this.rotationOffset, this.delta));
		vm.position += vm.rotation * this.positionOffset * -1f * this.delta;
	}

	// Token: 0x060029A1 RID: 10657 RVA: 0x00020615 File Offset: 0x0001E815
	public void SetIronsightsEnabled(bool b)
	{
		this.SetEnabled(b);
	}

	// Token: 0x060029A2 RID: 10658 RVA: 0x000D3D0C File Offset: 0x000D1F0C
	public void SetEnabled(bool val)
	{
		if (val != this.Enabled)
		{
			if (val)
			{
				if (this.upSound != null)
				{
					SoundManager.PlayOneshot(this.upSound, base.gameObject, true, default(Vector3));
				}
			}
			else if (this.downSound != null)
			{
				SoundManager.PlayOneshot(this.downSound, base.gameObject, true, default(Vector3));
			}
		}
		this.Enabled = val;
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x000D3D84 File Offset: 0x000D1F84
	public float GetFOVOffset(float fFOV)
	{
		float num = this.fieldOfViewOffset;
		if (this.ironsightsOverride != null)
		{
			num = Mathf.Lerp(this.fieldOfViewOffset, this.ironsightsOverride.fieldOfViewOffset, this.ironsightsOverride.fovBias);
		}
		return Mathf.Lerp(fFOV, 75f + num + (fFOV - ConVar.Graphics.fov), this.delta);
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x0002061E File Offset: 0x0001E81E
	public float GetDelta()
	{
		return this.delta;
	}
}
