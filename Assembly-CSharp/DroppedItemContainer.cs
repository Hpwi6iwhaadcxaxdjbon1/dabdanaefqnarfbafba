using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using GameMenu;
using Network;

// Token: 0x0200002D RID: 45
public class DroppedItemContainer : BaseCombatEntity
{
	// Token: 0x040001C2 RID: 450
	private Option __menuOption_Menu_Open;

	// Token: 0x040001C3 RID: 451
	public string lootPanelName = "generic";

	// Token: 0x040001C4 RID: 452
	[NonSerialized]
	public ulong playerSteamID;

	// Token: 0x040001C5 RID: 453
	[NonSerialized]
	public string _playerName;

	// Token: 0x06000424 RID: 1060 RVA: 0x0003A6F8 File Offset: 0x000388F8
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("DroppedItemContainer.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Open", 0.1f))
			{
				this.__menuOption_Menu_Open.show = true;
				this.__menuOption_Menu_Open.showDisabled = false;
				this.__menuOption_Menu_Open.longUseOnly = false;
				this.__menuOption_Menu_Open.order = 0;
				this.__menuOption_Menu_Open.icon = "open";
				this.__menuOption_Menu_Open.desc = "loot_corpse_desc";
				this.__menuOption_Menu_Open.title = "loot_corpse";
				if (this.__menuOption_Menu_Open.function == null)
				{
					this.__menuOption_Menu_Open.function = new Action<BasePlayer>(this.Menu_Open);
				}
				list.Add(this.__menuOption_Menu_Open);
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x06000425 RID: 1061 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool HasMenuOptions
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x0003A7F0 File Offset: 0x000389F0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DroppedItemContainer.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000427 RID: 1063 RVA: 0x00005BB9 File Offset: 0x00003DB9
	// (set) Token: 0x06000428 RID: 1064 RVA: 0x00005BE6 File Offset: 0x00003DE6
	public string playerName
	{
		get
		{
			if (base.isClient && Global.streamermode && this.playerSteamID > 0UL)
			{
				return RandomUsernames.Get(this.playerSteamID);
			}
			return this._playerName;
		}
		set
		{
			this._playerName = value;
		}
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x0003A834 File Offset: 0x00038A34
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerName = info.msg.lootableCorpse.playerName;
			this.playerSteamID = info.msg.lootableCorpse.playerID;
		}
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x00005BEF File Offset: 0x00003DEF
	[BaseEntity.Menu.Description("loot_corpse_desc", "Loot the corpse")]
	[BaseEntity.Menu("loot_corpse", "Loot")]
	[BaseEntity.Menu.Icon("open")]
	public void Menu_Open(BasePlayer player)
	{
		base.ServerRPC("RPC_OpenLoot");
	}
}
