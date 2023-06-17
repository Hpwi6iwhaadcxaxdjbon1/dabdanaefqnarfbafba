using System;

// Token: 0x02000448 RID: 1096
public class ItemModConditionContainerFlag : ItemMod
{
	// Token: 0x04001701 RID: 5889
	public ItemContainer.Flag flag;

	// Token: 0x04001702 RID: 5890
	public bool requiredState;

	// Token: 0x06001A41 RID: 6721 RVA: 0x00015B10 File Offset: 0x00013D10
	public override bool Passes(Item item)
	{
		if (item.parent == null)
		{
			return !this.requiredState;
		}
		if (!item.parent.HasFlag(this.flag))
		{
			return !this.requiredState;
		}
		return this.requiredState;
	}
}
