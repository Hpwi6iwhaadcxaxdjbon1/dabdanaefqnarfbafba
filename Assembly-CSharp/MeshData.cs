using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020001EA RID: 490
public class MeshData
{
	// Token: 0x04000C71 RID: 3185
	public List<int> triangles;

	// Token: 0x04000C72 RID: 3186
	public List<Vector3> vertices;

	// Token: 0x04000C73 RID: 3187
	public List<Vector3> normals;

	// Token: 0x04000C74 RID: 3188
	public List<Vector4> tangents;

	// Token: 0x04000C75 RID: 3189
	public List<Color32> colors32;

	// Token: 0x04000C76 RID: 3190
	public List<Vector2> uv;

	// Token: 0x04000C77 RID: 3191
	public List<Vector2> uv2;

	// Token: 0x04000C78 RID: 3192
	public List<Vector4> positions;

	// Token: 0x06000F83 RID: 3971 RVA: 0x0006A0C0 File Offset: 0x000682C0
	public void Alloc()
	{
		if (this.triangles == null)
		{
			this.triangles = Pool.GetList<int>();
		}
		if (this.vertices == null)
		{
			this.vertices = Pool.GetList<Vector3>();
		}
		if (this.normals == null)
		{
			this.normals = Pool.GetList<Vector3>();
		}
		if (this.tangents == null)
		{
			this.tangents = Pool.GetList<Vector4>();
		}
		if (this.colors32 == null)
		{
			this.colors32 = Pool.GetList<Color32>();
		}
		if (this.uv == null)
		{
			this.uv = Pool.GetList<Vector2>();
		}
		if (this.uv2 == null)
		{
			this.uv2 = Pool.GetList<Vector2>();
		}
		if (this.positions == null)
		{
			this.positions = Pool.GetList<Vector4>();
		}
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0006A168 File Offset: 0x00068368
	public void Free()
	{
		if (this.triangles != null)
		{
			Pool.FreeList<int>(ref this.triangles);
		}
		if (this.vertices != null)
		{
			Pool.FreeList<Vector3>(ref this.vertices);
		}
		if (this.normals != null)
		{
			Pool.FreeList<Vector3>(ref this.normals);
		}
		if (this.tangents != null)
		{
			Pool.FreeList<Vector4>(ref this.tangents);
		}
		if (this.colors32 != null)
		{
			Pool.FreeList<Color32>(ref this.colors32);
		}
		if (this.uv != null)
		{
			Pool.FreeList<Vector2>(ref this.uv);
		}
		if (this.uv2 != null)
		{
			Pool.FreeList<Vector2>(ref this.uv2);
		}
		if (this.positions != null)
		{
			Pool.FreeList<Vector4>(ref this.positions);
		}
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x0006A210 File Offset: 0x00068410
	public void Clear()
	{
		if (this.triangles != null)
		{
			this.triangles.Clear();
		}
		if (this.vertices != null)
		{
			this.vertices.Clear();
		}
		if (this.normals != null)
		{
			this.normals.Clear();
		}
		if (this.tangents != null)
		{
			this.tangents.Clear();
		}
		if (this.colors32 != null)
		{
			this.colors32.Clear();
		}
		if (this.uv != null)
		{
			this.uv.Clear();
		}
		if (this.uv2 != null)
		{
			this.uv2.Clear();
		}
		if (this.positions != null)
		{
			this.positions.Clear();
		}
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0006A2B8 File Offset: 0x000684B8
	public void Apply(UnityEngine.Mesh mesh)
	{
		mesh.Clear();
		if (this.vertices != null)
		{
			mesh.SetVertices(this.vertices);
		}
		if (this.triangles != null)
		{
			mesh.SetTriangles(this.triangles, 0);
		}
		if (this.normals != null)
		{
			if (this.normals.Count == this.vertices.Count)
			{
				mesh.SetNormals(this.normals);
			}
			else if (this.normals.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping mesh normals because some meshes were missing them.");
			}
		}
		if (this.tangents != null)
		{
			if (this.tangents.Count == this.vertices.Count)
			{
				mesh.SetTangents(this.tangents);
			}
			else if (this.tangents.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping mesh tangents because some meshes were missing them.");
			}
		}
		if (this.colors32 != null)
		{
			if (this.colors32.Count == this.vertices.Count)
			{
				mesh.SetColors(this.colors32);
			}
			else if (this.colors32.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping mesh colors because some meshes were missing them.");
			}
		}
		if (this.uv != null)
		{
			if (this.uv.Count == this.vertices.Count)
			{
				mesh.SetUVs(0, this.uv);
			}
			else if (this.uv.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping mesh uvs because some meshes were missing them.");
			}
		}
		if (this.uv2 != null)
		{
			if (this.uv2.Count == this.vertices.Count)
			{
				mesh.SetUVs(1, this.uv2);
			}
			else if (this.uv2.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping mesh uv2s because some meshes were missing them.");
			}
		}
		if (this.positions != null)
		{
			mesh.SetUVs(2, this.positions);
		}
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x0006A494 File Offset: 0x00068694
	public void Combine(MeshGroup meshGroup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshInstance meshInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshInstance.position, meshInstance.rotation, meshInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshInstance.data.vertices[k]));
				this.positions.Add(meshInstance.position);
			}
			for (int l = 0; l < meshInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshInstance.data.normals[l]));
			}
			for (int m = 0; m < meshInstance.data.tangents.Length; m++)
			{
				Vector4 vector = meshInstance.data.tangents[m];
				Vector3 vector2 = new Vector3(vector.x, vector.y, vector.z);
				Vector3 vector3 = matrix4x.MultiplyVector(vector2);
				this.tangents.Add(new Vector4(vector3.x, vector3.y, vector3.z, vector.w));
			}
			for (int n = 0; n < meshInstance.data.colors32.Length; n++)
			{
				this.colors32.Add(meshInstance.data.colors32[n]);
			}
			for (int num = 0; num < meshInstance.data.uv.Length; num++)
			{
				this.uv.Add(meshInstance.data.uv[num]);
			}
			for (int num2 = 0; num2 < meshInstance.data.uv2.Length; num2++)
			{
				this.uv2.Add(meshInstance.data.uv2[num2]);
			}
		}
	}
}
