using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class StorageContainer : DecayEntity
{
	// Token: 0x040002F2 RID: 754
	private Option __menuOption_Menu_Occpued;

	// Token: 0x040002F3 RID: 755
	private Option __menuOption_Menu_OnFire;

	// Token: 0x040002F4 RID: 756
	private Option __menuOption_Menu_Open;

	// Token: 0x040002F5 RID: 757
	public int inventorySlots = 6;

	// Token: 0x040002F6 RID: 758
	public float dropChance = 0.75f;

	// Token: 0x040002F7 RID: 759
	public bool isLootable = true;

	// Token: 0x040002F8 RID: 760
	public bool isLockable = true;

	// Token: 0x040002F9 RID: 761
	public string panelName = "generic";

	// Token: 0x040002FA RID: 762
	public ItemContainer.ContentsType allowedContents;

	// Token: 0x040002FB RID: 763
	public ItemDefinition allowedItem;

	// Token: 0x040002FC RID: 764
	public int maxStackSize;

	// Token: 0x040002FD RID: 765
	public bool needsBuildingPrivilegeToUse;

	// Token: 0x040002FE RID: 766
	public SoundDefinition openSound;

	// Token: 0x040002FF RID: 767
	public SoundDefinition closeSound;

	// Token: 0x04000300 RID: 768
	[Header("Item Dropping")]
	public Vector3 dropPosition;

	// Token: 0x04000301 RID: 769
	public Vector3 dropVelocity = Vector3.forward;

	// Token: 0x04000302 RID: 770
	public ItemCategory onlyAcceptCategory = ItemCategory.All;

	// Token: 0x04000303 RID: 771
	public bool onlyOneUser;

	// Token: 0x060005D3 RID: 1491 RVA: 0x0004134C File Offset: 0x0003F54C
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("StorageContainer.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Occpued", 0.1f))
			{
				if (this.Menu_Occupied_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Occpued.show = true;
					this.__menuOption_Menu_Occpued.showDisabled = false;
					this.__menuOption_Menu_Occpued.longUseOnly = false;
					this.__menuOption_Menu_Occpued.order = 0;
					this.__menuOption_Menu_Occpued.icon = "close";
					this.__menuOption_Menu_Occpued.desc = "occupied_desc";
					this.__menuOption_Menu_Occpued.title = "loot_occupied";
					if (this.__menuOption_Menu_Occpued.function == null)
					{
						this.__menuOption_Menu_Occpued.function = new Action<BasePlayer>(this.Menu_Occpued);
					}
					list.Add(this.__menuOption_Menu_Occpued);
				}
			}
			using (TimeWarning.New("Menu_OnFire", 0.1f))
			{
				if (this.Menu_OnFire_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_OnFire.show = true;
					this.__menuOption_Menu_OnFire.showDisabled = false;
					this.__menuOption_Menu_OnFire.longUseOnly = false;
					this.__menuOption_Menu_OnFire.order = 0;
					this.__menuOption_Menu_OnFire.icon = "ignite";
					this.__menuOption_Menu_OnFire.title = "loot_onfire";
					if (this.__menuOption_Menu_OnFire.function == null)
					{
						this.__menuOption_Menu_OnFire.function = new Action<BasePlayer>(this.Menu_OnFire);
					}
					list.Add(this.__menuOption_Menu_OnFire);
				}
			}
			using (TimeWarning.New("Menu_Open", 0.1f))
			{
				if (this.Menu_Open_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Open.show = true;
					this.__menuOption_Menu_Open.showDisabled = false;
					this.__menuOption_Menu_Open.longUseOnly = false;
					this.__menuOption_Menu_Open.order = -1;
					this.__menuOption_Menu_Open.icon = "open";
					this.__menuOption_Menu_Open.desc = "open_storage_desc";
					this.__menuOption_Menu_Open.title = "open_loot";
					if (this.__menuOption_Menu_Open.function == null)
					{
						this.__menuOption_Menu_Open.function = new Action<BasePlayer>(this.Menu_Open);
					}
					list.Add(this.__menuOption_Menu_Open);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060005D4 RID: 1492 RVA: 0x00007279 File Offset: 0x00005479
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Occupied_ShowIf(LocalPlayer.Entity) || this.Menu_OnFire_ShowIf(LocalPlayer.Entity) || this.Menu_Open_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00041604 File Offset: 0x0003F804
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StorageContainer.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00041648 File Offset: 0x0003F848
	public void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(this.dropPosition, Vector3.one * 0.1f);
		Gizmos.color = Color.white;
		Gizmos.DrawRay(this.dropPosition, this.dropVelocity);
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x000072AE File Offset: 0x000054AE
	public bool OccupiedCheck(BasePlayer player = null)
	{
		return (player != null && player.inventory.loot.entitySource == this) || !this.onlyOneUser || !base.IsOpen();
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual int GetMoveToContainerIndex(BasePlayer player)
	{
		return 0;
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x000072E6 File Offset: 0x000054E6
	public virtual int GetMoveToSlotIndex(BasePlayer player)
	{
		return -1;
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x000072E9 File Offset: 0x000054E9
	public virtual bool ShouldShowLootMenus()
	{
		return this.isLootable && !base.IsOnFire() && (!this.needsBuildingPrivilegeToUse || LocalPlayer.Entity.CanBuild()) && this.OccupiedCheck(null);
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00005BEF File Offset: 0x00003DEF
	[BaseEntity.Menu("open_loot", "Open", Order = -1)]
	[BaseEntity.Menu.Description("open_storage_desc", "Open the storage container")]
	[BaseEntity.Menu.ShowIf("Menu_Open_ShowIf")]
	[BaseEntity.Menu.Icon("open")]
	public void Menu_Open(BasePlayer player)
	{
		base.ServerRPC("RPC_OpenLoot");
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x0000722F File Offset: 0x0000542F
	public bool Menu_Open_ShowIf(BasePlayer player)
	{
		return this.ShouldShowLootMenus();
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00002ECE File Offset: 0x000010CE
	[BaseEntity.Menu.ShowIf("Menu_Occupied_ShowIf")]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.Description("occupied_desc", "Occupied by another player")]
	[BaseEntity.Menu("loot_occupied", "Occupied")]
	public void Menu_Occpued(BasePlayer player)
	{
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00007318 File Offset: 0x00005518
	public bool Menu_Occupied_ShowIf(BasePlayer player)
	{
		return !this.OccupiedCheck(player);
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00002ECE File Offset: 0x000010CE
	[BaseEntity.Menu.ShowIf("Menu_OnFire_ShowIf")]
	[BaseEntity.Menu("loot_onfire", "On Fire")]
	[BaseEntity.Menu.Icon("ignite")]
	public void Menu_OnFire(BasePlayer player)
	{
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00007324 File Offset: 0x00005524
	public bool Menu_OnFire_ShowIf(BasePlayer player)
	{
		return base.IsOnFire();
	}
}
