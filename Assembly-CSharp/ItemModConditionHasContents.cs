using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000449 RID: 1097
public class ItemModConditionHasContents : ItemMod
{
	// Token: 0x04001703 RID: 5891
	[Tooltip("Can be null to mean any item")]
	public ItemDefinition itemDef;

	// Token: 0x04001704 RID: 5892
	public bool requiredState;

	// Token: 0x06001A43 RID: 6723 RVA: 0x00092A64 File Offset: 0x00090C64
	public override bool Passes(Item item)
	{
		if (item.contents == null)
		{
			return !this.requiredState;
		}
		if (item.contents.itemList.Count == 0)
		{
			return !this.requiredState;
		}
		if (this.itemDef && !Enumerable.Any<Item>(item.contents.itemList, (Item x) => x.info == this.itemDef))
		{
			return !this.requiredState;
		}
		return this.requiredState;
	}
}
