using System;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class BucketVMFluidSim : MonoBehaviour
{
	// Token: 0x04000EA0 RID: 3744
	public Animator waterbucketAnim;

	// Token: 0x04000EA1 RID: 3745
	public ParticleSystem waterPour;

	// Token: 0x04000EA2 RID: 3746
	public ParticleSystem waterTurbulence;

	// Token: 0x04000EA3 RID: 3747
	public ParticleSystem waterFill;

	// Token: 0x04000EA4 RID: 3748
	public float waterLevel;

	// Token: 0x04000EA5 RID: 3749
	public float targetWaterLevel;

	// Token: 0x04000EA6 RID: 3750
	public AudioSource waterSpill;

	// Token: 0x04000EA7 RID: 3751
	private float PlayerEyePitch;

	// Token: 0x04000EA8 RID: 3752
	private float turb_forward;

	// Token: 0x04000EA9 RID: 3753
	private float turb_side;

	// Token: 0x04000EAA RID: 3754
	private Vector3 lastPosition;

	// Token: 0x04000EAB RID: 3755
	protected Vector3 groundSpeedLast;

	// Token: 0x04000EAC RID: 3756
	private Vector3 lastAngle;

	// Token: 0x04000EAD RID: 3757
	protected Vector3 vecAngleSpeedLast;

	// Token: 0x04000EAE RID: 3758
	private Vector3 initialPosition;

	// Token: 0x060011FB RID: 4603 RVA: 0x0000F9EB File Offset: 0x0000DBEB
	private void Start()
	{
		this.waterSpill.Stop();
		this.waterSpill.volume = 0f;
		this.initialPosition = this.waterbucketAnim.transform.localPosition;
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x0000FA1E File Offset: 0x0000DC1E
	private void SetFillingFromWorld(bool isFilling)
	{
		this.waterFill.enableEmission = isFilling;
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x0000FA2C File Offset: 0x0000DC2C
	private void UpdateWaterLevel(float newLevel)
	{
		this.targetWaterLevel = newLevel;
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x00076540 File Offset: 0x00074740
	private void Update()
	{
		this.PlayerEyePitch = base.transform.localEulerAngles.x;
		if (this.PlayerEyePitch > 180f)
		{
			this.PlayerEyePitch = 360f - this.PlayerEyePitch;
		}
		else
		{
			this.PlayerEyePitch *= -1f;
		}
		this.DoWaterTipping();
		this.UpdateWaterLine();
		this.CalculateTurbulence();
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x000765A8 File Offset: 0x000747A8
	private void CalculateTurbulence()
	{
		Vector3 direction = this.lastPosition - base.transform.position;
		this.lastPosition = base.transform.position;
		this.groundSpeedLast = Vector3.Lerp(this.groundSpeedLast, base.transform.InverseTransformDirection(direction) / Time.deltaTime, 0.01f);
		Vector3 direction2 = this.lastAngle - base.transform.eulerAngles;
		this.lastAngle = base.transform.eulerAngles;
		this.vecAngleSpeedLast = Vector3.Lerp(this.vecAngleSpeedLast, base.transform.InverseTransformDirection(direction2) / Time.deltaTime, 0.02f);
		this.turb_forward = this.groundSpeedLast.z * 0.6f;
		this.turb_side = this.groundSpeedLast.x * 0.6f;
		this.waterbucketAnim.SetFloat("turbulence_forward", this.turb_forward, 0.01f, Time.deltaTime * 5f);
		this.waterbucketAnim.SetFloat("turbulence_side", this.turb_side, 0.01f, Time.deltaTime * 5f);
		float num = Mathf.Abs(this.turb_forward) + Mathf.Abs(this.turb_side);
		ParticleSystem.EmissionModule emission = this.waterTurbulence.emission;
		ParticleSystem.MinMaxCurve rate = default(ParticleSystem.MinMaxCurve);
		rate.constantMax = ((num > 0.1f) ? (num * 200f) : 0f);
		emission.rate = rate;
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x00002ECE File Offset: 0x000010CE
	private void DoWaterTipping()
	{
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x0000FA35 File Offset: 0x0000DC35
	public void AddWater(float waterAmount)
	{
		this.waterLevel = Mathf.Clamp(this.waterLevel + waterAmount, 0f, 1f);
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x0007672C File Offset: 0x0007492C
	private void DisableWaterSpillEffects()
	{
		this.waterSpill.volume = Mathf.Lerp(this.waterSpill.volume, 0f, Time.deltaTime * 5f);
		if (this.waterSpill.isPlaying && this.waterSpill.volume <= 0.05f)
		{
			this.waterSpill.Stop();
		}
		if (this.waterPour.enableEmission)
		{
			this.waterPour.enableEmission = false;
		}
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x000767A8 File Offset: 0x000749A8
	private void UpdateWaterLine()
	{
		this.waterLevel = Mathf.Lerp(this.waterLevel, this.targetWaterLevel, Time.deltaTime * 5f);
		this.waterbucketAnim.SetFloat("waterlevel", this.waterLevel);
		float num = this.RemapValClamped(this.waterLevel, 1f, 0.45f, 0.1f, 1f);
		float y = Mathf.Abs(this.PlayerEyePitch / 45f) * 0.1f;
		this.PlayerEyePitch = Mathf.Clamp(this.PlayerEyePitch / 40f, -1f * num, num);
		this.waterbucketAnim.transform.localPosition = this.initialPosition - new Vector3(0f, y, 0f);
		this.waterbucketAnim.SetFloat("waterangle", this.PlayerEyePitch);
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x0000FA54 File Offset: 0x0000DC54
	private float fsel(float c, float x, float y)
	{
		if (c >= 0f)
		{
			return x;
		}
		return y;
	}

	// Token: 0x06001205 RID: 4613 RVA: 0x00076888 File Offset: 0x00074A88
	private float RemapValClamped(float val, float A, float B, float C, float D)
	{
		if (A == B)
		{
			return this.fsel(val - B, D, C);
		}
		float num = (val - A) / (B - A);
		num = Mathf.Clamp(num, 0f, 1f);
		return C + (D - C) * num;
	}
}
