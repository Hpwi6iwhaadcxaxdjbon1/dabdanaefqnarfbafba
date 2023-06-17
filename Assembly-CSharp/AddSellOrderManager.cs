using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000DC RID: 220
public class AddSellOrderManager : MonoBehaviour
{
	// Token: 0x040006F4 RID: 1780
	public VirtualItemIcon sellItemIcon;

	// Token: 0x040006F5 RID: 1781
	public VirtualItemIcon currencyItemIcon;

	// Token: 0x040006F6 RID: 1782
	public GameObject itemSearchParent;

	// Token: 0x040006F7 RID: 1783
	public ItemSearchEntry itemSearchEntryPrefab;

	// Token: 0x040006F8 RID: 1784
	public InputField sellItemInput;

	// Token: 0x040006F9 RID: 1785
	public InputField sellItemAmount;

	// Token: 0x040006FA RID: 1786
	public InputField currencyItemInput;

	// Token: 0x040006FB RID: 1787
	public InputField currencyItemAmount;

	// Token: 0x040006FC RID: 1788
	public VendingPanelAdmin adminPanel;

	// Token: 0x06000A8C RID: 2700 RVA: 0x00055870 File Offset: 0x00053A70
	public void ItemSelectionMade(ItemDefinition info, bool asBP)
	{
		this.itemSearchParent.transform.DestroyChildren();
		if (this.currencyItemInput.text == "")
		{
			this.sellItemIcon.SetVirtualItem(info, 1, 0UL, asBP);
			this.sellItemInput.text = "";
			this.sellItemAmount.text = "";
		}
		else
		{
			this.currencyItemIcon.SetVirtualItem(info, 1, 0UL, asBP);
			this.currencyItemInput.text = "";
			this.currencyItemAmount.text = "1";
		}
		this.ClampAmountValues();
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x0005590C File Offset: 0x00053B0C
	public void Search(string search)
	{
		if (search == "")
		{
			search = "BALLS BALLS BALLS";
		}
		IEnumerable<ItemDefinition> enumerable = Enumerable.OrderBy<ItemDefinition, int>(Enumerable.Take<ItemDefinition>(Enumerable.Where<ItemDefinition>(ItemManager.itemList, (ItemDefinition x) => StringEx.Contains(x.shortname, search, 1) || StringEx.Contains(x.displayName.translated, search, 1) || StringEx.Contains(x.displayDescription.translated, search, 1)), 60), (ItemDefinition y) => y.displayName.translated.Length);
		this.itemSearchParent.transform.DestroyChildren();
		int num = 0;
		foreach (ItemDefinition info in enumerable)
		{
			num++;
			ItemSearchEntry itemSearchEntry = Object.Instantiate<ItemSearchEntry>(this.itemSearchEntryPrefab);
			itemSearchEntry.transform.SetParent(this.itemSearchParent.transform, false);
			itemSearchEntry.Setup(info, this);
			if (num >= 5)
			{
				break;
			}
		}
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x0000A5C6 File Offset: 0x000087C6
	public void OnSellSearchChanged()
	{
		if (this.currencyItemInput.text != "")
		{
			this.currencyItemInput.text = "";
		}
		this.Search(this.sellItemInput.text);
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x0000A600 File Offset: 0x00008800
	public void OnCurrencySearchChanged()
	{
		if (this.sellItemInput.text != "")
		{
			this.sellItemInput.text = "";
		}
		this.Search(this.currencyItemInput.text);
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x00002ECE File Offset: 0x000010CE
	public void OnSellSearchComplete()
	{
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x000559FC File Offset: 0x00053BFC
	public void AddSellOrder()
	{
		VendingMachine vendingMachine = this.adminPanel.GetVendingMachine();
		if (vendingMachine == null)
		{
			return;
		}
		if (this.sellItemIcon.itemDef == null || this.currencyItemIcon.itemDef == null)
		{
			return;
		}
		byte arg = 0;
		if (this.sellItemIcon.asBlueprint)
		{
			arg = 1;
		}
		if (this.currencyItemIcon.asBlueprint)
		{
			arg = 2;
		}
		if (this.sellItemIcon.asBlueprint && this.currencyItemIcon.asBlueprint)
		{
			arg = 3;
		}
		vendingMachine.ServerRPC<int, int, int, int, byte>("RPC_AddSellOrder", this.sellItemIcon.itemDef.itemid, this.GetIntAmount(this.sellItemAmount.text), this.currencyItemIcon.itemDef.itemid, this.GetIntAmount(this.currencyItemAmount.text), arg);
		this.ResetSellOrderObjects();
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x0000A63A File Offset: 0x0000883A
	public void ResetSellOrderObjects()
	{
		this.sellItemIcon.SetVirtualItem(null, 0, 0UL, false);
		this.currencyItemIcon.SetVirtualItem(null, 0, 0UL, false);
		this.ClampAmountValues();
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x0000A662 File Offset: 0x00008862
	public void OnAmountTextChanged()
	{
		this.ClampAmountValues();
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x00055AD8 File Offset: 0x00053CD8
	public void ClampAmountValues()
	{
		int amount = this.ClampedAmountValue(this.currencyItemAmount.text, this.currencyItemIcon.itemDef, false);
		this.currencyItemAmount.text = amount.ToString();
		this.currencyItemIcon.UpdateAmount(amount);
		int amount2 = this.ClampedAmountValue(this.sellItemAmount.text, this.sellItemIcon.itemDef, true);
		this.sellItemAmount.text = amount2.ToString();
		this.sellItemIcon.UpdateAmount(amount2);
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x0000A66A File Offset: 0x0000886A
	public int ClampedAmountValue(string amount, ItemDefinition itemDef, bool limitToStackable = true)
	{
		return Mathf.Clamp(this.GetIntAmount(amount), 1, (itemDef == null) ? 1 : (limitToStackable ? itemDef.stackable : 100000));
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x00055B60 File Offset: 0x00053D60
	public int GetIntAmount(string text)
	{
		int result = 0;
		int.TryParse(text, ref result);
		return result;
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x00055B7C File Offset: 0x00053D7C
	public void CurrencyPlusMinus(int delta)
	{
		this.currencyItemAmount.text = (this.GetIntAmount(this.currencyItemAmount.text) + delta).ToString();
		this.ClampAmountValues();
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x00055BB8 File Offset: 0x00053DB8
	public void SellItemPlusMinus(int delta)
	{
		this.sellItemAmount.text = (this.GetIntAmount(this.sellItemAmount.text) + delta).ToString();
		this.ClampAmountValues();
	}
}
