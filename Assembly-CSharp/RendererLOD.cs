using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020003D2 RID: 978
public class RendererLOD : LODComponent, IBatchingHandler
{
	// Token: 0x0400150E RID: 5390
	[Horizontal(1, 0)]
	public RendererLOD.State[] States;

	// Token: 0x0400150F RID: 5391
	private RendererBatch meshBatch;

	// Token: 0x04001510 RID: 5392
	private int curlod;

	// Token: 0x04001511 RID: 5393
	private bool force;

	// Token: 0x0600188A RID: 6282 RVA: 0x0008D434 File Offset: 0x0008B634
	protected override void InitLOD()
	{
		this.meshBatch = base.GetComponent<RendererBatch>();
		for (int i = 0; i < this.States.Length; i++)
		{
			RendererLOD.State state = this.States[i];
			if (state.renderer)
			{
				state.filter = state.renderer.GetComponent<MeshFilter>();
				state.shadowMode = state.renderer.shadowCastingMode;
				Impostor component = state.renderer.GetComponent<Impostor>();
				state.isImpostor = (component != null);
				this.impostor = ((this.impostor == null) ? component : this.impostor);
			}
		}
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x00014873 File Offset: 0x00012A73
	protected override void EnableLOD()
	{
		this.force = true;
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x00002ECE File Offset: 0x000010CE
	protected override void DisableLOD()
	{
	}

	// Token: 0x0600188D RID: 6285 RVA: 0x0001487C File Offset: 0x00012A7C
	protected override void Show()
	{
		this.States[this.curlod].Show(this.meshBatch);
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x00014896 File Offset: 0x00012A96
	protected override void Hide()
	{
		this.States[this.curlod].Hide(this.meshBatch, false);
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x000148B1 File Offset: 0x00012AB1
	protected override void Hide(bool shadowsVisible)
	{
		this.States[this.curlod].Hide(this.meshBatch, shadowsVisible);
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x000148CC File Offset: 0x00012ACC
	protected override bool IsLODHiding()
	{
		return this.States[this.curlod].renderer == null;
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x0008D4D0 File Offset: 0x0008B6D0
	protected override void SetLOD(int newlod)
	{
		if (this.curlod != newlod)
		{
			this.States[this.curlod].Hide(this.meshBatch, false);
			this.States[newlod].Show(this.meshBatch);
			this.curlod = newlod;
			return;
		}
		if (this.force)
		{
			this.States[this.curlod].Show(this.meshBatch);
			this.force = false;
		}
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x0008D544 File Offset: 0x0008B744
	protected override int GetLOD(float distance)
	{
		for (int i = this.States.Length - 1; i >= 0; i--)
		{
			RendererLOD.State state = this.States[i];
			float num;
			if (state.renderer != null)
			{
				num = ConVar.Mesh.lod;
				if (num < 1f && i > 1)
				{
					num = 1f;
				}
			}
			else
			{
				num = ConVar.Mesh.cull;
			}
			if (distance >= LODUtil.VerifyDistance(state.distance * num))
			{
				return i;
			}
		}
		return 0;
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x0008D5B8 File Offset: 0x0008B7B8
	protected override bool ComputeCullingSphereBounds(out OcclusionCulling.Sphere sphereBounds)
	{
		if (this.States.Length != 0 && this.States[0].renderer != null && this.meshBatch == null)
		{
			Bounds bounds = this.States[0].renderer.bounds;
			sphereBounds = new OcclusionCulling.Sphere(bounds.center, bounds.extents.magnitude);
			return true;
		}
		sphereBounds = default(OcclusionCulling.Sphere);
		return false;
	}

	// Token: 0x020003D3 RID: 979
	[Serializable]
	public class State
	{
		// Token: 0x04001512 RID: 5394
		public float distance;

		// Token: 0x04001513 RID: 5395
		public Renderer renderer;

		// Token: 0x04001514 RID: 5396
		[NonSerialized]
		public MeshFilter filter;

		// Token: 0x04001515 RID: 5397
		[NonSerialized]
		public ShadowCastingMode shadowMode;

		// Token: 0x04001516 RID: 5398
		[NonSerialized]
		public bool isImpostor;

		// Token: 0x06001895 RID: 6293 RVA: 0x0008D630 File Offset: 0x0008B830
		public void Show(RendererBatch batch)
		{
			if (this.renderer != null)
			{
				this.renderer.enabled = true;
				this.renderer.shadowCastingMode = this.shadowMode;
			}
			if (this.renderer != null && !this.isImpostor && batch != null)
			{
				batch.BatchTransform = this.renderer.transform;
				batch.BatchRenderer = (this.renderer as MeshRenderer);
				batch.BatchFilter = this.filter;
				batch.Add();
			}
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x0008D6BC File Offset: 0x0008B8BC
		public void Hide(RendererBatch batch, bool shadowsVisible = false)
		{
			if (this.renderer != null && !this.isImpostor && batch != null)
			{
				batch.Remove();
			}
			if (this.renderer != null)
			{
				if (this.shadowMode == ShadowCastingMode.Off)
				{
					this.renderer.enabled = false;
					return;
				}
				this.renderer.enabled = shadowsVisible;
				this.renderer.shadowCastingMode = (shadowsVisible ? ShadowCastingMode.ShadowsOnly : this.shadowMode);
			}
		}
	}
}
