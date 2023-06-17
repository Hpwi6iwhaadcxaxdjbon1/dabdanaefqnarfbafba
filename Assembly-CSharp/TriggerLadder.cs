using System;
using UnityEngine;

// Token: 0x02000408 RID: 1032
public class TriggerLadder : TriggerBase
{
	// Token: 0x06001941 RID: 6465 RVA: 0x0008F528 File Offset: 0x0008D728
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
		if (baseEntity as BasePlayer == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
