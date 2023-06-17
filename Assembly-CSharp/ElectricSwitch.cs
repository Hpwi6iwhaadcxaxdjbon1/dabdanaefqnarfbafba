using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000030 RID: 48
public class ElectricSwitch : IOEntity
{
	// Token: 0x040001D0 RID: 464
	private Option __menuOption_Menu_TurnOff;

	// Token: 0x040001D1 RID: 465
	private Option __menuOption_Menu_TurnOn;

	// Token: 0x0600043D RID: 1085 RVA: 0x0003ACA8 File Offset: 0x00038EA8
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("ElectricSwitch.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_TurnOff", 0.1f))
			{
				if (this.Menu_TurnOff_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_TurnOff.show = true;
					this.__menuOption_Menu_TurnOff.showDisabled = false;
					this.__menuOption_Menu_TurnOff.longUseOnly = false;
					this.__menuOption_Menu_TurnOff.order = 0;
					this.__menuOption_Menu_TurnOff.icon = "close";
					this.__menuOption_Menu_TurnOff.desc = "turn_off_switch_desc";
					this.__menuOption_Menu_TurnOff.title = "turn_off";
					if (this.__menuOption_Menu_TurnOff.function == null)
					{
						this.__menuOption_Menu_TurnOff.function = new Action<BasePlayer>(this.Menu_TurnOff);
					}
					list.Add(this.__menuOption_Menu_TurnOff);
				}
			}
			using (TimeWarning.New("Menu_TurnOn", 0.1f))
			{
				if (this.Menu_TurnOn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_TurnOn.show = true;
					this.__menuOption_Menu_TurnOn.showDisabled = false;
					this.__menuOption_Menu_TurnOn.longUseOnly = false;
					this.__menuOption_Menu_TurnOn.order = 0;
					this.__menuOption_Menu_TurnOn.icon = "power";
					this.__menuOption_Menu_TurnOn.desc = "turn_on_switch_desc";
					this.__menuOption_Menu_TurnOn.title = "turn_on";
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

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x0600043E RID: 1086 RVA: 0x00005CBD File Offset: 0x00003EBD
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_TurnOff_ShowIf(LocalPlayer.Entity) || this.Menu_TurnOn_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x0003AE9C File Offset: 0x0003909C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ElectricSwitch.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x0000464B File Offset: 0x0000284B
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x00004AC9 File Offset: 0x00002CC9
	[BaseEntity.Menu.ShowIf("Menu_TurnOn_ShowIf")]
	[BaseEntity.Menu("turn_on", "Turn On")]
	[BaseEntity.Menu.Description("turn_on_switch_desc", "Turn On")]
	[BaseEntity.Menu.Icon("power")]
	public void Menu_TurnOn(BasePlayer player)
	{
		base.ServerRPC<bool>("SVSwitch", true);
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x00005862 File Offset: 0x00003A62
	public bool Menu_TurnOn_ShowIf(BasePlayer player)
	{
		return !base.IsOn();
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x00004AF7 File Offset: 0x00002CF7
	[BaseEntity.Menu.Description("turn_off_switch_desc", "Turn Off")]
	[BaseEntity.Menu.ShowIf("Menu_TurnOff_ShowIf")]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu("turn_off", "Turn Off")]
	public void Menu_TurnOff(BasePlayer player)
	{
		base.ServerRPC<bool>("SVSwitch", false);
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x0000464B File Offset: 0x0000284B
	public bool Menu_TurnOff_ShowIf(BasePlayer player)
	{
		return base.IsOn();
	}
}
