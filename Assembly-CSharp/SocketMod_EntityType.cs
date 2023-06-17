using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class SocketMod_EntityType : SocketMod
{
	// Token: 0x04000BC9 RID: 3017
	public float sphereRadius = 1f;

	// Token: 0x04000BCA RID: 3018
	public LayerMask layerMask;

	// Token: 0x04000BCB RID: 3019
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x04000BCC RID: 3020
	public BaseEntity searchType;

	// Token: 0x04000BCD RID: 3021
	public bool wantsCollide;

	// Token: 0x06000EB4 RID: 3764 RVA: 0x00066354 File Offset: 0x00064554
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x000663C4 File Offset: 0x000645C4
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		foreach (BaseEntity baseEntity in list)
		{
			bool flag = baseEntity.GetType().IsAssignableFrom(this.searchType.GetType());
			if (flag && this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
			if (flag && !this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return false;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}
}
