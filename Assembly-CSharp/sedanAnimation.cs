using System;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class sedanAnimation : MonoBehaviour
{
	// Token: 0x040000A0 RID: 160
	public Transform[] frontAxles;

	// Token: 0x040000A1 RID: 161
	public Transform FL_shock;

	// Token: 0x040000A2 RID: 162
	public Transform FL_wheel;

	// Token: 0x040000A3 RID: 163
	public Transform FR_shock;

	// Token: 0x040000A4 RID: 164
	public Transform FR_wheel;

	// Token: 0x040000A5 RID: 165
	public Transform RL_shock;

	// Token: 0x040000A6 RID: 166
	public Transform RL_wheel;

	// Token: 0x040000A7 RID: 167
	public Transform RR_shock;

	// Token: 0x040000A8 RID: 168
	public Transform RR_wheel;

	// Token: 0x040000A9 RID: 169
	public WheelCollider FL_wheelCollider;

	// Token: 0x040000AA RID: 170
	public WheelCollider FR_wheelCollider;

	// Token: 0x040000AB RID: 171
	public WheelCollider RL_wheelCollider;

	// Token: 0x040000AC RID: 172
	public WheelCollider RR_wheelCollider;

	// Token: 0x040000AD RID: 173
	public Transform steeringWheel;

	// Token: 0x040000AE RID: 174
	public float motorForceConstant = 150f;

	// Token: 0x040000AF RID: 175
	public float brakeForceConstant = 500f;

	// Token: 0x040000B0 RID: 176
	public float brakePedal;

	// Token: 0x040000B1 RID: 177
	public float gasPedal;

	// Token: 0x040000B2 RID: 178
	public float steering;

	// Token: 0x040000B3 RID: 179
	private Rigidbody myRigidbody;

	// Token: 0x040000B4 RID: 180
	public float GasLerpTime = 20f;

	// Token: 0x040000B5 RID: 181
	public float SteeringLerpTime = 20f;

	// Token: 0x040000B6 RID: 182
	private float wheelSpinConstant = 120f;

	// Token: 0x040000B7 RID: 183
	private float shockRestingPosY = -0.27f;

	// Token: 0x040000B8 RID: 184
	private float shockDistance = 0.3f;

	// Token: 0x040000B9 RID: 185
	private float traceDistanceNeutralPoint = 0.7f;

	// Token: 0x06000062 RID: 98 RVA: 0x000030D9 File Offset: 0x000012D9
	private void Start()
	{
		this.myRigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000063 RID: 99 RVA: 0x000030E7 File Offset: 0x000012E7
	private void Update()
	{
		this.DoSteering();
		this.ApplyForceAtWheels();
		this.UpdateTireAnimation();
		this.InputPlayer();
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00028878 File Offset: 0x00026A78
	private void InputPlayer()
	{
		if (Input.GetKey(KeyCode.W))
		{
			this.gasPedal = Mathf.Clamp(this.gasPedal + Time.deltaTime * this.GasLerpTime, -100f, 100f);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 0f, Time.deltaTime * this.GasLerpTime);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			this.gasPedal = Mathf.Clamp(this.gasPedal - Time.deltaTime * this.GasLerpTime, -100f, 100f);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 0f, Time.deltaTime * this.GasLerpTime);
		}
		else
		{
			this.gasPedal = Mathf.Lerp(this.gasPedal, 0f, Time.deltaTime * this.GasLerpTime);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 100f, Time.deltaTime * this.GasLerpTime / 5f);
		}
		if (Input.GetKey(KeyCode.A))
		{
			this.steering = Mathf.Clamp(this.steering - Time.deltaTime * this.SteeringLerpTime, -60f, 60f);
			return;
		}
		if (Input.GetKey(KeyCode.D))
		{
			this.steering = Mathf.Clamp(this.steering + Time.deltaTime * this.SteeringLerpTime, -60f, 60f);
			return;
		}
		this.steering = Mathf.Lerp(this.steering, 0f, Time.deltaTime * this.SteeringLerpTime);
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00003101 File Offset: 0x00001301
	private void DoSteering()
	{
		this.FL_wheelCollider.steerAngle = this.steering;
		this.FR_wheelCollider.steerAngle = this.steering;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00028A04 File Offset: 0x00026C04
	private void ApplyForceAtWheels()
	{
		if (this.FL_wheelCollider.isGrounded)
		{
			this.FL_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.FL_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.FR_wheelCollider.isGrounded)
		{
			this.FR_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.FR_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.RL_wheelCollider.isGrounded)
		{
			this.RL_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.RL_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.RR_wheelCollider.isGrounded)
		{
			this.RR_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.RR_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00028B08 File Offset: 0x00026D08
	private void UpdateTireAnimation()
	{
		float num = Vector3.Dot(this.myRigidbody.velocity, this.myRigidbody.transform.forward);
		if (this.FL_wheelCollider.isGrounded)
		{
			this.FL_shock.localPosition = new Vector3(this.FL_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.FL_wheelCollider), this.FL_shock.localPosition.z);
			this.FL_wheel.localEulerAngles = new Vector3(this.FL_wheel.localEulerAngles.x, this.FL_wheel.localEulerAngles.y, this.FL_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.FL_shock.localPosition = Vector3.Lerp(this.FL_shock.localPosition, new Vector3(this.FL_shock.localPosition.x, this.shockRestingPosY, this.FL_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (this.FR_wheelCollider.isGrounded)
		{
			this.FR_shock.localPosition = new Vector3(this.FR_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.FR_wheelCollider), this.FR_shock.localPosition.z);
			this.FR_wheel.localEulerAngles = new Vector3(this.FR_wheel.localEulerAngles.x, this.FR_wheel.localEulerAngles.y, this.FR_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.FR_shock.localPosition = Vector3.Lerp(this.FR_shock.localPosition, new Vector3(this.FR_shock.localPosition.x, this.shockRestingPosY, this.FR_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (this.RL_wheelCollider.isGrounded)
		{
			this.RL_shock.localPosition = new Vector3(this.RL_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.RL_wheelCollider), this.RL_shock.localPosition.z);
			this.RL_wheel.localEulerAngles = new Vector3(this.RL_wheel.localEulerAngles.x, this.RL_wheel.localEulerAngles.y, this.RL_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.RL_shock.localPosition = Vector3.Lerp(this.RL_shock.localPosition, new Vector3(this.RL_shock.localPosition.x, this.shockRestingPosY, this.RL_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (this.RR_wheelCollider.isGrounded)
		{
			this.RR_shock.localPosition = new Vector3(this.RR_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.RR_wheelCollider), this.RR_shock.localPosition.z);
			this.RR_wheel.localEulerAngles = new Vector3(this.RR_wheel.localEulerAngles.x, this.RR_wheel.localEulerAngles.y, this.RR_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.RR_shock.localPosition = Vector3.Lerp(this.RR_shock.localPosition, new Vector3(this.RR_shock.localPosition.x, this.shockRestingPosY, this.RR_shock.localPosition.z), Time.deltaTime * 2f);
		}
		foreach (Transform transform in this.frontAxles)
		{
			transform.localEulerAngles = new Vector3(this.steering, transform.localEulerAngles.y, transform.localEulerAngles.z);
		}
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00028F4C File Offset: 0x0002714C
	private float GetShockHeightDelta(WheelCollider wheel)
	{
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction"
		});
		RaycastHit raycastHit;
		Physics.Linecast(wheel.transform.position, wheel.transform.position - Vector3.up * 10f, ref raycastHit, mask);
		return Mathx.RemapValClamped(raycastHit.distance, this.traceDistanceNeutralPoint - this.shockDistance, this.traceDistanceNeutralPoint + this.shockDistance, this.shockDistance * 0.75f, -0.75f * this.shockDistance);
	}
}
