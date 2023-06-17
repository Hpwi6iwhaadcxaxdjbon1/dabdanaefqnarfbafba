using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020003CA RID: 970
public class MeshLOD : LODComponent, IBatchingHandler
{
	// Token: 0x040014F9 RID: 5369
	[Horizontal(1, 0)]
	public MeshLOD.State[] States;

	// Token: 0x040014FA RID: 5370
	private RendererBatch meshBatch;

	// Token: 0x040014FB RID: 5371
	private MeshRenderer meshRenderer;

	// Token: 0x040014FC RID: 5372
	private MeshFilter meshFilter;

	// Token: 0x040014FD RID: 5373
	private ShadowCastingMode meshShadowMode;

	// Token: 0x040014FE RID: 5374
	private int curlod;

	// Token: 0x040014FF RID: 5375
	private bool force;

	// Token: 0x06001870 RID: 6256 RVA: 0x00014726 File Offset: 0x00012926
	protected override void InitLOD()
	{
		this.meshBatch = base.GetComponent<RendererBatch>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshShadowMode = this.meshRenderer.shadowCastingMode;
	}

	// Token: 0x06001871 RID: 6257 RVA: 0x0001475D File Offset: 0x0001295D
	protected override void EnableLOD()
	{
		this.force = true;
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x00002ECE File Offset: 0x000010CE
	protected override void DisableLOD()
	{
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x00014766 File Offset: 0x00012966
	protected override void Show()
	{
		this.States[this.curlod].Show(this.meshFilter, this.meshRenderer, this.meshBatch, this.meshShadowMode);
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x00014792 File Offset: 0x00012992
	protected override void Hide()
	{
		this.States[this.curlod].Hide(this.meshFilter, this.meshRenderer, this.meshBatch, this.meshShadowMode, false);
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x000147BF File Offset: 0x000129BF
	protected override void Hide(bool shadowsVisible)
	{
		this.States[this.curlod].Hide(this.meshFilter, this.meshRenderer, this.meshBatch, this.meshShadowMode, shadowsVisible);
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x000147EC File Offset: 0x000129EC
	protected override bool IsLODHiding()
	{
		return this.States[this.curlod].mesh == null;
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x0008CFFC File Offset: 0x0008B1FC
	protected override void SetLOD(int newlod)
	{
		if (this.curlod != newlod)
		{
			this.States[this.curlod].Hide(this.meshFilter, this.meshRenderer, this.meshBatch, this.meshShadowMode, false);
			this.States[newlod].Show(this.meshFilter, this.meshRenderer, this.meshBatch, this.meshShadowMode);
			this.curlod = newlod;
			return;
		}
		if (this.force)
		{
			this.States[newlod].Show(this.meshFilter, this.meshRenderer, this.meshBatch, this.meshShadowMode);
			this.force = false;
		}
	}

	// Token: 0x06001878 RID: 6264 RVA: 0x0008D0A0 File Offset: 0x0008B2A0
	protected override int GetLOD(float distance)
	{
		for (int i = this.States.Length - 1; i >= 0; i--)
		{
			MeshLOD.State state = this.States[i];
			float num;
			if (state.mesh != null)
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

	// Token: 0x06001879 RID: 6265 RVA: 0x0008D114 File Offset: 0x0008B314
	protected override bool ComputeCullingSphereBounds(out OcclusionCulling.Sphere sphereBounds)
	{
		if (this.States.Length != 0 && this.States[0].mesh != null && this.meshBatch == null)
		{
			Bounds bounds = this.States[0].mesh.bounds;
			Vector3 position = this.cachedTransform.localToWorldMatrix.MultiplyPoint3x4(bounds.center);
			float magnitude = Vector3.Scale(bounds.extents, this.cachedTransform.lossyScale).magnitude;
			sphereBounds = new OcclusionCulling.Sphere(position, magnitude);
			return true;
		}
		sphereBounds = default(OcclusionCulling.Sphere);
		return false;
	}

	// Token: 0x020003CB RID: 971
	[Serializable]
	public class State
	{
		// Token: 0x04001500 RID: 5376
		public float distance;

		// Token: 0x04001501 RID: 5377
		public UnityEngine.Mesh mesh;

		// Token: 0x0600187B RID: 6267 RVA: 0x0008D1B8 File Offset: 0x0008B3B8
		public void Show(MeshFilter filter, MeshRenderer renderer, RendererBatch batch, ShadowCastingMode shadowMode)
		{
			filter.sharedMesh = this.mesh;
			renderer.enabled = (this.mesh != null);
			renderer.shadowCastingMode = shadowMode;
			if (this.mesh != null && batch != null)
			{
				batch.BatchTransform = renderer.transform;
				batch.BatchRenderer = renderer;
				batch.BatchFilter = filter;
				batch.Add();
			}
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x0008D224 File Offset: 0x0008B424
		public void Hide(MeshFilter filter, MeshRenderer renderer, RendererBatch batch, ShadowCastingMode shadowMode, bool shadowsVisible = false)
		{
			if (this.mesh != null && batch != null)
			{
				batch.Remove();
			}
			filter.sharedMesh = this.mesh;
			if (shadowMode == ShadowCastingMode.Off)
			{
				renderer.enabled = false;
				return;
			}
			renderer.enabled = shadowsVisible;
			renderer.shadowCastingMode = (shadowsVisible ? ShadowCastingMode.ShadowsOnly : shadowMode);
		}
	}
}
