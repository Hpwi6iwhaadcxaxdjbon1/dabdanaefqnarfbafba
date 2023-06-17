using System;
using UnityEngine;

// Token: 0x020007C3 RID: 1987
[ExecuteInEditMode]
public class VertexColorStream : MonoBehaviour
{
	// Token: 0x0400270A RID: 9994
	[HideInInspector]
	public Mesh originalMesh;

	// Token: 0x0400270B RID: 9995
	[HideInInspector]
	public Mesh paintedMesh;

	// Token: 0x0400270C RID: 9996
	[HideInInspector]
	public MeshHolder meshHold;

	// Token: 0x0400270D RID: 9997
	[HideInInspector]
	public Vector3[] _vertices;

	// Token: 0x0400270E RID: 9998
	[HideInInspector]
	public Vector3[] _normals;

	// Token: 0x0400270F RID: 9999
	[HideInInspector]
	public int[] _triangles;

	// Token: 0x04002710 RID: 10000
	[HideInInspector]
	public int[][] _Subtriangles;

	// Token: 0x04002711 RID: 10001
	[HideInInspector]
	public Matrix4x4[] _bindPoses;

	// Token: 0x04002712 RID: 10002
	[HideInInspector]
	public BoneWeight[] _boneWeights;

	// Token: 0x04002713 RID: 10003
	[HideInInspector]
	public Bounds _bounds;

	// Token: 0x04002714 RID: 10004
	[HideInInspector]
	public int _subMeshCount;

	// Token: 0x04002715 RID: 10005
	[HideInInspector]
	public Vector4[] _tangents;

	// Token: 0x04002716 RID: 10006
	[HideInInspector]
	public Vector2[] _uv;

	// Token: 0x04002717 RID: 10007
	[HideInInspector]
	public Vector2[] _uv2;

	// Token: 0x04002718 RID: 10008
	[HideInInspector]
	public Vector2[] _uv3;

	// Token: 0x04002719 RID: 10009
	[HideInInspector]
	public Color[] _colors;

	// Token: 0x0400271A RID: 10010
	[HideInInspector]
	public Vector2[] _uv4;

	// Token: 0x06002B56 RID: 11094 RVA: 0x00002ECE File Offset: 0x000010CE
	private void OnDidApplyAnimationProperties()
	{
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x000DDFD0 File Offset: 0x000DC1D0
	public void init(Mesh origMesh, bool destroyOld)
	{
		this.originalMesh = origMesh;
		this.paintedMesh = Object.Instantiate<Mesh>(origMesh);
		if (destroyOld)
		{
			Object.DestroyImmediate(origMesh);
		}
		this.paintedMesh.hideFlags = HideFlags.None;
		this.paintedMesh.name = "vpp_" + base.gameObject.name;
		this.meshHold = new MeshHolder();
		this.meshHold._vertices = this.paintedMesh.vertices;
		this.meshHold._normals = this.paintedMesh.normals;
		this.meshHold._triangles = this.paintedMesh.triangles;
		this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
		for (int i = 0; i < this.paintedMesh.subMeshCount; i++)
		{
			this.meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
			this.meshHold._TrianglesOfSubs[i].triangles = this.paintedMesh.GetTriangles(i);
		}
		this.meshHold._bindPoses = this.paintedMesh.bindposes;
		this.meshHold._boneWeights = this.paintedMesh.boneWeights;
		this.meshHold._bounds = this.paintedMesh.bounds;
		this.meshHold._subMeshCount = this.paintedMesh.subMeshCount;
		this.meshHold._tangents = this.paintedMesh.tangents;
		this.meshHold._uv = this.paintedMesh.uv;
		this.meshHold._uv2 = this.paintedMesh.uv2;
		this.meshHold._uv3 = this.paintedMesh.uv3;
		this.meshHold._colors = this.paintedMesh.colors;
		this.meshHold._uv4 = this.paintedMesh.uv4;
		base.GetComponent<MeshFilter>().sharedMesh = this.paintedMesh;
		if (base.GetComponent<MeshCollider>())
		{
			base.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
		}
	}

	// Token: 0x06002B58 RID: 11096 RVA: 0x000DE1E0 File Offset: 0x000DC3E0
	public void setWholeMesh(Mesh tmpMesh)
	{
		this.paintedMesh.vertices = tmpMesh.vertices;
		this.paintedMesh.triangles = tmpMesh.triangles;
		this.paintedMesh.normals = tmpMesh.normals;
		this.paintedMesh.colors = tmpMesh.colors;
		this.paintedMesh.uv = tmpMesh.uv;
		this.paintedMesh.uv2 = tmpMesh.uv2;
		this.paintedMesh.uv3 = tmpMesh.uv3;
		this.meshHold._vertices = tmpMesh.vertices;
		this.meshHold._triangles = tmpMesh.triangles;
		this.meshHold._normals = tmpMesh.normals;
		this.meshHold._colors = tmpMesh.colors;
		this.meshHold._uv = tmpMesh.uv;
		this.meshHold._uv2 = tmpMesh.uv2;
		this.meshHold._uv3 = tmpMesh.uv3;
	}

	// Token: 0x06002B59 RID: 11097 RVA: 0x000DE2DC File Offset: 0x000DC4DC
	public Vector3[] setVertices(Vector3[] _deformedVertices)
	{
		this.paintedMesh.vertices = _deformedVertices;
		this.meshHold._vertices = _deformedVertices;
		this.paintedMesh.RecalculateNormals();
		this.paintedMesh.RecalculateBounds();
		this.meshHold._normals = this.paintedMesh.normals;
		this.meshHold._bounds = this.paintedMesh.bounds;
		base.GetComponent<MeshCollider>().sharedMesh = null;
		if (base.GetComponent<MeshCollider>())
		{
			base.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
		}
		return this.meshHold._normals;
	}

	// Token: 0x06002B5A RID: 11098 RVA: 0x00021A09 File Offset: 0x0001FC09
	public Vector3[] getVertices()
	{
		return this.paintedMesh.vertices;
	}

	// Token: 0x06002B5B RID: 11099 RVA: 0x00021A16 File Offset: 0x0001FC16
	public Vector3[] getNormals()
	{
		return this.paintedMesh.normals;
	}

	// Token: 0x06002B5C RID: 11100 RVA: 0x00021A23 File Offset: 0x0001FC23
	public int[] getTriangles()
	{
		return this.paintedMesh.triangles;
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x00021A30 File Offset: 0x0001FC30
	public void setTangents(Vector4[] _meshTangents)
	{
		this.paintedMesh.tangents = _meshTangents;
		this.meshHold._tangents = _meshTangents;
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x00021A4A File Offset: 0x0001FC4A
	public Vector4[] getTangents()
	{
		return this.paintedMesh.tangents;
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x00021A57 File Offset: 0x0001FC57
	public void setColors(Color[] _vertexColors)
	{
		this.paintedMesh.colors = _vertexColors;
		this.meshHold._colors = _vertexColors;
	}

	// Token: 0x06002B60 RID: 11104 RVA: 0x00021A71 File Offset: 0x0001FC71
	public Color[] getColors()
	{
		return this.paintedMesh.colors;
	}

	// Token: 0x06002B61 RID: 11105 RVA: 0x00021A7E File Offset: 0x0001FC7E
	public Vector2[] getUVs()
	{
		return this.paintedMesh.uv;
	}

	// Token: 0x06002B62 RID: 11106 RVA: 0x00021A8B File Offset: 0x0001FC8B
	public void setUV4s(Vector2[] _uv4s)
	{
		this.paintedMesh.uv4 = _uv4s;
		this.meshHold._uv4 = _uv4s;
	}

	// Token: 0x06002B63 RID: 11107 RVA: 0x00021AA5 File Offset: 0x0001FCA5
	public Vector2[] getUV4s()
	{
		return this.paintedMesh.uv4;
	}

	// Token: 0x06002B64 RID: 11108 RVA: 0x00021AB2 File Offset: 0x0001FCB2
	public void unlink()
	{
		this.init(this.paintedMesh, false);
	}

	// Token: 0x06002B65 RID: 11109 RVA: 0x000DE378 File Offset: 0x000DC578
	public void rebuild()
	{
		if (!base.GetComponent<MeshFilter>())
		{
			return;
		}
		this.paintedMesh = new Mesh();
		this.paintedMesh.hideFlags = HideFlags.HideAndDontSave;
		this.paintedMesh.name = "vpp_" + base.gameObject.name;
		if (this.meshHold == null || this.meshHold._vertices.Length == 0 || this.meshHold._TrianglesOfSubs.Length == 0)
		{
			this.paintedMesh.subMeshCount = this._subMeshCount;
			this.paintedMesh.vertices = this._vertices;
			this.paintedMesh.normals = this._normals;
			this.paintedMesh.triangles = this._triangles;
			this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
			for (int i = 0; i < this.paintedMesh.subMeshCount; i++)
			{
				this.meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
				this.meshHold._TrianglesOfSubs[i].triangles = this.paintedMesh.GetTriangles(i);
			}
			this.paintedMesh.bindposes = this._bindPoses;
			this.paintedMesh.boneWeights = this._boneWeights;
			this.paintedMesh.bounds = this._bounds;
			this.paintedMesh.tangents = this._tangents;
			this.paintedMesh.uv = this._uv;
			this.paintedMesh.uv2 = this._uv2;
			this.paintedMesh.uv3 = this._uv3;
			this.paintedMesh.colors = this._colors;
			this.paintedMesh.uv4 = this._uv4;
			this.init(this.paintedMesh, true);
			return;
		}
		this.paintedMesh.subMeshCount = this.meshHold._subMeshCount;
		this.paintedMesh.vertices = this.meshHold._vertices;
		this.paintedMesh.normals = this.meshHold._normals;
		for (int j = 0; j < this.meshHold._subMeshCount; j++)
		{
			this.paintedMesh.SetTriangles(this.meshHold._TrianglesOfSubs[j].triangles, j);
		}
		this.paintedMesh.bindposes = this.meshHold._bindPoses;
		this.paintedMesh.boneWeights = this.meshHold._boneWeights;
		this.paintedMesh.bounds = this.meshHold._bounds;
		this.paintedMesh.tangents = this.meshHold._tangents;
		this.paintedMesh.uv = this.meshHold._uv;
		this.paintedMesh.uv2 = this.meshHold._uv2;
		this.paintedMesh.uv3 = this.meshHold._uv3;
		this.paintedMesh.colors = this.meshHold._colors;
		this.paintedMesh.uv4 = this.meshHold._uv4;
		this.init(this.paintedMesh, true);
	}

	// Token: 0x06002B66 RID: 11110 RVA: 0x00021AC1 File Offset: 0x0001FCC1
	private void Start()
	{
		if (!this.paintedMesh || this.meshHold == null)
		{
			this.rebuild();
		}
	}
}
