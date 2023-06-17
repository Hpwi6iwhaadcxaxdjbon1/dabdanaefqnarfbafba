using System;

namespace UnityEngine
{
	// Token: 0x02000836 RID: 2102
	public static class RayEx
	{
		// Token: 0x06002D89 RID: 11657 RVA: 0x00023698 File Offset: 0x00021898
		public static Vector3 ClosestPoint(this Ray ray, Vector3 pos)
		{
			return ray.origin + Vector3.Dot(pos - ray.origin, ray.direction) * ray.direction;
		}

		// Token: 0x06002D8A RID: 11658 RVA: 0x000E4D24 File Offset: 0x000E2F24
		public static float Distance(this Ray ray, Vector3 pos)
		{
			return Vector3.Cross(ray.direction, pos - ray.origin).magnitude;
		}

		// Token: 0x06002D8B RID: 11659 RVA: 0x000E4D54 File Offset: 0x000E2F54
		public static float SqrDistance(this Ray ray, Vector3 pos)
		{
			return Vector3.Cross(ray.direction, pos - ray.origin).sqrMagnitude;
		}

		// Token: 0x06002D8C RID: 11660 RVA: 0x000236CB File Offset: 0x000218CB
		public static bool IsNaNOrInfinity(this Ray r)
		{
			return Vector3Ex.IsNaNOrInfinity(r.origin) || Vector3Ex.IsNaNOrInfinity(r.direction);
		}
	}
}
