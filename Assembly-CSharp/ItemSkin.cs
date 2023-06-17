using System;
using Rust.Workshop;
using UnityEngine;

// Token: 0x020005DA RID: 1498
public class ItemSkin : SteamInventoryItem
{
	// Token: 0x04001E17 RID: 7703
	public Skinnable Skinnable;

	// Token: 0x04001E18 RID: 7704
	public Material[] Materials;

	// Token: 0x06002206 RID: 8710 RVA: 0x0001B00D File Offset: 0x0001920D
	public void ApplySkin(GameObject obj)
	{
		if (this.Skinnable == null)
		{
			return;
		}
		Skin.Apply(obj, this.Skinnable, this.Materials);
	}
}
