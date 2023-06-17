using System;
using ConVar;
using UnityEngine;

// Token: 0x020003C8 RID: 968
public class MeshCull : LODComponent, IBatchingHandler
{
	// Token: 0x040014EF RID: 5359
	public float Distance = 100f;

	// Token: 0x040014F0 RID: 5360
	private RendererBatch meshBatch;

	// Token: 0x040014F1 RID: 5361
	private Renderer meshRenderer;

	// Token: 0x040014F2 RID: 5362
	private MeshFilter meshFilter;

	// Token: 0x040014F3 RID: 5363
	private int curlod;

	// Token: 0x040014F4 RID: 5364
	private bool force;

	// Token: 0x06001860 RID: 6240 RVA: 0x00014659 File Offset: 0x00012859
	protected override void InitLOD()
	{
		this.meshBatch = base.GetComponent<RendererBatch>();
		this.meshRenderer = base.GetComponent<Renderer>();
		this.meshFilter = base.GetComponent<MeshFilter>();
	}

	// Token: 0x06001861 RID: 6241 RVA: 0x0001467F File Offset: 0x0001287F
	protected override void EnableLOD()
	{
		this.force = true;
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x00002ECE File Offset: 0x000010CE
	protected override void DisableLOD()
	{
	}

	// Token: 0x06001863 RID: 6243 RVA: 0x0008CE7C File Offset: 0x0008B07C
	protected override void Show()
	{
		if (this.meshRenderer != null)
		{
			this.meshRenderer.enabled = true;
		}
		if (this.meshBatch != null)
		{
			this.meshBatch.BatchTransform = this.meshRenderer.transform;
			this.meshBatch.BatchRenderer = (this.meshRenderer as MeshRenderer);
			this.meshBatch.BatchFilter = this.meshFilter;
			this.meshBatch.Add();
		}
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x00014688 File Offset: 0x00012888
	protected override void Hide()
	{
		if (this.meshBatch != null)
		{
			this.meshBatch.Remove();
		}
		if (this.meshRenderer != null)
		{
			this.meshRenderer.enabled = false;
		}
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x0008CEFC File Offset: 0x0008B0FC
	protected override void SetLOD(int newlod)
	{
		if (this.curlod != newlod)
		{
			if (newlod == 0)
			{
				this.Show();
			}
			else
			{
				this.Hide();
			}
			this.curlod = newlod;
			return;
		}
		if (this.force)
		{
			if (newlod == 0)
			{
				this.Show();
			}
			else
			{
				this.Hide();
			}
			this.force = false;
		}
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x000146BD File Offset: 0x000128BD
	protected override int GetLOD(float distance)
	{
		if (distance < LODUtil.VerifyDistance(this.Distance * ConVar.Mesh.cull))
		{
			return 0;
		}
		return 1;
	}
}
