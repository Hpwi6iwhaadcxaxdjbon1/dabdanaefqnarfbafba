using System;
using UnityEngine;

// Token: 0x020005B4 RID: 1460
public class ClothWindModify : FacepunchBehaviour
{
	// Token: 0x04001D7A RID: 7546
	public Cloth cloth;

	// Token: 0x04001D7B RID: 7547
	private Vector3 initialClothForce;

	// Token: 0x04001D7C RID: 7548
	public Vector3 worldWindScale = Vector3.one;

	// Token: 0x04001D7D RID: 7549
	public Vector3 turbulenceScale = Vector3.one;

	// Token: 0x060021A2 RID: 8610 RVA: 0x0001AC05 File Offset: 0x00018E05
	public void Awake()
	{
		this.initialClothForce = this.cloth.externalAcceleration;
		base.InvokeRandomized(new Action(this.DoWind), 0f, 0.2f, 0.05f);
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x000B643C File Offset: 0x000B463C
	public void DoWind()
	{
		Vector4 vector = WindZoneExManager.ComputeWindForceAtLocation(base.transform.position);
		Vector3 vector2 = new Vector3(vector.x * this.worldWindScale.x, vector.y * this.worldWindScale.y, vector.z * this.worldWindScale.z);
		float num = Mathf.Clamp01(vector.w);
		Vector3 a = new Vector3(vector2.x * this.turbulenceScale.x, vector2.y * this.turbulenceScale.y, vector2.z * this.turbulenceScale.z);
		vector2 += a * Mathf.PerlinNoise(base.transform.position.x + Time.time * num * 4f, base.transform.position.z);
		this.cloth.externalAcceleration = this.initialClothForce + vector2;
	}
}
