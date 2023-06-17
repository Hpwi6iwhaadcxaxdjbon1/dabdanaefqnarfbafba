using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001E2 RID: 482
public static class MeshCache
{
	// Token: 0x04000C53 RID: 3155
	public static Dictionary<Mesh, MeshCache.Data> dictionary = new Dictionary<Mesh, MeshCache.Data>();

	// Token: 0x06000F6D RID: 3949 RVA: 0x00069CD8 File Offset: 0x00067ED8
	public static MeshCache.Data Get(Mesh mesh)
	{
		MeshCache.Data data;
		if (!MeshCache.dictionary.TryGetValue(mesh, ref data))
		{
			data = new MeshCache.Data();
			data.mesh = mesh;
			data.vertices = mesh.vertices;
			data.normals = mesh.normals;
			data.tangents = mesh.tangents;
			data.colors32 = mesh.colors32;
			data.triangles = mesh.triangles;
			data.uv = mesh.uv;
			data.uv2 = mesh.uv2;
			data.uv3 = mesh.uv3;
			data.uv4 = mesh.uv4;
			MeshCache.dictionary.Add(mesh, data);
		}
		return data;
	}

	// Token: 0x020001E3 RID: 483
	[Serializable]
	public class Data
	{
		// Token: 0x04000C54 RID: 3156
		public Mesh mesh;

		// Token: 0x04000C55 RID: 3157
		public Vector3[] vertices;

		// Token: 0x04000C56 RID: 3158
		public Vector3[] normals;

		// Token: 0x04000C57 RID: 3159
		public Vector4[] tangents;

		// Token: 0x04000C58 RID: 3160
		public Color32[] colors32;

		// Token: 0x04000C59 RID: 3161
		public int[] triangles;

		// Token: 0x04000C5A RID: 3162
		public Vector2[] uv;

		// Token: 0x04000C5B RID: 3163
		public Vector2[] uv2;

		// Token: 0x04000C5C RID: 3164
		public Vector2[] uv3;

		// Token: 0x04000C5D RID: 3165
		public Vector2[] uv4;
	}
}
