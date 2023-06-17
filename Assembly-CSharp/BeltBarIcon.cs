using System;
using UnityEngine;

// Token: 0x02000641 RID: 1601
public class BeltBarIcon : MonoBehaviour
{
	// Token: 0x04001FCC RID: 8140
	private ItemIcon itemIcon;

	// Token: 0x04001FCD RID: 8141
	private bool wasSelected;

	// Token: 0x060023B0 RID: 9136 RVA: 0x0001C369 File Offset: 0x0001A569
	private void Start()
	{
		this.itemIcon = base.GetComponent<ItemIcon>();
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x000BD608 File Offset: 0x000BB808
	private void Update()
	{
		bool flag = PlayerBelt.SelectedSlot == this.itemIcon.slot;
		if (this.wasSelected == flag)
		{
			return;
		}
		this.wasSelected = flag;
		this.itemIcon.SetActive(this.wasSelected);
	}
}
