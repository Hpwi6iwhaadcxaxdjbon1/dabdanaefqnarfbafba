using System;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class CH47FlightTest : MonoBehaviour
{
	// Token: 0x040007AC RID: 1964
	public Rigidbody rigidBody;

	// Token: 0x040007AD RID: 1965
	public float engineThrustMax;

	// Token: 0x040007AE RID: 1966
	public Vector3 torqueScale;

	// Token: 0x040007AF RID: 1967
	public Transform com;

	// Token: 0x040007B0 RID: 1968
	public Transform[] GroundPoints;

	// Token: 0x040007B1 RID: 1969
	public Transform[] GroundEffects;

	// Token: 0x040007B2 RID: 1970
	public Transform AIMoveTarget;

	// Token: 0x040007B3 RID: 1971
	private static float altitudeTolerance = 1f;

	// Token: 0x040007B4 RID: 1972
	public float currentThrottle;

	// Token: 0x040007B5 RID: 1973
	public float avgThrust;

	// Token: 0x040007B6 RID: 1974
	public float liftDotMax = 0.75f;

	// Token: 0x06000B45 RID: 2885 RVA: 0x0000ADDC File Offset: 0x00008FDC
	public void Awake()
	{
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00057EEC File Offset: 0x000560EC
	public CH47FlightTest.HelicopterInputState_t GetHelicopterInputState()
	{
		CH47FlightTest.HelicopterInputState_t helicopterInputState_t = default(CH47FlightTest.HelicopterInputState_t);
		helicopterInputState_t.throttle = (Input.GetKey(KeyCode.W) ? 1f : 0f);
		helicopterInputState_t.throttle -= (Input.GetKey(KeyCode.S) ? 1f : 0f);
		helicopterInputState_t.pitch = Input.GetAxis("Mouse Y");
		helicopterInputState_t.roll = -Input.GetAxis("Mouse X");
		helicopterInputState_t.yaw = (Input.GetKey(KeyCode.D) ? 1f : 0f);
		helicopterInputState_t.yaw -= (Input.GetKey(KeyCode.A) ? 1f : 0f);
		helicopterInputState_t.pitch = (float)Mathf.RoundToInt(helicopterInputState_t.pitch);
		helicopterInputState_t.roll = (float)Mathf.RoundToInt(helicopterInputState_t.roll);
		return helicopterInputState_t;
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x00057FC4 File Offset: 0x000561C4
	public CH47FlightTest.HelicopterInputState_t GetAIInputState()
	{
		CH47FlightTest.HelicopterInputState_t result = default(CH47FlightTest.HelicopterInputState_t);
		Vector3 vector = Vector3.Cross(Vector3.up, base.transform.right);
		float num = Vector3.Dot(Vector3.Cross(Vector3.up, vector), Vector3Ex.Direction2D(this.AIMoveTarget.position, base.transform.position));
		result.yaw = ((num < 0f) ? 1f : 0f);
		result.yaw -= ((num > 0f) ? 1f : 0f);
		float num2 = Vector3.Dot(Vector3.up, base.transform.right);
		result.roll = ((num2 < 0f) ? 1f : 0f);
		result.roll -= ((num2 > 0f) ? 1f : 0f);
		float num3 = Vector3Ex.Distance2D(base.transform.position, this.AIMoveTarget.position);
		float num4 = Vector3.Dot(vector, Vector3Ex.Direction2D(this.AIMoveTarget.position, base.transform.position));
		float num5 = Vector3.Dot(Vector3.up, base.transform.forward);
		if (num3 > 10f)
		{
			result.pitch = ((num4 > 0.8f) ? -0.25f : 0f);
			result.pitch -= ((num4 < -0.8f) ? -0.25f : 0f);
			if (num5 < -0.35f)
			{
				result.pitch = -1f;
			}
			else if (num5 > 0.35f)
			{
				result.pitch = 1f;
			}
		}
		else if (num5 < --0f)
		{
			result.pitch = -1f;
		}
		else if (num5 > 0f)
		{
			result.pitch = 1f;
		}
		float idealAltitude = this.GetIdealAltitude();
		float y = base.transform.position.y;
		float num6;
		if (y > idealAltitude + CH47FlightTest.altitudeTolerance)
		{
			num6 = -1f;
		}
		else if (y < idealAltitude - CH47FlightTest.altitudeTolerance)
		{
			num6 = 1f;
		}
		else if (num3 > 20f)
		{
			num6 = Mathf.Lerp(0f, 1f, num3 / 20f);
		}
		else
		{
			num6 = 0f;
		}
		Debug.Log("desiredThrottle : " + num6);
		result.throttle = num6 * 1f;
		return result;
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x0000ADF4 File Offset: 0x00008FF4
	public float GetIdealAltitude()
	{
		return this.AIMoveTarget.transform.position.y;
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x00058238 File Offset: 0x00056438
	public void FixedUpdate()
	{
		CH47FlightTest.HelicopterInputState_t aiinputState = this.GetAIInputState();
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, aiinputState.throttle, 2f * Time.fixedDeltaTime);
		this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.2f, 1f);
		this.rigidBody.AddRelativeTorque(new Vector3(aiinputState.pitch * this.torqueScale.x, aiinputState.yaw * this.torqueScale.y, aiinputState.roll * this.torqueScale.z) * Time.fixedDeltaTime, 0);
		this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.fixedDeltaTime);
		float value = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
		float num = Mathf.InverseLerp(this.liftDotMax, 1f, value);
		Vector3 vector = Vector3.up * this.engineThrustMax * 0.5f * this.currentThrottle * num;
		Vector3 vector2 = (base.transform.up - Vector3.up).normalized * this.engineThrustMax * this.currentThrottle * (1f - num);
		float d = this.rigidBody.mass * -Physics.gravity.y;
		this.rigidBody.AddForce(base.transform.up * d * num * 0.99f, 0);
		this.rigidBody.AddForce(vector, 0);
		this.rigidBody.AddForce(vector2, 0);
		for (int i = 0; i < this.GroundEffects.Length; i++)
		{
			Component component = this.GroundPoints[i];
			Transform transform = this.GroundEffects[i];
			RaycastHit raycastHit;
			if (Physics.Raycast(component.transform.position, Vector3.down, ref raycastHit, 50f, 8388608))
			{
				transform.gameObject.SetActive(true);
				transform.transform.position = raycastHit.point + new Vector3(0f, 1f, 0f);
			}
			else
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x00058498 File Offset: 0x00056698
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(this.AIMoveTarget.transform.position, 1f);
		Vector3 vector = Vector3.Cross(base.transform.right, Vector3.up);
		Vector3 a = Vector3.Cross(vector, Vector3.up);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.position, base.transform.position + vector * 10f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, base.transform.position + a * 10f);
	}

	// Token: 0x02000105 RID: 261
	public struct HelicopterInputState_t
	{
		// Token: 0x040007B7 RID: 1975
		public float throttle;

		// Token: 0x040007B8 RID: 1976
		public float roll;

		// Token: 0x040007B9 RID: 1977
		public float yaw;

		// Token: 0x040007BA RID: 1978
		public float pitch;
	}
}
