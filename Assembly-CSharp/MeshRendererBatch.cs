using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200074E RID: 1870
public class MeshRendererBatch : MeshBatch
{
	// Token: 0x0400240F RID: 9231
	private Vector3 position;

	// Token: 0x04002410 RID: 9232
	private UnityEngine.Mesh meshBatch;

	// Token: 0x04002411 RID: 9233
	private MeshFilter meshFilter;

	// Token: 0x04002412 RID: 9234
	private MeshRenderer meshRenderer;

	// Token: 0x04002413 RID: 9235
	private MeshRendererData meshData;

	// Token: 0x04002414 RID: 9236
	private MeshRendererGroup meshGroup;

	// Token: 0x04002415 RID: 9237
	private MeshRendererLookup meshLookup;

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x060028B7 RID: 10423 RVA: 0x0001F96F File Offset: 0x0001DB6F
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x060028B8 RID: 10424 RVA: 0x0001F976 File Offset: 0x0001DB76
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x060028B9 RID: 10425 RVA: 0x0001FA32 File Offset: 0x0001DC32
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new MeshRendererData();
		this.meshGroup = new MeshRendererGroup();
		this.meshLookup = new MeshRendererLookup();
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x000D0B30 File Offset: 0x000CED30
	public void Setup(Vector3 position, Material material, ShadowCastingMode shadows, int layer)
	{
		base.transform.position = position;
		this.position = position;
		base.gameObject.layer = layer;
		this.meshRenderer.sharedMaterial = material;
		this.meshRenderer.shadowCastingMode = shadows;
		if (shadows == ShadowCastingMode.ShadowsOnly)
		{
			this.meshRenderer.receiveShadows = false;
			this.meshRenderer.motionVectors = false;
			this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			return;
		}
		this.meshRenderer.receiveShadows = true;
		this.meshRenderer.motionVectors = true;
		this.meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
		this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
	}

	// Token: 0x060028BB RID: 10427 RVA: 0x000D0BDC File Offset: 0x000CEDDC
	public void Add(MeshRendererInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x060028BC RID: 10428 RVA: 0x0001FA6D File Offset: 0x0001DC6D
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x060028BD RID: 10429 RVA: 0x0001FA85 File Offset: 0x0001DC85
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x060028BE RID: 10430 RVA: 0x0001FA9D File Offset: 0x0001DC9D
	protected override void RefreshMesh()
	{
		this.meshLookup.dst.Clear();
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup, this.meshLookup);
	}

	// Token: 0x060028BF RID: 10431 RVA: 0x000D0C2C File Offset: 0x000CEE2C
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshLookup.Apply();
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x060028C0 RID: 10432 RVA: 0x000D0C7C File Offset: 0x000CEE7C
	protected override void ToggleMesh(bool state)
	{
		List<MeshRendererLookup.LookupEntry> data = this.meshLookup.src.data;
		for (int i = 0; i < data.Count; i++)
		{
			Renderer renderer = data[i].renderer;
			if (renderer)
			{
				renderer.enabled = !state;
			}
		}
		if (state)
		{
			if (this.meshFilter)
			{
				this.meshFilter.sharedMesh = this.meshBatch;
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = true;
				return;
			}
		}
		else
		{
			if (this.meshFilter)
			{
				this.meshFilter.sharedMesh = null;
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = false;
			}
		}
	}

	// Token: 0x060028C1 RID: 10433 RVA: 0x000D0D38 File Offset: 0x000CEF38
	protected override void OnPooled()
	{
		if (this.meshFilter)
		{
			this.meshFilter.sharedMesh = null;
		}
		if (this.meshBatch)
		{
			AssetPool.Free(ref this.meshBatch);
		}
		this.meshData.Free();
		this.meshGroup.Free();
		this.meshLookup.src.Clear();
		this.meshLookup.dst.Clear();
	}
}
