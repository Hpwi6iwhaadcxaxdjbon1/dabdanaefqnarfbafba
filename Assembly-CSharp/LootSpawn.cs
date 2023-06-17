using System;
using UnityEngine;

// Token: 0x020005DF RID: 1503
[CreateAssetMenu(menuName = "Rust/Loot Spawn")]
public class LootSpawn : ScriptableObject
{
	// Token: 0x04001E22 RID: 7714
	public ItemAmountRanged[] items;

	// Token: 0x04001E23 RID: 7715
	public LootSpawn.Entry[] subSpawn;

	// Token: 0x020005E0 RID: 1504
	[Serializable]
	public struct Entry
	{
		// Token: 0x04001E24 RID: 7716
		[Tooltip("If a subcategory exists we'll choose from there instead of any items specified")]
		public LootSpawn category;

		// Token: 0x04001E25 RID: 7717
		[Tooltip("The higher this number, the more likely this will be chosen")]
		public int weight;
	}
}
