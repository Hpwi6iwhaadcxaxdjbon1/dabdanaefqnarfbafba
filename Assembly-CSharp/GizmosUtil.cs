using System;
using UnityEngine;

// Token: 0x0200073E RID: 1854
public static class GizmosUtil
{
	// Token: 0x06002850 RID: 10320 RVA: 0x000CF6EC File Offset: 0x000CD8EC
	public static void DrawWireCircleX(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(0f, 1f, 1f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x000CF73C File Offset: 0x000CD93C
	public static void DrawWireCircleY(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 0f, 1f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x000CF78C File Offset: 0x000CD98C
	public static void DrawWireCircleZ(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 1f, 0f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x06002853 RID: 10323 RVA: 0x000CF7DC File Offset: 0x000CD9DC
	public static void DrawCircleX(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(0f, 1f, 1f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x06002854 RID: 10324 RVA: 0x000CF82C File Offset: 0x000CDA2C
	public static void DrawCircleY(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 0f, 1f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x06002855 RID: 10325 RVA: 0x000CF87C File Offset: 0x000CDA7C
	public static void DrawCircleZ(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 1f, 0f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x06002856 RID: 10326 RVA: 0x000CF8CC File Offset: 0x000CDACC
	public static void DrawWireCylinderX(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleX(pos - new Vector3(0.5f * height, 0f, 0f), radius);
		GizmosUtil.DrawWireCircleX(pos + new Vector3(0.5f * height, 0f, 0f), radius);
	}

	// Token: 0x06002857 RID: 10327 RVA: 0x000CF920 File Offset: 0x000CDB20
	public static void DrawWireCylinderY(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleY(pos - new Vector3(0f, 0.5f * height, 0f), radius);
		GizmosUtil.DrawWireCircleY(pos + new Vector3(0f, 0.5f * height, 0f), radius);
	}

	// Token: 0x06002858 RID: 10328 RVA: 0x000CF974 File Offset: 0x000CDB74
	public static void DrawWireCylinderZ(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleZ(pos - new Vector3(0f, 0f, 0.5f * height), radius);
		GizmosUtil.DrawWireCircleZ(pos + new Vector3(0f, 0f, 0.5f * height), radius);
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x000CF9C8 File Offset: 0x000CDBC8
	public static void DrawCylinderX(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleX(pos - new Vector3(0.5f * height, 0f, 0f), radius);
		GizmosUtil.DrawCircleX(pos + new Vector3(0.5f * height, 0f, 0f), radius);
	}

	// Token: 0x0600285A RID: 10330 RVA: 0x000CFA1C File Offset: 0x000CDC1C
	public static void DrawCylinderY(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleY(pos - new Vector3(0f, 0.5f * height, 0f), radius);
		GizmosUtil.DrawCircleY(pos + new Vector3(0f, 0.5f * height, 0f), radius);
	}

	// Token: 0x0600285B RID: 10331 RVA: 0x000CFA70 File Offset: 0x000CDC70
	public static void DrawCylinderZ(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleZ(pos - new Vector3(0f, 0f, 0.5f * height), radius);
		GizmosUtil.DrawCircleZ(pos + new Vector3(0f, 0f, 0.5f * height), radius);
	}

	// Token: 0x0600285C RID: 10332 RVA: 0x000CFAC4 File Offset: 0x000CDCC4
	public static void DrawWireCapsuleX(Vector3 pos, float radius, float height)
	{
		Vector3 vector = pos - new Vector3(0.5f * height, 0f, 0f);
		Vector3 vector2 = pos + new Vector3(0.5f * height, 0f, 0f);
		Gizmos.DrawWireSphere(vector, radius);
		Gizmos.DrawWireSphere(vector2, radius);
		Gizmos.DrawLine(vector + Vector3.forward * radius, vector2 + Vector3.forward * radius);
		Gizmos.DrawLine(vector + Vector3.up * radius, vector2 + Vector3.up * radius);
		Gizmos.DrawLine(vector + Vector3.back * radius, vector2 + Vector3.back * radius);
		Gizmos.DrawLine(vector + Vector3.down * radius, vector2 + Vector3.down * radius);
	}

	// Token: 0x0600285D RID: 10333 RVA: 0x000CFBB4 File Offset: 0x000CDDB4
	public static void DrawWireCapsuleY(Vector3 pos, float radius, float height)
	{
		Vector3 vector = pos - new Vector3(0f, 0.5f * height, 0f);
		Vector3 vector2 = pos + new Vector3(0f, 0.5f * height, 0f);
		Gizmos.DrawWireSphere(vector, radius);
		Gizmos.DrawWireSphere(vector2, radius);
		Gizmos.DrawLine(vector + Vector3.forward * radius, vector2 + Vector3.forward * radius);
		Gizmos.DrawLine(vector + Vector3.right * radius, vector2 + Vector3.right * radius);
		Gizmos.DrawLine(vector + Vector3.back * radius, vector2 + Vector3.back * radius);
		Gizmos.DrawLine(vector + Vector3.left * radius, vector2 + Vector3.left * radius);
	}

	// Token: 0x0600285E RID: 10334 RVA: 0x000CFCA4 File Offset: 0x000CDEA4
	public static void DrawWireCapsuleZ(Vector3 pos, float radius, float height)
	{
		Vector3 vector = pos - new Vector3(0f, 0f, 0.5f * height);
		Vector3 vector2 = pos + new Vector3(0f, 0f, 0.5f * height);
		Gizmos.DrawWireSphere(vector, radius);
		Gizmos.DrawWireSphere(vector2, radius);
		Gizmos.DrawLine(vector + Vector3.up * radius, vector2 + Vector3.up * radius);
		Gizmos.DrawLine(vector + Vector3.right * radius, vector2 + Vector3.right * radius);
		Gizmos.DrawLine(vector + Vector3.down * radius, vector2 + Vector3.down * radius);
		Gizmos.DrawLine(vector + Vector3.left * radius, vector2 + Vector3.left * radius);
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x000CFD94 File Offset: 0x000CDF94
	public static void DrawCapsuleX(Vector3 pos, float radius, float height)
	{
		Vector3 center = pos - new Vector3(0.5f * height, 0f, 0f);
		Vector3 center2 = pos + new Vector3(0.5f * height, 0f, 0f);
		Gizmos.DrawSphere(center, radius);
		Gizmos.DrawSphere(center2, radius);
	}

	// Token: 0x06002860 RID: 10336 RVA: 0x000CFDE8 File Offset: 0x000CDFE8
	public static void DrawCapsuleY(Vector3 pos, float radius, float height)
	{
		Vector3 center = pos - new Vector3(0f, 0.5f * height, 0f);
		Vector3 center2 = pos + new Vector3(0f, 0.5f * height, 0f);
		Gizmos.DrawSphere(center, radius);
		Gizmos.DrawSphere(center2, radius);
	}

	// Token: 0x06002861 RID: 10337 RVA: 0x000CFE3C File Offset: 0x000CE03C
	public static void DrawCapsuleZ(Vector3 pos, float radius, float height)
	{
		Vector3 center = pos - new Vector3(0f, 0f, 0.5f * height);
		Vector3 center2 = pos + new Vector3(0f, 0f, 0.5f * height);
		Gizmos.DrawSphere(center, radius);
		Gizmos.DrawSphere(center2, radius);
	}

	// Token: 0x06002862 RID: 10338 RVA: 0x0001F6EA File Offset: 0x0001D8EA
	public static void DrawWireCube(Vector3 pos, Vector3 size, Quaternion rot)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(pos, rot, size);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		Gizmos.matrix = matrix;
	}

	// Token: 0x06002863 RID: 10339 RVA: 0x0001F712 File Offset: 0x0001D912
	public static void DrawCube(Vector3 pos, Vector3 size, Quaternion rot)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(pos, rot, size);
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
		Gizmos.matrix = matrix;
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x000CFE90 File Offset: 0x000CE090
	public static void DrawWirePath(Vector3 a, Vector3 b, float thickness)
	{
		GizmosUtil.DrawWireCircleY(a, thickness);
		GizmosUtil.DrawWireCircleY(b, thickness);
		Vector3 normalized = (b - a).normalized;
		Vector3 a2 = Quaternion.Euler(0f, 90f, 0f) * normalized;
		Gizmos.DrawLine(b + a2 * thickness, a + a2 * thickness);
		Gizmos.DrawLine(b - a2 * thickness, a - a2 * thickness);
	}

	// Token: 0x06002865 RID: 10341 RVA: 0x000CFF14 File Offset: 0x000CE114
	public static void DrawSemiCircle(float radius)
	{
		float num = radius * 0.017453292f * 0.5f;
		Vector3 vector = Mathf.Cos(num) * Vector3.forward + Mathf.Sin(num) * Vector3.right;
		Gizmos.DrawLine(Vector3.zero, vector);
		Vector3 vector2 = Mathf.Cos(-num) * Vector3.forward + Mathf.Sin(-num) * Vector3.right;
		Gizmos.DrawLine(Vector3.zero, vector2);
		float num2 = Mathf.Clamp(radius / 16f, 4f, 64f);
		float num3 = num / num2;
		for (float num4 = num; num4 > 0f; num4 -= num3)
		{
			Vector3 vector3 = Mathf.Cos(num4) * Vector3.forward + Mathf.Sin(num4) * Vector3.right;
			Gizmos.DrawLine(Vector3.zero, vector3);
			if (vector != Vector3.zero)
			{
				Gizmos.DrawLine(vector3, vector);
			}
			vector = vector3;
			Vector3 vector4 = Mathf.Cos(-num4) * Vector3.forward + Mathf.Sin(-num4) * Vector3.right;
			Gizmos.DrawLine(Vector3.zero, vector4);
			if (vector2 != Vector3.zero)
			{
				Gizmos.DrawLine(vector4, vector2);
			}
			vector2 = vector4;
		}
		Gizmos.DrawLine(vector, vector2);
	}

	// Token: 0x06002866 RID: 10342 RVA: 0x000D0070 File Offset: 0x000CE270
	public static void DrawMeshes(Transform transform)
	{
		foreach (MeshRenderer meshRenderer in transform.GetComponentsInChildren<MeshRenderer>())
		{
			if (meshRenderer.enabled)
			{
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (component)
				{
					Transform transform2 = meshRenderer.transform;
					Gizmos.DrawMesh(component.sharedMesh, transform2.position, transform2.rotation, transform2.lossyScale);
				}
			}
		}
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x000D00D8 File Offset: 0x000CE2D8
	public static void DrawBounds(Transform transform)
	{
		Bounds bounds = transform.GetBounds(true, false, true);
		Vector3 lossyScale = transform.lossyScale;
		Quaternion rotation = transform.rotation;
		Vector3 pos = transform.position + rotation * Vector3.Scale(lossyScale, bounds.center);
		Vector3 size = Vector3.Scale(lossyScale, bounds.size);
		GizmosUtil.DrawCube(pos, size, rotation);
		GizmosUtil.DrawWireCube(pos, size, rotation);
	}
}
