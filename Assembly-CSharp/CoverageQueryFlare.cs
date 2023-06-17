using System;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class CoverageQueryFlare : SimpleFlare
{
	// Token: 0x04000764 RID: 1892
	private CoverageQueries.Query query;

	// Token: 0x04000765 RID: 1893
	public CoverageQueries.RadiusSpace coverageRadiusSpace;

	// Token: 0x04000766 RID: 1894
	public float coverageRadius = 0.01f;

	// Token: 0x04000767 RID: 1895
	private bool isVisible;

	// Token: 0x06000B16 RID: 2838 RVA: 0x000570E0 File Offset: 0x000552E0
	public new void OnEnable()
	{
		base.OnEnable();
		if (CoverageQueries.Supported)
		{
			this.query = new CoverageQueries.Query(base.transform.position, this.coverageRadiusSpace, this.coverageRadius, 32, 50f);
		}
		this.tickRate = 0f;
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0000ABE3 File Offset: 0x00008DE3
	private void OnBecameVisible()
	{
		if (this.query != null)
		{
			this.query.Register();
		}
		this.isVisible = true;
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x0000ABFF File Offset: 0x00008DFF
	private void OnBecameInvisible()
	{
		if (this.query != null)
		{
			this.query.Unregister();
		}
		this.isVisible = false;
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x00057130 File Offset: 0x00055330
	public override float SampleVisibility()
	{
		if (!this.isVisible)
		{
			return 0f;
		}
		if (this.query != null && this.query.IsRegistered)
		{
			this.query.Update(base.transform.position, this.coverageRadius);
			return 2f * Mathf.Clamp01(0.5f - this.query.result.smoothCoverage);
		}
		return base.SampleVisibility();
	}
}
