using System;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class TargetTrigger : TriggerBase
{
	// Token: 0x060013D5 RID: 5077 RVA: 0x0007C560 File Offset: 0x0007A760
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
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
