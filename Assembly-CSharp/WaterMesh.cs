using System;
using UnityEngine;

// Token: 0x02000560 RID: 1376
[Serializable]
public class WaterMesh
{
	// Token: 0x04001B55 RID: 6997
	private Mesh borderMesh;

	// Token: 0x04001B56 RID: 6998
	private Mesh centerPatch;

	// Token: 0x04001B57 RID: 6999
	private int borderRingCount;

	// Token: 0x04001B58 RID: 7000
	private float borderRingSpacingFalloff;

	// Token: 0x04001B59 RID: 7001
	private int resolution;

	// Token: 0x04001B5A RID: 7002
	private Vector3[] borderVerticesLocal;

	// Token: 0x04001B5B RID: 7003
	private Vector3[] borderVerticesWorld;

	// Token: 0x04001B5C RID: 7004
	private bool initialized;

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x06001F1F RID: 7967 RVA: 0x00018958 File Offset: 0x00016B58
	public Mesh BorderMesh
	{
		get
		{
			return this.borderMesh;
		}
	}

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x06001F20 RID: 7968 RVA: 0x00018960 File Offset: 0x00016B60
	public Mesh CenterPatch
	{
		get
		{
			return this.centerPatch;
		}
	}

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x06001F21 RID: 7969 RVA: 0x00018968 File Offset: 0x00016B68
	public bool IsInitialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x000A9978 File Offset: 0x000A7B78
	public void Initialize(int patchResolution, float patchSizeInWorld, int borderRingCount, float borderRingSpacingFalloff)
	{
		if (!Mathf.IsPowerOfTwo(patchResolution))
		{
			Debug.LogError("[Water] Patch resolution must be a power-of-two number.");
			return;
		}
		this.borderRingCount = borderRingCount;
		this.borderRingSpacingFalloff = borderRingSpacingFalloff;
		this.borderMesh = this.CreateSortedBorderPatch(patchResolution, borderRingCount, patchSizeInWorld);
		this.centerPatch = this.CreateSortedCenterPatch(patchResolution, patchSizeInWorld, false);
		this.resolution = patchResolution;
		this.borderVerticesLocal = new Vector3[this.borderMesh.vertexCount];
		this.borderVerticesWorld = new Vector3[this.borderMesh.vertexCount];
		Array.Copy(this.borderMesh.vertices, this.borderVerticesLocal, this.borderMesh.vertexCount);
		this.initialized = true;
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x00018970 File Offset: 0x00016B70
	public void Destroy()
	{
		if (this.initialized)
		{
			Object.DestroyImmediate(this.borderMesh);
			Object.DestroyImmediate(this.centerPatch);
			this.initialized = false;
		}
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x000A9A20 File Offset: 0x000A7C20
	public void UpdateBorderMesh(Matrix4x4 centerLocalToWorld, Matrix4x4 borderLocalToWorld, bool collapseCenter)
	{
		int num = this.resolution * 4;
		int num2 = 0;
		int num3 = num;
		int num4 = this.borderMesh.vertexCount - num;
		int vertexCount = this.borderMesh.vertexCount;
		Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		Bounds bounds = default(Bounds);
		for (int i = num2; i < num3; i++)
		{
			Vector3 vector3 = borderLocalToWorld.MultiplyPoint3x4(this.borderVerticesLocal[i]);
			vector = Vector3.Min(vector, vector3);
			vector2 = Vector3.Max(vector2, vector3);
			this.borderVerticesWorld[i] = vector3;
		}
		bounds.SetMinMax(vector, vector2);
		if (!collapseCenter)
		{
			for (int j = num4; j < vertexCount; j++)
			{
				this.borderVerticesWorld[j] = centerLocalToWorld.MultiplyPoint3x4(this.borderVerticesLocal[j]);
			}
		}
		else
		{
			for (int k = num4; k < vertexCount; k++)
			{
				this.borderVerticesWorld[k] = centerLocalToWorld.MultiplyPoint3x4(Vector3.zero);
			}
		}
		int l = 1;
		int num5 = num3;
		while (l < this.borderRingCount)
		{
			float num6 = (float)l / (float)this.borderRingCount;
			num6 = Mathf.Clamp01(Mathf.Pow(num6, this.borderRingSpacingFalloff));
			int m = 0;
			while (m < num)
			{
				Vector3 vector4 = this.borderVerticesWorld[num2 + m];
				Vector3 vector5 = this.borderVerticesWorld[num4 + m];
				this.borderVerticesWorld[num5].x = vector4.x + (vector5.x - vector4.x) * num6;
				this.borderVerticesWorld[num5].y = vector4.y + (vector5.y - vector4.y) * num6;
				this.borderVerticesWorld[num5].z = vector4.z + (vector5.z - vector4.z) * num6;
				m++;
				num5++;
			}
			l++;
		}
		this.borderMesh.vertices = this.borderVerticesWorld;
		this.borderMesh.bounds = bounds;
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x000A9C60 File Offset: 0x000A7E60
	private Mesh CreateSortedBorderPatch(int resolution, int ringCount, float sizeInWorld)
	{
		float num = sizeInWorld / (float)resolution;
		int num2 = resolution * 4 * (ringCount + 1);
		int num3 = resolution * 4 * ringCount * 6;
		Vector3[] array = new Vector3[num2];
		Vector3[] array2 = new Vector3[num2];
		Color[] array3 = new Color[num2];
		int[] array4 = new int[num3];
		for (int i = 0; i < num2; i++)
		{
			array2[i] = Vector3.up;
		}
		for (int j = 0; j < num2; j++)
		{
			array3[j] = Color.clear;
		}
		int num4 = resolution * 4;
		float num5 = (float)resolution * num;
		Vector3 a = new Vector3(sizeInWorld, 0f, sizeInWorld) * 0.5f;
		int k = 0;
		int num6 = 0;
		while (k < ringCount + 1)
		{
			Vector3 a2 = -a;
			for (int l = 0; l < resolution; l++)
			{
				array[num6++] = a2 + new Vector3((float)l * num, 0f, 0f);
			}
			for (int m = 0; m < resolution; m++)
			{
				array[num6++] = a2 + new Vector3(num5, 0f, (float)m * num);
			}
			for (int n = resolution; n > 0; n--)
			{
				array[num6++] = a2 + new Vector3((float)n * num, 0f, num5);
			}
			for (int num7 = resolution; num7 > 0; num7--)
			{
				array[num6++] = a2 + new Vector3(0f, 0f, (float)num7 * num);
			}
			k++;
		}
		int num8 = 0;
		int num9 = 0;
		while (num8 < ringCount)
		{
			int num10 = num8 * num4;
			int num11 = num10 + num4;
			int num12 = num11;
			int num13 = num10 + num4 * 2;
			int num14 = num10;
			int num15 = num10 + num4 + 1;
			int num16 = num10 + num4;
			int num17 = num10 + 1;
			int num18 = num10 + num4 + 1;
			int num19 = num10;
			for (int num20 = 0; num20 < num4; num20++)
			{
				bool flag = num20 % resolution == 0;
				int num21 = num14;
				int num22 = flag ? (num15 - num4) : num15;
				int num23 = num16;
				int num24 = num17;
				int num25 = num18;
				int num26 = flag ? (num19 + num4) : num19;
				if (num22 >= num13)
				{
					num22 = num12;
				}
				if (num24 >= num11)
				{
					num24 = num10;
				}
				if (num25 >= num13)
				{
					num25 = num12;
				}
				array4[num9++] = num23;
				array4[num9++] = num22;
				array4[num9++] = num21;
				array4[num9++] = num26;
				array4[num9++] = num25;
				array4[num9++] = num24;
				num14++;
				num15++;
				num16++;
				num17++;
				num18++;
				num19++;
			}
			num8++;
		}
		Mesh mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.colors = array3;
		mesh.triangles = array4;
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x06001F26 RID: 7974 RVA: 0x000A9F64 File Offset: 0x000A8164
	private Mesh CreateSortedCenterPatch(int resolution, float sizeInWorld, bool borderOnly)
	{
		float num = sizeInWorld / (float)resolution;
		int num2 = resolution + 1;
		int num3;
		int num4;
		if (borderOnly)
		{
			num3 = resolution * 8 - 8;
			num4 = (resolution - 1) * 24;
		}
		else
		{
			num3 = num2 * num2;
			num4 = resolution * resolution * 6;
		}
		Vector3[] array = new Vector3[num3];
		Vector3[] array2 = new Vector3[num3];
		Color[] array3 = new Color[num3];
		int[] array4 = new int[num4];
		for (int i = 0; i < num3; i++)
		{
			array2[i] = Vector3.up;
		}
		int num5 = resolution / 2;
		int num6 = num5 - 1;
		int num7 = resolution;
		int num8 = resolution * 4;
		Vector3 b = new Vector3(sizeInWorld, 0f, sizeInWorld) * 0.5f;
		if (borderOnly)
		{
			for (int j = 0; j < num3; j++)
			{
				array3[j] = Color.clear;
			}
		}
		else
		{
			for (int k = 0; k < num3; k++)
			{
				if (k >= num8)
				{
					array3[k] = Color.white;
				}
				else
				{
					array3[k] = Color.clear;
				}
			}
		}
		int l = 0;
		int num9 = 0;
		while (l < num5 + 1)
		{
			Vector3 vector;
			vector.x = (float)l * num;
			vector.y = 0f;
			vector.z = vector.x;
			vector -= b;
			float num10 = (float)num7 * num;
			if (l <= num6)
			{
				for (int m = 0; m < num7; m++)
				{
					array[num9++] = vector + new Vector3((float)m * num, 0f, 0f);
				}
				for (int n = 0; n < num7; n++)
				{
					array[num9++] = vector + new Vector3(num10, 0f, (float)n * num);
				}
				for (int num11 = num7; num11 > 0; num11--)
				{
					array[num9++] = vector + new Vector3((float)num11 * num, 0f, num10);
				}
				for (int num12 = num7; num12 > 0; num12--)
				{
					array[num9++] = vector + new Vector3(0f, 0f, (float)num12 * num);
				}
			}
			else
			{
				array[num9++] = vector;
			}
			num7 -= 2;
			if (borderOnly && l >= 1)
			{
				break;
			}
			l++;
		}
		int num13 = resolution;
		int num14 = resolution - 2;
		int num15 = resolution * 4;
		int num16 = num15 - 8;
		int num17 = (resolution - 1) * 4;
		int num18 = num17 - 8;
		int num19 = 0;
		int num20 = num15;
		int num21 = 0;
		int num22 = 0;
		while (num21 < num5)
		{
			if (num21 < num6)
			{
				int num23 = num20;
				int num24 = num20 - 1;
				int num25 = num19;
				int num26 = 0;
				bool flag = true;
				for (int num27 = 0; num27 < num17; num27++)
				{
					int num28 = num23;
					int num29 = num24;
					int num30 = num25;
					num24 = num25;
					num25++;
					int num31 = num23;
					int num32 = num24;
					int num33 = num25;
					bool flag2 = (num26 & 1) == 0;
					if (flag2 || (borderOnly && flag && !flag2))
					{
						array4[num22++] = num33;
						array4[num22++] = num32;
						array4[num22++] = num31;
						array4[num22++] = num30;
						array4[num22++] = num29;
						array4[num22++] = num28;
					}
					else
					{
						array4[num22++] = num30;
						array4[num22++] = num29;
						array4[num22++] = num33;
						array4[num22++] = num33;
						array4[num22++] = num29;
						array4[num22++] = num28;
					}
					flag = ((num27 + 1) % (num13 - 1) == 0);
					if (flag)
					{
						num24++;
						num25++;
						num26++;
					}
					else
					{
						num24 = num23;
						num23 = ((num23 + 1 < num20 + num16) ? (num23 + 1) : num20);
					}
				}
				num17 -= 8;
				num18 -= 8;
				num15 -= 8;
				num16 -= 8;
				num13 -= 2;
				num14 -= 2;
				num19 = num20;
				num20 += num15;
			}
			else
			{
				int num34 = num20;
				int num35 = num20 - 1;
				int num36 = num19;
				int num37 = 0;
				for (int num38 = 0; num38 < num17; num38++)
				{
					int num39 = num34;
					int num40 = num35;
					int num41 = num36;
					num35 = num36;
					num36++;
					int num42 = num34;
					int num43 = num35;
					int num44 = num36;
					num35++;
					num36++;
					if ((num37 & 1) == 0)
					{
						array4[num22++] = num44;
						array4[num22++] = num43;
						array4[num22++] = num42;
						array4[num22++] = num41;
						array4[num22++] = num40;
						array4[num22++] = num39;
					}
					else
					{
						array4[num22++] = num41;
						array4[num22++] = num40;
						array4[num22++] = num44;
						array4[num22++] = num44;
						array4[num22++] = num40;
						array4[num22++] = num39;
					}
					num37++;
				}
			}
			if (borderOnly)
			{
				break;
			}
			num21++;
		}
		Mesh mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.colors = array3;
		mesh.triangles = array4;
		mesh.RecalculateBounds();
		return mesh;
	}
}
