using System;
using System.Collections.Generic;
using System.Linq;
using GameMenu;
using Network;
using ProtoBuf;

// Token: 0x02000025 RID: 37
public class BuildingPrivlidge : StorageContainer
{
	// Token: 0x04000198 RID: 408
	private Option __menuOption_Menu_RotateVM;

	// Token: 0x04000199 RID: 409
	private Option __menuOption_MenuAuthorize;

	// Token: 0x0400019A RID: 410
	private Option __menuOption_MenuClearList;

	// Token: 0x0400019B RID: 411
	private Option __menuOption_MenuDeauthorize;

	// Token: 0x0400019C RID: 412
	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	// Token: 0x0400019D RID: 413
	private float cachedProtectedMinutes;

	// Token: 0x0400019E RID: 414
	private float cachedUpkeepPeriodMinutes;

	// Token: 0x0400019F RID: 415
	private float cachedUpkeepCostFraction;

	// Token: 0x060003C3 RID: 963 RVA: 0x00038EC4 File Offset: 0x000370C4
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("BuildingPrivlidge.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_RotateVM", 0.1f))
			{
				if (this.Menu_RotateTC_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_RotateVM.show = true;
					this.__menuOption_Menu_RotateVM.showDisabled = false;
					this.__menuOption_Menu_RotateVM.longUseOnly = false;
					this.__menuOption_Menu_RotateVM.order = 1000;
					this.__menuOption_Menu_RotateVM.icon = "rotate";
					this.__menuOption_Menu_RotateVM.desc = "open_rotate_tc_desc";
					this.__menuOption_Menu_RotateVM.title = "rotate_tc";
					if (this.__menuOption_Menu_RotateVM.function == null)
					{
						this.__menuOption_Menu_RotateVM.function = new Action<BasePlayer>(this.Menu_RotateVM);
					}
					list.Add(this.__menuOption_Menu_RotateVM);
				}
			}
			using (TimeWarning.New("MenuAuthorize", 0.1f))
			{
				if (this.MenuAuthorize_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuAuthorize.show = true;
					this.__menuOption_MenuAuthorize.showDisabled = false;
					this.__menuOption_MenuAuthorize.longUseOnly = false;
					this.__menuOption_MenuAuthorize.order = 1;
					this.__menuOption_MenuAuthorize.icon = "authorize";
					this.__menuOption_MenuAuthorize.desc = "buildingpriv_authorize_desc";
					this.__menuOption_MenuAuthorize.title = "buildingpriv_authorize";
					if (this.__menuOption_MenuAuthorize.function == null)
					{
						this.__menuOption_MenuAuthorize.function = new Action<BasePlayer>(this.MenuAuthorize);
					}
					list.Add(this.__menuOption_MenuAuthorize);
				}
			}
			using (TimeWarning.New("MenuClearList", 0.1f))
			{
				if (this.MenuClearList_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuClearList.show = true;
					this.__menuOption_MenuClearList.showDisabled = false;
					this.__menuOption_MenuClearList.longUseOnly = false;
					this.__menuOption_MenuClearList.order = 1000;
					this.__menuOption_MenuClearList.icon = "clear_list";
					this.__menuOption_MenuClearList.desc = "buildingpriv_clear_desc";
					this.__menuOption_MenuClearList.title = "buildingpriv_clear";
					if (this.__menuOption_MenuClearList.function == null)
					{
						this.__menuOption_MenuClearList.function = new Action<BasePlayer>(this.MenuClearList);
					}
					list.Add(this.__menuOption_MenuClearList);
				}
			}
			using (TimeWarning.New("MenuDeauthorize", 0.1f))
			{
				if (this.MenuDeauthorize_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuDeauthorize.show = true;
					this.__menuOption_MenuDeauthorize.showDisabled = false;
					this.__menuOption_MenuDeauthorize.longUseOnly = false;
					this.__menuOption_MenuDeauthorize.order = 1;
					this.__menuOption_MenuDeauthorize.icon = "deauthorize";
					this.__menuOption_MenuDeauthorize.desc = "buildingpriv_deauthorize_desc";
					this.__menuOption_MenuDeauthorize.title = "buildingpriv_deauthorize";
					if (this.__menuOption_MenuDeauthorize.function == null)
					{
						this.__menuOption_MenuDeauthorize.function = new Action<BasePlayer>(this.MenuDeauthorize);
					}
					list.Add(this.__menuOption_MenuDeauthorize);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060003C4 RID: 964 RVA: 0x00039268 File Offset: 0x00037468
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_RotateTC_ShowIf(LocalPlayer.Entity) || this.MenuAuthorize_ShowIf(LocalPlayer.Entity) || this.MenuClearList_ShowIf(LocalPlayer.Entity) || this.MenuDeauthorize_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x000392B8 File Offset: 0x000374B8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BuildingPrivlidge.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x0000572F File Offset: 0x0000392F
	public override void ResetState()
	{
		base.ResetState();
		this.authorizedPlayers.Clear();
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x000392FC File Offset: 0x000374FC
	public bool IsAuthed(BasePlayer player)
	{
		return Enumerable.Any<PlayerNameID>(this.authorizedPlayers, (PlayerNameID x) => x.userid == player.userID);
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x00005742 File Offset: 0x00003942
	public bool AnyAuthed()
	{
		return this.authorizedPlayers.Count > 0;
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x00039330 File Offset: 0x00037530
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.authorizedPlayers.Clear();
		if (info.msg.buildingPrivilege != null && info.msg.buildingPrivilege.users != null)
		{
			this.authorizedPlayers = info.msg.buildingPrivilege.users;
			if (!info.fromDisk)
			{
				this.cachedUpkeepPeriodMinutes = info.msg.buildingPrivilege.upkeepPeriodMinutes;
				this.cachedUpkeepCostFraction = info.msg.buildingPrivilege.costFraction;
				this.cachedProtectedMinutes = info.msg.buildingPrivilege.protectedMinutes;
			}
			info.msg.buildingPrivilege.users = null;
		}
	}

	// Token: 0x060003CA RID: 970 RVA: 0x00002ECE File Offset: 0x000010CE
	public void BuildingDirty()
	{
	}

	// Token: 0x060003CB RID: 971 RVA: 0x000045B7 File Offset: 0x000027B7
	[BaseEntity.Menu.Icon("authorize")]
	[BaseEntity.Menu.ShowIf("MenuAuthorize_ShowIf")]
	[BaseEntity.Menu("buildingpriv_authorize", "Authorize", Order = 1)]
	[BaseEntity.Menu.Description("buildingpriv_authorize_desc", "Add yourself to this cupboards list so you can build within its radius.")]
	public void MenuAuthorize(BasePlayer player)
	{
		base.ServerRPC("AddSelfAuthorize");
	}

	// Token: 0x060003CC RID: 972 RVA: 0x00005752 File Offset: 0x00003952
	public bool MenuAuthorize_ShowIf(BasePlayer player)
	{
		return !this.IsAuthed(player);
	}

	// Token: 0x060003CD RID: 973 RVA: 0x000045D7 File Offset: 0x000027D7
	[BaseEntity.Menu.Icon("deauthorize")]
	[BaseEntity.Menu.ShowIf("MenuDeauthorize_ShowIf")]
	[BaseEntity.Menu.Description("buildingpriv_deauthorize_desc", "Remove yourself from this cupboards list. You won't be able to build within its radius.")]
	[BaseEntity.Menu("buildingpriv_deauthorize", "Deauthorize", Order = 1)]
	public void MenuDeauthorize(BasePlayer player)
	{
		base.ServerRPC("RemoveSelfAuthorize");
	}

	// Token: 0x060003CE RID: 974 RVA: 0x0000575E File Offset: 0x0000395E
	public bool MenuDeauthorize_ShowIf(BasePlayer player)
	{
		return this.IsAuthed(player);
	}

	// Token: 0x060003CF RID: 975 RVA: 0x000045E4 File Offset: 0x000027E4
	[BaseEntity.Menu.ShowIf("MenuClearList_ShowIf")]
	[BaseEntity.Menu.Description("buildingpriv_clear_desc", "Clear the authorized list, no-one will be able to build in this cupboards radius")]
	[BaseEntity.Menu("buildingpriv_clear", "Clear Authorized List", Order = 1000)]
	[BaseEntity.Menu.Icon("clear_list")]
	public void MenuClearList(BasePlayer player)
	{
		base.ServerRPC("ClearList");
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x0000575E File Offset: 0x0000395E
	public bool MenuClearList_ShowIf(BasePlayer player)
	{
		return this.IsAuthed(player);
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00005767 File Offset: 0x00003967
	public override bool ShouldShowLootMenus()
	{
		return base.ShouldShowLootMenus() && LocalPlayer.Entity.CanBuild();
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x0000577D File Offset: 0x0000397D
	[BaseEntity.Menu.ShowIf("Menu_RotateTC_ShowIf")]
	[BaseEntity.Menu.Description("open_rotate_tc_desc", "Rotate the Tool Cupboard")]
	[BaseEntity.Menu.Icon("rotate")]
	[BaseEntity.Menu("rotate_tc", "Rotate", Order = 1000)]
	public void Menu_RotateVM(BasePlayer player)
	{
		base.ServerRPC("RPC_Rotate");
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x0000578A File Offset: 0x0000398A
	public bool Menu_RotateTC_ShowIf(BasePlayer player)
	{
		return player.CanBuild() && player.GetHeldEntity() && player.GetHeldEntity().GetComponent<Hammer>() != null;
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x000057B6 File Offset: 0x000039B6
	public override bool HasSlot(BaseEntity.Slot slot)
	{
		return slot == BaseEntity.Slot.Lock || base.HasSlot(slot);
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x000057C4 File Offset: 0x000039C4
	public float CalculateUpkeepPeriodMinutes()
	{
		if (base.isClient)
		{
			return this.cachedUpkeepPeriodMinutes;
		}
		return 0f;
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x000057DA File Offset: 0x000039DA
	public float CalculateUpkeepCostFraction()
	{
		if (base.isClient)
		{
			return this.cachedUpkeepCostFraction;
		}
		return 0f;
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x000393E4 File Offset: 0x000375E4
	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts)
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building == null)
		{
			return;
		}
		if (!building.HasDecayEntities())
		{
			return;
		}
		float multiplier = this.CalculateUpkeepCostFraction();
		foreach (DecayEntity decayEntity in building.decayEntities)
		{
			decayEntity.CalculateUpkeepCostAmounts(itemAmounts, multiplier);
		}
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x000057F0 File Offset: 0x000039F0
	public float GetProtectedMinutes(bool force = false)
	{
		if (base.isClient)
		{
			return this.cachedProtectedMinutes;
		}
		return 0f;
	}
}
