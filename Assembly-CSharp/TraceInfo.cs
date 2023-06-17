using System;
using UnityEngine;

// Token: 0x0200023D RID: 573
public struct TraceInfo
{
	// Token: 0x04000DE9 RID: 3561
	public bool valid;

	// Token: 0x04000DEA RID: 3562
	public float distance;

	// Token: 0x04000DEB RID: 3563
	public BaseEntity entity;

	// Token: 0x04000DEC RID: 3564
	public Vector3 point;

	// Token: 0x04000DED RID: 3565
	public Vector3 normal;

	// Token: 0x04000DEE RID: 3566
	public Transform bone;

	// Token: 0x04000DEF RID: 3567
	public PhysicMaterial material;

	// Token: 0x04000DF0 RID: 3568
	public uint partID;

	// Token: 0x04000DF1 RID: 3569
	public Collider collider;

	// Token: 0x06001133 RID: 4403 RVA: 0x00072654 File Offset: 0x00070854
	public void UpdateHitTest(HitTest test)
	{
		test.DidHit = true;
		test.HitEntity = this.entity;
		test.HitDistance = this.distance;
		test.HitMaterial = ((this.material != null) ? this.material.GetName() : "generic");
		test.HitPart = this.partID;
		test.HitTransform = this.bone;
		test.HitPoint = this.point;
		test.HitNormal = this.normal;
		test.collider = this.collider;
		test.gameObject = (this.collider ? this.collider.gameObject : test.HitTransform.gameObject);
		if (test.HitTransform != null)
		{
			test.HitPoint = test.HitTransform.InverseTransformPoint(this.point);
			test.HitNormal = test.HitTransform.InverseTransformDirection(this.normal);
		}
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x0000F2D7 File Offset: 0x0000D4D7
	public uint BoneID()
	{
		if (this.bone != null)
		{
			return StringPool.Get(this.bone.name);
		}
		return 0U;
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x0000F2F9 File Offset: 0x0000D4F9
	public uint MaterialID()
	{
		if (this.material != null)
		{
			return StringPool.Get(this.material.GetName());
		}
		return 0U;
	}
}
