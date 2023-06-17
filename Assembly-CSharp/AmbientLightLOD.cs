using System;
using Rust;
using UnityEngine;
using VLB;

// Token: 0x020006FE RID: 1790
public class AmbientLightLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	// Token: 0x04002340 RID: 9024
	public bool isDynamic;

	// Token: 0x04002341 RID: 9025
	public float enabledRadius = 20f;

	// Token: 0x04002342 RID: 9026
	public bool toggleFade;

	// Token: 0x04002343 RID: 9027
	public float toggleFadeDuration = 0.5f;

	// Token: 0x04002344 RID: 9028
	public bool StickyGizmos;

	// Token: 0x04002345 RID: 9029
	private Light lightComponent;

	// Token: 0x04002346 RID: 9030
	private LightOccludee lightOccludee;

	// Token: 0x04002347 RID: 9031
	private VolumetricLightBeam volumetricBeam;

	// Token: 0x04002348 RID: 9032
	private LODCell cell;

	// Token: 0x04002349 RID: 9033
	private float startIntensity = 1f;

	// Token: 0x0400234A RID: 9034
	private float fadeStartTime;

	// Token: 0x0400234B RID: 9035
	private float fadeEndTime;

	// Token: 0x0400234C RID: 9036
	private bool fadeToState = true;

	// Token: 0x0400234D RID: 9037
	private bool queuedState = true;

	// Token: 0x0600275A RID: 10074 RVA: 0x0001EAE2 File Offset: 0x0001CCE2
	protected void Awake()
	{
		this.lightComponent = base.GetComponent<Light>();
		this.lightOccludee = base.GetComponent<LightOccludee>();
		this.volumetricBeam = base.GetComponent<VolumetricLightBeam>();
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x0001EB08 File Offset: 0x0001CD08
	protected void OnEnable()
	{
		if (!this.isDynamic)
		{
			LODGrid.Add(this, base.transform, ref this.cell);
		}
		else
		{
			LODManager.Add(this, base.transform);
		}
		this.startIntensity = this.lightComponent.intensity;
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x0001EB43 File Offset: 0x0001CD43
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (!this.isDynamic)
		{
			LODGrid.Remove(this, base.transform, ref this.cell);
			return;
		}
		LODManager.Remove(this, base.transform);
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000CC7C8 File Offset: 0x000CA9C8
	private void ToggleLight(bool state)
	{
		if (this.lightOccludee != null)
		{
			this.lightOccludee.SetLODVisible(state);
			return;
		}
		this.lightComponent.enabled = state;
		if (this.volumetricBeam != null)
		{
			this.volumetricBeam.enabled = state;
		}
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x000CC818 File Offset: 0x000CAA18
	private void FadingToggle()
	{
		float time = Time.time;
		float num = this.fadeEndTime - time;
		if (this.queuedState != this.fadeToState)
		{
			this.fadeStartTime = time;
			this.fadeEndTime = this.fadeStartTime + Mathf.Max(0f, this.toggleFadeDuration - num);
			this.fadeToState = this.queuedState;
			return;
		}
		if (this.fadeToState)
		{
			this.ToggleLight(this.fadeToState);
		}
		if (time > this.fadeEndTime)
		{
			if (!this.fadeToState)
			{
				this.ToggleLight(this.fadeToState);
			}
			base.CancelInvoke(new Action(this.FadingToggle));
			return;
		}
		float num2 = 1f - num / this.toggleFadeDuration;
		this.lightComponent.intensity = this.startIntensity * (this.fadeToState ? num2 : (1f - num2));
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x000CC8EC File Offset: 0x000CAAEC
	public void SetLightActive(bool lightOn)
	{
		if (this.toggleFade)
		{
			this.queuedState = lightOn;
			float time = Time.time;
			if (time > this.fadeEndTime && this.queuedState != this.lightComponent.enabled)
			{
				this.fadeStartTime = time;
				this.fadeEndTime = this.fadeStartTime + this.toggleFadeDuration;
				this.fadeToState = lightOn;
				base.InvokeRepeating(new Action(this.FadingToggle), 0.033f, 0.033f);
				return;
			}
		}
		else if (lightOn != this.lightComponent.enabled)
		{
			this.ToggleLight(lightOn);
		}
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x0001EB74 File Offset: 0x0001CD74
	public void RefreshLOD()
	{
		if (!this.isDynamic)
		{
			LODGrid.Refresh(this, base.transform, ref this.cell);
			return;
		}
		this.ChangeLOD();
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000CC980 File Offset: 0x000CAB80
	public void ChangeLOD()
	{
		float distance = LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ);
		this.SetLightActive(distance <= this.enabledRadius);
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x0000E19C File Offset: 0x0000C39C
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}
}
