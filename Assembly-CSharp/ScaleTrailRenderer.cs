using System;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class ScaleTrailRenderer : ScaleRenderer
{
	// Token: 0x04000F25 RID: 3877
	private TrailRenderer trailRenderer;

	// Token: 0x04000F26 RID: 3878
	[NonSerialized]
	private float startWidth;

	// Token: 0x04000F27 RID: 3879
	[NonSerialized]
	private float endWidth;

	// Token: 0x04000F28 RID: 3880
	[NonSerialized]
	private float duration;

	// Token: 0x0600127C RID: 4732 RVA: 0x00078EA4 File Offset: 0x000770A4
	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		if (this.myRenderer)
		{
			this.trailRenderer = this.myRenderer.GetComponent<TrailRenderer>();
		}
		else
		{
			this.trailRenderer = base.GetComponentInChildren<TrailRenderer>();
		}
		this.startWidth = this.trailRenderer.startWidth;
		this.endWidth = this.trailRenderer.endWidth;
		this.duration = this.trailRenderer.time;
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x00078F18 File Offset: 0x00077118
	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.trailRenderer.startWidth = this.startWidth * scale;
		this.trailRenderer.endWidth = this.endWidth * scale;
		this.trailRenderer.time = this.duration * scale;
	}
}
