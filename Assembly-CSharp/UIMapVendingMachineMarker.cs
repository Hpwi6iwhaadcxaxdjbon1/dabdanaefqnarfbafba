using System;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000638 RID: 1592
public class UIMapVendingMachineMarker : MonoBehaviour
{
	// Token: 0x04001F9D RID: 8093
	public Color inStock;

	// Token: 0x04001F9E RID: 8094
	public Color outOfStock;

	// Token: 0x04001F9F RID: 8095
	public Image colorBackground;

	// Token: 0x04001FA0 RID: 8096
	public string displayName;

	// Token: 0x04001FA1 RID: 8097
	public Tooltip toolTip;

	// Token: 0x04001FA2 RID: 8098
	private bool isInStock;

	// Token: 0x0600238B RID: 9099 RVA: 0x0001C221 File Offset: 0x0001A421
	public void SetOutOfStock(bool stock)
	{
		this.colorBackground.color = (stock ? this.inStock : this.outOfStock);
		this.isInStock = stock;
	}

	// Token: 0x0600238C RID: 9100 RVA: 0x000BC7CC File Offset: 0x000BA9CC
	public void UpdateDisplayName(string newName, VendingMachine.SellOrderContainer sellOrderContainer)
	{
		this.displayName = newName;
		this.toolTip.Text = this.displayName;
		if (this.isInStock && sellOrderContainer != null && sellOrderContainer.sellOrders != null && sellOrderContainer.sellOrders.Count > 0)
		{
			Tooltip tooltip = this.toolTip;
			tooltip.Text += "\n";
			foreach (VendingMachine.SellOrder sellOrder in sellOrderContainer.sellOrders)
			{
				if (sellOrder.inStock > 0)
				{
					string text = ItemManager.FindItemDefinition(sellOrder.itemToSellID).displayName.translated + (sellOrder.itemToSellIsBP ? " (BP)" : "");
					string text2 = ItemManager.FindItemDefinition(sellOrder.currencyID).displayName.translated + (sellOrder.currencyIsBP ? " (BP)" : "");
					Tooltip tooltip2 = this.toolTip;
					tooltip2.Text = string.Concat(new object[]
					{
						tooltip2.Text,
						"\n",
						sellOrder.itemToSellAmount,
						" ",
						text,
						" | ",
						sellOrder.currencyAmountPerItem,
						" ",
						text2
					});
					tooltip2 = this.toolTip;
					tooltip2.Text = string.Concat(new object[]
					{
						tooltip2.Text,
						" (",
						sellOrder.inStock,
						" Left)"
					});
				}
			}
		}
		this.toolTip.enabled = (this.toolTip.Text != "");
	}
}
