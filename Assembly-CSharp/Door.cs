using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x0200002B RID: 43
public class Door : AnimatedBuildingBlock
{
	// Token: 0x040001B3 RID: 435
	private Option __menuOption_Menu_CloseDoor;

	// Token: 0x040001B4 RID: 436
	private Option __menuOption_Menu_KnockDoor;

	// Token: 0x040001B5 RID: 437
	private Option __menuOption_Menu_OpenDoor;

	// Token: 0x040001B6 RID: 438
	private Option __menuOption_Menu_ToggleHatch;

	// Token: 0x040001B7 RID: 439
	public GameObjectRef knockEffect;

	// Token: 0x040001B8 RID: 440
	public bool canTakeLock = true;

	// Token: 0x040001B9 RID: 441
	public bool hasHatch;

	// Token: 0x040001BA RID: 442
	public bool canTakeCloser;

	// Token: 0x040001BB RID: 443
	public bool canTakeKnocker;

	// Token: 0x040001BC RID: 444
	public bool canNpcOpen = true;

	// Token: 0x040001BD RID: 445
	public bool canHandOpen = true;

	// Token: 0x040001BE RID: 446
	public bool isSecurityDoor;

	// Token: 0x0600040C RID: 1036 RVA: 0x0003A0D8 File Offset: 0x000382D8
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("Door.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_CloseDoor", 0.1f))
			{
				if (this.Menu_CloseDoor_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_CloseDoor.show = true;
					this.__menuOption_Menu_CloseDoor.showDisabled = false;
					this.__menuOption_Menu_CloseDoor.longUseOnly = false;
					this.__menuOption_Menu_CloseDoor.order = 0;
					this.__menuOption_Menu_CloseDoor.icon = "close_door";
					this.__menuOption_Menu_CloseDoor.desc = "close_door_desc";
					this.__menuOption_Menu_CloseDoor.title = "close_door";
					if (this.__menuOption_Menu_CloseDoor.function == null)
					{
						this.__menuOption_Menu_CloseDoor.function = new Action<BasePlayer>(this.Menu_CloseDoor);
					}
					list.Add(this.__menuOption_Menu_CloseDoor);
				}
			}
			using (TimeWarning.New("Menu_KnockDoor", 0.1f))
			{
				if (this.Menu_KnockDoor_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_KnockDoor.show = true;
					this.__menuOption_Menu_KnockDoor.showDisabled = false;
					this.__menuOption_Menu_KnockDoor.longUseOnly = false;
					this.__menuOption_Menu_KnockDoor.order = 50;
					this.__menuOption_Menu_KnockDoor.icon = "knock_door";
					this.__menuOption_Menu_KnockDoor.desc = "knock_door_desc";
					this.__menuOption_Menu_KnockDoor.title = "knock_door";
					if (this.__menuOption_Menu_KnockDoor.function == null)
					{
						this.__menuOption_Menu_KnockDoor.function = new Action<BasePlayer>(this.Menu_KnockDoor);
					}
					list.Add(this.__menuOption_Menu_KnockDoor);
				}
			}
			using (TimeWarning.New("Menu_OpenDoor", 0.1f))
			{
				if (this.Menu_OpenDoor_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_OpenDoor.show = true;
					this.__menuOption_Menu_OpenDoor.showDisabled = false;
					this.__menuOption_Menu_OpenDoor.longUseOnly = false;
					this.__menuOption_Menu_OpenDoor.order = 0;
					this.__menuOption_Menu_OpenDoor.icon = "open_door";
					this.__menuOption_Menu_OpenDoor.desc = "open_door_desc";
					this.__menuOption_Menu_OpenDoor.title = "open_door";
					if (this.__menuOption_Menu_OpenDoor.function == null)
					{
						this.__menuOption_Menu_OpenDoor.function = new Action<BasePlayer>(this.Menu_OpenDoor);
					}
					list.Add(this.__menuOption_Menu_OpenDoor);
				}
			}
			using (TimeWarning.New("Menu_ToggleHatch", 0.1f))
			{
				if (this.Menu_ToggleHatch_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_ToggleHatch.show = true;
					this.__menuOption_Menu_ToggleHatch.showDisabled = false;
					this.__menuOption_Menu_ToggleHatch.longUseOnly = false;
					this.__menuOption_Menu_ToggleHatch.order = 100;
					this.__menuOption_Menu_ToggleHatch.icon = "exit";
					this.__menuOption_Menu_ToggleHatch.desc = "toggle_hatch_desc";
					this.__menuOption_Menu_ToggleHatch.title = "toggle_hatch";
					if (this.__menuOption_Menu_ToggleHatch.function == null)
					{
						this.__menuOption_Menu_ToggleHatch.function = new Action<BasePlayer>(this.Menu_ToggleHatch);
					}
					list.Add(this.__menuOption_Menu_ToggleHatch);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x0600040D RID: 1037 RVA: 0x0003A474 File Offset: 0x00038674
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_CloseDoor_ShowIf(LocalPlayer.Entity) || this.Menu_KnockDoor_ShowIf(LocalPlayer.Entity) || this.Menu_OpenDoor_ShowIf(LocalPlayer.Entity) || this.Menu_ToggleHatch_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x0003A4C4 File Offset: 0x000386C4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Door.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00005A8F File Offset: 0x00003C8F
	public override bool HasSlot(BaseEntity.Slot slot)
	{
		return (slot == BaseEntity.Slot.Lock && this.canTakeLock) || slot == BaseEntity.Slot.UpperModifier || (slot == BaseEntity.Slot.CenterDecoration && this.canTakeCloser) || (slot == BaseEntity.Slot.LowerCenterDecoration && this.canTakeKnocker) || base.HasSlot(slot);
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x0003A508 File Offset: 0x00038708
	public override bool CanPickup(BasePlayer player)
	{
		return base.IsOpen() && !base.GetSlot(BaseEntity.Slot.Lock) && !base.GetSlot(BaseEntity.Slot.UpperModifier) && !base.GetSlot(BaseEntity.Slot.CenterDecoration) && !base.GetSlot(BaseEntity.Slot.LowerCenterDecoration) && base.CanPickup(player);
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00005AC7 File Offset: 0x00003CC7
	[BaseEntity.Menu.ShowIf("Menu_OpenDoor_ShowIf")]
	[BaseEntity.Menu.Icon("open_door")]
	[BaseEntity.Menu("open_door", "Open Door")]
	[BaseEntity.Menu.Description("open_door_desc", "Turn from being a closed door to an open one")]
	public void Menu_OpenDoor(BasePlayer player)
	{
		base.ServerRPC("RPC_OpenDoor");
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00005AD4 File Offset: 0x00003CD4
	public bool Menu_OpenDoor_ShowIf(BasePlayer player)
	{
		return !base.IsOpen() && !base.IsLocked() && this.canHandOpen;
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00005AEE File Offset: 0x00003CEE
	[BaseEntity.Menu.Description("close_door_desc", "Turn from being an open door to a closed one")]
	[BaseEntity.Menu.Icon("close_door")]
	[BaseEntity.Menu.ShowIf("Menu_CloseDoor_ShowIf")]
	[BaseEntity.Menu("close_door", "Close Door")]
	public void Menu_CloseDoor(BasePlayer player)
	{
		base.ServerRPC("RPC_CloseDoor");
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00005AFB File Offset: 0x00003CFB
	public bool Menu_CloseDoor_ShowIf(BasePlayer player)
	{
		return base.IsOpen() && !base.IsLocked() && this.canHandOpen;
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00005B15 File Offset: 0x00003D15
	[BaseEntity.Menu("knock_door", "Knock", Order = 50)]
	[BaseEntity.Menu.Icon("knock_door")]
	[BaseEntity.Menu.ShowIf("Menu_KnockDoor_ShowIf")]
	[BaseEntity.Menu.Description("knock_door_desc", "Knock the door to see if anyone's home")]
	public void Menu_KnockDoor(BasePlayer player)
	{
		base.ServerRPC("RPC_KnockDoor");
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00005B22 File Offset: 0x00003D22
	public bool Menu_KnockDoor_ShowIf(BasePlayer player)
	{
		return this.knockEffect.isValid && !base.IsOpen();
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x00005B3C File Offset: 0x00003D3C
	[BaseEntity.Menu.Icon("exit")]
	[BaseEntity.Menu.ShowIf("Menu_ToggleHatch_ShowIf")]
	[BaseEntity.Menu("toggle_hatch", "Toggle Hatch", Order = 100)]
	[BaseEntity.Menu.Description("toggle_hatch_desc", "Open or Close the doors hatch")]
	public void Menu_ToggleHatch(BasePlayer player)
	{
		base.ServerRPC("RPC_ToggleHatch");
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x00005B49 File Offset: 0x00003D49
	public bool Menu_ToggleHatch_ShowIf(BasePlayer player)
	{
		return this.hasHatch;
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool NeedsCrosshair()
	{
		return true;
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00004962 File Offset: 0x00002B62
	public override float BoundsPadding()
	{
		return 2f;
	}
}
