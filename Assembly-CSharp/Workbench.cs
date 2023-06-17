using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class Workbench : StorageContainer
{
	// Token: 0x04000333 RID: 819
	private Option __menuOption_Menu_RotateVM;

	// Token: 0x04000334 RID: 820
	private Option __menuOption_UseBench;

	// Token: 0x04000335 RID: 821
	public const int blueprintSlot = 0;

	// Token: 0x04000336 RID: 822
	public const int experimentSlot = 1;

	// Token: 0x04000337 RID: 823
	public int Workbenchlevel;

	// Token: 0x04000338 RID: 824
	public LootSpawn experimentalItems;

	// Token: 0x04000339 RID: 825
	public GameObjectRef experimentStartEffect;

	// Token: 0x0400033A RID: 826
	public GameObjectRef experimentSuccessEffect;

	// Token: 0x0400033B RID: 827
	public ItemDefinition experimentResource;

	// Token: 0x06000626 RID: 1574 RVA: 0x00042AA0 File Offset: 0x00040CA0
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("Workbench.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_RotateVM", 0.1f))
			{
				if (this.Menu_RotateWB_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_RotateVM.show = true;
					this.__menuOption_Menu_RotateVM.showDisabled = false;
					this.__menuOption_Menu_RotateVM.longUseOnly = false;
					this.__menuOption_Menu_RotateVM.order = 1000;
					this.__menuOption_Menu_RotateVM.icon = "rotate";
					this.__menuOption_Menu_RotateVM.desc = "open_rotate_WB_desc";
					this.__menuOption_Menu_RotateVM.title = "rotate_wb";
					if (this.__menuOption_Menu_RotateVM.function == null)
					{
						this.__menuOption_Menu_RotateVM.function = new Action<BasePlayer>(this.Menu_RotateVM);
					}
					list.Add(this.__menuOption_Menu_RotateVM);
				}
			}
			using (TimeWarning.New("UseBench", 0.1f))
			{
				if (this.Menu_UseBench_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_UseBench.show = true;
					this.__menuOption_UseBench.showDisabled = false;
					this.__menuOption_UseBench.longUseOnly = false;
					this.__menuOption_UseBench.order = -100;
					this.__menuOption_UseBench.icon = "gear";
					this.__menuOption_UseBench.desc = "usebench_desc";
					this.__menuOption_UseBench.title = "usebench";
					if (this.__menuOption_UseBench.function == null)
					{
						this.__menuOption_UseBench.function = new Action<BasePlayer>(this.UseBench);
					}
					list.Add(this.__menuOption_UseBench);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x06000627 RID: 1575 RVA: 0x00007686 File Offset: 0x00005886
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_RotateWB_ShowIf(LocalPlayer.Entity) || this.Menu_UseBench_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x00042C98 File Offset: 0x00040E98
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Workbench.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool ShouldShowLootMenus()
	{
		return false;
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x00005BEF File Offset: 0x00003DEF
	[BaseEntity.Menu.ShowIf("Menu_UseBench_ShowIf")]
	[BaseEntity.Menu("usebench", "Use", Order = -100)]
	[BaseEntity.Menu.Icon("gear")]
	[BaseEntity.Menu.Description("usebench_desc", "Open your crafting window and use this bench")]
	public void UseBench(BasePlayer player)
	{
		base.ServerRPC("RPC_OpenLoot");
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x000076AC File Offset: 0x000058AC
	public bool Menu_UseBench_ShowIf(BasePlayer player)
	{
		return LocalPlayer.HasCraftLevel(this.Workbenchlevel);
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x0000577D File Offset: 0x0000397D
	[BaseEntity.Menu.ShowIf("Menu_RotateWB_ShowIf")]
	[BaseEntity.Menu.Icon("rotate")]
	[BaseEntity.Menu("rotate_wb", "Rotate", Order = 1000)]
	[BaseEntity.Menu.Description("open_rotate_WB_desc", "Rotate the workbench")]
	public void Menu_RotateVM(BasePlayer player)
	{
		base.ServerRPC("RPC_Rotate");
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x0000578A File Offset: 0x0000398A
	public bool Menu_RotateWB_ShowIf(BasePlayer player)
	{
		return player.CanBuild() && player.GetHeldEntity() && player.GetHeldEntity().GetComponent<Hammer>() != null;
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x000076B9 File Offset: 0x000058B9
	public void TryExperiment()
	{
		base.ServerRPC("RPC_BeginExperiment");
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x00042CDC File Offset: 0x00040EDC
	public bool PlayerUnlockedThisTier()
	{
		BasePlayer entity = LocalPlayer.Entity;
		if (entity == null)
		{
			return true;
		}
		LootSpawn.Entry[] subSpawn = this.experimentalItems.subSpawn;
		for (int i = 0; i < subSpawn.Length; i++)
		{
			ItemDefinition itemDef = subSpawn[i].category.items[0].itemDef;
			if (itemDef.Blueprint && !itemDef.Blueprint.defaultBlueprint && itemDef.Blueprint.userCraftable && itemDef.Blueprint.isResearchable && !itemDef.Blueprint.NeedsSteamItem && !entity.blueprints.HasUnlocked(itemDef))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x000076C6 File Offset: 0x000058C6
	public int GetScrapForExperiment()
	{
		if (this.Workbenchlevel == 1)
		{
			return 75;
		}
		if (this.Workbenchlevel == 2)
		{
			return 300;
		}
		if (this.Workbenchlevel == 3)
		{
			return 1000;
		}
		Debug.LogWarning("GetScrapForExperiment fucked up big time.");
		return 0;
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x000056A3 File Offset: 0x000038A3
	public bool IsWorking()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}
}
