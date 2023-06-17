using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000005 RID: 5
public class FogMachine : StorageContainer
{
	// Token: 0x0400000C RID: 12
	public const BaseEntity.Flags FogFieldOn = BaseEntity.Flags.Reserved8;

	// Token: 0x0400000D RID: 13
	public const BaseEntity.Flags MotionMode = BaseEntity.Flags.Reserved7;

	// Token: 0x0400000E RID: 14
	public const BaseEntity.Flags Emitting = BaseEntity.Flags.Reserved6;

	// Token: 0x0400000F RID: 15
	public const BaseEntity.Flags Flag_HasJuice = BaseEntity.Flags.Reserved5;

	// Token: 0x04000010 RID: 16
	public float fogLength = 60f;

	// Token: 0x04000011 RID: 17
	public float nozzleBlastDuration = 5f;

	// Token: 0x04000012 RID: 18
	private Option __menuOption_Menu_FogOff;

	// Token: 0x04000013 RID: 19
	private Option __menuOption_Menu_MotionOff;

	// Token: 0x04000014 RID: 20
	private Option __menuOption_Menu_MotionOn;

	// Token: 0x04000015 RID: 21
	private Option __menuOption_Menu_TurnOn;

	// Token: 0x06000015 RID: 21 RVA: 0x00002D2A File Offset: 0x00000F2A
	public bool IsEmitting()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved6);
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002D37 File Offset: 0x00000F37
	public bool HasJuice()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool MotionModeEnabled()
	{
		return true;
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002D47 File Offset: 0x00000F47
	[BaseEntity.Menu.Icon("power")]
	[BaseEntity.Menu.Description("activate_fog_desc", "Activate the machine")]
	[BaseEntity.Menu.ShowIf("Menu_FogOn_ShowIf")]
	[BaseEntity.Menu("fog_activate", "Activate", Order = -100)]
	public void Menu_TurnOn(BasePlayer player)
	{
		base.ServerRPC("SetFogOn");
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00002D54 File Offset: 0x00000F54
	public bool Menu_FogOn_ShowIf(BasePlayer player)
	{
		return !base.IsOn() && !this.IsEmitting() && this.HasJuice() && this.ShouldShowLootMenus();
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002D76 File Offset: 0x00000F76
	[BaseEntity.Menu("fog_off", "Stop", Order = -100)]
	[BaseEntity.Menu.Description("stop_fog_desc", "Stop the machine")]
	[BaseEntity.Menu.ShowIf("Menu_FogOff_ShowIf")]
	[BaseEntity.Menu.Icon("close")]
	public void Menu_FogOff(BasePlayer player)
	{
		base.ServerRPC("SetFogOff");
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00002D83 File Offset: 0x00000F83
	public bool Menu_FogOff_ShowIf(BasePlayer player)
	{
		return !this.IsEmitting() && base.IsOn() && this.ShouldShowLootMenus();
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00002D9D File Offset: 0x00000F9D
	[BaseEntity.Menu("motion_mode_on", "Motion ON", Order = -90)]
	[BaseEntity.Menu.Description("motion_mode_on_desc", "Trigger fogging when motion is detected")]
	[BaseEntity.Menu.ShowIf("Menu_MotionOn_ShowIf")]
	[BaseEntity.Menu.Icon("examine")]
	public void Menu_MotionOn(BasePlayer player)
	{
		base.ServerRPC<bool>("SetMotionDetection", true);
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002DAB File Offset: 0x00000FAB
	public bool Menu_MotionOn_ShowIf(BasePlayer player)
	{
		return !base.HasFlag(BaseEntity.Flags.Reserved7) && this.HasJuice() && this.ShouldShowLootMenus() && this.MotionModeEnabled();
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002DD2 File Offset: 0x00000FD2
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.Description("motion_mode_off_desc", "Disable motion detection")]
	[BaseEntity.Menu("motion_mode_off", "Motion OFF", Order = -90)]
	[BaseEntity.Menu.ShowIf("Menu_MotionOff_ShowIf")]
	public void Menu_MotionOff(BasePlayer player)
	{
		base.ServerRPC<bool>("SetMotionDetection", false);
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00002DE0 File Offset: 0x00000FE0
	public bool Menu_MotionOff_ShowIf(BasePlayer player)
	{
		return base.HasFlag(BaseEntity.Flags.Reserved7) && this.ShouldShowLootMenus() && this.MotionModeEnabled();
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00026A5C File Offset: 0x00024C5C
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("FogMachine.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_FogOff", 0.1f))
			{
				if (this.Menu_FogOff_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_FogOff.show = true;
					this.__menuOption_Menu_FogOff.showDisabled = false;
					this.__menuOption_Menu_FogOff.longUseOnly = false;
					this.__menuOption_Menu_FogOff.order = -100;
					this.__menuOption_Menu_FogOff.icon = "close";
					this.__menuOption_Menu_FogOff.desc = "stop_fog_desc";
					this.__menuOption_Menu_FogOff.title = "fog_off";
					if (this.__menuOption_Menu_FogOff.function == null)
					{
						this.__menuOption_Menu_FogOff.function = new Action<BasePlayer>(this.Menu_FogOff);
					}
					list.Add(this.__menuOption_Menu_FogOff);
				}
			}
			using (TimeWarning.New("Menu_MotionOff", 0.1f))
			{
				if (this.Menu_MotionOff_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_MotionOff.show = true;
					this.__menuOption_Menu_MotionOff.showDisabled = false;
					this.__menuOption_Menu_MotionOff.longUseOnly = false;
					this.__menuOption_Menu_MotionOff.order = -90;
					this.__menuOption_Menu_MotionOff.icon = "close";
					this.__menuOption_Menu_MotionOff.desc = "motion_mode_off_desc";
					this.__menuOption_Menu_MotionOff.title = "motion_mode_off";
					if (this.__menuOption_Menu_MotionOff.function == null)
					{
						this.__menuOption_Menu_MotionOff.function = new Action<BasePlayer>(this.Menu_MotionOff);
					}
					list.Add(this.__menuOption_Menu_MotionOff);
				}
			}
			using (TimeWarning.New("Menu_MotionOn", 0.1f))
			{
				if (this.Menu_MotionOn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_MotionOn.show = true;
					this.__menuOption_Menu_MotionOn.showDisabled = false;
					this.__menuOption_Menu_MotionOn.longUseOnly = false;
					this.__menuOption_Menu_MotionOn.order = -90;
					this.__menuOption_Menu_MotionOn.icon = "examine";
					this.__menuOption_Menu_MotionOn.desc = "motion_mode_on_desc";
					this.__menuOption_Menu_MotionOn.title = "motion_mode_on";
					if (this.__menuOption_Menu_MotionOn.function == null)
					{
						this.__menuOption_Menu_MotionOn.function = new Action<BasePlayer>(this.Menu_MotionOn);
					}
					list.Add(this.__menuOption_Menu_MotionOn);
				}
			}
			using (TimeWarning.New("Menu_TurnOn", 0.1f))
			{
				if (this.Menu_FogOn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_TurnOn.show = true;
					this.__menuOption_Menu_TurnOn.showDisabled = false;
					this.__menuOption_Menu_TurnOn.longUseOnly = false;
					this.__menuOption_Menu_TurnOn.order = -100;
					this.__menuOption_Menu_TurnOn.icon = "power";
					this.__menuOption_Menu_TurnOn.desc = "activate_fog_desc";
					this.__menuOption_Menu_TurnOn.title = "fog_activate";
					if (this.__menuOption_Menu_TurnOn.function == null)
					{
						this.__menuOption_Menu_TurnOn.function = new Action<BasePlayer>(this.Menu_TurnOn);
					}
					list.Add(this.__menuOption_Menu_TurnOn);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000021 RID: 33 RVA: 0x00026DFC File Offset: 0x00024FFC
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_FogOff_ShowIf(LocalPlayer.Entity) || this.Menu_MotionOff_ShowIf(LocalPlayer.Entity) || this.Menu_MotionOn_ShowIf(LocalPlayer.Entity) || this.Menu_FogOn_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00026E4C File Offset: 0x0002504C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FogMachine.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}
}
