using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class SocketMod_EntityCheck : SocketMod
{
	// Token: 0x04000BC3 RID: 3011
	public float sphereRadius = 1f;

	// Token: 0x04000BC4 RID: 3012
	public LayerMask layerMask;

	// Token: 0x04000BC5 RID: 3013
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x04000BC6 RID: 3014
	public BaseEntity[] entityTypes;

	// Token: 0x04000BC7 RID: 3015
	public bool wantsCollide;

	// Token: 0x06000EAF RID: 3759 RVA: 0x000661F8 File Offset: 0x000643F8
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x00066268 File Offset: 0x00064468
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		using (List<BaseEntity>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BaseEntity ent = enumerator.Current;
				bool flag = Enumerable.Any<BaseEntity>(this.entityTypes, (BaseEntity x) => x.prefabID == ent.prefabID);
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
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}
}
