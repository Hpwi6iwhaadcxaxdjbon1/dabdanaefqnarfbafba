using System;
using UnityEngine;

// Token: 0x02000664 RID: 1636
public class LootPanelLocker : LootPanel
{
	// Token: 0x04002076 RID: 8310
	public GameObject[] controls;

	// Token: 0x06002473 RID: 9331 RVA: 0x0001CC2E File Offset: 0x0001AE2E
	public Locker GetLocker()
	{
		return base.GetContainerEntity<Locker>();
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x000C0C54 File Offset: 0x000BEE54
	public void EquipButtonPressed(int index)
	{
		Locker locker = this.GetLocker();
		if (locker != null)
		{
			locker.EquipFromIndex(index);
		}
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000C0C78 File Offset: 0x000BEE78
	public new void Update()
	{
		base.Update();
		if (!this.GetLocker())
		{
			return;
		}
		GameObject[] array = this.controls;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!this.GetLocker().IsEquipping());
		}
	}
}
