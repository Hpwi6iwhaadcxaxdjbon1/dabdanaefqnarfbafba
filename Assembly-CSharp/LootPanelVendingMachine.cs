using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class LootPanelVendingMachine : LootPanel
{
	// Token: 0x04000710 RID: 1808
	public GameObject sellOrderPrefab;

	// Token: 0x04000711 RID: 1809
	public GameObject sellOrderContainer;

	// Token: 0x04000712 RID: 1810
	public GameObject busyOverlayPrefab;

	// Token: 0x04000713 RID: 1811
	private GameObject busyOverlayInstance;

	// Token: 0x06000AB8 RID: 2744 RVA: 0x0000A845 File Offset: 0x00008A45
	public void OnEnable()
	{
		this.Initialize();
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0000A84D File Offset: 0x00008A4D
	public void Initialize()
	{
		this.UpdateSellOrders();
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x0000A855 File Offset: 0x00008A55
	public VendingMachine GetVendingMachine()
	{
		return base.GetContainerEntity() as VendingMachine;
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00055E14 File Offset: 0x00054014
	public void UpdateSellOrders()
	{
		VendingMachine vendingMachine = this.GetVendingMachine();
		if (vendingMachine == null)
		{
			return;
		}
		if (vendingMachine.IsVending())
		{
			return;
		}
		this.sellOrderContainer.transform.DestroyChildren();
		int num = 0;
		foreach (VendingMachine.SellOrder so in vendingMachine.sellOrders.sellOrders)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.sellOrderPrefab);
			gameObject.transform.SetParent(this.sellOrderContainer.transform, false);
			gameObject.GetComponent<SellOrderEntry>().Setup(so, num, this, null);
			num++;
		}
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00055EC8 File Offset: 0x000540C8
	public void SetBusyOverlay(bool on)
	{
		if (on && this.busyOverlayInstance == null)
		{
			this.sellOrderContainer.transform.DestroyChildren();
			this.busyOverlayInstance = Object.Instantiate<GameObject>(this.busyOverlayPrefab);
			this.busyOverlayInstance.transform.SetParent(this.sellOrderContainer.transform, false);
		}
		if (!on && this.busyOverlayInstance != null)
		{
			Object.Destroy(this.busyOverlayInstance.gameObject);
			this.busyOverlayInstance = null;
			this.UpdateSellOrders();
		}
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x00055F54 File Offset: 0x00054154
	public override void Update()
	{
		base.Update();
		VendingMachine vendingMachine = this.GetVendingMachine();
		if (vendingMachine)
		{
			this.SetBusyOverlay(vendingMachine.IsVending());
		}
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x00055F84 File Offset: 0x00054184
	public void BuyButtonClicked(int index, int multiplier = 1)
	{
		VendingMachine vendingMachine = this.GetVendingMachine();
		if (vendingMachine == null)
		{
			return;
		}
		if (vendingMachine.IsVending())
		{
			return;
		}
		vendingMachine.ServerRPC<int, int>("BuyItem", index, multiplier);
	}
}
