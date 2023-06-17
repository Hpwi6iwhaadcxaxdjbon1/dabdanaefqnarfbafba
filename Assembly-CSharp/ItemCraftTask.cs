using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x0200042A RID: 1066
public class ItemCraftTask
{
	// Token: 0x04001646 RID: 5702
	public ItemBlueprint blueprint;

	// Token: 0x04001647 RID: 5703
	public float endTime;

	// Token: 0x04001648 RID: 5704
	public int taskUID;

	// Token: 0x04001649 RID: 5705
	public BasePlayer owner;

	// Token: 0x0400164A RID: 5706
	public bool cancelled;

	// Token: 0x0400164B RID: 5707
	public Item.InstanceData instanceData;

	// Token: 0x0400164C RID: 5708
	public int amount = 1;

	// Token: 0x0400164D RID: 5709
	public int skinID;

	// Token: 0x0400164E RID: 5710
	public List<ulong> potentialOwners;

	// Token: 0x0400164F RID: 5711
	public List<Item> takenItems;

	// Token: 0x04001650 RID: 5712
	public int numCrafted;

	// Token: 0x04001651 RID: 5713
	public float conditionScale = 1f;

	// Token: 0x04001652 RID: 5714
	public float workSecondsComplete;

	// Token: 0x04001653 RID: 5715
	public float worksecondsRequired;
}
