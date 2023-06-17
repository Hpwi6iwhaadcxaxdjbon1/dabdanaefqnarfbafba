using System;
using UnityEngine;

// Token: 0x02000659 RID: 1625
public class ItemInformationPanel : MonoBehaviour
{
	// Token: 0x06002447 RID: 9287 RVA: 0x0001CA01 File Offset: 0x0001AC01
	public virtual bool EligableForDisplay(ItemDefinition info)
	{
		Debug.LogWarning("ItemInformationPanel.EligableForDisplay");
		return false;
	}

	// Token: 0x06002448 RID: 9288 RVA: 0x0001CA0E File Offset: 0x0001AC0E
	public virtual void SetupForItem(ItemDefinition info, Item item = null)
	{
		Debug.LogWarning("ItemInformationPanel.SetupForItem");
	}
}
