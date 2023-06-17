using System;
using UnityEngine;

// Token: 0x020007AF RID: 1967
public class Occludee : MonoBehaviour
{
	// Token: 0x04002653 RID: 9811
	public float minTimeVisible = 0.1f;

	// Token: 0x04002654 RID: 9812
	public bool isStatic = true;

	// Token: 0x04002655 RID: 9813
	public bool autoRegister;

	// Token: 0x04002656 RID: 9814
	public bool stickyGizmos;

	// Token: 0x04002657 RID: 9815
	public OccludeeState state;

	// Token: 0x04002658 RID: 9816
	protected int occludeeId = -1;

	// Token: 0x04002659 RID: 9817
	protected Vector3 center;

	// Token: 0x0400265A RID: 9818
	protected float radius;

	// Token: 0x0400265B RID: 9819
	protected Renderer renderer;

	// Token: 0x0400265C RID: 9820
	protected Collider collider;

	// Token: 0x06002AA6 RID: 10918 RVA: 0x0002134A File Offset: 0x0001F54A
	protected virtual void Awake()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.collider = base.GetComponent<Collider>();
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x00021364 File Offset: 0x0001F564
	public void OnEnable()
	{
		if (this.autoRegister && this.collider != null)
		{
			this.Register();
		}
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x00021382 File Offset: 0x0001F582
	public void OnDisable()
	{
		if (this.autoRegister && this.occludeeId >= 0)
		{
			this.Unregister();
		}
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x000D94B4 File Offset: 0x000D76B4
	public void Register()
	{
		this.center = this.collider.bounds.center;
		this.radius = Mathf.Max(Mathf.Max(this.collider.bounds.extents.x, this.collider.bounds.extents.y), this.collider.bounds.extents.z);
		this.occludeeId = OcclusionCulling.RegisterOccludee(this.center, this.radius, this.renderer.enabled, this.minTimeVisible, this.isStatic, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
		if (this.occludeeId < 0)
		{
			Debug.LogWarning("[OcclusionCulling] Occludee registration failed for " + base.name + ". Too many registered.");
		}
		this.state = OcclusionCulling.GetStateById(this.occludeeId);
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x0002139B File Offset: 0x0001F59B
	public void Unregister()
	{
		OcclusionCulling.UnregisterOccludee(this.occludeeId);
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x000213A8 File Offset: 0x0001F5A8
	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (this.renderer != null)
		{
			this.renderer.enabled = visible;
		}
	}
}
