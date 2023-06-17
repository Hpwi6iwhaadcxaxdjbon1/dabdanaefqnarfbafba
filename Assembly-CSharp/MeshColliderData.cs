using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class MeshColliderData
{
	// Token: 0x04000C5E RID: 3166
	public List<int> triangles;

	// Token: 0x04000C5F RID: 3167
	public List<Vector3> vertices;

	// Token: 0x04000C60 RID: 3168
	public List<Vector3> normals;

	// Token: 0x06000F70 RID: 3952 RVA: 0x0000DBF0 File Offset: 0x0000BDF0
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
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0000DC2B File Offset: 0x0000BE2B
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
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x0000DC66 File Offset: 0x0000BE66
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
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x00069D80 File Offset: 0x00067F80
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
				return;
			}
			if (this.normals.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping collider normals because some meshes were missing them.");
			}
		}
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x00069E0C File Offset: 0x0006800C
	public void Combine(MeshColliderGroup meshGroup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshColliderInstance meshColliderInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshColliderInstance.position, meshColliderInstance.rotation, meshColliderInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshColliderInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshColliderInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshColliderInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshColliderInstance.data.vertices[k]));
			}
			for (int l = 0; l < meshColliderInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshColliderInstance.data.normals[l]));
			}
		}
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x00069F20 File Offset: 0x00068120
	public void Combine(MeshColliderGroup meshGroup, MeshColliderLookup colliderLookup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshColliderInstance meshColliderInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshColliderInstance.position, meshColliderInstance.rotation, meshColliderInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshColliderInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshColliderInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshColliderInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshColliderInstance.data.vertices[k]));
			}
			for (int l = 0; l < meshColliderInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshColliderInstance.data.normals[l]));
			}
			colliderLookup.Add(meshColliderInstance);
		}
	}
}
