using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000044 RID: 68
public class ResourceContainer : EntityComponent<BaseEntity>
{
	// Token: 0x040002A6 RID: 678
	private Option __menuOption_MenuLoot;

	// Token: 0x040002A7 RID: 679
	public bool lootable = true;

	// Token: 0x0600054E RID: 1358 RVA: 0x0003EE68 File Offset: 0x0003D068
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("ResourceContainer.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("MenuLoot", 0.1f))
			{
				if (this.MenuLoot_Test(LocalPlayer.Entity))
				{
					this.__menuOption_MenuLoot.show = true;
					this.__menuOption_MenuLoot.showDisabled = false;
					this.__menuOption_MenuLoot.longUseOnly = false;
					this.__menuOption_MenuLoot.order = 0;
					this.__menuOption_MenuLoot.icon = "loot";
					this.__menuOption_MenuLoot.desc = "resource_loot_desc";
					this.__menuOption_MenuLoot.title = "resource_loot";
					if (this.__menuOption_MenuLoot.function == null)
					{
						this.__menuOption_MenuLoot.function = new Action<BasePlayer>(this.MenuLoot);
					}
					list.Add(this.__menuOption_MenuLoot);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x0600054F RID: 1359 RVA: 0x00006B89 File Offset: 0x00004D89
	public override bool HasMenuOptions
	{
		get
		{
			return this.MenuLoot_Test(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0003EF70 File Offset: 0x0003D170
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ResourceContainer.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x00006BA0 File Offset: 0x00004DA0
	[BaseEntity.Menu.Description("resource_loot_desc", "Open op and take a look to see if there's anything to steal")]
	[BaseEntity.Menu("resource_loot", "Loot")]
	[BaseEntity.Menu.ShowIf("MenuLoot_Test")]
	[BaseEntity.Menu.Icon("loot")]
	public void MenuLoot(BasePlayer player)
	{
		if (!this.lootable)
		{
			return;
		}
		UIInventory.OpenLoot("generic");
		base.baseEntity.ServerRPC("StartLootingContainer");
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x00006BC5 File Offset: 0x00004DC5
	public bool MenuLoot_Test(BasePlayer player)
	{
		return this.lootable;
	}
}
