using System;
using UnityEngine;

// Token: 0x020007C0 RID: 1984
[Serializable]
public class MeshHolder
{
	// Token: 0x040026F6 RID: 9974
	[HideInInspector]
	public Vector3[] _vertices;

	// Token: 0x040026F7 RID: 9975
	[HideInInspector]
	public Vector3[] _normals;

	// Token: 0x040026F8 RID: 9976
	[HideInInspector]
	public int[] _triangles;

	// Token: 0x040026F9 RID: 9977
	[HideInInspector]
	public trisPerSubmesh[] _TrianglesOfSubs;

	// Token: 0x040026FA RID: 9978
	[HideInInspector]
	public Matrix4x4[] _bindPoses;

	// Token: 0x040026FB RID: 9979
	[HideInInspector]
	public BoneWeight[] _boneWeights;

	// Token: 0x040026FC RID: 9980
	[HideInInspector]
	public Bounds _bounds;

	// Token: 0x040026FD RID: 9981
	[HideInInspector]
	public int _subMeshCount;

	// Token: 0x040026FE RID: 9982
	[HideInInspector]
	public Vector4[] _tangents;

	// Token: 0x040026FF RID: 9983
	[HideInInspector]
	public Vector2[] _uv;

	// Token: 0x04002700 RID: 9984
	[HideInInspector]
	public Vector2[] _uv2;

	// Token: 0x04002701 RID: 9985
	[HideInInspector]
	public Vector2[] _uv3;

	// Token: 0x04002702 RID: 9986
	[HideInInspector]
	public Color[] _colors;

	// Token: 0x04002703 RID: 9987
	[HideInInspector]
	public Vector2[] _uv4;

	// Token: 0x06002B4B RID: 11083 RVA: 0x00021995 File Offset: 0x0001FB95
	public void setAnimationData(Mesh mesh)
	{
		this._colors = mesh.colors;
	}
}
