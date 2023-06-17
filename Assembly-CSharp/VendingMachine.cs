using System;
using System.Collections.Generic;
using ConVar;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class VendingMachine : StorageContainer
{
	// Token: 0x0400030B RID: 779
	private Option __menuOption_Menu_BroadcastOff;

	// Token: 0x0400030C RID: 780
	private Option __menuOption_Menu_BroadcastOn;

	// Token: 0x0400030D RID: 781
	private Option __menuOption_Menu_OpenAdmin;

	// Token: 0x0400030E RID: 782
	private Option __menuOption_Menu_RotateVM;

	// Token: 0x0400030F RID: 783
	private Option __menuOption_Menu_Shop;

	// Token: 0x04000310 RID: 784
	[Header("VendingMachine")]
	public GameObjectRef adminMenuPrefab;

	// Token: 0x04000311 RID: 785
	public string customerPanel = "";

	// Token: 0x04000312 RID: 786
	public VendingMachine.SellOrderContainer sellOrders;

	// Token: 0x04000313 RID: 787
	public SoundPlayer buySound;

	// Token: 0x04000314 RID: 788
	public string shopName = "A Shop";

	// Token: 0x04000315 RID: 789
	public GameObjectRef mapMarkerPrefab;

	// Token: 0x04000316 RID: 790
	public ItemDefinition blueprintBaseDef;

	// Token: 0x04000317 RID: 791
	private List<SoundManager.ScheduledSound> scheduledPhysSounds = new List<SoundManager.ScheduledSound>();

	// Token: 0x060005F3 RID: 1523 RVA: 0x00041AD4 File Offset: 0x0003FCD4
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("VendingMachine.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_BroadcastOff", 0.1f))
			{
				if (this.Menu_Broadcast_Off_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_BroadcastOff.show = true;
					this.__menuOption_Menu_BroadcastOff.showDisabled = false;
					this.__menuOption_Menu_BroadcastOff.longUseOnly = false;
					this.__menuOption_Menu_BroadcastOff.order = 1000;
					this.__menuOption_Menu_BroadcastOff.icon = "close";
					this.__menuOption_Menu_BroadcastOff.desc = "broadcast_off_desc";
					this.__menuOption_Menu_BroadcastOff.title = "broadcast_off";
					if (this.__menuOption_Menu_BroadcastOff.function == null)
					{
						this.__menuOption_Menu_BroadcastOff.function = new Action<BasePlayer>(this.Menu_BroadcastOff);
					}
					list.Add(this.__menuOption_Menu_BroadcastOff);
				}
			}
			using (TimeWarning.New("Menu_BroadcastOn", 0.1f))
			{
				if (this.Menu_Broadcast_On_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_BroadcastOn.show = true;
					this.__menuOption_Menu_BroadcastOn.showDisabled = false;
					this.__menuOption_Menu_BroadcastOn.longUseOnly = false;
					this.__menuOption_Menu_BroadcastOn.order = 1000;
					this.__menuOption_Menu_BroadcastOn.icon = "broadcast";
					this.__menuOption_Menu_BroadcastOn.desc = "broadcast_on_desc";
					this.__menuOption_Menu_BroadcastOn.title = "broadcast_on";
					if (this.__menuOption_Menu_BroadcastOn.function == null)
					{
						this.__menuOption_Menu_BroadcastOn.function = new Action<BasePlayer>(this.Menu_BroadcastOn);
					}
					list.Add(this.__menuOption_Menu_BroadcastOn);
				}
			}
			using (TimeWarning.New("Menu_OpenAdmin", 0.1f))
			{
				if (this.Menu_OpenAdmin_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_OpenAdmin.show = true;
					this.__menuOption_Menu_OpenAdmin.showDisabled = false;
					this.__menuOption_Menu_OpenAdmin.longUseOnly = false;
					this.__menuOption_Menu_OpenAdmin.order = 0;
					this.__menuOption_Menu_OpenAdmin.icon = "gear";
					this.__menuOption_Menu_OpenAdmin.desc = "open_admin_desc";
					this.__menuOption_Menu_OpenAdmin.title = "open_admin";
					if (this.__menuOption_Menu_OpenAdmin.function == null)
					{
						this.__menuOption_Menu_OpenAdmin.function = new Action<BasePlayer>(this.Menu_OpenAdmin);
					}
					list.Add(this.__menuOption_Menu_OpenAdmin);
				}
			}
			using (TimeWarning.New("Menu_RotateVM", 0.1f))
			{
				if (this.Menu_RotateVM_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_RotateVM.show = true;
					this.__menuOption_Menu_RotateVM.showDisabled = false;
					this.__menuOption_Menu_RotateVM.longUseOnly = false;
					this.__menuOption_Menu_RotateVM.order = 1000;
					this.__menuOption_Menu_RotateVM.icon = "rotate";
					this.__menuOption_Menu_RotateVM.desc = "open_rotate_vm_desc";
					this.__menuOption_Menu_RotateVM.title = "rotate_vm";
					if (this.__menuOption_Menu_RotateVM.function == null)
					{
						this.__menuOption_Menu_RotateVM.function = new Action<BasePlayer>(this.Menu_RotateVM);
					}
					list.Add(this.__menuOption_Menu_RotateVM);
				}
			}
			using (TimeWarning.New("Menu_Shop", 0.1f))
			{
				if (this.Menu_Shop_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Shop.show = true;
					this.__menuOption_Menu_Shop.showDisabled = false;
					this.__menuOption_Menu_Shop.longUseOnly = false;
					this.__menuOption_Menu_Shop.order = 0;
					this.__menuOption_Menu_Shop.icon = "store";
					this.__menuOption_Menu_Shop.desc = "open_shop_desc";
					this.__menuOption_Menu_Shop.title = "open_shop";
					if (this.__menuOption_Menu_Shop.function == null)
					{
						this.__menuOption_Menu_Shop.function = new Action<BasePlayer>(this.Menu_Shop);
					}
					list.Add(this.__menuOption_Menu_Shop);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060005F4 RID: 1524 RVA: 0x00041F50 File Offset: 0x00040150
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Broadcast_Off_ShowIf(LocalPlayer.Entity) || this.Menu_Broadcast_On_ShowIf(LocalPlayer.Entity) || this.Menu_OpenAdmin_ShowIf(LocalPlayer.Entity) || this.Menu_RotateVM_ShowIf(LocalPlayer.Entity) || this.Menu_Shop_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x00041FB0 File Offset: 0x000401B0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VendingMachine.OnRpcMessage", 0.1f))
		{
			if (rpc == 3920571215U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_CancelVendingSounds ");
				}
				using (TimeWarning.New("CLIENT_CancelVendingSounds", 0.1f))
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
							this.CLIENT_CancelVendingSounds(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_CancelVendingSounds", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
			if (rpc == 1729284359U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_OpenAdminMenu ");
				}
				using (TimeWarning.New("CLIENT_OpenAdminMenu", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CLIENT_OpenAdminMenu(rpc2);
						}
					}
					catch (Exception exception2)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_OpenAdminMenu", true);
						Debug.LogException(exception2);
					}
				}
				return true;
			}
			if (rpc == 1689377500U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_ReceiveSellOrders ");
				}
				using (TimeWarning.New("CLIENT_ReceiveSellOrders", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CLIENT_ReceiveSellOrders(msg3);
						}
					}
					catch (Exception exception3)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_ReceiveSellOrders", true);
						Debug.LogException(exception3);
					}
				}
				return true;
			}
			if (rpc == 1253216304U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_StartVendingSounds ");
				}
				using (TimeWarning.New("CLIENT_StartVendingSounds", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CLIENT_StartVendingSounds(msg4);
						}
					}
					catch (Exception exception4)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_StartVendingSounds", true);
						Debug.LogException(exception4);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x000423F4 File Offset: 0x000405F4
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vendingMachine != null)
		{
			this.shopName = info.msg.vendingMachine.shopName;
			if (info.msg.vendingMachine.sellOrderContainer != null)
			{
				this.sellOrders = info.msg.vendingMachine.sellOrderContainer;
				this.sellOrders.ShouldPool = false;
			}
		}
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00007392 File Offset: 0x00005592
	public virtual void InstallDefaultSellOrders()
	{
		this.sellOrders = new VendingMachine.SellOrderContainer();
		this.sellOrders.ShouldPool = false;
		this.sellOrders.sellOrders = new List<VendingMachine.SellOrder>();
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00042460 File Offset: 0x00040660
	[BaseEntity.RPC_Client]
	public void CLIENT_StartVendingSounds(BaseEntity.RPCMessage msg)
	{
		if (this.buySound != null)
		{
			this.buySound.Play();
		}
		int num = msg.read.Int32();
		if (num < 0 || num > this.sellOrders.sellOrders.Count)
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(this.sellOrders.sellOrders[num].itemToSellID);
		if (itemDefinition == null)
		{
			return;
		}
		SoundDefinition physImpactSoundDef = itemDefinition.physImpactSoundDef;
		if (physImpactSoundDef == null)
		{
			return;
		}
		this.scheduledPhysSounds.Add(SoundManager.ScheduleOneshot(physImpactSoundDef, UnityEngine.Time.time + 1.46f, base.transform.position, 0.5f));
		this.scheduledPhysSounds.Add(SoundManager.ScheduleOneshot(physImpactSoundDef, UnityEngine.Time.time + 1.82f, base.transform.position, 0.7f));
		this.scheduledPhysSounds.Add(SoundManager.ScheduleOneshot(physImpactSoundDef, UnityEngine.Time.time + 2.08f, base.transform.position, 1f));
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x00042568 File Offset: 0x00040768
	[BaseEntity.RPC_Client]
	public void CLIENT_CancelVendingSounds(BaseEntity.RPCMessage msg)
	{
		if (this.buySound != null)
		{
			this.buySound.FadeOutAndRecycle(0.2f);
		}
		for (int i = 0; i < this.scheduledPhysSounds.Count; i++)
		{
			SoundManager.CancelScheduledSound(this.scheduledPhysSounds[i]);
		}
		this.scheduledPhysSounds.Clear();
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x000073BB File Offset: 0x000055BB
	[BaseEntity.RPC_Client]
	public void CLIENT_ReceiveSellOrders(BaseEntity.RPCMessage msg)
	{
		this.sellOrders = VendingMachine.SellOrderContainer.Deserialize(msg.read);
		this.sellOrders.ShouldPool = false;
		if (UIDialog.isOpen)
		{
			(ListComponent<UIDialog>.InstanceList[0] as VendingPanelAdmin).UpdateSellOrders();
		}
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool ShouldShowAdminPanel()
	{
		return true;
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x000425C8 File Offset: 0x000407C8
	public override bool ShouldShowLootMenus()
	{
		bool flag = LocalPlayer.Entity != null && this.PlayerBehind(LocalPlayer.Entity);
		return base.ShouldShowLootMenus() && flag;
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x000073F6 File Offset: 0x000055F6
	[BaseEntity.Menu.Description("open_shop_desc", "Open the shopping panel")]
	[BaseEntity.Menu.ShowIf("Menu_Shop_ShowIf")]
	[BaseEntity.Menu("open_shop", "Shop")]
	[BaseEntity.Menu.Icon("store")]
	public void Menu_Shop(BasePlayer player)
	{
		base.ServerRPC("RPC_OpenShop");
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00007403 File Offset: 0x00005603
	public bool Menu_Shop_ShowIf(BasePlayer player)
	{
		return this.PlayerInfront(player) && base.OccupiedCheck(player);
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00007417 File Offset: 0x00005617
	[BaseEntity.Menu.Description("open_admin_desc", "Open the Administration Panel")]
	[BaseEntity.Menu("open_admin", "Administrate")]
	[BaseEntity.Menu.Icon("gear")]
	[BaseEntity.Menu.ShowIf("Menu_OpenAdmin_ShowIf")]
	public void Menu_OpenAdmin(BasePlayer player)
	{
		base.ServerRPC("RPC_OpenAdmin");
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x00007424 File Offset: 0x00005624
	public bool Menu_OpenAdmin_ShowIf(BasePlayer player)
	{
		return this.PlayerBehind(player) && this.ShouldShowAdminPanel();
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x00007437 File Offset: 0x00005637
	[BaseEntity.Menu.Description("broadcast_on_desc", "Enable broadcasting the position of this object")]
	[BaseEntity.Menu("broadcast_on", "Broadcast Position", Order = 1000)]
	[BaseEntity.Menu.Icon("broadcast")]
	[BaseEntity.Menu.ShowIf("Menu_Broadcast_On_ShowIf")]
	public void Menu_BroadcastOn(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_Broadcast", true);
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00007445 File Offset: 0x00005645
	public bool Menu_Broadcast_On_ShowIf(BasePlayer player)
	{
		return !this.IsBroadcasting() && this.CanPlayerAdmin(player);
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00007458 File Offset: 0x00005658
	[BaseEntity.Menu("broadcast_off", "Disable Broadcasting", Order = 1000)]
	[BaseEntity.Menu.Description("broadcast_off_desc", "Disable broadcasting the position of this object")]
	[BaseEntity.Menu.ShowIf("Menu_Broadcast_Off_ShowIf")]
	[BaseEntity.Menu.Icon("close")]
	public void Menu_BroadcastOff(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_Broadcast", false);
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x00007466 File Offset: 0x00005666
	public bool Menu_Broadcast_Off_ShowIf(BasePlayer player)
	{
		return this.IsBroadcasting() && this.CanPlayerAdmin(player);
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x00007479 File Offset: 0x00005679
	[BaseEntity.Menu("rotate_vm", "Rotate", Order = 1000)]
	[BaseEntity.Menu.Icon("rotate")]
	[BaseEntity.Menu.ShowIf("Menu_RotateVM_ShowIf")]
	[BaseEntity.Menu.Description("open_rotate_vm_desc", "Rotate the vending machine")]
	public void Menu_RotateVM(BasePlayer player)
	{
		base.ServerRPC("RPC_RotateVM");
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00007486 File Offset: 0x00005686
	public bool Menu_RotateVM_ShowIf(BasePlayer player)
	{
		return player.CanBuild() && this.IsInventoryEmpty();
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00007498 File Offset: 0x00005698
	[BaseEntity.RPC_Client]
	private void CLIENT_OpenAdminMenu(BaseEntity.RPCMessage rpc)
	{
		GameManager.client.CreatePrefab(this.adminMenuPrefab.resourcePath, true).GetComponent<VendingPanelAdmin>().SetVendingMachine(this);
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x000425FC File Offset: 0x000407FC
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		if (UIDialog.isOpen)
		{
			VendingPanelAdmin vendingPanelAdmin = ListComponent<UIDialog>.InstanceList[0] as VendingPanelAdmin;
			if (vendingPanelAdmin && vendingPanelAdmin.GetVendingMachine() == this)
			{
				vendingPanelAdmin.VendingMachineUpdated();
			}
		}
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00002FD4 File Offset: 0x000011D4
	public bool IsBroadcasting()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x00004723 File Offset: 0x00002923
	public bool IsInventoryEmpty()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x00005DC6 File Offset: 0x00003FC6
	public bool IsVending()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x00042644 File Offset: 0x00040844
	public bool PlayerBehind(BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) <= -0.7f;
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00042690 File Offset: 0x00040890
	public bool PlayerInfront(BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) >= 0.7f;
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x000074BB File Offset: 0x000056BB
	public virtual bool CanPlayerAdmin(BasePlayer player)
	{
		return this.PlayerBehind(player) && base.OccupiedCheck(player);
	}

	// Token: 0x02000054 RID: 84
	public static class VendingMachineFlags
	{
		// Token: 0x04000318 RID: 792
		public const BaseEntity.Flags EmptyInv = BaseEntity.Flags.Reserved1;

		// Token: 0x04000319 RID: 793
		public const BaseEntity.Flags IsVending = BaseEntity.Flags.Reserved2;

		// Token: 0x0400031A RID: 794
		public const BaseEntity.Flags Broadcasting = BaseEntity.Flags.Reserved4;

		// Token: 0x0400031B RID: 795
		public const BaseEntity.Flags OutOfStock = BaseEntity.Flags.Reserved5;
	}
}
