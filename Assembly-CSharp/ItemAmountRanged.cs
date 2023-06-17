using System;
using UnityEngine;

// Token: 0x0200046F RID: 1135
[Serializable]
public class ItemAmountRanged : ItemAmount
{
	// Token: 0x04001782 RID: 6018
	public float maxAmount = -1f;

	// Token: 0x06001A98 RID: 6808 RVA: 0x000160C6 File Offset: 0x000142C6
	public override void OnAfterDeserialize()
	{
		base.OnAfterDeserialize();
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x000160CE File Offset: 0x000142CE
	public ItemAmountRanged(ItemDefinition item = null, float amt = 0f, float max = -1f) : base(item, amt)
	{
		this.maxAmount = max;
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x000160EA File Offset: 0x000142EA
	public override float GetAmount()
	{
		if (this.maxAmount > 0f && this.maxAmount > this.amount)
		{
			return Random.Range(this.amount, this.maxAmount);
		}
		return this.amount;
	}
}
