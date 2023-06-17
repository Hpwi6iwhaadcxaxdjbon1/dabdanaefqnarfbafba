using System;
using System.Collections.Generic;
using GameMenu;

// Token: 0x0200045F RID: 1119
public class ItemModRecycleInto : ItemMod
{
	// Token: 0x0400174F RID: 5967
	public ItemDefinition recycleIntoItem;

	// Token: 0x04001750 RID: 5968
	public int numRecycledItemMin = 1;

	// Token: 0x04001751 RID: 5969
	public int numRecycledItemMax = 1;

	// Token: 0x04001752 RID: 5970
	public GameObjectRef successEffect;

	// Token: 0x06001A70 RID: 6768 RVA: 0x00092D38 File Offset: 0x00090F38
	public override void GetMenuOptions(Item item, List<Option> list, BasePlayer player)
	{
		list.Add(new Option
		{
			icon = "gear",
			title = "recycle_into",
			desc = "recycle_into_desc",
			command = "recycle_item",
			order = 0
		});
	}
}
