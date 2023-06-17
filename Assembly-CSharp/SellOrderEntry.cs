using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000E4 RID: 228
public class SellOrderEntry : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04000714 RID: 1812
	public VirtualItemIcon MerchandiseIcon;

	// Token: 0x04000715 RID: 1813
	public VirtualItemIcon CurrencyIcon;

	// Token: 0x04000716 RID: 1814
	private ItemDefinition merchandiseInfo;

	// Token: 0x04000717 RID: 1815
	private ItemDefinition currencyInfo;

	// Token: 0x04000718 RID: 1816
	public GameObject buyButton;

	// Token: 0x04000719 RID: 1817
	public GameObject cantaffordNotification;

	// Token: 0x0400071A RID: 1818
	public GameObject outOfStockNotification;

	// Token: 0x0400071B RID: 1819
	private LootPanelVendingMachine vendingPanel;

	// Token: 0x0400071C RID: 1820
	public UIIntegerEntry intEntry;

	// Token: 0x0400071D RID: 1821
	private bool dirty = true;

	// Token: 0x0400071E RID: 1822
	private bool merchIsBP;

	// Token: 0x0400071F RID: 1823
	private bool currencyIsBP;

	// Token: 0x04000720 RID: 1824
	private int merchandiseSellSize;

	// Token: 0x04000721 RID: 1825
	private int currencyAmountPerItem;

	// Token: 0x04000722 RID: 1826
	private int index;

	// Token: 0x06000AC0 RID: 2752 RVA: 0x0000A862 File Offset: 0x00008A62
	public void OnEnable()
	{
		GlobalMessages.onInventoryChanged.Add(this);
		this.intEntry.textChanged += new Action(this.AmountTextChanged);
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x0000A886 File Offset: 0x00008A86
	public void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onInventoryChanged.Remove(this);
		this.intEntry.textChanged -= new Action(this.AmountTextChanged);
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x00055FB8 File Offset: 0x000541B8
	public void Setup(VendingMachine.SellOrder so, int newIndex, LootPanelVendingMachine panel = null, VendingPanelAdmin admin = null)
	{
		this.merchandiseInfo = ItemManager.FindItemDefinition(so.itemToSellID);
		this.merchandiseSellSize = Mathf.Max(1, so.itemToSellAmount);
		this.merchIsBP = so.itemToSellIsBP;
		this.currencyInfo = ItemManager.FindItemDefinition(so.currencyID);
		this.currencyAmountPerItem = Mathf.Max(1, so.currencyAmountPerItem);
		this.currencyIsBP = so.currencyIsBP;
		this.index = newIndex;
		this.vendingPanel = panel;
		this.UpdateIcons();
		this.UpdateNotifications();
		this.AmountTextChanged();
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x0000A8B3 File Offset: 0x00008AB3
	public void OnInventoryChanged()
	{
		this.dirty = true;
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x0000A8BC File Offset: 0x00008ABC
	public void Update()
	{
		if (this.dirty)
		{
			this.UpdateIcons();
			this.UpdateNotifications();
			this.AmountTextChanged();
		}
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x00056044 File Offset: 0x00054244
	public void AmountTextChanged()
	{
		int amount = Mathf.Clamp(this.intEntry.GetIntAmount(), 1, this.GetMaxTransactionCount());
		this.intEntry.SetAmount(amount);
		this.UpdateIcons();
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x0000A8D8 File Offset: 0x00008AD8
	public int GetDesiredTransactionCount()
	{
		return Mathf.Clamp(this.intEntry.GetIntAmount(), 1, this.GetMaxTransactionCount());
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x0005607C File Offset: 0x0005427C
	public int MerchAvailable()
	{
		int num = 0;
		foreach (ItemContainer itemContainer in LocalPlayer.Entity.inventory.loot.containers)
		{
			List<Item> list = itemContainer.FindItemsByItemID(this.merchandiseInfo.itemid);
			num += Enumerable.Sum<Item>(list, (Item x) => x.amount);
		}
		return num;
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x00056114 File Offset: 0x00054314
	public int CurrencyAvailable()
	{
		List<Item> list;
		if (this.currencyIsBP)
		{
			list = Enumerable.ToList<Item>(Enumerable.Where<Item>(LocalPlayer.Entity.inventory.FindItemIDs(ItemManager.FindItemDefinition("blueprintbase").itemid), (Item x) => x.blueprintTarget == this.currencyInfo.itemid));
		}
		else
		{
			list = LocalPlayer.Entity.inventory.FindItemIDs(this.currencyInfo.itemid);
			list = Enumerable.ToList<Item>(Enumerable.Where<Item>(list, (Item x) => !x.hasCondition || (x.conditionNormalized >= 0.5f && x.maxConditionNormalized > 0.5f)));
		}
		return Enumerable.Sum<Item>(list, (Item x) => x.amount);
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x0000A8F1 File Offset: 0x00008AF1
	public int MaxTransactionsAffordable()
	{
		return this.CurrencyAvailable() / this.currencyAmountPerItem;
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x0000A900 File Offset: 0x00008B00
	public int MaxTransactionsAvailable()
	{
		return Mathf.FloorToInt((float)this.MerchAvailable() / (float)this.merchandiseSellSize);
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x000561CC File Offset: 0x000543CC
	public int GetMaxTransactionCount()
	{
		int result = Mathf.Max(Mathf.Min(this.MaxTransactionsAvailable(), this.MaxTransactionsAffordable()), 1);
		if (this.merchandiseInfo.condition.enabled)
		{
			result = 1;
		}
		if (this.merchandiseInfo.stackable <= 1)
		{
			result = 1;
		}
		if (this.currencyInfo.stackable <= 1)
		{
			result = 1;
		}
		return result;
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x00056228 File Offset: 0x00054428
	public void UpdateIcons()
	{
		int desiredTransactionCount = this.GetDesiredTransactionCount();
		this.CurrencyIcon.SetVirtualItem(this.currencyInfo, desiredTransactionCount * this.currencyAmountPerItem, 0UL, this.currencyIsBP);
		Item item = null;
		if (this.vendingPanel != null && this.vendingPanel.GetVendingMachine() != null && this.merchandiseInfo != null)
		{
			item = LocalPlayer.Entity.inventory.loot.containers[0].FindItemByItemID(this.merchandiseInfo.itemid);
		}
		this.MerchandiseIcon.SetVirtualItem(this.merchandiseInfo, desiredTransactionCount * this.merchandiseSellSize, (item == null) ? 0UL : item.skin, this.merchIsBP);
		if (item != null && this.merchandiseInfo.condition.enabled)
		{
			this.MerchandiseIcon.UpdateCondition(item.maxCondition, item.conditionNormalized);
		}
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x00056314 File Offset: 0x00054514
	public void UpdateNotifications()
	{
		if (this.vendingPanel == null)
		{
			return;
		}
		if (this.vendingPanel.GetVendingMachine() == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		int num2 = this.CurrencyAvailable();
		int desiredTransactionCount = this.GetDesiredTransactionCount();
		if (num2 >= this.currencyAmountPerItem * desiredTransactionCount)
		{
			flag = true;
		}
		foreach (ItemContainer itemContainer in LocalPlayer.Entity.inventory.loot.containers)
		{
			List<Item> list;
			if (this.merchIsBP)
			{
				list = Enumerable.ToList<Item>(Enumerable.Where<Item>(itemContainer.FindItemsByItemID(ItemManager.FindItemDefinition("blueprintbase").itemid), (Item x) => x.blueprintTarget == this.merchandiseInfo.itemid));
			}
			else
			{
				list = itemContainer.FindItemsByItemID(this.merchandiseInfo.itemid);
			}
			num += Enumerable.Sum<Item>(list, (Item x) => x.amount);
			if (num >= this.merchandiseSellSize * desiredTransactionCount)
			{
				flag2 = true;
			}
		}
		this.outOfStockNotification.SetActive(!flag2);
		this.cantaffordNotification.SetActive(flag2 && !flag);
		this.buyButton.GetComponent<Button>().enabled = (flag2 && flag);
		this.dirty = false;
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x00056478 File Offset: 0x00054678
	public bool CanAfford()
	{
		int num = this.CurrencyAvailable();
		int desiredTransactionCount = this.GetDesiredTransactionCount();
		return num >= this.currencyAmountPerItem * desiredTransactionCount;
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x0000A916 File Offset: 0x00008B16
	public void BuyClick()
	{
		if (this.vendingPanel)
		{
			this.vendingPanel.BuyButtonClicked(this.index, this.GetDesiredTransactionCount());
		}
	}
}
