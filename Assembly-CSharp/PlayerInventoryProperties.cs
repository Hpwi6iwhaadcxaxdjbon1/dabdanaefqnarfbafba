using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005E4 RID: 1508
[CreateAssetMenu(menuName = "Rust/Player Inventory Properties")]
public class PlayerInventoryProperties : ScriptableObject
{
	// Token: 0x04001E2A RID: 7722
	public string niceName;

	// Token: 0x04001E2B RID: 7723
	public int order = 100;

	// Token: 0x04001E2C RID: 7724
	public List<ItemAmount> belt;

	// Token: 0x04001E2D RID: 7725
	public List<ItemAmount> main;

	// Token: 0x04001E2E RID: 7726
	public List<ItemAmount> wear;

	// Token: 0x04001E2F RID: 7727
	public List<PlayerInventoryProperties.ItemAmountSkinned> skinnedWear;

	// Token: 0x020005E5 RID: 1509
	[Serializable]
	public class ItemAmountSkinned : ItemAmount
	{
		// Token: 0x04001E30 RID: 7728
		public ulong skinOverride;

		// Token: 0x06002215 RID: 8725 RVA: 0x0001B0C3 File Offset: 0x000192C3
		public ulong GetRandomSkin()
		{
			return this.skinOverride;
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x0001B0CB File Offset: 0x000192CB
		public ItemAmountSkinned() : base(null, 0f)
		{
		}
	}
}
