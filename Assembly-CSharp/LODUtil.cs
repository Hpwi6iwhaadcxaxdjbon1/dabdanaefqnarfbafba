using System;
using UnityEngine;

// Token: 0x020003C6 RID: 966
public static class LODUtil
{
	// Token: 0x0600185D RID: 6237 RVA: 0x00014615 File Offset: 0x00012815
	public static float GetDistance(Transform transform, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		return LODUtil.GetDistance(transform.position, mode);
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x00014623 File Offset: 0x00012823
	public static float GetDistance(Vector3 worldPos, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		if (!MainCamera.isValid)
		{
			return 1000f;
		}
		if (mode != LODDistanceMode.XYZ)
		{
			return Vector3Ex.Distance2D(MainCamera.position, worldPos);
		}
		return Vector3.Distance(MainCamera.position, worldPos);
	}

	// Token: 0x0600185F RID: 6239 RVA: 0x0001464C File Offset: 0x0001284C
	public static float VerifyDistance(float distance)
	{
		return Mathf.Min(500f, distance);
	}
}
