using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class SoundSource : MonoBehaviour, IClientComponentEx, ILOD, ISoundBudgetedUpdate
{
	// Token: 0x04000B16 RID: 2838
	[Header("Occlusion")]
	public bool handleOcclusionChecks;

	// Token: 0x04000B17 RID: 2839
	public LayerMask occlusionLayerMask;

	// Token: 0x04000B18 RID: 2840
	public List<SoundSource.OcclusionPoint> occlusionPoints = new List<SoundSource.OcclusionPoint>();

	// Token: 0x04000B19 RID: 2841
	public bool isOccluded;

	// Token: 0x04000B1A RID: 2842
	public float occlusionAmount;

	// Token: 0x04000B1B RID: 2843
	public float lodDistance = 100f;

	// Token: 0x04000B1C RID: 2844
	public bool inRange;

	// Token: 0x04000B1D RID: 2845
	private bool wasInRange;

	// Token: 0x04000B1E RID: 2846
	private LODCell cell;

	// Token: 0x04000B1F RID: 2847
	private float lastOcclusionCheck;

	// Token: 0x04000B20 RID: 2848
	private float occlusionCheckInterval = 0.25f;

	// Token: 0x04000B21 RID: 2849
	private int lastOcclusionPointIdx;

	// Token: 0x04000B22 RID: 2850
	private Ray ray;

	// Token: 0x04000B23 RID: 2851
	private List<RaycastHit> hits = new List<RaycastHit>();

	// Token: 0x06000E0D RID: 3597 RVA: 0x00063360 File Offset: 0x00061560
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		for (int i = 0; i < this.occlusionPoints.Count; i++)
		{
			Gizmos.DrawWireSphere(base.transform.position + this.occlusionPoints[i].offset, 0.1f);
		}
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x0000CF3F File Offset: 0x0000B13F
	private void OnValidate()
	{
		if (this.occlusionPoints.Count <= 0)
		{
			this.occlusionPoints.Add(new SoundSource.OcclusionPoint());
		}
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x0000CF5F File Offset: 0x0000B15F
	private void OnEnable()
	{
		LODGrid.Add(this, base.transform, ref this.cell);
		SoundManager.AddBudgetedUpdatable(this);
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x0000CF79 File Offset: 0x0000B179
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		LODGrid.Remove(this, base.transform, ref this.cell);
		SoundManager.RemoveBudgetedUpdatable(this);
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x000633B8 File Offset: 0x000615B8
	public void DoUpdate()
	{
		if (this.inRange && Audio.advancedocclusion)
		{
			if (!this.wasInRange)
			{
				Sound[] componentsInChildren = base.GetComponentsInChildren<Sound>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].soundSource = this;
				}
			}
			this.DoOcclusionCheck();
		}
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x00063400 File Offset: 0x00061600
	public void DoOcclusionCheck()
	{
		if (UnityEngine.Time.time < this.lastOcclusionCheck + this.occlusionCheckInterval)
		{
			return;
		}
		using (TimeWarning.New("SoundOcclusion.DoOcclusionCheck", 0.1f))
		{
			this.lastOcclusionPointIdx++;
			if (this.lastOcclusionPointIdx >= this.occlusionPoints.Count)
			{
				this.lastOcclusionPointIdx = 0;
			}
			SoundSource.OcclusionPoint occlusionPoint = this.occlusionPoints[this.lastOcclusionPointIdx];
			this.hits.Clear();
			occlusionPoint.isOccluded = false;
			Vector3 vector = base.transform.position + occlusionPoint.offset;
			float maxDistance = Vector3.Distance(vector, MainCamera.position);
			this.ray.origin = vector;
			this.ray.direction = MainCamera.position - vector;
			GamePhysics.TraceAll(this.ray, 0f, this.hits, maxDistance, this.occlusionLayerMask, 0);
			for (int i = 0; i < this.hits.Count; i++)
			{
				BaseEntity componentInParent = this.hits[i].collider.GetComponentInParent<BaseEntity>();
				if (!base.transform.IsChildOf(this.hits[i].collider.transform) && (componentInParent == null || (!base.transform.IsChildOf(componentInParent.transform) && componentInParent.isClient)))
				{
					occlusionPoint.isOccluded = true;
				}
			}
			this.occlusionAmount = 0f;
			for (int j = 0; j < this.occlusionPoints.Count; j++)
			{
				if (this.occlusionPoints[j].isOccluded)
				{
					this.occlusionAmount += 1f;
				}
			}
			this.occlusionAmount /= (float)this.occlusionPoints.Count;
			this.isOccluded = (this.occlusionAmount >= 0.01f);
			this.lastOcclusionCheck = UnityEngine.Time.time;
		}
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x0000CF9B File Offset: 0x0000B19B
	public void RefreshLOD()
	{
		LODGrid.Refresh(this, base.transform, ref this.cell);
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x0000CFAF File Offset: 0x0000B1AF
	public void ChangeLOD()
	{
		this.inRange = (LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ) < LODUtil.VerifyDistance(this.lodDistance));
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x0000CFD0 File Offset: 0x0000B1D0
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this);
	}

	// Token: 0x02000194 RID: 404
	[Serializable]
	public class OcclusionPoint
	{
		// Token: 0x04000B24 RID: 2852
		public Vector3 offset = Vector3.zero;

		// Token: 0x04000B25 RID: 2853
		public bool isOccluded;
	}
}
