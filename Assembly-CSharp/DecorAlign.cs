using System;
using UnityEngine;

// Token: 0x020004A4 RID: 1188
public class DecorAlign : DecorComponent
{
	// Token: 0x04001882 RID: 6274
	public float NormalAlignment = 1f;

	// Token: 0x04001883 RID: 6275
	public float GradientAlignment = 1f;

	// Token: 0x04001884 RID: 6276
	public Vector3 SlopeOffset = Vector3.zero;

	// Token: 0x04001885 RID: 6277
	public Vector3 SlopeScale = Vector3.one;

	// Token: 0x06001BAC RID: 7084 RVA: 0x00098DAC File Offset: 0x00096FAC
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		Vector3 normal = TerrainMeta.HeightMap.GetNormal(pos);
		Vector3 vector = (normal == Vector3.up) ? Vector3.forward : Vector3.Cross(normal, Vector3.up);
		Vector3 vector2 = Vector3.Cross(normal, vector);
		if (this.SlopeOffset != Vector3.zero || this.SlopeScale != Vector3.one)
		{
			float slope = TerrainMeta.HeightMap.GetSlope01(pos);
			if (this.SlopeOffset != Vector3.zero)
			{
				Vector3 vector3 = this.SlopeOffset * slope;
				pos += vector3.x * vector;
				pos += vector3.y * normal;
				pos -= vector3.z * vector2;
			}
			if (this.SlopeScale != Vector3.one)
			{
				Vector3 vector4 = Vector3.Lerp(Vector3.one, Vector3.one + Quaternion.Inverse(rot) * (this.SlopeScale - Vector3.one), slope);
				scale.x *= vector4.x;
				scale.y *= vector4.y;
				scale.z *= vector4.z;
			}
		}
		Vector3 up = Vector3.Lerp(rot * Vector3.up, normal, this.NormalAlignment);
		Quaternion lhs = QuaternionEx.LookRotationForcedUp(Vector3.Lerp(rot * Vector3.forward, vector2, this.GradientAlignment), up);
		rot = lhs * rot;
	}
}
