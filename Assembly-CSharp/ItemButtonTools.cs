using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200024B RID: 587
public class ItemButtonTools : MonoBehaviour
{
	// Token: 0x04000E49 RID: 3657
	public Image image;

	// Token: 0x04000E4A RID: 3658
	public ItemDefinition itemDef;

	// Token: 0x06001190 RID: 4496 RVA: 0x0000F64B File Offset: 0x0000D84B
	public void GiveSelf(int amount)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.giveid", new object[]
		{
			this.itemDef.itemid,
			amount
		});
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x0000F67F File Offset: 0x0000D87F
	public void GiveArmed()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.givearm", new object[]
		{
			this.itemDef.itemid
		});
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x00002ECE File Offset: 0x000010CE
	public void GiveBlueprint()
	{
	}
}
