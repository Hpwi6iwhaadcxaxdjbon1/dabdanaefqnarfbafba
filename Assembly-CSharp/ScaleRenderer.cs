using System;
using UnityEngine;

// Token: 0x02000289 RID: 649
public class ScaleRenderer : MonoBehaviour
{
	// Token: 0x04000F1F RID: 3871
	public bool useRandomScale;

	// Token: 0x04000F20 RID: 3872
	public float scaleMin = 1f;

	// Token: 0x04000F21 RID: 3873
	public float scaleMax = 1f;

	// Token: 0x04000F22 RID: 3874
	private float lastScale = -1f;

	// Token: 0x04000F23 RID: 3875
	protected bool hasInitialValues;

	// Token: 0x04000F24 RID: 3876
	public Renderer myRenderer;

	// Token: 0x06001275 RID: 4725 RVA: 0x0000FD75 File Offset: 0x0000DF75
	private bool ScaleDifferent(float newScale)
	{
		return newScale != this.lastScale;
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x0000FD83 File Offset: 0x0000DF83
	public void Start()
	{
		if (this.useRandomScale)
		{
			this.SetScale(Random.Range(this.scaleMin, this.scaleMax));
		}
	}

	// Token: 0x06001277 RID: 4727 RVA: 0x0000FDA4 File Offset: 0x0000DFA4
	public void SetScale(float scale)
	{
		if (!this.hasInitialValues)
		{
			this.GatherInitialValues();
		}
		if (this.ScaleDifferent(scale))
		{
			this.SetRendererEnabled(scale != 0f);
			this.SetScale_Internal(scale);
		}
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x0000FDD5 File Offset: 0x0000DFD5
	public virtual void SetScale_Internal(float scale)
	{
		this.lastScale = scale;
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x0000FDDE File Offset: 0x0000DFDE
	public virtual void SetRendererEnabled(bool isEnabled)
	{
		if (this.myRenderer && this.myRenderer.enabled != isEnabled)
		{
			this.myRenderer.enabled = isEnabled;
		}
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x0000FE07 File Offset: 0x0000E007
	public virtual void GatherInitialValues()
	{
		this.hasInitialValues = true;
	}
}
