using System;
using UnityEngine;

// Token: 0x020000D8 RID: 216
[CreateAssetMenu(menuName = "Rust/NPC Vending Order")]
public class NPCVendingOrder : ScriptableObject
{
	// Token: 0x040006EA RID: 1770
	public NPCVendingOrder.Entry[] orders;

	// Token: 0x020000D9 RID: 217
	[Serializable]
	public struct Entry
	{
		// Token: 0x040006EB RID: 1771
		public ItemDefinition sellItem;

		// Token: 0x040006EC RID: 1772
		public int sellItemAmount;

		// Token: 0x040006ED RID: 1773
		public bool sellItemAsBP;

		// Token: 0x040006EE RID: 1774
		public ItemDefinition currencyItem;

		// Token: 0x040006EF RID: 1775
		public int currencyAmount;

		// Token: 0x040006F0 RID: 1776
		public bool currencyAsBP;

		// Token: 0x040006F1 RID: 1777
		[Tooltip("The higher this number, the more likely this will be chosen")]
		public int weight;
	}
}
