using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000244 RID: 580
public class ArticulatedOccludee : BaseMonoBehaviour
{
	// Token: 0x04000E1A RID: 3610
	private const float UpdateBoundsFadeStart = 20f;

	// Token: 0x04000E1B RID: 3611
	private const float UpdateBoundsFadeLength = 1000f;

	// Token: 0x04000E1C RID: 3612
	private const float UpdateBoundsMaxFrequency = 15f;

	// Token: 0x04000E1D RID: 3613
	private const float UpdateBoundsMinFrequency = 0.5f;

	// Token: 0x04000E1E RID: 3614
	private LODGroup lodGroup;

	// Token: 0x04000E1F RID: 3615
	public List<Collider> colliders = new List<Collider>();

	// Token: 0x04000E20 RID: 3616
	private OccludeeSphere localOccludee = new OccludeeSphere(-1);

	// Token: 0x04000E21 RID: 3617
	private List<Renderer> renderers = new List<Renderer>();

	// Token: 0x04000E22 RID: 3618
	private bool isVisible = true;

	// Token: 0x04000E23 RID: 3619
	private Action TriggerUpdateVisibilityBoundsDelegate;

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06001149 RID: 4425 RVA: 0x0000F3DD File Offset: 0x0000D5DD
	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x0000F3E5 File Offset: 0x0000D5E5
	protected virtual void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.UnregisterFromCulling();
		this.ClearVisibility();
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x0007315C File Offset: 0x0007135C
	public void ClearVisibility()
	{
		if (this.lodGroup != null)
		{
			this.lodGroup.localReferencePoint = Vector3.zero;
			this.lodGroup.RecalculateBounds();
			this.lodGroup = null;
		}
		if (this.renderers != null)
		{
			this.renderers.Clear();
		}
		this.localOccludee = new OccludeeSphere(-1);
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x000731B8 File Offset: 0x000713B8
	public void ProcessVisibility(LODGroup lod)
	{
		this.lodGroup = lod;
		if (lod != null)
		{
			this.renderers = new List<Renderer>(16);
			LOD[] lods = lod.GetLODs();
			for (int i = 0; i < lods.Length; i++)
			{
				foreach (Renderer renderer in lods[i].renderers)
				{
					if (renderer != null)
					{
						this.renderers.Add(renderer);
					}
				}
			}
		}
		this.UpdateCullingBounds();
	}

	// Token: 0x0600114D RID: 4429 RVA: 0x00073234 File Offset: 0x00071434
	private void RegisterForCulling(OcclusionCulling.Sphere sphere, bool visible)
	{
		if (this.localOccludee.IsRegistered)
		{
			this.UnregisterFromCulling();
		}
		int num = OcclusionCulling.RegisterOccludee(sphere.position, sphere.radius, visible, 0.25f, false, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
		if (num >= 0)
		{
			this.localOccludee = new OccludeeSphere(num, this.localOccludee.sphere);
			return;
		}
		this.localOccludee.Invalidate();
		Debug.LogWarning("[OcclusionCulling] Occludee registration failed for " + base.name + ". Too many registered.");
	}

	// Token: 0x0600114E RID: 4430 RVA: 0x0000F3FB File Offset: 0x0000D5FB
	private void UnregisterFromCulling()
	{
		if (this.localOccludee.IsRegistered)
		{
			OcclusionCulling.UnregisterOccludee(this.localOccludee.id);
			this.localOccludee.Invalidate();
		}
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x000732C8 File Offset: 0x000714C8
	public void UpdateCullingBounds()
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		bool flag = false;
		int num = (this.renderers != null) ? this.renderers.Count : 0;
		int num2 = (this.colliders != null) ? this.colliders.Count : 0;
		if (num > 0 && (num2 == 0 || num < num2))
		{
			for (int i = 0; i < this.renderers.Count; i++)
			{
				if (this.renderers[i].isVisible)
				{
					Bounds bounds = this.renderers[i].bounds;
					Vector3 min = bounds.min;
					Vector3 max = bounds.max;
					if (!flag)
					{
						vector = min;
						vector2 = max;
						flag = true;
					}
					else
					{
						vector.x = ((vector.x < min.x) ? vector.x : min.x);
						vector.y = ((vector.y < min.y) ? vector.y : min.y);
						vector.z = ((vector.z < min.z) ? vector.z : min.z);
						vector2.x = ((vector2.x > max.x) ? vector2.x : max.x);
						vector2.y = ((vector2.y > max.y) ? vector2.y : max.y);
						vector2.z = ((vector2.z > max.z) ? vector2.z : max.z);
					}
				}
			}
		}
		if (!flag && num2 > 0)
		{
			flag = true;
			vector = this.colliders[0].bounds.min;
			vector2 = this.colliders[0].bounds.max;
			for (int j = 1; j < this.colliders.Count; j++)
			{
				Bounds bounds2 = this.colliders[j].bounds;
				Vector3 min2 = bounds2.min;
				Vector3 max2 = bounds2.max;
				vector.x = ((vector.x < min2.x) ? vector.x : min2.x);
				vector.y = ((vector.y < min2.y) ? vector.y : min2.y);
				vector.z = ((vector.z < min2.z) ? vector.z : min2.z);
				vector2.x = ((vector2.x > max2.x) ? vector2.x : max2.x);
				vector2.y = ((vector2.y > max2.y) ? vector2.y : max2.y);
				vector2.z = ((vector2.z > max2.z) ? vector2.z : max2.z);
			}
		}
		if (flag)
		{
			Vector3 vector3 = vector2 - vector;
			Vector3 position = vector + vector3 * 0.5f;
			float radius = Mathf.Max(Mathf.Max(vector3.x, vector3.y), vector3.z) * 0.5f;
			OcclusionCulling.Sphere sphere = new OcclusionCulling.Sphere(position, radius);
			if (this.localOccludee.IsRegistered)
			{
				OcclusionCulling.UpdateDynamicOccludee(this.localOccludee.id, sphere.position, sphere.radius);
				this.localOccludee.sphere = sphere;
				return;
			}
			bool visible = true;
			if (this.lodGroup != null)
			{
				visible = this.lodGroup.enabled;
			}
			this.RegisterForCulling(sphere, visible);
		}
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x0000F425 File Offset: 0x0000D625
	protected virtual bool CheckVisibility()
	{
		return this.localOccludee.state == null || this.localOccludee.state.isVisible;
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x00073690 File Offset: 0x00071890
	private void ApplyVisibility(bool vis)
	{
		if (this.lodGroup != null)
		{
			float num = (float)(vis ? 0 : 100000);
			if (num != this.lodGroup.localReferencePoint.x)
			{
				this.lodGroup.localReferencePoint = new Vector3(num, num, num);
			}
		}
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x000736E0 File Offset: 0x000718E0
	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (MainCamera.mainCamera != null && this.localOccludee.IsRegistered)
		{
			float dist = Vector3.Distance(MainCamera.mainCamera.transform.position, base.transform.position);
			this.VisUpdateUsingCulling(dist, visible);
			this.ApplyVisibility(this.isVisible);
		}
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x0007373C File Offset: 0x0007193C
	private void UpdateVisibility(float delay)
	{
		if (Culling.toggle && MainCamera.mainCamera != null)
		{
			float dist = Vector3.Distance(MainCamera.mainCamera.transform.position, base.transform.position);
			this.VisUpdateUsingCulling(dist, this.CheckVisibility());
			if (OcclusionCulling.DebugFilterIsDynamic(Culling.debug) && this.localOccludee.IsRegistered)
			{
				OcclusionCulling.Sphere sphere = this.localOccludee.sphere;
				Color color = this.isVisible ? Color.green : Color.red;
				UnityEngine.DDraw.SphereGizmo(sphere.position, sphere.radius, color, delay, false, false);
			}
		}
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x000737DC File Offset: 0x000719DC
	private void VisUpdateUsingCulling(float dist, bool visibility)
	{
		this.UpdateCullingBounds();
		float entityMinCullDist = Culling.entityMinCullDist;
		float entityMaxDist = Culling.entityMaxDist;
		bool flag = dist <= entityMaxDist && (dist <= entityMinCullDist || visibility);
		this.isVisible = flag;
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x00073818 File Offset: 0x00071A18
	public virtual void TriggerUpdateVisibilityBounds()
	{
		if (base.enabled)
		{
			float sqrMagnitude = (base.transform.position - MainCamera.mainCamera.transform.position).sqrMagnitude;
			float num = 400f;
			float num2;
			if (sqrMagnitude < num)
			{
				num2 = 1f / Random.Range(5f, 25f);
			}
			else
			{
				float t = Mathf.Clamp01((Mathf.Sqrt(sqrMagnitude) - 20f) * 0.001f);
				float num3 = Mathf.Lerp(0.06666667f, 2f, t);
				num2 = Random.Range(num3, num3 + 0.06666667f);
			}
			this.UpdateVisibility(num2);
			this.ApplyVisibility(this.isVisible);
			if (this.TriggerUpdateVisibilityBoundsDelegate == null)
			{
				this.TriggerUpdateVisibilityBoundsDelegate = new Action(this.TriggerUpdateVisibilityBounds);
			}
			base.Invoke(this.TriggerUpdateVisibilityBoundsDelegate, num2);
		}
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x000738EC File Offset: 0x00071AEC
	protected virtual void OnDrawGizmos()
	{
		if (Culling.toggle && this.localOccludee.IsRegistered)
		{
			Gizmos.color = (this.localOccludee.state.isVisible ? Color.blue : Color.red);
			Gizmos.DrawWireSphere(this.localOccludee.sphere.position, this.localOccludee.sphere.radius);
		}
	}
}
