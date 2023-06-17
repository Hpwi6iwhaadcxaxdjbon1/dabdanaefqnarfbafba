using System;
using UnityEngine;

// Token: 0x020005EF RID: 1519
public class SteamInventoryItem : ScriptableObject
{
	// Token: 0x04001E87 RID: 7815
	public int id;

	// Token: 0x04001E88 RID: 7816
	public Sprite icon;

	// Token: 0x04001E89 RID: 7817
	public Translate.Phrase displayName;

	// Token: 0x04001E8A RID: 7818
	public Translate.Phrase displayDescription;

	// Token: 0x04001E8B RID: 7819
	[Header("Steam Inventory")]
	public SteamInventoryItem.Category category;

	// Token: 0x04001E8C RID: 7820
	public SteamInventoryItem.SubCategory subcategory;

	// Token: 0x04001E8D RID: 7821
	public SteamInventoryCategory steamCategory;

	// Token: 0x04001E8E RID: 7822
	[Tooltip("Dtop this item being broken down into cloth etc")]
	public bool PreventBreakingDown;

	// Token: 0x04001E8F RID: 7823
	[Header("Meta")]
	public string itemname;

	// Token: 0x04001E90 RID: 7824
	public ulong workshopID;

	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06002230 RID: 8752 RVA: 0x0001B1E6 File Offset: 0x000193E6
	public ItemDefinition itemDefinition
	{
		get
		{
			return ItemManager.FindItemDefinition(this.itemname);
		}
	}

	// Token: 0x020005F0 RID: 1520
	public enum Category
	{
		// Token: 0x04001E92 RID: 7826
		None,
		// Token: 0x04001E93 RID: 7827
		Clothing,
		// Token: 0x04001E94 RID: 7828
		Weapon,
		// Token: 0x04001E95 RID: 7829
		Decoration,
		// Token: 0x04001E96 RID: 7830
		Crate,
		// Token: 0x04001E97 RID: 7831
		Resource
	}

	// Token: 0x020005F1 RID: 1521
	public enum SubCategory
	{
		// Token: 0x04001E99 RID: 7833
		None,
		// Token: 0x04001E9A RID: 7834
		Shirt,
		// Token: 0x04001E9B RID: 7835
		Pants,
		// Token: 0x04001E9C RID: 7836
		Jacket,
		// Token: 0x04001E9D RID: 7837
		Hat,
		// Token: 0x04001E9E RID: 7838
		Mask,
		// Token: 0x04001E9F RID: 7839
		Footwear,
		// Token: 0x04001EA0 RID: 7840
		Weapon,
		// Token: 0x04001EA1 RID: 7841
		Misc,
		// Token: 0x04001EA2 RID: 7842
		Crate,
		// Token: 0x04001EA3 RID: 7843
		Resource
	}
}
