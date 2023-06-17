using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class SocketMod_PlantCheck : SocketMod
{
	// Token: 0x04000BD0 RID: 3024
	public float sphereRadius = 1f;

	// Token: 0x04000BD1 RID: 3025
	public LayerMask layerMask;

	// Token: 0x04000BD2 RID: 3026
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x04000BD3 RID: 3027
	public bool wantsCollide;

	// Token: 0x06000EBD RID: 3773 RVA: 0x00066528 File Offset: 0x00064728
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x00066598 File Offset: 0x00064798
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		foreach (BaseEntity baseEntity in list)
		{
			PlantEntity component = baseEntity.GetComponent<PlantEntity>();
			if (component && this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
			if (component && !this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return false;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}
}
