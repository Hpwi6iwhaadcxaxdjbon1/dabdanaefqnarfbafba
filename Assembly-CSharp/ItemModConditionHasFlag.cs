using System;

// Token: 0x0200044A RID: 1098
public class ItemModConditionHasFlag : ItemMod
{
	// Token: 0x04001705 RID: 5893
	public Item.Flag flag;

	// Token: 0x04001706 RID: 5894
	public bool requiredState;

	// Token: 0x06001A46 RID: 6726 RVA: 0x00015B5A File Offset: 0x00013D5A
	public override bool Passes(Item item)
	{
		return item.HasFlag(this.flag) == this.requiredState;
	}
}
