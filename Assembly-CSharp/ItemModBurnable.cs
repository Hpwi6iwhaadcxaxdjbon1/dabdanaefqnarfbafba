using System;

// Token: 0x02000447 RID: 1095
public class ItemModBurnable : ItemMod
{
	// Token: 0x040016FD RID: 5885
	public float fuelAmount = 10f;

	// Token: 0x040016FE RID: 5886
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition byproductItem;

	// Token: 0x040016FF RID: 5887
	public int byproductAmount = 1;

	// Token: 0x04001700 RID: 5888
	public float byproductChance = 0.5f;
}
