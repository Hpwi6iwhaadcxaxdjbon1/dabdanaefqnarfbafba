using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class LootableCorpse : BaseCorpse
{
	// Token: 0x0400022A RID: 554
	private Option __menuOption_Menu_Open;

	// Token: 0x0400022B RID: 555
	public string lootPanelName = "generic";

	// Token: 0x0400022C RID: 556
	[NonSerialized]
	public ulong playerSteamID;

	// Token: 0x0400022D RID: 557
	[NonSerialized]
	public string _playerName;

	// Token: 0x060004B5 RID: 1205 RVA: 0x0003C8BC File Offset: 0x0003AABC
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("LootableCorpse.GetMenuOptions", 0.1f))
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

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x060004B6 RID: 1206 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool HasMenuOptions
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0003C9B4 File Offset: 0x0003ABB4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LootableCorpse.OnRpcMessage", 0.1f))
		{
			if (rpc == 2915451487U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: RPC_ClientLootCorpse ");
				}
				using (TimeWarning.New("RPC_ClientLootCorpse", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_ClientLootCorpse(rpc2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in RPC_ClientLootCorpse", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x060004B8 RID: 1208 RVA: 0x000062C0 File Offset: 0x000044C0
	// (set) Token: 0x060004B9 RID: 1209 RVA: 0x000062E3 File Offset: 0x000044E3
	public string playerName
	{
		get
		{
			if (base.isClient && Global.streamermode)
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

	// Token: 0x060004BA RID: 1210 RVA: 0x0003CAD0 File Offset: 0x0003ACD0
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerName = info.msg.lootableCorpse.playerName;
			this.playerSteamID = info.msg.lootableCorpse.playerID;
		}
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x000062EC File Offset: 0x000044EC
	[BaseEntity.RPC_Client]
	private void RPC_ClientLootCorpse(BaseEntity.RPCMessage rpc)
	{
		UIInventory.OpenLoot("player_corpse");
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x000062F8 File Offset: 0x000044F8
	[BaseEntity.Menu.Description("loot_corpse_desc", "Loot the corpse")]
	[BaseEntity.Menu.Icon("open")]
	[BaseEntity.Menu("loot_corpse", "Loot")]
	public void Menu_Open(BasePlayer player)
	{
		base.ServerRPC("RPC_LootCorpse");
	}
}
