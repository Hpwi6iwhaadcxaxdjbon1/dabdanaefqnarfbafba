using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200042C RID: 1068
public class ItemEventFlag : MonoBehaviour, IItemUpdate
{
	// Token: 0x04001657 RID: 5719
	public Item.Flag flag;

	// Token: 0x04001658 RID: 5720
	public UnityEvent onEnabled = new UnityEvent();

	// Token: 0x04001659 RID: 5721
	public UnityEvent onDisable = new UnityEvent();

	// Token: 0x0400165A RID: 5722
	internal bool firstRun = true;

	// Token: 0x0400165B RID: 5723
	internal bool lastState;

	// Token: 0x060019B1 RID: 6577 RVA: 0x000911D8 File Offset: 0x0008F3D8
	public void OnItemUpdate(Item item)
	{
		bool flag = item.HasFlag(this.flag);
		if (!this.firstRun && flag == this.lastState)
		{
			return;
		}
		if (flag)
		{
			this.onEnabled.Invoke();
		}
		else
		{
			this.onDisable.Invoke();
		}
		this.lastState = flag;
		this.firstRun = false;
	}
}
