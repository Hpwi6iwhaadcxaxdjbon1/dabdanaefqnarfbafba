using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000029 RID: 41
public class CustomDoorManipulator : DoorManipulator
{
	// Token: 0x040001AE RID: 430
	private Option __menuOption_Menu_Pair;

	// Token: 0x040001AF RID: 431
	private Option __menuOption_Menu_SetClose;

	// Token: 0x040001B0 RID: 432
	private Option __menuOption_Menu_SetOpen;

	// Token: 0x060003F5 RID: 1013 RVA: 0x00039C2C File Offset: 0x00037E2C
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("CustomDoorManipulator.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Pair", 0.1f))
			{
				if (this.Menu_Pair_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Pair.show = true;
					this.__menuOption_Menu_Pair.showDisabled = false;
					this.__menuOption_Menu_Pair.longUseOnly = false;
					this.__menuOption_Menu_Pair.order = 0;
					this.__menuOption_Menu_Pair.icon = "enter";
					this.__menuOption_Menu_Pair.desc = "pairdoor_desc";
					this.__menuOption_Menu_Pair.title = "pairdoor";
					if (this.__menuOption_Menu_Pair.function == null)
					{
						this.__menuOption_Menu_Pair.function = new Action<BasePlayer>(this.Menu_Pair);
					}
					list.Add(this.__menuOption_Menu_Pair);
				}
			}
			using (TimeWarning.New("Menu_SetClose", 0.1f))
			{
				if (this.Menu_SetClose_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_SetClose.show = true;
					this.__menuOption_Menu_SetClose.showDisabled = false;
					this.__menuOption_Menu_SetClose.longUseOnly = false;
					this.__menuOption_Menu_SetClose.order = 0;
					this.__menuOption_Menu_SetClose.icon = "exit";
					this.__menuOption_Menu_SetClose.desc = "setclosepower_desc";
					this.__menuOption_Menu_SetClose.title = "setclosepower";
					if (this.__menuOption_Menu_SetClose.function == null)
					{
						this.__menuOption_Menu_SetClose.function = new Action<BasePlayer>(this.Menu_SetClose);
					}
					list.Add(this.__menuOption_Menu_SetClose);
				}
			}
			using (TimeWarning.New("Menu_SetOpen", 0.1f))
			{
				if (this.Menu_SetOpen_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_SetOpen.show = true;
					this.__menuOption_Menu_SetOpen.showDisabled = false;
					this.__menuOption_Menu_SetOpen.longUseOnly = false;
					this.__menuOption_Menu_SetOpen.order = 0;
					this.__menuOption_Menu_SetOpen.icon = "enter";
					this.__menuOption_Menu_SetOpen.desc = "setopenpower_desc";
					this.__menuOption_Menu_SetOpen.title = "setopenpower";
					if (this.__menuOption_Menu_SetOpen.function == null)
					{
						this.__menuOption_Menu_SetOpen.function = new Action<BasePlayer>(this.Menu_SetOpen);
					}
					list.Add(this.__menuOption_Menu_SetOpen);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x060003F6 RID: 1014 RVA: 0x0000596A File Offset: 0x00003B6A
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Pair_ShowIf(LocalPlayer.Entity) || this.Menu_SetClose_ShowIf(LocalPlayer.Entity) || this.Menu_SetOpen_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x00039EF4 File Offset: 0x000380F4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CustomDoorManipulator.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool PairWithLockedDoors()
	{
		return false;
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x0000599F File Offset: 0x00003B9F
	public bool CanPlayerAdmin(BasePlayer player)
	{
		return player != null && player.CanBuild() && !base.IsOn();
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x000059BD File Offset: 0x00003BBD
	public bool IsPaired()
	{
		return this.targetDoor != null;
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x000059CB File Offset: 0x00003BCB
	[BaseEntity.Menu.ShowIf("Menu_Pair_ShowIf")]
	[BaseEntity.Menu.Icon("enter")]
	[BaseEntity.Menu("pairdoor", "Pair to Door")]
	[BaseEntity.Menu.Description("pairdoor_desc", "Pair with a nearby unlocked door")]
	public void Menu_Pair(BasePlayer player)
	{
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		this.RequestPair();
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x000059DD File Offset: 0x00003BDD
	public bool Menu_Pair_ShowIf(BasePlayer player)
	{
		return this.CanPlayerAdmin(player);
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x000059EB File Offset: 0x00003BEB
	public void RequestPair()
	{
		base.ServerRPC("DoPair");
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x000059F8 File Offset: 0x00003BF8
	public void RequestActionChange(DoorManipulator.DoorEffect newAction)
	{
		if (newAction != this.powerAction)
		{
			base.ServerRPC<int>("ServerActionChange", (int)newAction);
		}
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x00005A0F File Offset: 0x00003C0F
	[BaseEntity.Menu.Description("setopenpower_desc", "Set the door to be open when power is receivedr")]
	[BaseEntity.Menu.ShowIf("Menu_SetOpen_ShowIf")]
	[BaseEntity.Menu.Icon("enter")]
	[BaseEntity.Menu("setopenpower", "Open when powered")]
	public void Menu_SetOpen(BasePlayer player)
	{
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		this.RequestActionChange(DoorManipulator.DoorEffect.Open);
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x0000508F File Offset: 0x0000328F
	public bool Menu_SetOpen_ShowIf(BasePlayer player)
	{
		return false;
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00005A0F File Offset: 0x00003C0F
	[BaseEntity.Menu.Description("setclosepower_desc", "Set the door to be closed when power is received")]
	[BaseEntity.Menu.Icon("exit")]
	[BaseEntity.Menu.ShowIf("Menu_SetClose_ShowIf")]
	[BaseEntity.Menu("setclosepower", "Close when powered")]
	public void Menu_SetClose(BasePlayer player)
	{
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		this.RequestActionChange(DoorManipulator.DoorEffect.Open);
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x0000508F File Offset: 0x0000328F
	public bool Menu_SetClose_ShowIf(BasePlayer player)
	{
		return false;
	}
}
