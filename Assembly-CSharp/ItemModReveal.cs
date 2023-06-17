using System;
using System.Collections.Generic;
using GameMenu;

// Token: 0x02000461 RID: 1121
public class ItemModReveal : ItemMod
{
	// Token: 0x04001756 RID: 5974
	public int numForReveal = 10;

	// Token: 0x04001757 RID: 5975
	public ItemDefinition revealedItemOverride;

	// Token: 0x04001758 RID: 5976
	public int revealedItemAmount = 1;

	// Token: 0x04001759 RID: 5977
	public LootSpawn revealList;

	// Token: 0x0400175A RID: 5978
	public GameObjectRef successEffect;

	// Token: 0x06001A75 RID: 6773 RVA: 0x00092DF8 File Offset: 0x00090FF8
	public override void GetMenuOptions(Item item, List<Option> list, BasePlayer player)
	{
		if (item.amount >= this.numForReveal)
		{
			list.Add(new Option
			{
				icon = "examine",
				title = "reveal_item",
				desc = "reveal_item_desc",
				command = "reveal",
				order = 0
			});
		}
	}
}
