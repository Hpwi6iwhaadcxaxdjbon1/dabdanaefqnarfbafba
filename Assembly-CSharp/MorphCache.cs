using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rust;
using UnityEngine;

// Token: 0x020005A4 RID: 1444
public class MorphCache : MonoBehaviour
{
	// Token: 0x04001CE0 RID: 7392
	public bool fallback;

	// Token: 0x04001CE1 RID: 7393
	public int blendShape = -1;

	// Token: 0x04001CE2 RID: 7394
	[Range(0f, 1f)]
	public float[] blendWeights;

	// Token: 0x04001CE3 RID: 7395
	private bool dirty = true;

	// Token: 0x04001CE4 RID: 7396
	private SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x04001CE5 RID: 7397
	private Mesh sourceMesh;

	// Token: 0x04001CE6 RID: 7398
	private Mesh prevSourceMesh;

	// Token: 0x04001CE7 RID: 7399
	private BoneWeight[] sourceBoneWeights;

	// Token: 0x04001CE8 RID: 7400
	private Matrix4x4[] sourceBindposes;

	// Token: 0x04001CE9 RID: 7401
	private Mesh bakedMesh;

	// Token: 0x04001CEA RID: 7402
	private static int MaxVertexCount = 16384;

	// Token: 0x04001CEB RID: 7403
	private static List<Vector3> tempVertices = new List<Vector3>(MorphCache.MaxVertexCount);

	// Token: 0x04001CEC RID: 7404
	private static List<Vector3> tempNormals = new List<Vector3>(MorphCache.MaxVertexCount);

	// Token: 0x04001CED RID: 7405
	private static List<Vector4> tempTangents = new List<Vector4>(MorphCache.MaxVertexCount);

	// Token: 0x04001CEE RID: 7406
	private List<MorphCache.CompressedBlendShape> blendShapes = new List<MorphCache.CompressedBlendShape>(32);

	// Token: 0x04001CEF RID: 7407
	private Dictionary<string, MorphCache.CompressedBlendShape> blendShapesByName;

	// Token: 0x04001CF0 RID: 7408
	private MorphCache.CompressedMeshData sourceMeshData;

	// Token: 0x04001CF1 RID: 7409
	private static Dictionary<string, MorphCache.CompressedBlendShape> compressedBlendShapeCache = new Dictionary<string, MorphCache.CompressedBlendShape>();

	// Token: 0x04001CF2 RID: 7410
	private static Dictionary<Mesh, MorphCache.CompressedMeshData> compressedMeshCache = new Dictionary<Mesh, MorphCache.CompressedMeshData>();

	// Token: 0x04001CF3 RID: 7411
	private static long totalMemoryFootprint = 0L;

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x0600211B RID: 8475 RVA: 0x0001A52D File Offset: 0x0001872D
	public List<MorphCache.CompressedBlendShape> BlendShapes
	{
		get
		{
			return this.blendShapes;
		}
	}

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x0600211C RID: 8476 RVA: 0x0001A535 File Offset: 0x00018735
	public static long TotalMemoryFootprint
	{
		get
		{
			return MorphCache.totalMemoryFootprint;
		}
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x000B28D8 File Offset: 0x000B0AD8
	public void Setup()
	{
		this.skinnedMeshRenderer = base.GetComponent<SkinnedMeshRenderer>();
		this.sourceMesh = this.skinnedMeshRenderer.sharedMesh;
		if (this.sourceMesh.vertexCount > MorphCache.MaxVertexCount)
		{
			Debug.Log(string.Concat(new object[]
			{
				"[MorphCache] Too many vertices found in mesh ",
				this.sourceMesh.name,
				". Max for caching is ",
				MorphCache.MaxVertexCount
			}));
			this.skinnedMeshRenderer = null;
			this.sourceMesh = null;
			base.enabled = false;
			return;
		}
		if (this.fallback)
		{
			this.sourceBoneWeights = this.sourceMesh.boneWeights;
			this.sourceBindposes = this.sourceMesh.bindposes;
		}
		this.sourceMeshData = MorphCache.CacheMeshData(this.sourceMesh);
		MorphCache.CacheBlendShapes(this.sourceMeshData, ref this.blendShapes);
		this.blendShapesByName = ((this.blendShapesByName == null) ? new Dictionary<string, MorphCache.CompressedBlendShape>() : this.blendShapesByName);
		this.blendShapesByName.Clear();
		foreach (MorphCache.CompressedBlendShape compressedBlendShape in this.blendShapes)
		{
			this.blendShapesByName.Add(compressedBlendShape.Name, compressedBlendShape);
		}
		this.CheckResizeBlendWeights(this.blendShapes.Count);
		if (this.bakedMesh == null || this.prevSourceMesh != this.sourceMesh)
		{
			if (this.bakedMesh == null)
			{
				this.bakedMesh = new Mesh();
			}
			this.bakedMesh.Clear();
			this.bakedMesh.name = this.sourceMeshData.MorphCacheName;
			this.sourceMeshData.ExtractTo(this.bakedMesh);
			this.prevSourceMesh = this.sourceMesh;
		}
		for (int i = 0; i < this.blendWeights.Length; i++)
		{
			this.blendWeights[i] = 0f;
		}
		this.blendShape = -1;
		this.dirty = true;
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x0001A53C File Offset: 0x0001873C
	private void OnDestroy()
	{
		if (Application.isQuitting)
		{
			return;
		}
		MorphCache.ReleaseBlendShapes(ref this.blendShapes);
		MorphCache.ReleaseMeshData(ref this.sourceMeshData);
		this.blendShapesByName = null;
		this.blendWeights = null;
		this.dirty = true;
	}

	// Token: 0x0600211F RID: 8479 RVA: 0x000B2AE0 File Offset: 0x000B0CE0
	public static void ReleaseBlendShapes(ref List<MorphCache.CompressedBlendShape> blendShapes)
	{
		if (blendShapes != null)
		{
			foreach (MorphCache.CompressedBlendShape compressedBlendShape in blendShapes)
			{
				compressedBlendShape.Release();
				if (compressedBlendShape.HasZeroRefs)
				{
					MorphCache.compressedBlendShapeCache.Remove(compressedBlendShape.Name);
					MorphCache.totalMemoryFootprint -= (long)compressedBlendShape.MemoryFootprint();
				}
			}
			blendShapes.Clear();
		}
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x0001A571 File Offset: 0x00018771
	public static void ReleaseMeshData(ref MorphCache.CompressedMeshData meshData)
	{
		if (meshData != null)
		{
			meshData.Release();
			if (meshData.HasZeroRefs)
			{
				MorphCache.totalMemoryFootprint -= (long)meshData.MemoryFootprint();
				MorphCache.compressedMeshCache.Remove(meshData.Mesh);
			}
			meshData = null;
		}
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x000B2B64 File Offset: 0x000B0D64
	public static MorphCache.CompressedMeshData CacheMeshData(Mesh mesh)
	{
		MorphCache.CompressedMeshData compressedMeshData;
		if (!MorphCache.compressedMeshCache.TryGetValue(mesh, ref compressedMeshData))
		{
			compressedMeshData = new MorphCache.CompressedMeshData(mesh);
			MorphCache.compressedMeshCache.Add(mesh, compressedMeshData);
			MorphCache.totalMemoryFootprint += (long)compressedMeshData.MemoryFootprint();
		}
		compressedMeshData.AddRef();
		return compressedMeshData;
	}

	// Token: 0x06002122 RID: 8482 RVA: 0x000B2BAC File Offset: 0x000B0DAC
	public static void CacheBlendShapes(MorphCache.CompressedMeshData meshData, ref List<MorphCache.CompressedBlendShape> blendShapes)
	{
		blendShapes.Clear();
		int blendShapeCount = meshData.BlendShapeCount;
		for (int i = 0; i < blendShapeCount; i++)
		{
			string text = meshData.BlendShapeName(i);
			MorphCache.CompressedBlendShape compressedBlendShape;
			if (!MorphCache.compressedBlendShapeCache.TryGetValue(text, ref compressedBlendShape))
			{
				compressedBlendShape = new MorphCache.CompressedBlendShape(meshData.Mesh, i, 0);
				MorphCache.compressedBlendShapeCache.Add(text, compressedBlendShape);
				MorphCache.totalMemoryFootprint += (long)compressedBlendShape.MemoryFootprint();
			}
			compressedBlendShape.AddRef();
			blendShapes.Add(compressedBlendShape);
		}
	}

	// Token: 0x06002123 RID: 8483 RVA: 0x000B2C24 File Offset: 0x000B0E24
	private static void BakeMesh(Mesh bakedMesh, MorphCache.CompressedMeshData sourceMeshData, List<MorphCache.CompressedBlendShape> blendShapes, float[] weights)
	{
		int vertexCount = sourceMeshData.VertexCount;
		sourceMeshData.Extract(MorphCache.tempVertices, MorphCache.tempNormals, MorphCache.tempTangents);
		for (int i = 0; i < blendShapes.Count; i++)
		{
			float num = weights[i];
			if (num != 0f)
			{
				MorphCache.CompressedDeltas[] deltas = blendShapes[i].Deltas;
				for (int j = 0; j < vertexCount; j++)
				{
					Vector3 vector;
					Vector3 vector2;
					Vector3 vector3;
					deltas[j].Extract(out vector, out vector2, out vector3);
					MorphCache.tempVertices[j] = new Vector3(MorphCache.tempVertices[j].x + vector.x * num, MorphCache.tempVertices[j].y + vector.y * num, MorphCache.tempVertices[j].z + vector.z * num);
					MorphCache.tempNormals[j] = new Vector3(MorphCache.tempNormals[j].x + vector2.x * num, MorphCache.tempNormals[j].y + vector2.y * num, MorphCache.tempNormals[j].z + vector2.z * num);
					MorphCache.tempTangents[j] = new Vector4(MorphCache.tempTangents[j].x + vector3.x * num, MorphCache.tempTangents[j].y + vector3.y * num, MorphCache.tempTangents[j].z + vector3.z * num, MorphCache.tempTangents[j].w);
				}
			}
		}
		bakedMesh.SetVertices(MorphCache.tempVertices);
		bakedMesh.SetNormals(MorphCache.tempNormals);
		bakedMesh.SetTangents(MorphCache.tempTangents);
	}

	// Token: 0x06002124 RID: 8484 RVA: 0x000B2E04 File Offset: 0x000B1004
	private Mesh FallbackBakeMesh()
	{
		this.skinnedMeshRenderer.sharedMesh = this.sourceMesh;
		for (int i = 0; i < this.blendWeights.Length; i++)
		{
			this.skinnedMeshRenderer.SetBlendShapeWeight(i, this.blendWeights[i] * 100f);
		}
		this.skinnedMeshRenderer.BakeMesh(this.bakedMesh);
		this.bakedMesh.bindposes = this.sourceBindposes;
		this.bakedMesh.boneWeights = this.sourceBoneWeights;
		return this.bakedMesh;
	}

	// Token: 0x06002125 RID: 8485 RVA: 0x000B2E88 File Offset: 0x000B1088
	public MorphCache.CompressedBlendShape FindBlendShapeByName(string name, bool partial = false)
	{
		MorphCache.CompressedBlendShape result = null;
		if (partial)
		{
			using (List<MorphCache.CompressedBlendShape>.Enumerator enumerator = this.blendShapes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MorphCache.CompressedBlendShape compressedBlendShape = enumerator.Current;
					if (compressedBlendShape.Name.Contains(name))
					{
						result = compressedBlendShape;
						break;
					}
				}
				return result;
			}
		}
		this.blendShapesByName.TryGetValue(name, ref result);
		return result;
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x0001A5AF File Offset: 0x000187AF
	private void CheckResizeBlendWeights(int length)
	{
		if (this.blendWeights == null || this.blendWeights.Length == 0 || this.blendWeights.Length < length)
		{
			this.blendWeights = new float[length];
		}
	}

	// Token: 0x06002127 RID: 8487 RVA: 0x0001A5D9 File Offset: 0x000187D9
	public void SetBlendShapeWeights(float[] weights)
	{
		this.CheckResizeBlendWeights(weights.Length);
		Array.Copy(weights, this.blendWeights, weights.Length);
		this.dirty = true;
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x0001A5FA File Offset: 0x000187FA
	public void SetBlendShape(int index)
	{
		this.blendShape = index;
		this.dirty = true;
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000B2EFC File Offset: 0x000B10FC
	private void Update()
	{
		if (this.dirty)
		{
			for (int i = 0; i < this.blendWeights.Length; i++)
			{
				this.blendWeights[i] = ((i == this.blendShape) ? 1f : 0f);
			}
			bool flag = false;
			if (!this.fallback && this.sourceMeshData != null)
			{
				MorphCache.BakeMesh(this.bakedMesh, this.sourceMeshData, this.blendShapes, this.blendWeights);
				flag = true;
			}
			else if (this.sourceMesh != null)
			{
				this.FallbackBakeMesh();
				flag = true;
			}
			if (flag)
			{
				this.skinnedMeshRenderer.sharedMesh = this.bakedMesh;
				this.dirty = false;
			}
		}
	}

	// Token: 0x020005A5 RID: 1445
	public class CacheEntry
	{
		// Token: 0x04001CF4 RID: 7412
		private int refCount;

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x0600212C RID: 8492 RVA: 0x0001A62D File Offset: 0x0001882D
		public bool HasZeroRefs
		{
			get
			{
				return this.refCount <= 0;
			}
		}

		// Token: 0x0600212D RID: 8493 RVA: 0x0001A63B File Offset: 0x0001883B
		public void AddRef()
		{
			this.refCount++;
		}

		// Token: 0x0600212E RID: 8494 RVA: 0x0001A64B File Offset: 0x0001884B
		public void Release()
		{
			this.refCount--;
		}
	}

	// Token: 0x020005A6 RID: 1446
	public struct CompressedDeltas
	{
		// Token: 0x04001CF5 RID: 7413
		public Vector3 vertex;

		// Token: 0x04001CF6 RID: 7414
		public Vector3 normal;

		// Token: 0x04001CF7 RID: 7415
		public Vector3 tangent;

		// Token: 0x06002130 RID: 8496 RVA: 0x0001A65B File Offset: 0x0001885B
		public CompressedDeltas(Vector3 vertex, Vector3 normal, Vector3 tangent)
		{
			this.vertex = vertex;
			this.normal = normal;
			this.tangent = tangent;
		}

		// Token: 0x06002131 RID: 8497 RVA: 0x0001A672 File Offset: 0x00018872
		public void Extract(out Vector3 vertex, out Vector3 normal, out Vector3 tangent)
		{
			vertex = this.vertex;
			normal = this.normal;
			tangent = this.tangent;
		}
	}

	// Token: 0x020005A7 RID: 1447
	public class CompressedBlendShape : MorphCache.CacheEntry
	{
		// Token: 0x04001CF8 RID: 7416
		protected string name;

		// Token: 0x04001CF9 RID: 7417
		private MorphCache.CompressedDeltas[] deltas;

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06002132 RID: 8498 RVA: 0x0001A698 File Offset: 0x00018898
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06002133 RID: 8499 RVA: 0x0001A6A0 File Offset: 0x000188A0
		public MorphCache.CompressedDeltas[] Deltas
		{
			get
			{
				return this.deltas;
			}
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x000B300C File Offset: 0x000B120C
		public CompressedBlendShape(Mesh mesh, int shape, int frame = 0)
		{
			Debug.Assert(shape >= 0 && shape < mesh.blendShapeCount);
			Debug.Assert(frame >= 0 && frame < mesh.GetBlendShapeFrameCount(shape));
			this.name = mesh.GetBlendShapeName(shape);
			Vector3[] array = new Vector3[mesh.vertexCount];
			Vector3[] array2 = new Vector3[mesh.vertexCount];
			Vector3[] array3 = new Vector3[mesh.vertexCount];
			mesh.GetBlendShapeFrameVertices(shape, frame, array, array2, array3);
			this.deltas = new MorphCache.CompressedDeltas[mesh.vertexCount];
			for (int i = 0; i < mesh.vertexCount; i++)
			{
				this.deltas[i] = new MorphCache.CompressedDeltas(array[i], array2[i], array3[i]);
			}
		}

		// Token: 0x06002135 RID: 8501 RVA: 0x0001A6A8 File Offset: 0x000188A8
		public int MemoryFootprint()
		{
			return Marshal.SizeOf(typeof(MorphCache.CompressedDeltas)) * this.deltas.Length;
		}
	}

	// Token: 0x020005A8 RID: 1448
	public class CompressedMeshData : MorphCache.CacheEntry
	{
		// Token: 0x04001CFA RID: 7418
		private Mesh mesh;

		// Token: 0x04001CFB RID: 7419
		private string name = "";

		// Token: 0x04001CFC RID: 7420
		private string[] blendShapeNames = new string[0];

		// Token: 0x04001CFD RID: 7421
		private string morphCacheName = "";

		// Token: 0x04001CFE RID: 7422
		private int vertexCount;

		// Token: 0x04001CFF RID: 7423
		private Vector3[] vertices;

		// Token: 0x04001D00 RID: 7424
		private Vector3[] normals;

		// Token: 0x04001D01 RID: 7425
		private Vector4[] tangents;

		// Token: 0x04001D02 RID: 7426
		public Vector2[] uvs;

		// Token: 0x04001D03 RID: 7427
		public Color32[] colors;

		// Token: 0x04001D04 RID: 7428
		public Matrix4x4[] bindposes;

		// Token: 0x04001D05 RID: 7429
		public BoneWeight[] boneWeights;

		// Token: 0x04001D06 RID: 7430
		public List<int[]> subMeshes;

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06002136 RID: 8502 RVA: 0x0001A6C2 File Offset: 0x000188C2
		public Mesh Mesh
		{
			get
			{
				return this.mesh;
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06002137 RID: 8503 RVA: 0x0001A6CA File Offset: 0x000188CA
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06002138 RID: 8504 RVA: 0x0001A6D2 File Offset: 0x000188D2
		public int BlendShapeCount
		{
			get
			{
				return this.blendShapeNames.Length;
			}
		}

		// Token: 0x06002139 RID: 8505 RVA: 0x0001A6DC File Offset: 0x000188DC
		public string BlendShapeName(int index)
		{
			return this.blendShapeNames[index];
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x0600213A RID: 8506 RVA: 0x0001A6E6 File Offset: 0x000188E6
		public string MorphCacheName
		{
			get
			{
				return this.morphCacheName;
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x0600213B RID: 8507 RVA: 0x0001A6EE File Offset: 0x000188EE
		public int VertexCount
		{
			get
			{
				return this.vertexCount;
			}
		}

		// Token: 0x0600213C RID: 8508 RVA: 0x000B30D0 File Offset: 0x000B12D0
		public CompressedMeshData(Mesh mesh)
		{
			this.mesh = mesh;
			this.name = mesh.name;
			this.blendShapeNames = new string[mesh.blendShapeCount];
			for (int i = 0; i < mesh.blendShapeCount; i++)
			{
				this.blendShapeNames[i] = mesh.GetBlendShapeName(i);
			}
			this.morphCacheName = this.name + " (MorphCache)";
			this.vertexCount = mesh.vertexCount;
			this.vertices = new Vector3[this.vertexCount];
			this.normals = new Vector3[this.vertexCount];
			this.tangents = new Vector4[this.vertexCount];
			Array.Copy(mesh.vertices, this.vertices, this.vertexCount);
			Array.Copy(mesh.normals, this.normals, this.vertexCount);
			Array.Copy(mesh.tangents, this.tangents, this.vertexCount);
			this.uvs = mesh.uv;
			this.colors = mesh.colors32;
			this.bindposes = mesh.bindposes;
			this.boneWeights = mesh.boneWeights;
			this.subMeshes = new List<int[]>(mesh.subMeshCount);
			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				this.subMeshes.Add(mesh.GetTriangles(j));
			}
		}

		// Token: 0x0600213D RID: 8509 RVA: 0x000B3248 File Offset: 0x000B1448
		public void Extract(List<Vector3> vertices, List<Vector3> normals, List<Vector4> tangents)
		{
			Debug.Assert(vertices.Capacity >= this.vertexCount && normals.Capacity >= this.vertexCount && tangents.Capacity >= this.vertexCount);
			vertices.Clear();
			normals.Clear();
			tangents.Clear();
			for (int i = 0; i < this.vertexCount; i++)
			{
				vertices.Add(this.vertices[i]);
				normals.Add(this.normals[i]);
				tangents.Add(this.tangents[i]);
			}
		}

		// Token: 0x0600213E RID: 8510 RVA: 0x000B32E4 File Offset: 0x000B14E4
		public void ExtractTo(Mesh mesh)
		{
			mesh.vertices = this.vertices;
			mesh.normals = this.normals;
			mesh.tangents = this.tangents;
			mesh.uv = this.uvs;
			mesh.colors32 = this.colors;
			mesh.bindposes = this.bindposes;
			mesh.boneWeights = this.boneWeights;
			mesh.subMeshCount = this.subMeshes.Count;
			for (int i = 0; i < this.subMeshes.Count; i++)
			{
				mesh.SetTriangles(this.subMeshes[i], i);
			}
		}

		// Token: 0x0600213F RID: 8511 RVA: 0x000B3380 File Offset: 0x000B1580
		public int MemoryFootprint()
		{
			int num = 0;
			num += this.vertexCount * Marshal.SizeOf(typeof(Vector3));
			num += this.vertexCount * Marshal.SizeOf(typeof(Vector3));
			num += this.vertexCount * Marshal.SizeOf(typeof(Vector4));
			num += this.vertexCount * Marshal.SizeOf(typeof(Vector2));
			num += this.vertexCount * Marshal.SizeOf(typeof(Color32));
			num += this.bindposes.Length * Marshal.SizeOf(typeof(Matrix4x4));
			num += this.boneWeights.Length * Marshal.SizeOf(typeof(BoneWeight));
			for (int i = 0; i < this.subMeshes.Count; i++)
			{
				num += this.subMeshes[i].Length * Marshal.SizeOf(typeof(int));
			}
			return num;
		}
	}
}
