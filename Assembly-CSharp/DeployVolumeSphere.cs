using System;
using UnityEngine;

// Token: 0x0200037D RID: 893
public class DeployVolumeSphere : DeployVolume
{
	// Token: 0x040013A4 RID: 5028
	public Vector3 center = Vector3.zero;

	// Token: 0x040013A5 RID: 5029
	public float radius = 0.5f;

	// Token: 0x060016C7 RID: 5831 RVA: 0x0008840C File Offset: 0x0008660C
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		if (DeployVolume.CheckSphere(position, this.radius, this.layers & mask, this.ignore))
		{
			if (Input.GetKey(KeyCode.KeypadDivide))
			{
				DDraw.Sphere(position, this.radius, Color.red, 0.1f, true);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x0008848C File Offset: 0x0008668C
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		if ((this.layers & mask) != 0 && Vector3.Distance(position, obb.ClosestPoint(position)) <= this.radius)
		{
			if (Input.GetKey(KeyCode.KeypadDivide))
			{
				DDraw.Sphere(position, this.radius, Color.red, 0.1f, true);
			}
			return true;
		}
		return false;
	}
}
