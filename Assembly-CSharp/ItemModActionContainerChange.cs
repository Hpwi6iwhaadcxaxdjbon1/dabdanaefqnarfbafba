using System;
using UnityEngine;

// Token: 0x02000444 RID: 1092
public class ItemModActionContainerChange : ItemMod
{
	// Token: 0x040016FA RID: 5882
	public ItemMod[] actions;

	// Token: 0x06001A3B RID: 6715 RVA: 0x00015AD1 File Offset: 0x00013CD1
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null!", base.gameObject);
		}
	}
}
