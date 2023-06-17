using System;
using UnityEngine;

// Token: 0x02000454 RID: 1108
public class ItemModCookable : ItemMod
{
	// Token: 0x04001723 RID: 5923
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition becomeOnCooked;

	// Token: 0x04001724 RID: 5924
	public float cookTime = 30f;

	// Token: 0x04001725 RID: 5925
	public int amountOfBecome = 1;

	// Token: 0x04001726 RID: 5926
	public int lowTemp;

	// Token: 0x04001727 RID: 5927
	public int highTemp;

	// Token: 0x04001728 RID: 5928
	public bool setCookingFlag;

	// Token: 0x06001A56 RID: 6742 RVA: 0x00015C29 File Offset: 0x00013E29
	public void OnValidate()
	{
		if (this.amountOfBecome < 1)
		{
			this.amountOfBecome = 1;
		}
		if (this.becomeOnCooked == null)
		{
			Debug.LogWarning("[ItemModCookable] becomeOnCooked is unset! [" + base.name + "]", base.gameObject);
		}
	}
}
