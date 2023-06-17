using System;

// Token: 0x02000452 RID: 1106
public class ItemModContainerRestriction : ItemMod
{
	// Token: 0x04001720 RID: 5920
	[InspectorFlags]
	public ItemModContainerRestriction.SlotFlags slotFlags;

	// Token: 0x06001A54 RID: 6740 RVA: 0x00015C0A File Offset: 0x00013E0A
	public bool CanExistWith(ItemModContainerRestriction other)
	{
		return other == null || (this.slotFlags & other.slotFlags) == (ItemModContainerRestriction.SlotFlags)0;
	}

	// Token: 0x02000453 RID: 1107
	[Flags]
	public enum SlotFlags
	{
		// Token: 0x04001722 RID: 5922
		Map = 1
	}
}
