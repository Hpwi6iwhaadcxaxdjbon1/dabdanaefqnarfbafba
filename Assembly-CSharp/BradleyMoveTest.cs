using System;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class BradleyMoveTest : MonoBehaviour
{
	// Token: 0x040007C3 RID: 1987
	public WheelCollider[] leftWheels;

	// Token: 0x040007C4 RID: 1988
	public WheelCollider[] rightWheels;

	// Token: 0x040007C5 RID: 1989
	public float moveForceMax = 2000f;

	// Token: 0x040007C6 RID: 1990
	public float brakeForce = 100f;

	// Token: 0x040007C7 RID: 1991
	public float throttle = 1f;

	// Token: 0x040007C8 RID: 1992
	public float turnForce = 2000f;

	// Token: 0x040007C9 RID: 1993
	public float sideStiffnessMax = 1f;

	// Token: 0x040007CA RID: 1994
	public float sideStiffnessMin = 0.5f;

	// Token: 0x040007CB RID: 1995
	public Transform centerOfMass;

	// Token: 0x040007CC RID: 1996
	public float turning;

	// Token: 0x040007CD RID: 1997
	public bool brake;

	// Token: 0x040007CE RID: 1998
	public Rigidbody myRigidBody;

	// Token: 0x040007CF RID: 1999
	public Vector3 destination;

	// Token: 0x040007D0 RID: 2000
	public float stoppingDist = 5f;

	// Token: 0x040007D1 RID: 2001
	public GameObject followTest;

	// Token: 0x06000B58 RID: 2904 RVA: 0x0000AE70 File Offset: 0x00009070
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x0000AE78 File Offset: 0x00009078
	public void Initialize()
	{
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.destination = base.transform.position;
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x0000AEA1 File Offset: 0x000090A1
	public void SetDestination(Vector3 dest)
	{
		this.destination = dest;
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00058768 File Offset: 0x00056968
	public void FixedUpdate()
	{
		Vector3 velocity = this.myRigidBody.velocity;
		this.SetDestination(this.followTest.transform.position);
		float num = Vector3.Distance(base.transform.position, this.destination);
		if (num > this.stoppingDist)
		{
			Vector3 zero = Vector3.zero;
			float num2 = Vector3.Dot(zero, base.transform.right);
			float num3 = Vector3.Dot(zero, -base.transform.right);
			float num4 = Vector3.Dot(zero, base.transform.right);
			if (Vector3.Dot(zero, -base.transform.forward) > num4)
			{
				if (num2 >= num3)
				{
					this.turning = 1f;
				}
				else
				{
					this.turning = -1f;
				}
			}
			else
			{
				this.turning = num4;
			}
			this.throttle = Mathf.InverseLerp(this.stoppingDist, 30f, num);
		}
		this.throttle = Mathf.Clamp(this.throttle, -1f, 1f);
		float num5 = this.throttle;
		float num6 = this.throttle;
		if (this.turning > 0f)
		{
			num6 = -this.turning;
			num5 = this.turning;
		}
		else if (this.turning < 0f)
		{
			num5 = this.turning;
			num6 = this.turning * -1f;
		}
		this.ApplyBrakes(this.brake ? 1f : 0f);
		float num7 = this.throttle;
		num5 = Mathf.Clamp(num5 + num7, -1f, 1f);
		num6 = Mathf.Clamp(num6 + num7, -1f, 1f);
		this.AdjustFriction();
		float t = Mathf.InverseLerp(3f, 1f, velocity.magnitude * Mathf.Abs(Vector3.Dot(velocity.normalized, base.transform.forward)));
		float torqueAmount = Mathf.Lerp(this.moveForceMax, this.turnForce, t);
		this.SetMotorTorque(num5, false, torqueAmount);
		this.SetMotorTorque(num6, true, torqueAmount);
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x0000AEAA File Offset: 0x000090AA
	public void ApplyBrakes(float amount)
	{
		this.ApplyBrakeTorque(amount, true);
		this.ApplyBrakeTorque(amount, false);
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x0005896C File Offset: 0x00056B6C
	public float GetMotorTorque(bool rightSide)
	{
		float num = 0f;
		foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
		{
			num += wheelCollider.motorTorque;
		}
		return num / (float)this.rightWheels.Length;
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x000589BC File Offset: 0x00056BBC
	public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
	{
		newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
		float motorTorque = torqueAmount * newThrottle;
		WheelCollider[] array = rightSide ? this.rightWheels : this.leftWheels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].motorTorque = motorTorque;
		}
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x00058A08 File Offset: 0x00056C08
	public void ApplyBrakeTorque(float amount, bool rightSide)
	{
		WheelCollider[] array = rightSide ? this.rightWheels : this.leftWheels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].brakeTorque = this.brakeForce * amount;
		}
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x00002ECE File Offset: 0x000010CE
	public void AdjustFriction()
	{
	}
}
