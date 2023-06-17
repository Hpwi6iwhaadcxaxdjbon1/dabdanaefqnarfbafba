using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000036 RID: 54
public class ItemBasedFlowRestrictor : IOEntity
{
	// Token: 0x0400020D RID: 525
	private Option __menuOption_Menu_Open;

	// Token: 0x0400020E RID: 526
	public ItemDefinition passthroughItem;

	// Token: 0x0400020F RID: 527
	public ItemContainer.ContentsType allowedContents = ItemContainer.ContentsType.Generic;

	// Token: 0x04000210 RID: 528
	public int maxStackSize = 1;

	// Token: 0x04000211 RID: 529
	public int numSlots;

	// Token: 0x04000212 RID: 530
	public string lootPanelName = "generic";

	// Token: 0x04000213 RID: 531
	public const BaseEntity.Flags HasPassthrough = BaseEntity.Flags.Reserved1;

	// Token: 0x04000214 RID: 532
	public const BaseEntity.Flags Sparks = BaseEntity.Flags.Reserved2;

	// Token: 0x04000215 RID: 533
	public float passthroughItemConditionLossPerSec = 1f;

	// Token: 0x0600048B RID: 1163 RVA: 0x0003BF08 File Offset: 0x0003A108
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("ItemBasedFlowRestrictor.GetMenuOptions", 0.1f))
		{
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

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600048C RID: 1164 RVA: 0x000060E9 File Offset: 0x000042E9
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Open_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x0003C010 File Offset: 0x0003A210
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ItemBasedFlowRestrictor.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x00005BEF File Offset: 0x00003DEF
	[BaseEntity.Menu.ShowIf("Menu_Open_ShowIf")]
	[BaseEntity.Menu("open_loot", "Open", Order = -1)]
	[BaseEntity.Menu.Description("open_storage_desc", "Open the storage container")]
	[BaseEntity.Menu.Icon("open")]
	public void Menu_Open(BasePlayer player)
	{
		base.ServerRPC("RPC_OpenLoot");
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00002D44 File Offset: 0x00000F44
	public bool Menu_Open_ShowIf(BasePlayer player)
	{
		return true;
	}
}
