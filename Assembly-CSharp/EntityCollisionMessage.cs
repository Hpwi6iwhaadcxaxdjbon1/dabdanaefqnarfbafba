using System;
using UnityEngine;

// Token: 0x020002B0 RID: 688
public class EntityCollisionMessage : EntityComponent<BaseEntity>
{
	// Token: 0x0600134A RID: 4938 RVA: 0x0007B45C File Offset: 0x0007965C
	private void OnCollisionEnter(Collision collision)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		if (base.baseEntity.IsDestroyed)
		{
			return;
		}
		BaseEntity baseEntity = collision.GetEntity();
		if (baseEntity == base.baseEntity)
		{
			return;
		}
		if (baseEntity != null)
		{
			if (baseEntity.IsDestroyed)
			{
				return;
			}
			if (base.baseEntity.isClient)
			{
				baseEntity = baseEntity.ToClient<BaseEntity>();
			}
		}
		base.baseEntity.OnCollision(collision, baseEntity);
	}
}
