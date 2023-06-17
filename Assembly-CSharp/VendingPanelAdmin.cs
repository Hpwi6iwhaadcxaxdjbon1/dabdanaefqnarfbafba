using System;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000E7 RID: 231
public class VendingPanelAdmin : UIDialog
{
	// Token: 0x0400072B RID: 1835
	public GameObject sellOrderAdminContainer;

	// Token: 0x0400072C RID: 1836
	public GameObject sellOrderAdminPrefab;

	// Token: 0x0400072D RID: 1837
	public InputField storeNameInputField;

	// Token: 0x0400072E RID: 1838
	private VendingMachine vendingMachine;

	// Token: 0x06000AE2 RID: 2786 RVA: 0x0000AA40 File Offset: 0x00008C40
	public override void OpenDialog()
	{
		base.OpenDialog();
		this.UpdateSellOrders();
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x0005650C File Offset: 0x0005470C
	public override void CloseDialog()
	{
		if (this.vendingMachine)
		{
			string arg = this.storeNameInputField.text;
			arg = StringEx.ToPrintable(this.storeNameInputField.text, 32);
			this.vendingMachine.ServerRPC<string>("RPC_UpdateShopName", arg);
		}
		base.CloseDialog();
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x0000AA4E File Offset: 0x00008C4E
	public void Awake()
	{
		this.UpdateSellOrders();
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0000AA56 File Offset: 0x00008C56
	public void VendingMachineUpdated()
	{
		this.UpdateSellOrders();
		this.storeNameInputField.text = this.vendingMachine.shopName;
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0005655C File Offset: 0x0005475C
	public void UpdateSellOrders()
	{
		VendingMachine vendingMachine = this.GetVendingMachine();
		if (vendingMachine == null)
		{
			return;
		}
		this.sellOrderAdminContainer.transform.DestroyChildren();
		int num = 0;
		foreach (VendingMachine.SellOrder so in vendingMachine.sellOrders.sellOrders)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.sellOrderAdminPrefab);
			gameObject.transform.SetParent(this.sellOrderAdminContainer.transform, false);
			gameObject.GetComponent<AdminExistingSellOrder>().Setup(so, num, this);
			num++;
		}
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0000AA74 File Offset: 0x00008C74
	public void SetVendingMachine(VendingMachine vend)
	{
		this.vendingMachine = vend;
		this.VendingMachineUpdated();
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0000AA83 File Offset: 0x00008C83
	public VendingMachine GetVendingMachine()
	{
		return this.vendingMachine;
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0000AA8B File Offset: 0x00008C8B
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || this.vendingMachine == null)
		{
			this.CloseDialog();
		}
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x00056604 File Offset: 0x00054804
	public void DeleteClicked(int index)
	{
		VendingMachine vendingMachine = this.GetVendingMachine();
		if (vendingMachine)
		{
			vendingMachine.ServerRPC<int>("RPC_DeleteSellOrder", index);
		}
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x00002ECE File Offset: 0x000010CE
	public void StoreNameTextChanged()
	{
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x00002ECE File Offset: 0x000010CE
	public void StoreNameTextComplete()
	{
	}
}
