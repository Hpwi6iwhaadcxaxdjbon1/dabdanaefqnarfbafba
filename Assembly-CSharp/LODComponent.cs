using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020003BF RID: 959
public abstract class LODComponent : BaseMonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x040014B6 RID: 5302
	public LODDistanceMode DistanceMode;

	// Token: 0x040014B7 RID: 5303
	public LODComponent.OccludeeParameters OccludeeParams = new LODComponent.OccludeeParameters
	{
		isDynamic = false,
		dynamicUpdateInterval = 0.2f,
		shadowRangeScale = 3f,
		showBounds = false
	};

	// Token: 0x040014B8 RID: 5304
	protected Transform cachedTransform;

	// Token: 0x040014B9 RID: 5305
	protected Impostor impostor;

	// Token: 0x040014BA RID: 5306
	private bool culled;

	// Token: 0x040014BB RID: 5307
	private LODCell cell;

	// Token: 0x040014BC RID: 5308
	private float currentDistance = 1000f;

	// Token: 0x040014BD RID: 5309
	private bool occludeeCulled;

	// Token: 0x040014BE RID: 5310
	private bool occludeeShadowsVisible = true;

	// Token: 0x040014BF RID: 5311
	private float occludeeShadowRange = 50f;

	// Token: 0x040014C0 RID: 5312
	private OccludeeSphere occludee = new OccludeeSphere(-1);

	// Token: 0x040014C1 RID: 5313
	private const float OccludeeMinTimeVisible = 0.1f;

	// Token: 0x040014C2 RID: 5314
	private static HashSet<LODComponent> occludeeSet = new HashSet<LODComponent>();

	// Token: 0x040014C3 RID: 5315
	private static readonly int DynamicOccludeeLowPerFrame = 50;

	// Token: 0x040014C4 RID: 5316
	private static readonly float DynamicOccludeeMinimumLowInterval = 0.2f;

	// Token: 0x040014C5 RID: 5317
	private static ListHashSet<LODComponent> dynamicOccludees = new ListHashSet<LODComponent>(8);

	// Token: 0x040014C6 RID: 5318
	private static int dynamicOccludeeLowIndex = 0;

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06001813 RID: 6163 RVA: 0x00014160 File Offset: 0x00012360
	public static HashSet<LODComponent> OccludeeSet
	{
		get
		{
			return LODComponent.occludeeSet;
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06001814 RID: 6164 RVA: 0x00014167 File Offset: 0x00012367
	public float CurrentDistance
	{
		get
		{
			return this.currentDistance;
		}
	}

	// Token: 0x06001815 RID: 6165 RVA: 0x0001416F File Offset: 0x0001236F
	protected void Awake()
	{
		this.cachedTransform = base.GetComponent<Transform>();
		this.InitLOD();
	}

	// Token: 0x06001816 RID: 6166 RVA: 0x00014183 File Offset: 0x00012383
	protected void OnEnable()
	{
		this.EnableLOD();
		if (!this.OccludeeParams.isDynamic)
		{
			LODGrid.Add(this, base.transform, ref this.cell);
		}
		else
		{
			LODManager.Add(this, base.transform);
		}
		this.EnableCulling();
	}

	// Token: 0x06001817 RID: 6167 RVA: 0x0008C330 File Offset: 0x0008A530
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.DisableLOD();
		if (!this.OccludeeParams.isDynamic)
		{
			LODGrid.Remove(this, base.transform, ref this.cell);
		}
		else
		{
			LODManager.Remove(this, base.transform);
		}
		this.DisableCulling();
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x000141BE File Offset: 0x000123BE
	private void EnableCulling()
	{
		LODComponent.occludeeSet.Add(this);
		this.ChangeCulling();
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x000141D2 File Offset: 0x000123D2
	private void DisableCulling()
	{
		LODComponent.occludeeSet.Remove(this);
		this.UnregisterFromCulling();
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x0008C380 File Offset: 0x0008A580
	public void RefreshLOD()
	{
		if (!this.OccludeeParams.isDynamic)
		{
			LODGrid.Refresh(this, base.transform, ref this.cell);
		}
		else
		{
			this.ChangeLOD();
		}
		if (this.impostor != null)
		{
			this.impostor.UpdateInstance();
		}
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x0008C3D0 File Offset: 0x0008A5D0
	public void ChangeLOD()
	{
		if (this.cell)
		{
			this.currentDistance = this.cell.GetDistance(base.transform, this.DistanceMode);
		}
		else
		{
			this.currentDistance = LODUtil.GetDistance(base.transform, this.DistanceMode);
		}
		if (!this.culled && !this.occludeeCulled)
		{
			int lod = this.GetLOD(this.currentDistance);
			this.SetLOD(lod);
		}
		this.ChangeCulling(this.currentDistance);
	}

	// Token: 0x0600181C RID: 6172 RVA: 0x0008C450 File Offset: 0x0008A650
	private void ChangeCulling(float distance)
	{
		if (Culling.env && base.enabled && !this.culled && !this.IsLODHiding())
		{
			this.RegisterToCulling(true);
		}
		else
		{
			this.UnregisterFromCulling();
		}
		bool flag = !this.occludeeCulled || distance <= this.occludeeShadowRange;
		if (flag != this.occludeeShadowsVisible)
		{
			if (this.occludeeCulled)
			{
				this.Hide(flag);
			}
			this.occludeeShadowsVisible = flag;
		}
		if (this.impostor != null)
		{
			this.impostor.UpdateInstance();
		}
	}

	// Token: 0x0600181D RID: 6173 RVA: 0x000141E6 File Offset: 0x000123E6
	private void ChangeCulling()
	{
		this.ChangeCulling(this.currentDistance);
	}

	// Token: 0x0600181E RID: 6174 RVA: 0x0008C4DC File Offset: 0x0008A6DC
	public static void ChangeCullingAll()
	{
		LODComponent.OccludeeSet.RemoveWhere((LODComponent i) => i == null);
		foreach (LODComponent lodcomponent in LODComponent.OccludeeSet)
		{
			lodcomponent.ChangeCulling();
		}
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x0008C558 File Offset: 0x0008A758
	private void UpdateVisibility()
	{
		if (!this.culled && !this.occludeeCulled)
		{
			this.currentDistance = LODUtil.GetDistance(base.transform, this.DistanceMode);
			this.Show();
			this.SetLOD(this.GetLOD(this.currentDistance));
			return;
		}
		this.Hide(this.occludeeShadowsVisible);
	}

	// Token: 0x06001820 RID: 6176 RVA: 0x000141F4 File Offset: 0x000123F4
	public void SetVisible(bool state)
	{
		this.culled = !state;
		this.UpdateVisibility();
	}

	// Token: 0x06001821 RID: 6177
	protected abstract void InitLOD();

	// Token: 0x06001822 RID: 6178
	protected abstract void EnableLOD();

	// Token: 0x06001823 RID: 6179
	protected abstract void DisableLOD();

	// Token: 0x06001824 RID: 6180
	protected abstract int GetLOD(float distance);

	// Token: 0x06001825 RID: 6181
	protected abstract void SetLOD(int newlod);

	// Token: 0x06001826 RID: 6182
	protected abstract void Show();

	// Token: 0x06001827 RID: 6183
	protected abstract void Hide();

	// Token: 0x06001828 RID: 6184 RVA: 0x00014206 File Offset: 0x00012406
	protected virtual void Hide(bool shadowsVisible)
	{
		this.Hide();
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x00002D44 File Offset: 0x00000F44
	protected virtual bool IsLODHiding()
	{
		return true;
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x0001420E File Offset: 0x0001240E
	public void WorkshopMode()
	{
		this.SetLOD(0);
		base.enabled = false;
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x0001421E File Offset: 0x0001241E
	protected virtual bool ComputeCullingSphereBounds(out OcclusionCulling.Sphere sphereBounds)
	{
		sphereBounds = default(OcclusionCulling.Sphere);
		return false;
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x00014228 File Offset: 0x00012428
	private void UpdateShadowRange()
	{
		this.occludeeShadowRange = this.OccludeeParams.shadowRangeScale * (this.occludee.sphere.position.y + this.occludee.sphere.radius);
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x00014262 File Offset: 0x00012462
	public static void ClearOccludees()
	{
		LODComponent.occludeeSet.Clear();
		LODComponent.dynamicOccludees.Clear();
		LODComponent.dynamicOccludeeLowIndex = 0;
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x0008C5B4 File Offset: 0x0008A7B4
	private static void UpdateDynamicOccludeeBounds(LODComponent lodcomp)
	{
		OcclusionCulling.Sphere sphere;
		if (lodcomp.ComputeCullingSphereBounds(out sphere))
		{
			sphere.position -= lodcomp.cachedTransform.position;
			lodcomp.occludee.sphere = sphere;
			lodcomp.UpdateShadowRange();
		}
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x0008C600 File Offset: 0x0008A800
	public static void UpdateDynamicOccludees()
	{
		if (LODComponent.dynamicOccludees.Count > 0)
		{
			for (int i = 0; i < LODComponent.DynamicOccludeeLowPerFrame; i++)
			{
				LODComponent lodcomponent = LODComponent.dynamicOccludees[LODComponent.dynamicOccludeeLowIndex++];
				if (lodcomponent != null && lodcomponent.OccludeeParams.dynamicUpdateInterval >= LODComponent.DynamicOccludeeMinimumLowInterval)
				{
					LODComponent.UpdateDynamicOccludeeBounds(lodcomponent);
				}
				LODComponent.dynamicOccludeeLowIndex = ((LODComponent.dynamicOccludeeLowIndex < LODComponent.dynamicOccludees.Count) ? LODComponent.dynamicOccludeeLowIndex : 0);
			}
			for (int j = 0; j < LODComponent.dynamicOccludees.Count; j++)
			{
				LODComponent lodcomponent2 = LODComponent.dynamicOccludees[j];
				if (lodcomponent2 != null)
				{
					if (lodcomponent2.OccludeeParams.dynamicUpdateInterval < LODComponent.DynamicOccludeeMinimumLowInterval)
					{
						LODComponent.UpdateDynamicOccludeeBounds(lodcomponent2);
					}
					OccludeeSphere occludeeSphere = lodcomponent2.occludee;
					Vector3 center = lodcomponent2.cachedTransform.position + occludeeSphere.sphere.position;
					float radius = occludeeSphere.sphere.radius;
					OcclusionCulling.UpdateDynamicOccludee(occludeeSphere.id, center, radius);
				}
			}
		}
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x0008C710 File Offset: 0x0008A910
	public void ResetCulling()
	{
		if (this.occludee.IsRegistered)
		{
			bool isVisible = !this.occludeeCulled;
			this.UnregisterFromCulling();
			this.RegisterToCulling(isVisible);
		}
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x0008C744 File Offset: 0x0008A944
	protected virtual void RegisterToCulling(bool isVisible = true)
	{
		if (!this.occludee.IsRegistered && this.ComputeCullingSphereBounds(out this.occludee.sphere))
		{
			int num = OcclusionCulling.RegisterOccludee(this.occludee.sphere.position, this.occludee.sphere.radius, isVisible, 0.1f, !this.OccludeeParams.isDynamic, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
			if (num >= 0)
			{
				this.occludee = new OccludeeSphere(num, this.occludee.sphere);
				if (this.OccludeeParams.isDynamic)
				{
					LODComponent.dynamicOccludees.Add(this);
				}
				this.occludeeCulled = false;
				this.occludeeShadowsVisible = true;
				this.UpdateShadowRange();
				return;
			}
			this.occludee.Invalidate();
		}
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x0008C81C File Offset: 0x0008AA1C
	protected virtual void UnregisterFromCulling()
	{
		if (this.occludee.IsRegistered)
		{
			LODComponent.dynamicOccludees.Remove(this);
			OcclusionCulling.UnregisterOccludee(this.occludee.id);
			this.occludeeCulled = false;
			this.occludeeShadowsVisible = true;
			this.occludee.Invalidate();
			this.UpdateVisibility();
		}
	}

	// Token: 0x06001833 RID: 6195 RVA: 0x0008C874 File Offset: 0x0008AA74
	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (this.occludee.IsRegistered)
		{
			this.occludeeCulled = (!visible && this.currentDistance > Culling.envMinDist);
			if (!this.culled && !this.occludeeCulled)
			{
				this.Show();
				this.SetLOD(this.GetLOD(this.currentDistance));
			}
			else
			{
				this.occludeeShadowsVisible = (!this.occludeeCulled || this.currentDistance <= this.occludeeShadowRange);
				this.Hide(this.occludeeShadowsVisible);
			}
			this.ChangeCulling(this.currentDistance);
		}
	}

	// Token: 0x020003C0 RID: 960
	[Serializable]
	public struct OccludeeParameters
	{
		// Token: 0x040014C7 RID: 5319
		[Tooltip("Is Occludee dynamic or static?")]
		public bool isDynamic;

		// Token: 0x040014C8 RID: 5320
		[Tooltip("Dynamic occludee update interval in seconds; 0 = every frame")]
		public float dynamicUpdateInterval;

		// Token: 0x040014C9 RID: 5321
		[Tooltip("Distance scale combined with occludee max bounds size at which culled occludee shadows are still visible")]
		public float shadowRangeScale;

		// Token: 0x040014CA RID: 5322
		[Tooltip("Show culling bounds via gizmos; editor only")]
		public bool showBounds;
	}
}
