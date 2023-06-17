using System;
using System.Collections.Generic;
using GameMenu;

// Token: 0x02000467 RID: 1127
public class ItemModUpgrade : ItemMod
{
	// Token: 0x04001767 RID: 5991
	public int numForUpgrade = 10;

	// Token: 0x04001768 RID: 5992
	public float upgradeSuccessChance = 1f;

	// Token: 0x04001769 RID: 5993
	public int numToLoseOnFail = 2;

	// Token: 0x0400176A RID: 5994
	public ItemDefinition upgradedItem;

	// Token: 0x0400176B RID: 5995
	public int numUpgradedItem = 1;

	// Token: 0x0400176C RID: 5996
	public GameObjectRef successEffect;

	// Token: 0x0400176D RID: 5997
	public GameObjectRef failEffect;

	// Token: 0x06001A7B RID: 6779 RVA: 0x00092E5C File Offset: 0x0009105C
	public override void GetMenuOptions(Item item, List<Option> list, BasePlayer player)
	{
		if (item.amount >= this.numForUpgrade)
		{
			list.Add(new Option
			{
				icon = "authorize",
				title = "upgrade_item",
				desc = "upgrade_item_desc",
				command = "upgrade_item",
				order = 0
			});
		}
	}
}
