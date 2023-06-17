using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class FreeableLootContainer : LootContainer
{
	// Token: 0x040001D4 RID: 468
	private Option __menuOption_Menu_FreeCrate;

	// Token: 0x040001D5 RID: 469
	private const BaseEntity.Flags tiedDown = BaseEntity.Flags.Reserved8;

	// Token: 0x040001D6 RID: 470
	public Buoyancy buoyancy;

	// Token: 0x040001D7 RID: 471
	public GameObjectRef freedEffect;

	// Token: 0x040001D8 RID: 472
	private Rigidbody rb;

	// Token: 0x040001D9 RID: 473
	public uint skinOverride;

	// Token: 0x0600044E RID: 1102 RVA: 0x0003B118 File Offset: 0x00039318
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("FreeableLootContainer.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_FreeCrate", 0.1f))
			{
				if (this.Menu_FreeCrate_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_FreeCrate.show = true;
					this.__menuOption_Menu_FreeCrate.showDisabled = false;
					this.__menuOption_Menu_FreeCrate.longUseOnly = false;
					this.__menuOption_Menu_FreeCrate.order = 0;
					this.__menuOption_Menu_FreeCrate.time = 15f;
					this.__menuOption_Menu_FreeCrate.icon = "upgrade";
					this.__menuOption_Menu_FreeCrate.desc = "free_crate_desc";
					this.__menuOption_Menu_FreeCrate.title = "free_crate";
					if (this.__menuOption_Menu_FreeCrate.function == null)
					{
						this.__menuOption_Menu_FreeCrate.function = new Action<BasePlayer>(this.Menu_FreeCrate);
					}
					if (this.__menuOption_Menu_FreeCrate.timeStart == null)
					{
						this.__menuOption_Menu_FreeCrate.timeStart = new Action(this.Menu_FreeCrate_Start);
					}
					list.Add(this.__menuOption_Menu_FreeCrate);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x0600044F RID: 1103 RVA: 0x00005D37 File Offset: 0x00003F37
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_FreeCrate_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x0003B254 File Offset: 0x00039454
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FreeableLootContainer.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00005D4E File Offset: 0x00003F4E
	public Rigidbody GetRB()
	{
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		return this.rb;
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00005D70 File Offset: 0x00003F70
	public bool IsTiedDown()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00005D7D File Offset: 0x00003F7D
	public override bool ShouldShowLootMenus()
	{
		return base.ShouldShowLootMenus() && !this.IsTiedDown();
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00005D92 File Offset: 0x00003F92
	[BaseEntity.Menu("free_crate", "Untie", Time = 15f, OnStart = "Menu_FreeCrate_Start")]
	[BaseEntity.Menu.Description("free_crate_desc", "Untie the crate allowing it to be free")]
	[BaseEntity.Menu.ShowIf("Menu_FreeCrate_ShowIf")]
	[BaseEntity.Menu.Icon("upgrade")]
	public void Menu_FreeCrate(BasePlayer player)
	{
		base.ServerRPC("RPC_FreeCrate");
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00002ECE File Offset: 0x000010CE
	public void Menu_FreeCrate_Start()
	{
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00005D9F File Offset: 0x00003F9F
	public bool Menu_FreeCrate_ShowIf(BasePlayer player)
	{
		return this.IsTiedDown();
	}
}
