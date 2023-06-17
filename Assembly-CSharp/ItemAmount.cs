using System;
using UnityEngine;

// Token: 0x0200046D RID: 1133
[Serializable]
public class ItemAmount : ISerializationCallbackReceiver
{
	// Token: 0x0400177D RID: 6013
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	// Token: 0x0400177E RID: 6014
	public float amount;

	// Token: 0x0400177F RID: 6015
	[NonSerialized]
	public float startAmount;

	// Token: 0x06001A91 RID: 6801 RVA: 0x00016050 File Offset: 0x00014250
	public ItemAmount(ItemDefinition item = null, float amt = 0f)
	{
		this.itemDef = item;
		this.amount = amt;
		this.startAmount = this.amount;
	}

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06001A92 RID: 6802 RVA: 0x00016072 File Offset: 0x00014272
	public int itemid
	{
		get
		{
			if (this.itemDef == null)
			{
				return 0;
			}
			return this.itemDef.itemid;
		}
	}

	// Token: 0x06001A93 RID: 6803 RVA: 0x0001608F File Offset: 0x0001428F
	public virtual float GetAmount()
	{
		return this.amount;
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x00016097 File Offset: 0x00014297
	public virtual void OnAfterDeserialize()
	{
		this.startAmount = this.amount;
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnBeforeSerialize()
	{
	}
}
