using System;
using System.Collections.Generic;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000B2 RID: 178
public class PlayerLoot : EntityComponent<BasePlayer>
{
	// Token: 0x04000646 RID: 1606
	public BaseEntity entitySource;

	// Token: 0x04000647 RID: 1607
	public Item itemSource;

	// Token: 0x04000648 RID: 1608
	public List<ItemContainer> containers = new List<ItemContainer>();

	// Token: 0x04000649 RID: 1609
	[NonSerialized]
	private EntityRef clientEntity;

	// Token: 0x060009C3 RID: 2499 RVA: 0x0005237C File Offset: 0x0005057C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerLoot.OnRpcMessage", 0.1f))
		{
			if (rpc == 1748134015U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: UpdateLoot ");
				}
				using (TimeWarning.New("UpdateLoot", 0.1f))
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
							this.UpdateLoot(rpc2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in UpdateLoot", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00009BB6 File Offset: 0x00007DB6
	public bool IsLooting()
	{
		return this.containers.Count > 0;
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00009BC6 File Offset: 0x00007DC6
	public void Clear()
	{
		if (!this.IsLooting())
		{
			return;
		}
		this.containers.Clear();
		this.entitySource = null;
		this.itemSource = null;
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x00052498 File Offset: 0x00050698
	public ItemContainer FindContainer(uint id)
	{
		if (!this.IsLooting())
		{
			return null;
		}
		foreach (ItemContainer itemContainer in this.containers)
		{
			ItemContainer itemContainer2 = itemContainer.FindContainer(id);
			if (itemContainer2 != null)
			{
				return itemContainer2;
			}
		}
		return null;
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x00052500 File Offset: 0x00050700
	public Item FindItem(uint id)
	{
		if (!this.IsLooting())
		{
			return null;
		}
		foreach (ItemContainer itemContainer in this.containers)
		{
			Item item = itemContainer.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x00009BEA File Offset: 0x00007DEA
	public BaseEntity GetClientEntity()
	{
		return this.clientEntity.Get(false);
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00002ECE File Offset: 0x000010CE
	public void ClientInit(BasePlayer owner)
	{
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00052570 File Offset: 0x00050770
	[BaseEntity.RPC_Client]
	private void UpdateLoot(BaseEntity.RPCMessage rpc)
	{
		PlayerUpdateLoot playerUpdateLoot = PlayerUpdateLoot.Deserialize(rpc.read);
		if (playerUpdateLoot.containers.Count == 0)
		{
			this.Clear();
			return;
		}
		this.containers.Clear();
		foreach (ItemContainer container in playerUpdateLoot.containers)
		{
			ItemContainer itemContainer = new ItemContainer();
			itemContainer.Load(container);
			this.containers.Add(itemContainer);
		}
		this.clientEntity.uid = playerUpdateLoot.entityID;
		LocalPlayer.OnInventoryChanged();
	}
}
