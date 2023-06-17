using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x0200001B RID: 27
public class BaseLock : BaseEntity
{
	// Token: 0x04000109 RID: 265
	private Option __menuOption_Menu_RemoveLock;

	// Token: 0x0400010A RID: 266
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;

	// Token: 0x0600029B RID: 667 RVA: 0x000328A8 File Offset: 0x00030AA8
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("BaseLock.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_RemoveLock", 0.1f))
			{
				if (this.Menu_RemoveLock_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_RemoveLock.show = true;
					this.__menuOption_Menu_RemoveLock.showDisabled = false;
					this.__menuOption_Menu_RemoveLock.longUseOnly = false;
					this.__menuOption_Menu_RemoveLock.order = 10;
					this.__menuOption_Menu_RemoveLock.icon = "pickup";
					this.__menuOption_Menu_RemoveLock.desc = "remove_lock_desc";
					this.__menuOption_Menu_RemoveLock.title = "remove_lock";
					if (this.__menuOption_Menu_RemoveLock.function == null)
					{
						this.__menuOption_Menu_RemoveLock.function = new Action<BasePlayer>(this.Menu_RemoveLock);
					}
					list.Add(this.__menuOption_Menu_RemoveLock);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x0600029C RID: 668 RVA: 0x0000492A File Offset: 0x00002B2A
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_RemoveLock_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600029D RID: 669 RVA: 0x000329B0 File Offset: 0x00030BB0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseLock.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600029E RID: 670 RVA: 0x00004941 File Offset: 0x00002B41
	public virtual bool GetPlayerLockPermission(BasePlayer player)
	{
		return this.OnTryToOpen(player);
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0000494A File Offset: 0x00002B4A
	public virtual bool OnTryToOpen(BasePlayer player)
	{
		return !base.IsLocked();
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool OnTryToClose(BasePlayer player)
	{
		return true;
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool HasLockPermission(BasePlayer player)
	{
		return true;
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x00004955 File Offset: 0x00002B55
	[BaseEntity.Menu.ShowIf("Menu_RemoveLock_ShowIf")]
	[BaseEntity.Menu("remove_lock", "Remove Lock", Order = 10)]
	[BaseEntity.Menu.Icon("pickup")]
	[BaseEntity.Menu.Description("remove_lock_desc", "Remove this lock and place it in your inventory")]
	public void Menu_RemoveLock(BasePlayer player)
	{
		base.ServerRPC("RPC_TakeLock");
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x0000494A File Offset: 0x00002B4A
	public bool Menu_RemoveLock_ShowIf(BasePlayer player)
	{
		return !base.IsLocked();
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x000329F4 File Offset: 0x00030BF4
	public override List<Option> GetMenuItems(BasePlayer player)
	{
		List<Option> list = base.GetMenuItems(player);
		BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity)
		{
			if (parentEntity.IsBusy())
			{
				return null;
			}
			List<Option> menuItems = parentEntity.GetMenuItems(player);
			if (menuItems != null)
			{
				if (list == null)
				{
					return menuItems;
				}
				menuItems.AddRange(list);
				list = menuItems;
			}
		}
		return list;
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x00004962 File Offset: 0x00002B62
	public override float BoundsPadding()
	{
		return 2f;
	}
}
