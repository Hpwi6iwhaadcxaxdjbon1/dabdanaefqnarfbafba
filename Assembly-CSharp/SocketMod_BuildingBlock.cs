using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020001BD RID: 445
public class SocketMod_BuildingBlock : SocketMod
{
	// Token: 0x04000BBF RID: 3007
	public float sphereRadius = 1f;

	// Token: 0x04000BC0 RID: 3008
	public LayerMask layerMask;

	// Token: 0x04000BC1 RID: 3009
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x04000BC2 RID: 3010
	public bool wantsCollide;

	// Token: 0x06000EAC RID: 3756 RVA: 0x000660F8 File Offset: 0x000642F8
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x00066168 File Offset: 0x00064368
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities<BuildingBlock>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		bool flag = list.Count > 0;
		if (flag && this.wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return true;
		}
		if (flag && !this.wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return false;
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return !this.wantsCollide;
	}
}
