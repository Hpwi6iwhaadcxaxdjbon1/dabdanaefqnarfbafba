using System;
using UnityEngine;

// Token: 0x02000443 RID: 1091
public class ItemModActionChange : ItemMod
{
	// Token: 0x040016F9 RID: 5881
	public ItemMod[] actions;

	// Token: 0x06001A39 RID: 6713 RVA: 0x00015AB7 File Offset: 0x00013CB7
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null!", base.gameObject);
		}
	}
}
