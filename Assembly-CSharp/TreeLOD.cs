using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020003D5 RID: 981
public class TreeLOD : LODComponent, IComparable<TreeLOD>
{
	// Token: 0x0400151C RID: 5404
	[Horizontal(1, 0)]
	public TreeLOD.State[] States;

	// Token: 0x0400151D RID: 5405
	private int requestedlod;

	// Token: 0x0400151E RID: 5406
	private int curlod;

	// Token: 0x0400151F RID: 5407
	private bool force;

	// Token: 0x04001520 RID: 5408
	public static Comparison<TreeLOD> Comparison = (TreeLOD x, TreeLOD y) => x.CompareTo(y);

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x060018A0 RID: 6304 RVA: 0x00014989 File Offset: 0x00012B89
	public int CulledLOD
	{
		get
		{
			return this.States.Length - 1;
		}
	}

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x060018A1 RID: 6305 RVA: 0x00014995 File Offset: 0x00012B95
	public int BillboardLOD
	{
		get
		{
			return this.States.Length - 2;
		}
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x000149A1 File Offset: 0x00012BA1
	public bool IsMesh(int lod)
	{
		return lod < this.BillboardLOD;
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x0008D784 File Offset: 0x0008B984
	protected override void InitLOD()
	{
		for (int i = 0; i < this.States.Length; i++)
		{
			TreeLOD.State state = this.States[i];
			if (state.renderer)
			{
				state.filter = state.renderer.GetComponent<MeshFilter>();
				state.shadowMode = state.renderer.shadowCastingMode;
				Impostor component = state.renderer.GetComponent<Impostor>();
				this.impostor = ((this.impostor == null) ? component : this.impostor);
			}
		}
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x000149AC File Offset: 0x00012BAC
	protected override void EnableLOD()
	{
		this.force = true;
		if (this.IsMesh(this.requestedlod))
		{
			LODGrid.AddTreeMesh(this);
		}
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x000149C9 File Offset: 0x00012BC9
	protected override void DisableLOD()
	{
		if (this.IsMesh(this.requestedlod))
		{
			LODGrid.RemoveTreeMesh(this);
		}
	}

	// Token: 0x060018A6 RID: 6310 RVA: 0x000149DF File Offset: 0x00012BDF
	protected override void Show()
	{
		this.States[this.curlod].Show();
		if (this.IsMesh(this.requestedlod))
		{
			LODGrid.AddTreeMesh(this);
		}
	}

	// Token: 0x060018A7 RID: 6311 RVA: 0x00014A07 File Offset: 0x00012C07
	protected override void Hide()
	{
		this.States[this.curlod].Hide(false);
		if (this.IsMesh(this.requestedlod))
		{
			LODGrid.RemoveTreeMesh(this);
		}
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x00014A30 File Offset: 0x00012C30
	protected override void Hide(bool shadowsVisible)
	{
		this.States[this.curlod].Hide(shadowsVisible);
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x00014A45 File Offset: 0x00012C45
	protected override bool IsLODHiding()
	{
		return this.States[this.curlod].renderer == null;
	}

	// Token: 0x060018AA RID: 6314 RVA: 0x0008D808 File Offset: 0x0008BA08
	protected override void SetLOD(int newlod)
	{
		if (this.IsMesh(newlod) && !this.IsMesh(this.requestedlod))
		{
			LODGrid.AddTreeMesh(this);
		}
		else if (!this.IsMesh(newlod) && this.IsMesh(this.requestedlod))
		{
			LODGrid.RemoveTreeMesh(this);
		}
		this.requestedlod = newlod;
		if (this.IsMesh(newlod) && base.CurrentDistance - (this.IsMesh(this.curlod) ? 2f : 0f) > LODGrid.TreeMeshDistance)
		{
			newlod = this.BillboardLOD;
		}
		if (this.curlod != newlod)
		{
			this.States[this.curlod].Hide(false);
			this.States[newlod].Show();
			this.curlod = newlod;
			return;
		}
		if (this.force)
		{
			this.States[this.curlod].Show();
			this.force = false;
		}
	}

	// Token: 0x060018AB RID: 6315 RVA: 0x0008D8E4 File Offset: 0x0008BAE4
	protected override int GetLOD(float distance)
	{
		for (int i = this.States.Length - 1; i >= 0; i--)
		{
			TreeLOD.State state = this.States[i];
			float num;
			if (i >= this.States.Length - 1)
			{
				if (Tree.quality > 50f)
				{
					num = state.distance;
				}
				else
				{
					num = LODUtil.VerifyDistance(state.distance * Tree.cull);
				}
			}
			else if (i >= this.States.Length - 2)
			{
				num = LODUtil.VerifyDistance(state.distance * Tree.cull);
			}
			else if (i >= 2)
			{
				num = LODUtil.VerifyDistance(state.distance * Tree.cull);
			}
			else
			{
				num = LODUtil.VerifyDistance(state.distance * Tree.lod);
			}
			if (LODUtil.VerifyDistance(distance) >= num)
			{
				return i;
			}
		}
		return 0;
	}

	// Token: 0x060018AC RID: 6316 RVA: 0x0008D9A4 File Offset: 0x0008BBA4
	protected override bool ComputeCullingSphereBounds(out OcclusionCulling.Sphere sphereBounds)
	{
		if (this.States.Length != 0 && this.States[0].renderer != null)
		{
			Bounds bounds = this.States[0].renderer.bounds;
			sphereBounds = new OcclusionCulling.Sphere(bounds.center, bounds.extents.magnitude);
			return true;
		}
		sphereBounds = default(OcclusionCulling.Sphere);
		return false;
	}

	// Token: 0x060018AD RID: 6317 RVA: 0x0008DA10 File Offset: 0x0008BC10
	public int CompareTo(TreeLOD other)
	{
		float currentDistance = base.CurrentDistance;
		float currentDistance2 = other.CurrentDistance;
		if (currentDistance < currentDistance2)
		{
			return -1;
		}
		if (currentDistance > currentDistance2)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x020003D6 RID: 982
	[Serializable]
	public class State
	{
		// Token: 0x04001521 RID: 5409
		public float distance;

		// Token: 0x04001522 RID: 5410
		public Renderer renderer;

		// Token: 0x04001523 RID: 5411
		[NonSerialized]
		public MeshFilter filter;

		// Token: 0x04001524 RID: 5412
		[NonSerialized]
		public ShadowCastingMode shadowMode;

		// Token: 0x060018B0 RID: 6320 RVA: 0x00014A76 File Offset: 0x00012C76
		public void Show()
		{
			if (this.renderer != null)
			{
				this.renderer.enabled = true;
				this.renderer.shadowCastingMode = this.shadowMode;
			}
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x0008DA38 File Offset: 0x0008BC38
		public void Hide(bool shadowsVisible = false)
		{
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
