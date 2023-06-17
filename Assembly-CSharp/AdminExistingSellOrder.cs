using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class AdminExistingSellOrder : MonoBehaviour
{
	// Token: 0x04000700 RID: 1792
	public VirtualItemIcon MerchandiseIcon;

	// Token: 0x04000701 RID: 1793
	public VirtualItemIcon CurrencyIcon;

	// Token: 0x04000702 RID: 1794
	private VendingPanelAdmin adminPanel;

	// Token: 0x04000703 RID: 1795
	private int index;

	// Token: 0x06000A9F RID: 2719 RVA: 0x00055C48 File Offset: 0x00053E48
	public void Setup(VendingMachine.SellOrder so, int newIndex, VendingPanelAdmin admin)
	{
		this.adminPanel = admin;
		this.index = newIndex;
		ItemDefinition info = ItemManager.FindItemDefinition(so.itemToSellID);
		ItemDefinition info2 = ItemManager.FindItemDefinition(so.currencyID);
		this.MerchandiseIcon.SetVirtualItem(info, so.itemToSellAmount, 0UL, so.itemToSellIsBP);
		this.CurrencyIcon.SetVirtualItem(info2, so.currencyAmountPerItem, 0UL, so.currencyIsBP);
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x0000A6B3 File Offset: 0x000088B3
	public void DeleteClick()
	{
		if (this.adminPanel)
		{
			this.adminPanel.DeleteClicked(this.index);
		}
	}
}
