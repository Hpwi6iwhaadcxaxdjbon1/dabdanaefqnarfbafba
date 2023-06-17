using System;
using UnityEngine;

// Token: 0x0200033E RID: 830
public class CrushTrigger : TriggerHurt
{
	// Token: 0x040012CD RID: 4813
	public bool includeNPCs = true;

	// Token: 0x060015D9 RID: 5593 RVA: 0x00085C1C File Offset: 0x00083E1C
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
		if (!this.includeNPCs && baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
