using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020002FC RID: 764
public class VendingMachineMapMarker : MapMarker
{
	// Token: 0x040010FF RID: 4351
	public string markerShopName;

	// Token: 0x04001100 RID: 4352
	public VendingMachine server_vendingMachine;

	// Token: 0x04001101 RID: 4353
	public VendingMachine.SellOrderContainer client_sellOrders;

	// Token: 0x06001425 RID: 5157 RVA: 0x0007D7FC File Offset: 0x0007B9FC
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vendingMachine != null)
		{
			this.markerShopName = info.msg.vendingMachine.shopName;
			if (info.msg.vendingMachine.sellOrderContainer != null)
			{
				this.client_sellOrders = info.msg.vendingMachine.sellOrderContainer;
				this.client_sellOrders.ShouldPool = false;
			}
		}
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x0007D868 File Offset: 0x0007BA68
	public override void SetupUIMarker(GameObject marker)
	{
		if (marker == null)
		{
			return;
		}
		UIMapVendingMachineMarker component = marker.GetComponent<UIMapVendingMachineMarker>();
		if (component)
		{
			component.SetOutOfStock(base.HasFlag(BaseEntity.Flags.Busy));
			component.UpdateDisplayName(this.markerShopName, this.client_sellOrders);
		}
	}
}
