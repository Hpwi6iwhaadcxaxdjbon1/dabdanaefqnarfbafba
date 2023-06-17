using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020003B4 RID: 948
public class RendererBatch : MonoBehaviour, IClientComponent
{
	// Token: 0x0400147F RID: 5247
	private bool disabled;

	// Token: 0x04001480 RID: 5248
	private RendererGroup batchGroup;

	// Token: 0x04001481 RID: 5249
	private MeshRendererInstance batchInstance;

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x060017AB RID: 6059 RVA: 0x00013C3F File Offset: 0x00011E3F
	// (set) Token: 0x060017AC RID: 6060 RVA: 0x00013C47 File Offset: 0x00011E47
	public Transform BatchTransform { get; set; }

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x060017AD RID: 6061 RVA: 0x00013C50 File Offset: 0x00011E50
	// (set) Token: 0x060017AE RID: 6062 RVA: 0x00013C58 File Offset: 0x00011E58
	public MeshRenderer BatchRenderer { get; set; }

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x060017AF RID: 6063 RVA: 0x00013C61 File Offset: 0x00011E61
	// (set) Token: 0x060017B0 RID: 6064 RVA: 0x00013C69 File Offset: 0x00011E69
	public MeshFilter BatchFilter { get; set; }

	// Token: 0x060017B1 RID: 6065 RVA: 0x00013C72 File Offset: 0x00011E72
	protected void OnEnable()
	{
		if (base.GetComponent<IBatchingHandler>() == null)
		{
			this.BatchTransform = base.transform;
			this.BatchRenderer = base.GetComponent<MeshRenderer>();
			this.BatchFilter = base.GetComponent<MeshFilter>();
			this.Add();
		}
	}

	// Token: 0x060017B2 RID: 6066 RVA: 0x00013CA6 File Offset: 0x00011EA6
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.Remove();
	}

	// Token: 0x060017B3 RID: 6067 RVA: 0x0008B27C File Offset: 0x0008947C
	public void Add()
	{
		if (this.batchGroup != null)
		{
			this.Remove();
		}
		if (this.disabled)
		{
			return;
		}
		if (!Batching.renderers)
		{
			return;
		}
		if (this.BatchTransform == null)
		{
			return;
		}
		if (this.BatchRenderer == null)
		{
			return;
		}
		if (this.BatchRenderer.sharedMaterial == null)
		{
			return;
		}
		if (this.BatchFilter == null)
		{
			return;
		}
		if (this.BatchFilter.sharedMesh == null)
		{
			return;
		}
		if (this.BatchFilter.sharedMesh.subMeshCount > Batching.renderer_submeshes)
		{
			return;
		}
		if (this.BatchFilter.sharedMesh.vertexCount > Batching.renderer_vertices)
		{
			return;
		}
		if (SingletonComponent<RendererGrid>.Instance == null)
		{
			return;
		}
		RendererCell rendererCell = SingletonComponent<RendererGrid>.Instance[this.BatchTransform.position];
		this.batchGroup = rendererCell.FindBatchGroup(this);
		this.batchGroup.Add(this);
		this.batchInstance.mesh = this.BatchFilter.sharedMesh;
		this.batchInstance.position = this.BatchTransform.position;
		this.batchInstance.rotation = this.BatchTransform.rotation;
		this.batchInstance.scale = this.BatchTransform.lossyScale;
		this.batchInstance.renderer = this.BatchRenderer;
	}

	// Token: 0x060017B4 RID: 6068 RVA: 0x00013CB6 File Offset: 0x00011EB6
	public void Remove()
	{
		if (this.batchGroup == null)
		{
			return;
		}
		this.batchGroup.Invalidate();
		this.batchGroup.Remove(this);
		this.batchGroup = null;
	}

	// Token: 0x060017B5 RID: 6069 RVA: 0x00013CDF File Offset: 0x00011EDF
	public void Refresh()
	{
		this.Remove();
		this.Add();
	}

	// Token: 0x060017B6 RID: 6070 RVA: 0x00013CED File Offset: 0x00011EED
	public void AddBatch(RendererGroup batchGroup)
	{
		batchGroup.Add(this.batchInstance);
	}

	// Token: 0x060017B7 RID: 6071 RVA: 0x00013CFB File Offset: 0x00011EFB
	public void WorkshopMode()
	{
		this.Remove();
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x00013D03 File Offset: 0x00011F03
	public void Toggle(bool state)
	{
		this.disabled = !state;
		if (this.disabled)
		{
			this.Remove();
			return;
		}
		this.Add();
	}
}
