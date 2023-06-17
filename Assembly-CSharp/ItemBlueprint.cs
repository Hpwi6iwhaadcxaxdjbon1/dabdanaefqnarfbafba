using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000437 RID: 1079
public class ItemBlueprint : MonoBehaviour
{
	// Token: 0x04001699 RID: 5785
	public List<ItemAmount> ingredients = new List<ItemAmount>();

	// Token: 0x0400169A RID: 5786
	public bool defaultBlueprint;

	// Token: 0x0400169B RID: 5787
	public bool userCraftable = true;

	// Token: 0x0400169C RID: 5788
	public bool isResearchable = true;

	// Token: 0x0400169D RID: 5789
	public Rarity rarity;

	// Token: 0x0400169E RID: 5790
	[Header("Workbench")]
	public int workbenchLevelRequired;

	// Token: 0x0400169F RID: 5791
	[Header("Scrap")]
	public int scrapRequired;

	// Token: 0x040016A0 RID: 5792
	public int scrapFromRecycle;

	// Token: 0x040016A1 RID: 5793
	[Tooltip("This item won't show anywhere unless you have the corresponding SteamItem in your inventory - which is defined on the ItemDefinition")]
	[Header("Unlocking")]
	public bool NeedsSteamItem;

	// Token: 0x040016A2 RID: 5794
	public int blueprintStackSize = -1;

	// Token: 0x040016A3 RID: 5795
	public float time = 1f;

	// Token: 0x040016A4 RID: 5796
	public int amountToCreate = 1;

	// Token: 0x040016A5 RID: 5797
	public string UnlockAchievment;

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06001A12 RID: 6674 RVA: 0x00015933 File Offset: 0x00013B33
	public ItemDefinition targetItem
	{
		get
		{
			return base.GetComponent<ItemDefinition>();
		}
	}

	// Token: 0x06001A13 RID: 6675 RVA: 0x000925B4 File Offset: 0x000907B4
	public string GetIngredientString(bool colorMissing)
	{
		string text = "";
		for (int i = 0; i < this.ingredients.Count; i++)
		{
			ItemAmount itemAmount = this.ingredients[i];
			if (itemAmount.itemDef == null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Blueprint missing ingredient: ",
					this.targetItem.displayName.english,
					" / ",
					itemAmount.itemid
				}));
			}
			else
			{
				bool flag = (float)LocalPlayer.Entity.inventory.GetAmount(itemAmount.itemDef.itemid) >= itemAmount.amount;
				text += string.Format("<color={2}>{0:N0} {1}</color>", itemAmount.amount, itemAmount.itemDef.displayName.translated, flag ? "#F7EBE1BB" : "#FFCC5AFF");
				if (i < this.ingredients.Count - 1)
				{
					text += ", ";
				}
			}
		}
		return text;
	}
}
