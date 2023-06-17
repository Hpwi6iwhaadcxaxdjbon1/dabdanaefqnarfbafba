using System;
using UnityEngine;

// Token: 0x02000305 RID: 773
public class LifeScale : BaseMonoBehaviour
{
	// Token: 0x04001125 RID: 4389
	[NonSerialized]
	private bool initialized;

	// Token: 0x04001126 RID: 4390
	[NonSerialized]
	private Vector3 initialScale;

	// Token: 0x04001127 RID: 4391
	public Vector3 finalScale = Vector3.one;

	// Token: 0x04001128 RID: 4392
	private Vector3 targetLerpScale = Vector3.zero;

	// Token: 0x04001129 RID: 4393
	private Action updateScaleAction;

	// Token: 0x06001442 RID: 5186 RVA: 0x00011410 File Offset: 0x0000F610
	protected void Awake()
	{
		this.updateScaleAction = new Action(this.UpdateScale);
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x00011424 File Offset: 0x0000F624
	public void OnEnable()
	{
		this.Init();
		base.transform.localScale = this.initialScale;
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x0001143D File Offset: 0x0000F63D
	public void SetProgress(float progress)
	{
		this.Init();
		this.targetLerpScale = Vector3.Lerp(this.initialScale, this.finalScale, progress);
		base.InvokeRepeating(this.updateScaleAction, 0f, 0.015f);
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x00011473 File Offset: 0x0000F673
	public void Init()
	{
		if (!this.initialized)
		{
			this.initialScale = base.transform.localScale;
			this.initialized = true;
		}
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x0007DCDC File Offset: 0x0007BEDC
	public void UpdateScale()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.targetLerpScale, Time.deltaTime);
		if (base.transform.localScale == this.targetLerpScale)
		{
			this.targetLerpScale = Vector3.zero;
			base.CancelInvoke(this.updateScaleAction);
		}
	}
}
