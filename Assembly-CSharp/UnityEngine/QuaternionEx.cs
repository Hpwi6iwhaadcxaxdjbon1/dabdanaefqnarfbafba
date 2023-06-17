using System;

namespace UnityEngine
{
	// Token: 0x02000835 RID: 2101
	public static class QuaternionEx
	{
		// Token: 0x06002D83 RID: 11651 RVA: 0x00023650 File Offset: 0x00021850
		public static Quaternion AlignToNormal(this Quaternion rot, Vector3 normal)
		{
			return Quaternion.FromToRotation(Vector3.up, normal) * rot;
		}

		// Token: 0x06002D84 RID: 11652 RVA: 0x00023663 File Offset: 0x00021863
		public static Quaternion LookRotationWithOffset(Vector3 offset, Vector3 forward, Vector3 up)
		{
			return Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(offset, Vector3.up));
		}

		// Token: 0x06002D85 RID: 11653 RVA: 0x000E4C24 File Offset: 0x000E2E24
		public static Quaternion LookRotationForcedUp(Vector3 forward, Vector3 up)
		{
			if (forward == up)
			{
				return Quaternion.LookRotation(up);
			}
			Vector3 rhs = Vector3.Cross(forward, up);
			forward = Vector3.Cross(up, rhs);
			return Quaternion.LookRotation(forward, up);
		}

		// Token: 0x06002D86 RID: 11654 RVA: 0x000E4C5C File Offset: 0x000E2E5C
		public static Quaternion LookRotationGradient(Vector3 normal, Vector3 up)
		{
			Vector3 rhs = (normal == Vector3.up) ? Vector3.forward : Vector3.Cross(normal, Vector3.up);
			return QuaternionEx.LookRotationForcedUp(Vector3.Cross(normal, rhs), up);
		}

		// Token: 0x06002D87 RID: 11655 RVA: 0x000E4C98 File Offset: 0x000E2E98
		public static Quaternion LookRotationNormal(Vector3 normal, Vector3 up = default(Vector3))
		{
			if (up != Vector3.zero)
			{
				return QuaternionEx.LookRotationForcedUp(up, normal);
			}
			if (normal == Vector3.up)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.forward, normal);
			}
			if (normal == Vector3.down)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.back, normal);
			}
			if (normal.y == 0f)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.up, normal);
			}
			Vector3 rhs = Vector3.Cross(normal, Vector3.up);
			return QuaternionEx.LookRotationForcedUp(-Vector3.Cross(normal, rhs), normal);
		}

		// Token: 0x06002D88 RID: 11656 RVA: 0x00023685 File Offset: 0x00021885
		public static Quaternion EnsureValid(this Quaternion rot, float epsilon = 1E-45f)
		{
			if (Quaternion.Dot(rot, rot) < epsilon)
			{
				return Quaternion.identity;
			}
			return rot;
		}
	}
}
