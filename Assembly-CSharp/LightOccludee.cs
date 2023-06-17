using System;
using UnityEngine;
using VLB;

// Token: 0x0200070A RID: 1802
public class LightOccludee : MonoBehaviour
{
	// Token: 0x04002372 RID: 9074
	public float RadiusScale = 0.5f;

	// Token: 0x04002373 RID: 9075
	public float MinTimeVisible = 0.1f;

	// Token: 0x04002374 RID: 9076
	public bool IsDynamic;

	// Token: 0x04002375 RID: 9077
	public bool StickyGizmos;

	// Token: 0x04002376 RID: 9078
	private Light light;

	// Token: 0x04002377 RID: 9079
	private VolumetricLightBeam volumetricBeam;

	// Token: 0x04002378 RID: 9080
	private OccludeeSphere occludee;

	// Token: 0x04002379 RID: 9081
	private bool lodVisible = true;

	// Token: 0x0400237A RID: 9082
	private bool volumeVisible = true;

	// Token: 0x0400237B RID: 9083
	private bool occludeeCulled;

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x06002793 RID: 10131 RVA: 0x0001EE4E File Offset: 0x0001D04E
	public bool IsVisible
	{
		get
		{
			return !(this.light != null) || this.light.enabled;
		}
	}

	// Token: 0x06002794 RID: 10132 RVA: 0x0001EE6B File Offset: 0x0001D06B
	private void Awake()
	{
		this.light = base.GetComponent<Light>();
		this.volumetricBeam = base.GetComponent<VolumetricLightBeam>();
	}

	// Token: 0x06002795 RID: 10133 RVA: 0x0001EE85 File Offset: 0x0001D085
	private void OnEnable()
	{
		if (!this.IsDynamic && this.light != null)
		{
			this.RegisterToOcclusionCulling();
		}
	}

	// Token: 0x06002796 RID: 10134 RVA: 0x0001EEA3 File Offset: 0x0001D0A3
	private void OnDisable()
	{
		if (!this.IsDynamic && this.light != null)
		{
			this.UnregisterFromOcclusionCulling();
		}
	}

	// Token: 0x06002797 RID: 10135 RVA: 0x000CD294 File Offset: 0x000CB494
	private void UpdateCullingSphere()
	{
		Vector3 position = Vector3.zero;
		float num = 0f;
		if (this.light.type == LightType.Point || this.light.type == LightType.Spot)
		{
			position = base.transform.position;
			num = this.light.range * this.RadiusScale;
			if (this.light.type == LightType.Spot)
			{
				float num2 = 1f / Mathf.Cos(this.light.spotAngle * 0.5f * 0.017453292f);
				num *= num2;
			}
		}
		this.occludee.sphere = new OcclusionCulling.Sphere(position, num);
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x000CD330 File Offset: 0x000CB530
	public void UpdateDynamicOccludee()
	{
		if (this.occludee.IsRegistered)
		{
			this.UpdateCullingSphere();
			OcclusionCulling.UpdateDynamicOccludee(this.occludee.id, this.occludee.sphere.position, this.occludee.sphere.radius);
		}
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x000CD380 File Offset: 0x000CB580
	private void RegisterToOcclusionCulling()
	{
		this.UpdateCullingSphere();
		if (this.occludee.sphere.IsValid())
		{
			this.occludee.id = OcclusionCulling.RegisterOccludee(this.occludee.sphere.position, this.occludee.sphere.radius, this.light.enabled, this.MinTimeVisible, !this.IsDynamic, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
			if (this.occludee.id < 0)
			{
				Debug.LogWarning("[OcclusionCulling] Occludee registration failed for " + base.name + ". Too many registered.");
			}
			this.occludee.state = OcclusionCulling.GetStateById(this.occludee.id);
			return;
		}
		this.occludee.sphere.position = Vector3.zero;
		this.occludee.sphere.radius = 0f;
		Debug.LogWarning(string.Concat(new object[]
		{
			"[LightOccludee] Light type ",
			this.light.type,
			" unsupported as occludee on light ",
			base.name
		}));
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x0001EEC1 File Offset: 0x0001D0C1
	private void UnregisterFromOcclusionCulling()
	{
		OcclusionCulling.UnregisterOccludee(this.occludee.id);
		this.occludee.Invalidate();
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000CD4B4 File Offset: 0x000CB6B4
	private void UpdateVisibility()
	{
		bool enabled = this.lodVisible && this.volumeVisible && !this.occludeeCulled;
		if (this.light != null)
		{
			this.light.enabled = enabled;
		}
		if (this.volumetricBeam != null)
		{
			this.volumetricBeam.enabled = enabled;
		}
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x0001EEDE File Offset: 0x0001D0DE
	public void SetLODVisible(bool state)
	{
		this.lodVisible = state;
		this.UpdateVisibility();
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x0001EEED File Offset: 0x0001D0ED
	public void SetVolumeVisible(bool state)
	{
		this.volumeVisible = state;
		this.UpdateVisibility();
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x0001EEFC File Offset: 0x0001D0FC
	protected virtual void OnVisibilityChanged(bool visible)
	{
		this.occludeeCulled = !visible;
		this.UpdateVisibility();
	}
}
