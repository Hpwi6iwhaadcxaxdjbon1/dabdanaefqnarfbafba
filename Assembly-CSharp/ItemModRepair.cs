using System;
using System.Collections.Generic;
using GameMenu;

// Token: 0x02000460 RID: 1120
public class ItemModRepair : ItemMod
{
	// Token: 0x04001753 RID: 5971
	public float conditionLost = 0.05f;

	// Token: 0x04001754 RID: 5972
	public GameObjectRef successEffect;

	// Token: 0x04001755 RID: 5973
	public int workbenchLvlRequired;

	// Token: 0x06001A72 RID: 6770 RVA: 0x00015E7B File Offset: 0x0001407B
	public bool HasCraftLevel(BasePlayer player = null)
	{
		return LocalPlayer.Entity && LocalPlayer.HasCraftLevel(this.workbenchLvlRequired);
	}

	// Token: 0x06001A73 RID: 6771 RVA: 0x00092D8C File Offset: 0x00090F8C
	public override void GetMenuOptions(Item item, List<Option> list, BasePlayer player)
	{
		if (this.HasCraftLevel(null) && item.conditionNormalized < 1f)
		{
			list.Add(new Option
			{
				icon = "refresh",
				title = "refill_item",
				desc = "refill_item_desc",
				command = "refill",
				order = 0
			});
		}
	}
}
