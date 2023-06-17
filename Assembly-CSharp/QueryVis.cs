using System;
using UnityEngine;

// Token: 0x02000220 RID: 544
public class QueryVis : BaseMonoBehaviour, IClientComponent, IOnParentDestroying
{
	// Token: 0x04000D89 RID: 3465
	public Collider checkCollider;

	// Token: 0x04000D8A RID: 3466
	private CoverageQueries.Query query;

	// Token: 0x04000D8B RID: 3467
	public CoverageQueries.RadiusSpace coverageRadiusSpace = CoverageQueries.RadiusSpace.World;

	// Token: 0x04000D8C RID: 3468
	public float coverageRadius = 0.2f;

	// Token: 0x04000D8D RID: 3469
	private float nextCPUSampleTime;

	// Token: 0x04000D8E RID: 3470
	private float lastCPUSampleValue;

	// Token: 0x0600109F RID: 4255 RVA: 0x0000EA04 File Offset: 0x0000CC04
	public void OnEnable()
	{
		if (CoverageQueries.Supported)
		{
			this.query = new CoverageQueries.Query(base.transform.position, this.coverageRadiusSpace, this.coverageRadius, 32, 50f);
			this.query.Register();
		}
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x0000EA41 File Offset: 0x0000CC41
	public void OnDisable()
	{
		if (this.query != null)
		{
			this.query.Unregister();
		}
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x0007035C File Offset: 0x0006E55C
	public float SampleVisibility()
	{
		if (this.query != null && this.query.IsRegistered)
		{
			this.query.Update(base.transform.position, this.coverageRadius);
			return 2f * Mathf.Clamp01(0.5f - this.query.result.smoothCoverage);
		}
		return this.SampleVisibilityCPU();
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x00002ECE File Offset: 0x000010CE
	public void OnParentDestroying()
	{
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x000703C4 File Offset: 0x0006E5C4
	public float SampleVisibilityCPU()
	{
		if (Time.realtimeSinceStartup < this.nextCPUSampleTime)
		{
			return this.lastCPUSampleValue;
		}
		this.nextCPUSampleTime = Time.realtimeSinceStartup + 0.25f;
		Camera mainCamera = MainCamera.mainCamera;
		Vector3 normalized = (base.transform.position - mainCamera.transform.position).normalized;
		float num = Vector3.Distance(mainCamera.transform.position, base.transform.position);
		RaycastHit raycastHit;
		if (!Physics.Raycast(new Ray(mainCamera.transform.position, normalized), ref raycastHit, num, 1252073729) || (this.checkCollider != null && raycastHit.collider == this.checkCollider))
		{
			this.lastCPUSampleValue = 1f;
		}
		else
		{
			this.lastCPUSampleValue = 0f;
		}
		return this.lastCPUSampleValue;
	}
}
