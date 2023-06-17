using System;
using UnityEngine;

// Token: 0x02000441 RID: 1089
public class ItemSelector : PropertyAttribute
{
	// Token: 0x040016F7 RID: 5879
	public ItemCategory category = ItemCategory.All;

	// Token: 0x06001A2D RID: 6701 RVA: 0x00015A92 File Offset: 0x00013C92
	public ItemSelector(ItemCategory category = ItemCategory.All)
	{
		this.category = category;
	}
}
