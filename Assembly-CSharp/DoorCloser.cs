using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x0200002C RID: 44
public class DoorCloser : BaseEntity
{
	// Token: 0x040001BF RID: 447
	private Option __menuOption_Menu_Remove;

	// Token: 0x040001C0 RID: 448
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;

	// Token: 0x040001C1 RID: 449
	public float delay = 3f;

	// Token: 0x0600041C RID: 1052 RVA: 0x0003A568 File Offset: 0x00038768
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("DoorCloser.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Remove", 0.1f))
			{
				if (this.Menu_Remove_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Remove.show = true;
					this.__menuOption_Menu_Remove.showDisabled = false;
					this.__menuOption_Menu_Remove.longUseOnly = false;
					this.__menuOption_Menu_Remove.order = 100;
					this.__menuOption_Menu_Remove.icon = "pickup";
					this.__menuOption_Menu_Remove.desc = "remove_desc";
					this.__menuOption_Menu_Remove.title = "remove";
					if (this.__menuOption_Menu_Remove.function == null)
					{
						this.__menuOption_Menu_Remove.function = new Action<BasePlayer>(this.Menu_Remove);
					}
					list.Add(this.__menuOption_Menu_Remove);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x0600041D RID: 1053 RVA: 0x00005B6E File Offset: 0x00003D6E
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Remove_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x0003A670 File Offset: 0x00038870
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DoorCloser.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x00005B85 File Offset: 0x00003D85
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x00005B8C File Offset: 0x00003D8C
	public Door GetDoor()
	{
		return base.GetParentEntity() as Door;
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x00005B99 File Offset: 0x00003D99
	[BaseEntity.Menu("remove", "Remove", Order = 100)]
	[BaseEntity.Menu.ShowIf("Menu_Remove_ShowIf")]
	[BaseEntity.Menu.Icon("pickup")]
	[BaseEntity.Menu.Description("remove_desc", "Remove this object and place it into your inventory")]
	public void Menu_Remove(BasePlayer player)
	{
		base.ServerRPC("RPC_Take");
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x0003A6B4 File Offset: 0x000388B4
	public bool Menu_Remove_ShowIf(BasePlayer player)
	{
		if (!this.GetDoor())
		{
			return false;
		}
		BaseLock baseLock = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		return (baseLock == null || !baseLock.IsLocked()) && player.CanBuild();
	}
}
