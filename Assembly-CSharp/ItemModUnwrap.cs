using System;
using System.Collections.Generic;
using GameMenu;

// Token: 0x020000FB RID: 251
public class ItemModUnwrap : ItemMod
{
	// Token: 0x0400078B RID: 1931
	public LootSpawn revealList;

	// Token: 0x0400078C RID: 1932
	public GameObjectRef successEffect;

	// Token: 0x0400078D RID: 1933
	public int minTries = 1;

	// Token: 0x0400078E RID: 1934
	public int maxTries = 1;

	// Token: 0x06000B32 RID: 2866 RVA: 0x00057C2C File Offset: 0x00055E2C
	public override void GetMenuOptions(Item item, List<Option> list, BasePlayer player)
	{
		list.Add(new Option
		{
			icon = "examine",
			title = "unwrap_gift",
			desc = "unwrap_gift_desc",
			command = "unwrap",
			order = 0
		});
	}
}
