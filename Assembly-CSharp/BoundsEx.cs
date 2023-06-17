using System;
using UnityEngine;

// Token: 0x02000724 RID: 1828
public static class BoundsEx
{
	// Token: 0x06002803 RID: 10243 RVA: 0x0001F322 File Offset: 0x0001D522
	public static Bounds XZ3D(this Bounds bounds)
	{
		return new Bounds(Vector3Ex.XZ3D(bounds.center), Vector3Ex.XZ3D(bounds.size));
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x000CE464 File Offset: 0x000CC664
	public static Bounds Transform(this Bounds bounds, Matrix4x4 matrix)
	{
		Vector3 center = matrix.MultiplyPoint3x4(bounds.center);
		Vector3 extents = bounds.extents;
		Vector3 vector = matrix.MultiplyVector(new Vector3(extents.x, 0f, 0f));
		Vector3 vector2 = matrix.MultiplyVector(new Vector3(0f, extents.y, 0f));
		Vector3 vector3 = matrix.MultiplyVector(new Vector3(0f, 0f, extents.z));
		extents.x = Mathf.Abs(vector.x) + Mathf.Abs(vector2.x) + Mathf.Abs(vector3.x);
		extents.y = Mathf.Abs(vector.y) + Mathf.Abs(vector2.y) + Mathf.Abs(vector3.y);
		extents.z = Mathf.Abs(vector.z) + Mathf.Abs(vector2.z) + Mathf.Abs(vector3.z);
		return new Bounds
		{
			center = center,
			extents = extents
		};
	}
}
