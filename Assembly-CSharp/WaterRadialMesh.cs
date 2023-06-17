using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000561 RID: 1377
[Serializable]
public class WaterRadialMesh
{
	// Token: 0x04001B5D RID: 7005
	private const float AlignmentGranularity = 1f;

	// Token: 0x04001B5E RID: 7006
	private const float MaxHorizontalDisplacement = 1f;

	// Token: 0x04001B5F RID: 7007
	private Mesh[] meshes;

	// Token: 0x04001B60 RID: 7008
	private bool initialized;

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x06001F28 RID: 7976 RVA: 0x00018997 File Offset: 0x00016B97
	public Mesh[] Meshes
	{
		get
		{
			return this.meshes;
		}
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x06001F29 RID: 7977 RVA: 0x0001899F File Offset: 0x00016B9F
	public bool IsInitialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x000189A7 File Offset: 0x00016BA7
	public void Initialize(int vertexCount)
	{
		this.meshes = this.GenerateMeshes(vertexCount, false);
		this.initialized = true;
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x000AA494 File Offset: 0x000A8694
	public void Destroy()
	{
		if (this.initialized)
		{
			Mesh[] array = this.meshes;
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i]);
			}
			this.meshes = null;
			this.initialized = false;
		}
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x000189BE File Offset: 0x00016BBE
	private Mesh CreateMesh(string name, Vector3[] vertices, int[] indices)
	{
		Mesh mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		mesh.name = name;
		mesh.vertices = vertices;
		mesh.SetIndices(indices, MeshTopology.Quads, 0);
		mesh.RecalculateBounds();
		mesh.UploadMeshData(true);
		return mesh;
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x000AA4D4 File Offset: 0x000A86D4
	private Mesh[] GenerateMeshes(int vertexCount, bool volume = false)
	{
		int num = Mathf.RoundToInt((float)Mathf.RoundToInt(Mathf.Sqrt((float)vertexCount)) * 0.4f);
		int num2 = Mathf.RoundToInt((float)vertexCount / (float)num);
		int num3 = volume ? (num2 / 2) : num2;
		List<Mesh> list = new List<Mesh>();
		List<Vector3> list2 = new List<Vector3>();
		List<int> list3 = new List<int>();
		Vector2[] array = new Vector2[num];
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i < num; i++)
		{
			float f = ((float)i / (float)(num - 1) * 2f - 1f) * 3.1415927f * 0.25f;
			array[i] = new Vector2(Mathf.Sin(f), Mathf.Cos(f)).normalized;
		}
		for (int j = 0; j < num3; j++)
		{
			float num6 = (float)j / (float)(num2 - 1);
			num6 = 1f - Mathf.Cos(num6 * 3.1415927f * 0.5f);
			for (int k = 0; k < num; k++)
			{
				Vector2 vector = array[k] * num6;
				if (j < num3 - 2 || !volume)
				{
					list2.Add(new Vector3(vector.x, 0f, vector.y));
				}
				else if (j == num3 - 2)
				{
					list2.Add(new Vector3(vector.x * 10f, -0.9f, vector.y) * 0.5f);
				}
				else
				{
					list2.Add(new Vector3(vector.x * 10f, -0.9f, vector.y * -10f) * 0.5f);
				}
				if (k != 0 && j != 0 && num4 > num)
				{
					list3.Add(num4);
					list3.Add(num4 - num);
					list3.Add(num4 - num - 1);
					list3.Add(num4 - 1);
				}
				num4++;
				if (num4 >= 65000)
				{
					list.Add(this.CreateMesh(string.Concat(new object[]
					{
						"WaterMesh_",
						num,
						"x",
						num2,
						"_",
						num5
					}), list2.ToArray(), list3.ToArray()));
					k--;
					j--;
					num6 = 1f - Mathf.Cos((float)j / (float)(num2 - 1) * 3.1415927f * 0.5f);
					num4 = 0;
					list2.Clear();
					list3.Clear();
					num5++;
				}
			}
		}
		if (num4 != 0)
		{
			list.Add(this.CreateMesh(string.Concat(new object[]
			{
				"WaterMesh_",
				num,
				"x",
				num2,
				"_",
				num5
			}), list2.ToArray(), list3.ToArray()));
		}
		return list.ToArray();
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x000AA7D0 File Offset: 0x000A89D0
	private Vector3 RaycastPlane(Camera camera, float planeHeight, Vector3 pos)
	{
		Ray ray = camera.ViewportPointToRay(pos);
		if (camera.transform.position.y > planeHeight)
		{
			if (ray.direction.y > -0.01f)
			{
				ray.direction = new Vector3(ray.direction.x, -ray.direction.y - 0.02f, ray.direction.z);
			}
		}
		else if (ray.direction.y < 0.01f)
		{
			ray.direction = new Vector3(ray.direction.x, -ray.direction.y + 0.02f, ray.direction.z);
		}
		float d = -(ray.origin.y - planeHeight) / ray.direction.y;
		return Quaternion.AngleAxis(-camera.transform.eulerAngles.y, Vector3.up) * (ray.direction * d);
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x000AA8DC File Offset: 0x000A8ADC
	public Matrix4x4 ComputeLocalToWorldMatrix(Camera camera, float oceanWaterLevel)
	{
		if (camera == null)
		{
			return Matrix4x4.identity;
		}
		Vector3 vector = camera.worldToCameraMatrix.MultiplyVector(Vector3.up);
		Vector3 vector2 = camera.worldToCameraMatrix.MultiplyVector(Vector3.Cross(camera.transform.forward, Vector3.up));
		vector = new Vector3(vector.x, vector.y, 0f).normalized * 0.5f + new Vector3(0.5f, 0f, 0.5f);
		vector2 = new Vector3(vector2.x, vector2.y, 0f).normalized * 0.5f;
		Vector3 vector3 = this.RaycastPlane(camera, oceanWaterLevel, vector - vector2);
		Vector3 vector4 = this.RaycastPlane(camera, oceanWaterLevel, vector + vector2);
		float num = Mathf.Min(camera.farClipPlane, 5000f);
		Vector3 vector5 = camera.transform.position;
		Vector3 vector6 = default(Vector3);
		vector6.x = num * Mathf.Tan(camera.fieldOfView * 0.5f * 0.017453292f) * camera.aspect + 2f;
		vector6.y = num;
		vector6.z = num;
		float num2 = Mathf.Abs(vector4.x - vector3.x);
		float num3 = Mathf.Min(vector3.z, vector4.z) - (num2 + 2f) * vector6.z / vector6.x;
		Vector3 forward = camera.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		vector6.z -= num3;
		vector5 = new Vector3(vector5.x, oceanWaterLevel, vector5.z) + forward * num3;
		Quaternion q = Quaternion.AngleAxis(Mathf.Atan2(forward.x, forward.z) * 57.29578f, Vector3.up);
		return Matrix4x4.TRS(vector5, q, vector6);
	}
}
