using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000027 RID: 39
public class Candle : BaseCombatEntity
{
	// Token: 0x040001A1 RID: 417
	private Option __menuOption_Menu_Extinguish;

	// Token: 0x040001A2 RID: 418
	private Option __menuOption_Menu_Ignite;

	// Token: 0x060003DC RID: 988 RVA: 0x0003944C File Offset: 0x0003764C
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("Candle.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Extinguish", 0.1f))
			{
				if (this.Menu_Extinguish_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Extinguish.show = true;
					this.__menuOption_Menu_Extinguish.showDisabled = false;
					this.__menuOption_Menu_Extinguish.longUseOnly = false;
					this.__menuOption_Menu_Extinguish.order = 0;
					this.__menuOption_Menu_Extinguish.icon = "extinguish";
					this.__menuOption_Menu_Extinguish.desc = "Extinguish_desc";
					this.__menuOption_Menu_Extinguish.title = "extinguish";
					if (this.__menuOption_Menu_Extinguish.function == null)
					{
						this.__menuOption_Menu_Extinguish.function = new Action<BasePlayer>(this.Menu_Extinguish);
					}
					list.Add(this.__menuOption_Menu_Extinguish);
				}
			}
			using (TimeWarning.New("Menu_Ignite", 0.1f))
			{
				if (this.Menu_Ignite_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Ignite.show = true;
					this.__menuOption_Menu_Ignite.showDisabled = false;
					this.__menuOption_Menu_Ignite.longUseOnly = false;
					this.__menuOption_Menu_Ignite.order = 0;
					this.__menuOption_Menu_Ignite.icon = "ignite";
					this.__menuOption_Menu_Ignite.desc = "ignite_candle_desc";
					this.__menuOption_Menu_Ignite.title = "ignite";
					if (this.__menuOption_Menu_Ignite.function == null)
					{
						this.__menuOption_Menu_Ignite.function = new Action<BasePlayer>(this.Menu_Ignite);
					}
					list.Add(this.__menuOption_Menu_Ignite);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x060003DD RID: 989 RVA: 0x0000582E File Offset: 0x00003A2E
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Extinguish_ShowIf(LocalPlayer.Entity) || this.Menu_Ignite_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00039640 File Offset: 0x00037840
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Candle.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00005854 File Offset: 0x00003A54
	[BaseEntity.Menu.Icon("ignite")]
	[BaseEntity.Menu.ShowIf("Menu_Ignite_ShowIf")]
	[BaseEntity.Menu("ignite", "Burn")]
	[BaseEntity.Menu.Description("ignite_candle_desc", "Ignite")]
	public void Menu_Ignite(BasePlayer player)
	{
		base.ServerRPC<bool>("SetWantsOn", true);
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00005862 File Offset: 0x00003A62
	public bool Menu_Ignite_ShowIf(BasePlayer player)
	{
		return !base.IsOn();
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x0000586D File Offset: 0x00003A6D
	[BaseEntity.Menu.ShowIf("Menu_Extinguish_ShowIf")]
	[BaseEntity.Menu("extinguish", "Extinguish")]
	[BaseEntity.Menu.Icon("extinguish")]
	[BaseEntity.Menu.Description("Extinguish_desc", "Blow out the candle")]
	public void Menu_Extinguish(BasePlayer player)
	{
		base.ServerRPC<bool>("SetWantsOn", false);
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x0000464B File Offset: 0x0000284B
	public bool Menu_Extinguish_ShowIf(BasePlayer player)
	{
		return base.IsOn();
	}
}
