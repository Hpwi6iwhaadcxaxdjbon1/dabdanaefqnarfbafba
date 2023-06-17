using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200074D RID: 1869
public class MeshDataBatch : MeshBatch
{
	// Token: 0x04002409 RID: 9225
	private Vector3 position;

	// Token: 0x0400240A RID: 9226
	private UnityEngine.Mesh meshBatch;

	// Token: 0x0400240B RID: 9227
	private MeshFilter meshFilter;

	// Token: 0x0400240C RID: 9228
	private MeshRenderer meshRenderer;

	// Token: 0x0400240D RID: 9229
	private MeshData meshData;

	// Token: 0x0400240E RID: 9230
	private MeshGroup meshGroup;

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x060028AB RID: 10411 RVA: 0x0001F96F File Offset: 0x0001DB6F
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x060028AC RID: 10412 RVA: 0x0001F976 File Offset: 0x0001DB76
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x060028AD RID: 10413 RVA: 0x0001F97D File Offset: 0x0001DB7D
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new MeshData();
		this.meshGroup = new MeshGroup();
	}

	// Token: 0x060028AE RID: 10414 RVA: 0x000D0964 File Offset: 0x000CEB64
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

	// Token: 0x060028AF RID: 10415 RVA: 0x000D0A10 File Offset: 0x000CEC10
	public void Add(MeshInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x0001F9AD File Offset: 0x0001DBAD
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x0001F9C5 File Offset: 0x0001DBC5
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x0001F9DD File Offset: 0x0001DBDD
	protected override void RefreshMesh()
	{
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup);
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x0001F9FB File Offset: 0x0001DBFB
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x060028B4 RID: 10420 RVA: 0x000D0A60 File Offset: 0x000CEC60
	protected override void ToggleMesh(bool state)
	{
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

	// Token: 0x060028B5 RID: 10421 RVA: 0x000D0ADC File Offset: 0x000CECDC
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
	}
}
