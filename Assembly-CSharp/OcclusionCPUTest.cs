using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020007B0 RID: 1968
public class OcclusionCPUTest
{
	// Token: 0x06002AAD RID: 10925 RVA: 0x000213E5 File Offset: 0x0001F5E5
	private static float DistanceToPlane(Vector4 vPlane, Vector4 vPoint)
	{
		return Vector4.Dot(new Vector4(vPoint.x, vPoint.y, vPoint.z, 1f), vPlane);
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x000D95AC File Offset: 0x000D77AC
	private static float FrustumCullSphere(Vector4[] frustumPlanes, Vector3 vCenter, float fRadius)
	{
		float a = Mathf.Min(OcclusionCPUTest.DistanceToPlane(frustumPlanes[0], vCenter), OcclusionCPUTest.DistanceToPlane(frustumPlanes[1], vCenter));
		float b = Mathf.Min(OcclusionCPUTest.DistanceToPlane(frustumPlanes[2], vCenter), OcclusionCPUTest.DistanceToPlane(frustumPlanes[3], vCenter));
		float b2 = Mathf.Min(OcclusionCPUTest.DistanceToPlane(frustumPlanes[4], vCenter), OcclusionCPUTest.DistanceToPlane(frustumPlanes[5], vCenter));
		return Mathf.Min(Mathf.Min(a, b), b2) + fRadius;
	}

	// Token: 0x06002AAF RID: 10927 RVA: 0x00021409 File Offset: 0x0001F609
	private static Vector4 ToVector4(Vector3 v, float w)
	{
		return new Vector4(v.x, v.y, v.z, w);
	}

	// Token: 0x06002AB0 RID: 10928 RVA: 0x00021423 File Offset: 0x0001F623
	private static Vector4 ToVector4(float x)
	{
		return new Vector4(x, x, x, x);
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x000D9644 File Offset: 0x000D7844
	private static float Linear01Depth(Camera camera, float depth)
	{
		float nearClipPlane = camera.nearClipPlane;
		float farClipPlane = camera.farClipPlane;
		float num;
		float num2;
		if (SystemInfo.usesReversedZBuffer)
		{
			num = -1f + farClipPlane / nearClipPlane;
			num2 = 1f;
		}
		else
		{
			num = 1f - farClipPlane / nearClipPlane;
			num2 = farClipPlane / nearClipPlane;
		}
		return 1f / (num * depth + num2);
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x000D9694 File Offset: 0x000D7894
	public static Vector4 CullSphere(Camera camera, Vector4 Bounds, Matrix4x4 View, Matrix4x4 Projection, Matrix4x4 ViewProjection, Vector4[] frustumPlanes, Vector2 ViewportSize)
	{
		Vector3 vector = new Vector3(Bounds.x, Bounds.y, Bounds.z);
		float w = Bounds.w;
		if (OcclusionCPUTest.FrustumCullSphere(frustumPlanes, vector, Bounds.w) <= 0f)
		{
			return Vector4.zero;
		}
		Vector3 vector2 = new Vector3(View.m10, View.m11, View.m12);
		Vector3 position = camera.transform.position;
		Vector3 normalized = Vector3.Cross(position - vector, vector2).normalized;
		float num = Vector3.Distance(position, vector);
		if (num > w)
		{
			float d = num * Mathf.Tan(Mathf.Asin(w / num));
			Vector3 b = vector2 * d;
			Vector3 b2 = normalized * d;
			Vector4 vector3 = OcclusionCPUTest.ToVector4(vector + b - b2, 1f);
			Vector4 vector4 = OcclusionCPUTest.ToVector4(vector + b + b2, 1f);
			Vector4 vector5 = OcclusionCPUTest.ToVector4(vector - b - b2, 1f);
			Vector4 vector6 = OcclusionCPUTest.ToVector4(vector - b + b2, 1f);
			Vector4 vector7 = ViewProjection * vector3;
			Vector4 vector8 = ViewProjection * vector4;
			Vector4 vector9 = ViewProjection * vector5;
			Vector4 vector10 = ViewProjection * vector6;
			Vector2 a = new Vector2(vector7.x, vector7.y) / vector7.w;
			Vector2 vector11 = new Vector2(vector8.x, vector8.y) / vector8.w;
			Vector2 a2 = new Vector2(vector9.x, vector9.y) / vector9.w;
			Vector2 a3 = new Vector2(vector10.x, vector10.y) / vector10.w;
			Vector2 v = a * 0.5f + new Vector2(0.5f, 0.5f);
			vector11 = vector11 * 0.5f + new Vector2(0.5f, 0.5f);
			a2 = a2 * 0.5f + new Vector2(0.5f, 0.5f);
			a3 = a3 * 0.5f + new Vector2(0.5f, 0.5f);
			float num2 = Vector3.Distance(v, vector11);
			Vector3 vector12 = View * OcclusionCPUTest.ToVector4(vector, 1f);
			Vector3 v2 = vector12 - Vector3.Normalize(vector12) * w;
			Vector4 vector13 = Projection * OcclusionCPUTest.ToVector4(v2, 1f);
			float num3 = vector13.z / vector13.w;
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore)
			{
				num3 = num3 * 0.5f + 0.5f;
			}
			float w2 = Mathf.Ceil(Mathf.Log(num2 * Mathf.Max(ViewportSize.x, ViewportSize.y), 2f));
			Vector3 vPos = vector3;
			Vector3 vector14 = vector4;
			Vector3 vector15 = vector5;
			Vector3 vPosB = vector6;
			DDraw.Line(vPos, vector14, Color.cyan, 0.1f, false, false);
			DDraw.Line(vPos, vector15, Color.cyan, 0.1f, false, false);
			DDraw.Line(vector15, vPosB, Color.cyan, 0.1f, false, false);
			DDraw.Line(vector14, vPosB, Color.cyan, 0.1f, false, false);
			return new Vector4(OcclusionCPUTest.Linear01Depth(camera, num3), 0f, num2, w2);
		}
		return Vector4.one;
	}
}
