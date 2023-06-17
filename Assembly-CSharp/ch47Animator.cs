using System;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class ch47Animator : MonoBehaviour
{
	// Token: 0x0400079C RID: 1948
	public Animator animator;

	// Token: 0x0400079D RID: 1949
	public bool bottomDoorOpen;

	// Token: 0x0400079E RID: 1950
	public bool landingGearDown;

	// Token: 0x0400079F RID: 1951
	public bool leftDoorOpen;

	// Token: 0x040007A0 RID: 1952
	public bool rightDoorOpen;

	// Token: 0x040007A1 RID: 1953
	public bool rearDoorOpen;

	// Token: 0x040007A2 RID: 1954
	public bool rearDoorExtensionOpen;

	// Token: 0x040007A3 RID: 1955
	public Transform rearRotorBlade;

	// Token: 0x040007A4 RID: 1956
	public Transform frontRotorBlade;

	// Token: 0x040007A5 RID: 1957
	public float rotorBladeSpeed;

	// Token: 0x040007A6 RID: 1958
	public float wheelTurnSpeed;

	// Token: 0x040007A7 RID: 1959
	public float wheelTurnAngle;

	// Token: 0x040007A8 RID: 1960
	public SkinnedMeshRenderer[] blurredRotorBlades;

	// Token: 0x040007A9 RID: 1961
	public SkinnedMeshRenderer[] RotorBlades;

	// Token: 0x040007AA RID: 1962
	private bool blurredRotorBladesEnabled;

	// Token: 0x040007AB RID: 1963
	public float blurSpeedThreshold = 100f;

	// Token: 0x06000B3F RID: 2879 RVA: 0x0000ADA6 File Offset: 0x00008FA6
	private void Start()
	{
		this.EnableBlurredRotorBlades(false);
		this.animator.SetBool("rotorblade_stop", false);
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0000ADC0 File Offset: 0x00008FC0
	public void SetDropDoorOpen(bool isOpen)
	{
		this.bottomDoorOpen = isOpen;
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x00057D18 File Offset: 0x00055F18
	private void Update()
	{
		this.animator.SetBool("bottomdoor", this.bottomDoorOpen);
		this.animator.SetBool("landinggear", this.landingGearDown);
		this.animator.SetBool("leftdoor", this.leftDoorOpen);
		this.animator.SetBool("rightdoor", this.rightDoorOpen);
		this.animator.SetBool("reardoor", this.rearDoorOpen);
		this.animator.SetBool("reardoor_extension", this.rearDoorExtensionOpen);
		if (this.rotorBladeSpeed >= this.blurSpeedThreshold && !this.blurredRotorBladesEnabled)
		{
			this.EnableBlurredRotorBlades(true);
		}
		else if (this.rotorBladeSpeed < this.blurSpeedThreshold && this.blurredRotorBladesEnabled)
		{
			this.EnableBlurredRotorBlades(false);
		}
		if (this.rotorBladeSpeed <= 0f)
		{
			this.animator.SetBool("rotorblade_stop", true);
			return;
		}
		this.animator.SetBool("rotorblade_stop", false);
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x00057E18 File Offset: 0x00056018
	private void LateUpdate()
	{
		float num = Time.deltaTime * this.rotorBladeSpeed * 15f;
		Vector3 localEulerAngles = this.frontRotorBlade.localEulerAngles;
		this.frontRotorBlade.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y + num, localEulerAngles.z);
		localEulerAngles = this.rearRotorBlade.localEulerAngles;
		this.rearRotorBlade.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y - num, localEulerAngles.z);
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x00057E98 File Offset: 0x00056098
	private void EnableBlurredRotorBlades(bool enabled)
	{
		this.blurredRotorBladesEnabled = enabled;
		SkinnedMeshRenderer[] rotorBlades = this.blurredRotorBlades;
		for (int i = 0; i < rotorBlades.Length; i++)
		{
			rotorBlades[i].enabled = enabled;
		}
		rotorBlades = this.RotorBlades;
		for (int i = 0; i < rotorBlades.Length; i++)
		{
			rotorBlades[i].enabled = !enabled;
		}
	}
}
