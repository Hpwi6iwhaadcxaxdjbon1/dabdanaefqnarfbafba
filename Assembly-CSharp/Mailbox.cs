using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x0200003C RID: 60
public class Mailbox : StorageContainer
{
	// Token: 0x0400022E RID: 558
	private Option __menuOption_Full;

	// Token: 0x0400022F RID: 559
	public string ownerPanel;

	// Token: 0x04000230 RID: 560
	public GameObjectRef mailDropSound;

	// Token: 0x04000231 RID: 561
	public bool autoSubmitWhenClosed;

	// Token: 0x04000232 RID: 562
	public bool shouldMarkAsFull;

	// Token: 0x060004BE RID: 1214 RVA: 0x0003CB20 File Offset: 0x0003AD20
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("Mailbox.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Full", 0.1f))
			{
				if (this.Full_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Full.show = true;
					this.__menuOption_Full.showDisabled = false;
					this.__menuOption_Full.longUseOnly = false;
					this.__menuOption_Full.order = 0;
					this.__menuOption_Full.icon = "close";
					this.__menuOption_Full.desc = "full_desc";
					this.__menuOption_Full.title = "full";
					if (this.__menuOption_Full.function == null)
					{
						this.__menuOption_Full.function = new Action<BasePlayer>(this.Full);
					}
					list.Add(this.__menuOption_Full);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060004BF RID: 1215 RVA: 0x00006318 File Offset: 0x00004518
	public override bool HasMenuOptions
	{
		get
		{
			return this.Full_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0003CC28 File Offset: 0x0003AE28
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Mailbox.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060004C1 RID: 1217 RVA: 0x0000632F File Offset: 0x0000452F
	public int mailInputSlot
	{
		get
		{
			return this.inventorySlots - 1;
		}
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x00002E6B File Offset: 0x0000106B
	public virtual bool PlayerIsOwner(BasePlayer player)
	{
		return player.CanBuild();
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x00006339 File Offset: 0x00004539
	public bool IsFull()
	{
		return this.shouldMarkAsFull && base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x00006350 File Offset: 0x00004550
	public void MarkFull(bool full)
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, this.shouldMarkAsFull && full, false, true);
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x00002ECE File Offset: 0x000010CE
	[BaseEntity.Menu.Description("full_desc", "Box is full")]
	[BaseEntity.Menu("full", "Full")]
	[BaseEntity.Menu.ShowIf("Full_ShowIf")]
	[BaseEntity.Menu.Icon("close")]
	public void Full(BasePlayer player)
	{
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x00006367 File Offset: 0x00004567
	public bool Full_ShowIf(BasePlayer player)
	{
		return this.IsFull() && !this.PlayerIsOwner(player);
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0000637D File Offset: 0x0000457D
	public override bool ShouldShowLootMenus()
	{
		return base.ShouldShowLootMenus() && (this.PlayerIsOwner(LocalPlayer.Entity) || !this.IsFull());
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x000063A1 File Offset: 0x000045A1
	public void TrySubmit()
	{
		base.ServerRPC("RPC_Submit");
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x000063AE File Offset: 0x000045AE
	public override int GetMoveToSlotIndex(BasePlayer player)
	{
		if (this.PlayerIsOwner(player))
		{
			return -1;
		}
		return this.mailInputSlot;
	}
}
