using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;
using VLB;

// Token: 0x02000709 RID: 1801
public class LightLOD : MonoBehaviour, ILOD, IClientComponent
{
	// Token: 0x04002366 RID: 9062
	public float DistanceBias;

	// Token: 0x04002367 RID: 9063
	public bool ToggleLight;

	// Token: 0x04002368 RID: 9064
	public bool ToggleShadows = true;

	// Token: 0x04002369 RID: 9065
	private static List<LightLOD> pointLights = new List<LightLOD>();

	// Token: 0x0400236A RID: 9066
	private static List<LightLOD> spotLights = new List<LightLOD>();

	// Token: 0x0400236B RID: 9067
	private Light lightComponent;

	// Token: 0x0400236C RID: 9068
	private LightOccludee lightOccludee;

	// Token: 0x0400236D RID: 9069
	private VolumetricLightBeam volumetricBeam;

	// Token: 0x0400236E RID: 9070
	private float cameraDist;

	// Token: 0x0400236F RID: 9071
	private float fadeSign;

	// Token: 0x04002370 RID: 9072
	private float maxLightIntensity;

	// Token: 0x04002371 RID: 9073
	private float maxShadowStrength;

	// Token: 0x06002785 RID: 10117 RVA: 0x0001ED93 File Offset: 0x0001CF93
	private int GetMaxLightCount()
	{
		if (this.lightComponent.type == LightType.Point)
		{
			return Mathf.Max(0, ConVar.Graphics.shadowlights);
		}
		return Mathf.Max(0, ConVar.Graphics.shadowlights) * 3;
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x0001EDBC File Offset: 0x0001CFBC
	private List<LightLOD> GetCurrentLights()
	{
		if (this.lightComponent.type == LightType.Point)
		{
			return LightLOD.pointLights;
		}
		return LightLOD.spotLights;
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x0001EDD7 File Offset: 0x0001CFD7
	private void FadeIn()
	{
		this.fadeSign = 1f;
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x0001EDE4 File Offset: 0x0001CFE4
	private void FadeOut()
	{
		this.fadeSign = -1f;
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x000CCFE0 File Offset: 0x000CB1E0
	protected void Awake()
	{
		this.lightComponent = base.GetComponent<Light>();
		this.volumetricBeam = base.GetComponent<VolumetricLightBeam>();
		this.lightOccludee = base.GetComponent<LightOccludee>();
		this.maxLightIntensity = this.lightComponent.intensity;
		this.maxShadowStrength = this.lightComponent.shadowStrength;
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x000CD034 File Offset: 0x000CB234
	protected void OnEnable()
	{
		LODManager.Add(this, base.transform);
		if (this.GetCurrentLights().Contains(this))
		{
			this.FadeIn();
			this.UpdateLight(1f, 1f);
			return;
		}
		this.FadeOut();
		this.UpdateLight(0f, 0f);
	}

	// Token: 0x0600278B RID: 10123 RVA: 0x000CD088 File Offset: 0x000CB288
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		LODManager.Remove(this, base.transform);
		List<LightLOD> currentLights = this.GetCurrentLights();
		if (currentLights.Contains(this))
		{
			currentLights.Remove(this);
		}
		this.FadeOut();
		this.UpdateLight(0f, 0f);
	}

	// Token: 0x0600278C RID: 10124 RVA: 0x000CD0D8 File Offset: 0x000CB2D8
	private void UpdateLight(float lightIntensity, float shadowIntensity)
	{
		if (this.ToggleLight)
		{
			this.lightComponent.intensity = Mathf.Clamp(lightIntensity, 0f, this.maxLightIntensity);
		}
		else if (this.ToggleShadows)
		{
			this.lightComponent.shadowStrength = Mathf.Clamp(shadowIntensity, 0f, this.maxShadowStrength);
		}
		if (this.ToggleLight)
		{
			bool flag = this.lightComponent.intensity > 0f;
			if (this.lightOccludee != null)
			{
				this.lightOccludee.SetLODVisible(flag);
				flag = this.lightOccludee.IsVisible;
			}
			else
			{
				this.lightComponent.enabled = flag;
				if (this.volumetricBeam != null)
				{
					this.volumetricBeam.enabled = flag;
				}
			}
		}
		if (this.ToggleShadows)
		{
			this.lightComponent.shadows = ((this.lightComponent.intensity == 0f || this.lightComponent.shadowStrength == 0f) ? LightShadows.None : LightShadows.Soft);
		}
	}

	// Token: 0x0600278D RID: 10125 RVA: 0x00002ECE File Offset: 0x000010CE
	public void RefreshLOD()
	{
	}

	// Token: 0x0600278E RID: 10126 RVA: 0x000CD1D4 File Offset: 0x000CB3D4
	public void ChangeLOD()
	{
		this.cameraDist = LODUtil.GetDistance(base.transform, LODDistanceMode.XYZ) + this.DistanceBias;
		int maxLightCount = this.GetMaxLightCount();
		List<LightLOD> currentLights = this.GetCurrentLights();
		while (currentLights.Count > maxLightCount)
		{
			currentLights[currentLights.Count - 1].FadeOut();
			currentLights.RemoveAt(currentLights.Count - 1);
		}
		if (currentLights.Contains(this))
		{
			return;
		}
		if (currentLights.Count < maxLightCount)
		{
			this.FadeIn();
			currentLights.Add(this);
			return;
		}
		for (int i = 0; i < currentLights.Count; i++)
		{
			LightLOD lightLOD = currentLights[i];
			if (this.cameraDist < lightLOD.cameraDist)
			{
				lightLOD.FadeOut();
				this.FadeIn();
				currentLights[i] = this;
				return;
			}
		}
	}

	// Token: 0x0600278F RID: 10127 RVA: 0x0001EDF1 File Offset: 0x0001CFF1
	protected void Update()
	{
		this.UpdateLight(this.lightComponent.intensity + this.fadeSign * UnityEngine.Time.deltaTime, this.lightComponent.shadowStrength + this.fadeSign * UnityEngine.Time.deltaTime);
	}

	// Token: 0x06002790 RID: 10128 RVA: 0x0000E19C File Offset: 0x0000C39C
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}
}
