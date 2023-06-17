using System;
using UnityEngine;

// Token: 0x0200040E RID: 1038
public class TriggerWorkbench : TriggerBase
{
	// Token: 0x040015C6 RID: 5574
	public Workbench parentBench;

	// Token: 0x0600194C RID: 6476 RVA: 0x0007C560 File Offset: 0x0007A760
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

	// Token: 0x0600194D RID: 6477 RVA: 0x00014FDA File Offset: 0x000131DA
	public float WorkbenchLevel()
	{
		return (float)this.parentBench.Workbenchlevel;
	}
}
