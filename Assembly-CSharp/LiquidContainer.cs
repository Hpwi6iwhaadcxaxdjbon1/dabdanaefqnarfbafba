using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x0200003A RID: 58
public class LiquidContainer : StorageContainer, ISplashable
{
	// Token: 0x04000227 RID: 551
	private Option __menuOption_MenuDrink;

	// Token: 0x04000228 RID: 552
	public ItemDefinition defaultLiquid;

	// Token: 0x04000229 RID: 553
	public int startingAmount;

	// Token: 0x060004AE RID: 1198 RVA: 0x0003C76C File Offset: 0x0003A96C
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("LiquidContainer.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("MenuDrink", 0.1f))
			{
				if (this.MenuDrink_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuDrink.show = true;
					this.__menuOption_MenuDrink.showDisabled = false;
					this.__menuOption_MenuDrink.longUseOnly = false;
					this.__menuOption_MenuDrink.order = 10000;
					this.__menuOption_MenuDrink.icon = "cup_water";
					this.__menuOption_MenuDrink.desc = "drink_desc";
					this.__menuOption_MenuDrink.title = "drink";
					if (this.__menuOption_MenuDrink.function == null)
					{
						this.__menuOption_MenuDrink.function = new Action<BasePlayer>(this.MenuDrink);
					}
					list.Add(this.__menuOption_MenuDrink);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x060004AF RID: 1199 RVA: 0x00006271 File Offset: 0x00004471
	public override bool HasMenuOptions
	{
		get
		{
			return this.MenuDrink_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0003C878 File Offset: 0x0003AA78
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LiquidContainer.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x00006288 File Offset: 0x00004488
	[BaseEntity.Menu.Description("drink_desc", "Take a drink of the contents of this water catcher")]
	[BaseEntity.Menu.ShowIf("MenuDrink_ShowIf")]
	[BaseEntity.Menu("drink", "Drink", Order = 10000)]
	[BaseEntity.Menu.Icon("cup_water")]
	public void MenuDrink(BasePlayer player)
	{
		base.ServerRPC("SVDrink");
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x00006295 File Offset: 0x00004495
	public bool MenuDrink_ShowIf(BasePlayer player)
	{
		return player.metabolism.CanConsume() && this.DrinkEligable(player) && base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool DrinkEligable(BasePlayer player)
	{
		return true;
	}
}
