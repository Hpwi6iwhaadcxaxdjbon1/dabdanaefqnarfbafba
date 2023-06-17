using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A2 RID: 418
public class ConstructionGrade : PrefabAttribute
{
	// Token: 0x04000B63 RID: 2915
	[NonSerialized]
	public Construction construction;

	// Token: 0x04000B64 RID: 2916
	public BuildingGrade gradeBase;

	// Token: 0x04000B65 RID: 2917
	public GameObjectRef skinObject;

	// Token: 0x04000B66 RID: 2918
	internal List<ItemAmount> _costToBuild;

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000E41 RID: 3649 RVA: 0x0000D181 File Offset: 0x0000B381
	public float maxHealth
	{
		get
		{
			if (!this.gradeBase || !this.construction)
			{
				return 0f;
			}
			return this.gradeBase.baseHealth * this.construction.healthMultiplier;
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000E42 RID: 3650 RVA: 0x000646AC File Offset: 0x000628AC
	public List<ItemAmount> costToBuild
	{
		get
		{
			if (this._costToBuild != null)
			{
				return this._costToBuild;
			}
			this._costToBuild = new List<ItemAmount>();
			foreach (ItemAmount itemAmount in this.gradeBase.baseCost)
			{
				this._costToBuild.Add(new ItemAmount(itemAmount.itemDef, Mathf.Ceil(itemAmount.amount * this.construction.costMultiplier)));
			}
			return this._costToBuild;
		}
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x0000D1BA File Offset: 0x0000B3BA
	protected override Type GetIndexedType()
	{
		return typeof(ConstructionGrade);
	}
}
