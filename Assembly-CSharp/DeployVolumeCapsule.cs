using System;
using UnityEngine;

// Token: 0x02000379 RID: 889
public class DeployVolumeCapsule : DeployVolume
{
	// Token: 0x0400139D RID: 5021
	public Vector3 center = Vector3.zero;

	// Token: 0x0400139E RID: 5022
	public float radius = 0.5f;

	// Token: 0x0400139F RID: 5023
	public float height = 1f;

	// Token: 0x060016B9 RID: 5817 RVA: 0x00087FF4 File Offset: 0x000861F4
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		Vector3 vector = position + rotation * this.worldRotation * Vector3.up * this.height * 0.5f;
		Vector3 vector2 = position + rotation * this.worldRotation * Vector3.down * this.height * 0.5f;
		if (DeployVolume.CheckCapsule(vector, vector2, this.radius, this.layers & mask, this.ignore))
		{
			if (Input.GetKey(KeyCode.KeypadDivide))
			{
				DDraw.Sphere(vector, this.radius, Color.red, 0.1f, true);
				DDraw.Sphere(vector2, this.radius, Color.red, 0.1f, true);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x0000508F File Offset: 0x0000328F
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		return false;
	}
}
