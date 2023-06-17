using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class GraveyardFence : SimpleBuildingBlock
{
	// Token: 0x0400075E RID: 1886
	public BoxCollider[] pillars;

	// Token: 0x06000B0B RID: 2827 RVA: 0x0000AB97 File Offset: 0x00008D97
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.UpdatePillars();
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x00056F24 File Offset: 0x00055124
	public override void DestroyShared()
	{
		base.DestroyShared();
		List<GraveyardFence> list = Pool.GetList<GraveyardFence>();
		Vis.Entities<GraveyardFence>(base.transform.position, 5f, list, 2097152, 2);
		foreach (GraveyardFence graveyardFence in list)
		{
			graveyardFence.UpdatePillars();
		}
		Pool.FreeList<GraveyardFence>(ref list);
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x00056FA0 File Offset: 0x000551A0
	public void UpdatePillars()
	{
		foreach (BoxCollider boxCollider in this.pillars)
		{
			boxCollider.gameObject.SetActive(true);
			foreach (Collider collider in Physics.OverlapBox(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.size * 0.5f, boxCollider.transform.rotation, 2097152))
			{
				if (collider.CompareTag("Usable Auxiliary"))
				{
					BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
					if (!(baseEntity == null) && !base.EqualNetID(baseEntity) && collider != boxCollider)
					{
						boxCollider.gameObject.SetActive(false);
					}
				}
			}
		}
	}
}
