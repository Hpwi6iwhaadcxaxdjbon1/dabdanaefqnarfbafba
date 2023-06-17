using System;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class ShopFront : StorageContainer
{
	// Token: 0x0400066C RID: 1644
	public BasePlayer vendorPlayer;

	// Token: 0x0400066D RID: 1645
	public BasePlayer customerPlayer;

	// Token: 0x0400066E RID: 1646
	public GameObjectRef transactionCompleteEffect;

	// Token: 0x060009EE RID: 2542 RVA: 0x00053068 File Offset: 0x00051268
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ShopFront.OnRpcMessage", 0.1f))
		{
			if (rpc == 2438600542U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_ReceivePlayers ");
				}
				using (TimeWarning.New("CLIENT_ReceivePlayers", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CLIENT_ReceivePlayers(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_ReceivePlayers", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x0000508F File Offset: 0x0000328F
	public bool TradeLocked()
	{
		return false;
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00009D86 File Offset: 0x00007F86
	public bool IsTradingPlayer(BasePlayer player)
	{
		return player != null && (this.IsPlayerCustomer(player) || this.IsPlayerVendor(player));
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00009DA5 File Offset: 0x00007FA5
	public bool IsPlayerCustomer(BasePlayer player)
	{
		return player == this.customerPlayer;
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x00009DB3 File Offset: 0x00007FB3
	public bool IsPlayerVendor(BasePlayer player)
	{
		return player == this.vendorPlayer;
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x00053184 File Offset: 0x00051384
	public bool PlayerInVendorPos(BasePlayer player)
	{
		return Vector3.Dot(base.transform.right, (player.transform.position - base.transform.position).normalized) <= -0.7f;
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x000531D0 File Offset: 0x000513D0
	public bool PlayerInCustomerPos(BasePlayer player)
	{
		return Vector3.Dot(base.transform.right, (player.transform.position - base.transform.position).normalized) >= 0.7f;
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00009DC1 File Offset: 0x00007FC1
	public bool LootEligable(BasePlayer player)
	{
		return !(player == null) && ((this.PlayerInVendorPos(player) && this.vendorPlayer == null) || (this.PlayerInCustomerPos(player) && this.customerPlayer == null));
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00009E01 File Offset: 0x00008001
	public override int GetMoveToContainerIndex(BasePlayer player)
	{
		if (player == this.customerPlayer)
		{
			return 1;
		}
		return base.GetMoveToContainerIndex(player);
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x00009E1A File Offset: 0x0000801A
	public override bool ShouldShowLootMenus()
	{
		return this.LootEligable(LocalPlayer.Entity) && base.ShouldShowLootMenus();
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x0005321C File Offset: 0x0005141C
	[BaseEntity.RPC_Client]
	public void CLIENT_ReceivePlayers(BaseEntity.RPCMessage msg)
	{
		uint uid = msg.read.UInt32();
		uint uid2 = msg.read.UInt32();
		this.vendorPlayer = (BaseNetworkable.clientEntities.Find(uid) as BasePlayer);
		this.customerPlayer = (BaseNetworkable.clientEntities.Find(uid2) as BasePlayer);
	}

	// Token: 0x020000B8 RID: 184
	public static class ShopFrontFlags
	{
		// Token: 0x0400066F RID: 1647
		public const BaseEntity.Flags VendorAccepted = BaseEntity.Flags.Reserved1;

		// Token: 0x04000670 RID: 1648
		public const BaseEntity.Flags CustomerAccepted = BaseEntity.Flags.Reserved2;

		// Token: 0x04000671 RID: 1649
		public const BaseEntity.Flags Exchanging = BaseEntity.Flags.Reserved3;
	}
}
