using System;

// Token: 0x02000465 RID: 1125
public class ItemModSwap : ItemMod
{
	// Token: 0x04001760 RID: 5984
	public GameObjectRef actionEffect;

	// Token: 0x04001761 RID: 5985
	public ItemAmount[] becomeItem;

	// Token: 0x04001762 RID: 5986
	public bool sendPlayerPickupNotification;

	// Token: 0x04001763 RID: 5987
	public bool sendPlayerDropNotification;

	// Token: 0x04001764 RID: 5988
	public float xpScale = 1f;
}
