using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000575 RID: 1397
[ExecuteInEditMode]
public class AdaptMeshToTerrain : MonoBehaviour
{
	// Token: 0x04001BEE RID: 7150
	public LayerMask LayerMask = -1;

	// Token: 0x04001BEF RID: 7151
	public float RayHeight = 10f;

	// Token: 0x04001BF0 RID: 7152
	public float RayMaxDistance = 20f;

	// Token: 0x04001BF1 RID: 7153
	public float MinDisplacement = 0.01f;

	// Token: 0x04001BF2 RID: 7154
	public float MaxDisplacement = 0.33f;

	// Token: 0x04001BF3 RID: 7155
	[Range(8f, 64f)]
	public int PlaneResolution = 24;

	// Token: 0x04001BF4 RID: 7156
	private const int LODCount = 3;

	// Token: 0x04001BF5 RID: 7157
	private MeshFilter meshFilter;

	// Token: 0x04001BF6 RID: 7158
	private MeshRenderer meshRenderer;

	// Token: 0x04001BF7 RID: 7159
	private MeshCollider meshCollider;

	// Token: 0x04001BF8 RID: 7160
	private MeshLOD meshLOD;

	// Token: 0x04001BF9 RID: 7161
	private Mesh[] meshes = new Mesh[3];

	// Token: 0x04001BFA RID: 7162
	private Mesh colliderMesh;

	// Token: 0x04001BFB RID: 7163
	private static Dictionary<int, Mesh> referenceMeshes = new Dictionary<int, Mesh>();

	// Token: 0x04001BFC RID: 7164
	private MaterialPropertyBlock block;

	// Token: 0x06001FE6 RID: 8166 RVA: 0x0001943E File Offset: 0x0001763E
	private void CheckInitialize()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.meshLOD = base.GetComponent<MeshLOD>();
		this.CheckReferenceMeshes();
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x0001946A File Offset: 0x0001766A
	private void Awake()
	{
		this.CheckInitialize();
		this.CheckReferenceMeshes();
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x00019478 File Offset: 0x00017678
	private void OnEnable()
	{
		this.CheckInitialize();
		this.CheckReferenceMeshes();
		this.Adapt();
		this.SetMaterialPropertyBlock();
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x000ADE44 File Offset: 0x000AC044
	private void OnDisable()
	{
		this.DestroyMeshes();
		Mesh sharedMesh;
		if (AdaptMeshToTerrain.referenceMeshes.TryGetValue(this.PlaneResolution, ref sharedMesh))
		{
			if (this.meshFilter != null)
			{
				this.meshFilter.sharedMesh = sharedMesh;
			}
			if (this.meshCollider != null)
			{
				this.meshCollider.sharedMesh = sharedMesh;
			}
		}
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x000ADEA0 File Offset: 0x000AC0A0
	private void SetMaterialPropertyBlock()
	{
		this.block = ((this.block != null) ? this.block : new MaterialPropertyBlock());
		this.meshRenderer = ((this.meshRenderer != null) ? this.meshRenderer : base.GetComponent<MeshRenderer>());
		this.block.SetFloat("_SnowArea_MinDisplacement", this.MinDisplacement);
		this.block.SetFloat("_SnowArea_MaxDisplacement", this.MaxDisplacement);
		this.meshRenderer.SetPropertyBlock(this.block);
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x000ADF28 File Offset: 0x000AC128
	private void DestroyMeshes()
	{
		for (int i = 0; i < 3; i++)
		{
			if (this.meshes[i] != null)
			{
				Object.DestroyImmediate(this.meshes[i]);
				this.meshes[i] = null;
			}
		}
		if (this.colliderMesh != null)
		{
			Object.DestroyImmediate(this.colliderMesh);
			this.colliderMesh = null;
		}
	}

	// Token: 0x06001FEC RID: 8172 RVA: 0x000ADF88 File Offset: 0x000AC188
	private void CheckReferenceMeshes()
	{
		for (int i = 0; i < 3; i++)
		{
			int num = this.PlaneResolution >> i;
			Mesh x;
			if (!AdaptMeshToTerrain.referenceMeshes.TryGetValue(num, ref x) || x == null)
			{
				AdaptMeshToTerrain.referenceMeshes[num] = this.CreatePlaneMesh(num);
			}
		}
		if (this.meshFilter != null && this.meshFilter.sharedMesh == null)
		{
			this.meshFilter.sharedMesh = AdaptMeshToTerrain.referenceMeshes[this.PlaneResolution];
		}
		if (this.meshCollider != null && this.meshCollider.sharedMesh == null)
		{
			this.meshCollider.sharedMesh = AdaptMeshToTerrain.referenceMeshes[this.PlaneResolution];
		}
		if (this.meshLOD != null)
		{
			if (this.meshLOD.States == null || this.meshLOD.States.Length != 3)
			{
				this.meshLOD.States = new MeshLOD.State[3];
			}
			for (int j = 0; j < 3; j++)
			{
				if (this.meshLOD.States[j] == null)
				{
					this.meshLOD.States[j] = new MeshLOD.State();
				}
				if (this.meshLOD.States[j].mesh == null)
				{
					int num2 = this.PlaneResolution >> j;
					this.meshLOD.States[j].mesh = AdaptMeshToTerrain.referenceMeshes[num2];
					if (j > 0 && this.meshLOD.States[j].distance == 0f)
					{
						this.meshLOD.States[j].distance = (float)(j * 50);
					}
				}
			}
		}
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x000AE13C File Offset: 0x000AC33C
	public void Adapt()
	{
		this.CheckReferenceMeshes();
		if (this.meshFilter != null && AdaptMeshToTerrain.referenceMeshes != null)
		{
			for (int i = 0; i < 3; i++)
			{
				int num = this.PlaneResolution >> i;
				Mesh mesh = AdaptMeshToTerrain.referenceMeshes[num];
				int[] triangles = mesh.triangles;
				Vector3[] vertices = mesh.vertices;
				Vector2[] uv = mesh.uv;
				Vector3[] normals = mesh.normals;
				Color[] array = new Color[mesh.vertexCount];
				Vector3[] array2 = (i == 1) ? new Vector3[mesh.vertexCount] : null;
				Vector4[] array3 = new Vector4[mesh.vertexCount];
				Bounds bounds = mesh.bounds;
				float num2 = Mathf.Min(bounds.extents.x, bounds.extents.z);
				for (int j = 0; j < mesh.vertexCount; j++)
				{
					Vector3 vector = vertices[j];
					Vector3 vector2 = normals[j];
					Vector3 vector3 = -base.transform.TransformDirection(vector2);
					Vector3 vector4 = base.transform.TransformPoint(vector) - vector3 * this.RayHeight;
					bool flag = false;
					RaycastHit[] array4 = Physics.RaycastAll(vector4, vector3, this.RayMaxDistance, this.LayerMask);
					if (array4.Length != 0)
					{
						foreach (RaycastHit raycastHit in array4)
						{
							if (raycastHit.collider != this.meshCollider)
							{
								vector = raycastHit.point;
								vector2 = raycastHit.normal;
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						Vector3 vector5 = base.transform.InverseTransformPoint(vector);
						Vector3 vector6 = base.transform.InverseTransformDirection(vector2);
						float num3 = Vector3Ex.MagnitudeXZ(vector5);
						float num4 = Mathf.Clamp01(num3 / num2);
						float num5 = Mathf.Clamp01(num3 / (num2 - 0.1f));
						num4 = num4 * num4 * num4;
						num5 = 1f - num5 * num5 * num4;
						array3[j] = new Vector4(vector.x, vector.y, vector.z, 1f);
						vertices[j] = vector5;
						normals[j] = vector6;
						array[j] = new Color(num5, num5, num5, num4);
						if (array2 != null)
						{
							array2[j] = base.transform.InverseTransformPoint(vector + new Vector3(0f, this.MinDisplacement, 0f));
						}
					}
					else
					{
						array3[j] = Vector4.zero;
						vertices[j] = vector;
						normals[j] = vector2;
						array[j] = new Color(1f, 1f, 1f, 0f);
						if (array2 != null)
						{
							array2[j] = vector + new Vector3(0f, this.MinDisplacement, 0f);
						}
					}
				}
				List<int> list = new List<int>();
				List<Vector3> list2 = new List<Vector3>();
				List<Vector3> list3 = new List<Vector3>();
				List<Vector2> list4 = new List<Vector2>();
				List<Color> list5 = new List<Color>();
				List<Vector3> list6 = (array2 != null) ? new List<Vector3>(array2.Length) : null;
				int num6 = triangles.Length / 3;
				int num7 = 0;
				for (int l = 0; l < num6; l++)
				{
					int num8 = triangles[l * 3];
					int num9 = triangles[l * 3 + 1];
					int num10 = triangles[l * 3 + 2];
					Vector4 vector7 = array3[num8];
					Vector4 vector8 = array3[num9];
					Vector4 vector9 = array3[num10];
					bool flag2 = vector7.w > 0f;
					bool flag3 = vector8.w > 0f;
					bool flag4 = vector9.w > 0f;
					bool flag5 = TerrainMeta.AlphaMap.GetAlpha(vector7) > 0f;
					bool flag6 = TerrainMeta.AlphaMap.GetAlpha(vector8) > 0f;
					bool flag7 = TerrainMeta.AlphaMap.GetAlpha(vector9) > 0f;
					if (flag5 && flag6 && flag7 && flag2 && flag3 && flag4)
					{
						list2.Add(vertices[num8]);
						list2.Add(vertices[num9]);
						list2.Add(vertices[num10]);
						list3.Add(normals[num8]);
						list3.Add(normals[num9]);
						list3.Add(normals[num10]);
						list5.Add(array[num8]);
						list5.Add(array[num9]);
						list5.Add(array[num10]);
						list4.Add(uv[num8]);
						list4.Add(uv[num9]);
						list4.Add(uv[num10]);
						list.Add(num7++);
						list.Add(num7++);
						list.Add(num7++);
						if (list6 != null)
						{
							list6.Add(array2[num8]);
							list6.Add(array2[num9]);
							list6.Add(array2[num10]);
						}
					}
				}
				Mesh mesh2 = this.meshes[i];
				if (mesh2 == null)
				{
					Object.DestroyImmediate(mesh2);
					mesh2 = new Mesh();
					this.meshes[i] = mesh2;
				}
				mesh2.SetVertices(list2);
				mesh2.SetNormals(list3);
				mesh2.SetColors(list5);
				mesh2.SetUVs(0, list4);
				mesh2.SetTriangles(list, 0);
				mesh2.RecalculateTangents();
				mesh2.RecalculateBounds();
				if (i == 0)
				{
					this.meshFilter.sharedMesh = mesh2;
				}
				if (this.meshLOD != null)
				{
					this.meshLOD.States[i].mesh = mesh2;
				}
				if (this.meshCollider != null && list6 != null)
				{
					if (this.colliderMesh == null || this.colliderMesh.vertexCount != list6.Count)
					{
						Object.DestroyImmediate(this.colliderMesh);
						this.colliderMesh = new Mesh();
						this.colliderMesh.SetVertices(list6);
						this.colliderMesh.SetTriangles(list, 0);
						this.colliderMesh.RecalculateBounds();
					}
					this.meshCollider.sharedMesh = this.colliderMesh;
				}
			}
		}
		if (Application.isPlaying)
		{
			this.meshLOD.SetVisible(true);
		}
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x000AE7A8 File Offset: 0x000AC9A8
	private Mesh CreatePlaneMesh(int resolution)
	{
		Vector3[] array = new Vector3[(resolution + 1) * (resolution + 1)];
		Vector2[] array2 = new Vector2[array.Length];
		Vector3[] array3 = new Vector3[array.Length];
		Vector4[] array4 = new Vector4[array.Length];
		int[] array5 = new int[resolution * resolution * 6];
		Vector3 vector = new Vector3(0f, 1f, 0f);
		Vector4 vector2 = new Vector4(1f, 0f, 0f, -1f);
		int num = 0;
		for (int i = 0; i <= resolution; i++)
		{
			int j = 0;
			while (j <= resolution)
			{
				float x = (float)j / (float)resolution;
				float num2 = (float)i / (float)resolution;
				array[num] = new Vector3(x, 0f, num2) - new Vector3(0.5f, 0f, 0.5f);
				array2[num] = new Vector2(x, num2);
				array4[num] = vector2;
				array3[num] = vector;
				j++;
				num++;
			}
		}
		int num3 = 0;
		int num4 = 0;
		int k = 0;
		while (k < resolution)
		{
			int l = 0;
			while (l < resolution)
			{
				array5[num3] = num4;
				array5[num3 + 3] = (array5[num3 + 2] = num4 + 1);
				array5[num3 + 4] = (array5[num3 + 1] = num4 + resolution + 1);
				array5[num3 + 5] = num4 + resolution + 2;
				l++;
				num3 += 6;
				num4++;
			}
			k++;
			num4++;
		}
		Mesh mesh = new Mesh();
		mesh.name = "CustomPlane" + resolution;
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.normals = array3;
		mesh.tangents = array4;
		mesh.triangles = array5;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		return mesh;
	}
}
