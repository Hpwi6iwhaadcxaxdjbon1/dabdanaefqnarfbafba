using System;
using ConVar;
using Facepunch.Steamworks;
using Network;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class SteamInventory : EntityComponent<BasePlayer>
{
	// Token: 0x04000678 RID: 1656
	private Inventory.Item[] Items;

	// Token: 0x06000A03 RID: 2563 RVA: 0x00053488 File Offset: 0x00051688
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SteamInventory.OnRpcMessage", 0.1f))
		{
			if (rpc == 1523970835U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: ClientUpdateSteamInventory ");
				}
				using (TimeWarning.New("ClientUpdateSteamInventory", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ClientUpdateSteamInventory(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in ClientUpdateSteamInventory", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x000535A4 File Offset: 0x000517A4
	public bool HasItem(int itemid)
	{
		if (this.Items == null)
		{
			return false;
		}
		Inventory.Item[] items = this.Items;
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].DefinitionId == itemid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x00009EAD File Offset: 0x000080AD
	[BaseEntity.RPC_Client]
	private void ClientUpdateSteamInventory(BaseEntity.RPCMessage msg)
	{
		Debug.Log("Server has requested inventory update");
		SingletonComponent<SteamClient>.Instance.SendUpdatedInventory();
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x000535E0 File Offset: 0x000517E0
	public void ClientUpdate()
	{
		if (global::Client.Steam == null)
		{
			return;
		}
		if (global::Client.Steam.Inventory.SerializedItems == null)
		{
			return;
		}
		this.Items = global::Client.Steam.Inventory.Items;
		base.baseEntity.ServerRPC<uint, byte[]>("UpdateSteamInventory", (uint)global::Client.Steam.Inventory.SerializedItems.Length, global::Client.Steam.Inventory.SerializedItems);
		UIBlueprints.Refresh();
	}
}
