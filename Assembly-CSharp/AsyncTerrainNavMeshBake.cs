using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200049C RID: 1180
public class AsyncTerrainNavMeshBake : CustomYieldInstruction
{
	// Token: 0x04001862 RID: 6242
	private List<int> indices;

	// Token: 0x04001863 RID: 6243
	private List<Vector3> vertices;

	// Token: 0x04001864 RID: 6244
	private List<Vector3> normals;

	// Token: 0x04001865 RID: 6245
	private List<int> triangles;

	// Token: 0x04001866 RID: 6246
	private Vector3 pivot;

	// Token: 0x04001867 RID: 6247
	private int width;

	// Token: 0x04001868 RID: 6248
	private int height;

	// Token: 0x04001869 RID: 6249
	private bool normal;

	// Token: 0x0400186A RID: 6250
	private bool alpha;

	// Token: 0x0400186B RID: 6251
	private Action worker;

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06001B5E RID: 7006 RVA: 0x000167A2 File Offset: 0x000149A2
	public override bool keepWaiting
	{
		get
		{
			return this.worker != null;
		}
	}

	// Token: 0x17000183 RID: 387
	// (get) Token: 0x06001B5F RID: 7007 RVA: 0x000167AD File Offset: 0x000149AD
	public bool isDone
	{
		get
		{
			return this.worker == null;
		}
	}

	// Token: 0x06001B60 RID: 7008 RVA: 0x00097EEC File Offset: 0x000960EC
	public NavMeshBuildSource CreateNavMeshBuildSource(bool addSourceObject)
	{
		NavMeshBuildSource result = default(NavMeshBuildSource);
		result.transform = Matrix4x4.TRS(this.pivot, Quaternion.identity, Vector3.one);
		result.shape = 0;
		if (addSourceObject)
		{
			result.sourceObject = this.mesh;
		}
		return result;
	}

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06001B61 RID: 7009 RVA: 0x00097F38 File Offset: 0x00096138
	public Mesh mesh
	{
		get
		{
			Mesh mesh = new Mesh();
			if (this.vertices != null)
			{
				mesh.SetVertices(this.vertices);
				Pool.FreeList<Vector3>(ref this.vertices);
			}
			if (this.normals != null)
			{
				mesh.SetNormals(this.normals);
				Pool.FreeList<Vector3>(ref this.normals);
			}
			if (this.triangles != null)
			{
				mesh.SetTriangles(this.triangles, 0);
				Pool.FreeList<int>(ref this.triangles);
			}
			if (this.indices != null)
			{
				Pool.FreeList<int>(ref this.indices);
			}
			return mesh;
		}
	}

	// Token: 0x06001B62 RID: 7010 RVA: 0x00097FC0 File Offset: 0x000961C0
	public AsyncTerrainNavMeshBake(Vector3 pivot, int width, int height, bool normal, bool alpha)
	{
		this.pivot = pivot;
		this.width = width;
		this.height = height;
		this.normal = normal;
		this.alpha = alpha;
		this.indices = Pool.GetList<int>();
		this.vertices = Pool.GetList<Vector3>();
		this.normals = (normal ? Pool.GetList<Vector3>() : null);
		this.triangles = Pool.GetList<int>();
		this.Invoke();
	}

	// Token: 0x06001B63 RID: 7011 RVA: 0x00098034 File Offset: 0x00096234
	private void DoWork()
	{
		Vector3 vector = new Vector3((float)(this.width / 2), 0f, (float)(this.height / 2));
		Vector3 b = new Vector3(this.pivot.x - vector.x, 0f, this.pivot.z - vector.z);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainAlphaMap alphaMap = TerrainMeta.AlphaMap;
		int num = 0;
		for (int i = 0; i <= this.height; i++)
		{
			int j = 0;
			while (j <= this.width)
			{
				Vector3 worldPos = new Vector3((float)j, 0f, (float)i) + b;
				Vector3 vector2 = new Vector3((float)j, 0f, (float)i) - vector;
				float num2 = heightMap.GetHeight(worldPos);
				if (num2 < -1f)
				{
					this.indices.Add(-1);
				}
				else if (this.alpha && alphaMap.GetAlpha(worldPos) < 0.1f)
				{
					this.indices.Add(-1);
				}
				else
				{
					if (this.normal)
					{
						Vector3 vector3 = heightMap.GetNormal(worldPos);
						this.normals.Add(vector3);
					}
					worldPos.y = (vector2.y = num2 - this.pivot.y);
					this.indices.Add(this.vertices.Count);
					this.vertices.Add(vector2);
				}
				j++;
				num++;
			}
		}
		int num3 = 0;
		int k = 0;
		while (k < this.height)
		{
			int l = 0;
			while (l < this.width)
			{
				int num4 = this.indices[num3];
				int num5 = this.indices[num3 + this.width + 1];
				int num6 = this.indices[num3 + 1];
				int num7 = this.indices[num3 + 1];
				int num8 = this.indices[num3 + this.width + 1];
				int num9 = this.indices[num3 + this.width + 2];
				if (num4 != -1 && num5 != -1 && num6 != -1)
				{
					this.triangles.Add(num4);
					this.triangles.Add(num5);
					this.triangles.Add(num6);
				}
				if (num7 != -1 && num8 != -1 && num9 != -1)
				{
					this.triangles.Add(num7);
					this.triangles.Add(num8);
					this.triangles.Add(num9);
				}
				l++;
				num3++;
			}
			k++;
			num3++;
		}
	}

	// Token: 0x06001B64 RID: 7012 RVA: 0x000167B8 File Offset: 0x000149B8
	private void Invoke()
	{
		this.worker = new Action(this.DoWork);
		this.worker.BeginInvoke(new AsyncCallback(this.Callback), null);
	}

	// Token: 0x06001B65 RID: 7013 RVA: 0x000167E5 File Offset: 0x000149E5
	private void Callback(IAsyncResult result)
	{
		this.worker.EndInvoke(result);
		this.worker = null;
	}
}
