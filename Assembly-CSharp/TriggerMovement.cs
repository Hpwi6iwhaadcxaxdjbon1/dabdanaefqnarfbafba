using System;
using UnityEngine;

// Token: 0x02000234 RID: 564
public class TriggerMovement : TriggerBase, IClientComponent
{
	// Token: 0x04000DD5 RID: 3541
	public BaseEntity.MovementModify movementModify;

	// Token: 0x060010F4 RID: 4340 RVA: 0x00072130 File Offset: 0x00070330
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (!baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
