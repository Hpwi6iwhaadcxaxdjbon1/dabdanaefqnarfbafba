using System;
using System.Collections.Generic;
using GameMenu;

// Token: 0x02000446 RID: 1094
public class ItemModBlueprintCraft : ItemMod
{
	// Token: 0x040016FC RID: 5884
	public GameObjectRef successEffect;

	// Token: 0x06001A3E RID: 6718 RVA: 0x000929B8 File Offset: 0x00090BB8
	public override void GetMenuOptions(Item item, List<Option> list, BasePlayer player)
	{
		if (item.GetOwnerPlayer() != player)
		{
			return;
		}
		list.Add(new Option
		{
			icon = "gear",
			title = "craft_item",
			desc = "craft_item_desc",
			command = "craft",
			order = 0
		});
		list.Add(new Option
		{
			icon = "upgrade",
			title = "craft_all",
			desc = "craft_all_desc",
			command = "craft_all",
			order = 0
		});
	}
}
