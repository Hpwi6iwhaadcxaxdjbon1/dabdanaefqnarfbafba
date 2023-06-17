using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000552 RID: 1362
[ExecuteInEditMode]
public class WaterCullingVolume : MonoBehaviour
{
	// Token: 0x04001AF6 RID: 6902
	public bool isDynamic;

	// Token: 0x04001AF7 RID: 6903
	private Bounds worldBounds;

	// Token: 0x04001AF8 RID: 6904
	private Vector4[] worldToLocal = new Vector4[3];

	// Token: 0x04001AF9 RID: 6905
	private bool isVisible;

	// Token: 0x04001AFA RID: 6906
	private float distanceToCamera = float.MaxValue;

	// Token: 0x04001AFB RID: 6907
	private static HashSet<WaterCullingVolume> volumes = new HashSet<WaterCullingVolume>();

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x06001E9A RID: 7834 RVA: 0x000182AF File Offset: 0x000164AF
	public Bounds WorldBounds
	{
		get
		{
			return this.worldBounds;
		}
	}

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06001E9B RID: 7835 RVA: 0x000182B7 File Offset: 0x000164B7
	public Vector4[] WorldToLocal
	{
		get
		{
			return this.worldToLocal;
		}
	}

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06001E9C RID: 7836 RVA: 0x000182BF File Offset: 0x000164BF
	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
	}

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x06001E9D RID: 7837 RVA: 0x000182C7 File Offset: 0x000164C7
	public float DistanceToCamera
	{
		get
		{
			return this.distanceToCamera;
		}
	}

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x06001E9E RID: 7838 RVA: 0x000182CF File Offset: 0x000164CF
	public static HashSet<WaterCullingVolume> Volumes
	{
		get
		{
			return WaterCullingVolume.volumes;
		}
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x000A7DB4 File Offset: 0x000A5FB4
	private void UpdateTransformInfo()
	{
		Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
		this.worldBounds = bounds.Transform(base.transform.localToWorldMatrix);
		this.worldToLocal[0] = base.transform.worldToLocalMatrix.GetRow(0);
		this.worldToLocal[1] = base.transform.worldToLocalMatrix.GetRow(1);
		this.worldToLocal[2] = base.transform.worldToLocalMatrix.GetRow(2);
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x000182D6 File Offset: 0x000164D6
	private void OnEnable()
	{
		this.UpdateTransformInfo();
		WaterCullingVolume.volumes.Add(this);
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x000182EA File Offset: 0x000164EA
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		WaterCullingVolume.volumes.Remove(this);
	}

	// Token: 0x06001EA2 RID: 7842 RVA: 0x00018300 File Offset: 0x00016500
	private void Update()
	{
		if (this.isDynamic || (Application.isEditor && !Application.isPlaying))
		{
			this.UpdateTransformInfo();
		}
	}

	// Token: 0x06001EA3 RID: 7843 RVA: 0x000A7E4C File Offset: 0x000A604C
	public bool UpdateVisibility(Plane[] frustumPlanes, Vector3 cameraWorldPos)
	{
		this.isVisible = false;
		this.distanceToCamera = float.MaxValue;
		if (base.enabled)
		{
			this.isVisible = GeometryUtility.TestPlanesAABB(frustumPlanes, this.worldBounds);
			if (this.isVisible)
			{
				this.distanceToCamera = Vector3.Distance(this.worldBounds.center, cameraWorldPos);
			}
		}
		return this.isVisible;
	}

	// Token: 0x06001EA4 RID: 7844 RVA: 0x0001831E File Offset: 0x0001651E
	protected void OnDrawGizmos()
	{
		this.DrawGizmos(0.33f);
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x0001832B File Offset: 0x0001652B
	protected void OnDrawGizmosSelected()
	{
		this.DrawGizmos(0.5f);
	}

	// Token: 0x06001EA6 RID: 7846 RVA: 0x000A7EAC File Offset: 0x000A60AC
	private void DrawGizmos(float alpha)
	{
		Quaternion rotation = base.transform.rotation;
		Vector3 position = base.transform.position;
		Vector3 lossyScale = base.transform.lossyScale;
		Gizmos.color = new Color(0f, 0.75f, 1f, alpha);
		GizmosUtil.DrawCube(position, lossyScale, rotation);
		GizmosUtil.DrawWireCube(position, lossyScale, rotation);
	}
}
