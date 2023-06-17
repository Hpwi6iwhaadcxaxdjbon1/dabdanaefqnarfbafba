using System;
using System.Collections.Generic;

// Token: 0x02000451 RID: 1105
public class ItemModContainer : ItemMod
{
	// Token: 0x04001717 RID: 5911
	public int capacity = 6;

	// Token: 0x04001718 RID: 5912
	public int maxStackSize;

	// Token: 0x04001719 RID: 5913
	[InspectorFlags]
	public ItemContainer.Flag containerFlags;

	// Token: 0x0400171A RID: 5914
	public ItemContainer.ContentsType onlyAllowedContents = ItemContainer.ContentsType.Generic;

	// Token: 0x0400171B RID: 5915
	public ItemDefinition onlyAllowedItemType;

	// Token: 0x0400171C RID: 5916
	public List<ItemSlot> availableSlots = new List<ItemSlot>();

	// Token: 0x0400171D RID: 5917
	public bool openInDeployed = true;

	// Token: 0x0400171E RID: 5918
	public bool openInInventory = true;

	// Token: 0x0400171F RID: 5919
	public List<ItemAmount> defaultContents = new List<ItemAmount>();
}
