using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000031 RID: 49
public class EngineSwitch : BaseEntity
{
	// Token: 0x040001D2 RID: 466
	private Option __menuOption_Menu_StartEngine;

	// Token: 0x040001D3 RID: 467
	private Option __menuOption_Menu_StopEngine;

	// Token: 0x06000446 RID: 1094 RVA: 0x0003AEE0 File Offset: 0x000390E0
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("EngineSwitch.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_StartEngine", 0.1f))
			{
				if (this.Menu_EngineOn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_StartEngine.show = true;
					this.__menuOption_Menu_StartEngine.showDisabled = false;
					this.__menuOption_Menu_StartEngine.longUseOnly = false;
					this.__menuOption_Menu_StartEngine.order = 0;
					this.__menuOption_Menu_StartEngine.icon = "gear";
					this.__menuOption_Menu_StartEngine.desc = "engine_on_desc";
					this.__menuOption_Menu_StartEngine.title = "engine_on";
					if (this.__menuOption_Menu_StartEngine.function == null)
					{
						this.__menuOption_Menu_StartEngine.function = new Action<BasePlayer>(this.Menu_StartEngine);
					}
					list.Add(this.__menuOption_Menu_StartEngine);
				}
			}
			using (TimeWarning.New("Menu_StopEngine", 0.1f))
			{
				if (this.Menu_EngineOff_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_StopEngine.show = true;
					this.__menuOption_Menu_StopEngine.showDisabled = false;
					this.__menuOption_Menu_StopEngine.longUseOnly = false;
					this.__menuOption_Menu_StopEngine.order = 0;
					this.__menuOption_Menu_StopEngine.icon = "close";
					this.__menuOption_Menu_StopEngine.desc = "engine_off_desc";
					this.__menuOption_Menu_StopEngine.title = "engine_off";
					if (this.__menuOption_Menu_StopEngine.function == null)
					{
						this.__menuOption_Menu_StopEngine.function = new Action<BasePlayer>(this.Menu_StopEngine);
					}
					list.Add(this.__menuOption_Menu_StopEngine);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000447 RID: 1095 RVA: 0x00005CEB File Offset: 0x00003EEB
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_EngineOn_ShowIf(LocalPlayer.Entity) || this.Menu_EngineOff_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x0003B0D4 File Offset: 0x000392D4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("EngineSwitch.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00005D11 File Offset: 0x00003F11
	[BaseEntity.Menu.ShowIf("Menu_EngineOn_ShowIf")]
	[BaseEntity.Menu.Description("engine_on_desc", "Start the engine")]
	[BaseEntity.Menu.Icon("gear")]
	[BaseEntity.Menu("engine_on", "Start Engine")]
	public void Menu_StartEngine(BasePlayer player)
	{
		base.ServerRPC("StartEngine");
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00005D1E File Offset: 0x00003F1E
	public bool Menu_EngineOn_ShowIf(BasePlayer player)
	{
		return !base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00005D2A File Offset: 0x00003F2A
	[BaseEntity.Menu.ShowIf("Menu_EngineOff_ShowIf")]
	[BaseEntity.Menu("engine_off", "Stop Engine")]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.Description("engine_off_desc", "Stop the engine")]
	public void Menu_StopEngine(BasePlayer player)
	{
		base.ServerRPC("StopEngine");
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x000056A3 File Offset: 0x000038A3
	public bool Menu_EngineOff_ShowIf(BasePlayer player)
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}
}
