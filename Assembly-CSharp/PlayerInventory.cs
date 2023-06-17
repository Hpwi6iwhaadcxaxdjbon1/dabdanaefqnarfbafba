using System;
using System.Collections.Generic;
using ConVar;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class PlayerInventory : EntityComponent<BasePlayer>
{
	// Token: 0x0400063D RID: 1597
	public ItemContainer containerMain;

	// Token: 0x0400063E RID: 1598
	public ItemContainer containerBelt;

	// Token: 0x0400063F RID: 1599
	public ItemContainer containerWear;

	// Token: 0x04000640 RID: 1600
	public ItemCrafter crafting;

	// Token: 0x04000641 RID: 1601
	public PlayerLoot loot;

	// Token: 0x060009B1 RID: 2481 RVA: 0x00051CE8 File Offset: 0x0004FEE8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerInventory.OnRpcMessage", 0.1f))
		{
			if (rpc == 1571447769U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: UpdatedItemContainer ");
				}
				using (TimeWarning.New("UpdatedItemContainer", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage packet = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdatedItemContainer(packet);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in UpdatedItemContainer", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x00051E04 File Offset: 0x00050004
	protected void Initialize()
	{
		this.containerMain = new ItemContainer();
		this.containerMain.SetFlag(ItemContainer.Flag.IsPlayer, true);
		this.containerBelt = new ItemContainer();
		this.containerBelt.SetFlag(ItemContainer.Flag.IsPlayer, true);
		this.containerBelt.SetFlag(ItemContainer.Flag.Belt, true);
		this.containerWear = new ItemContainer();
		this.containerWear.SetFlag(ItemContainer.Flag.IsPlayer, true);
		this.containerWear.SetFlag(ItemContainer.Flag.Clothing, true);
		this.crafting = base.GetComponent<ItemCrafter>();
		this.crafting.AddContainer(this.containerMain);
		this.crafting.AddContainer(this.containerBelt);
		this.loot = base.GetComponent<PlayerLoot>();
		if (!this.loot)
		{
			this.loot = base.gameObject.AddComponent<PlayerLoot>();
		}
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x00051ECC File Offset: 0x000500CC
	public void DoDestroy()
	{
		if (this.containerMain != null)
		{
			this.containerMain.Kill();
			this.containerMain = null;
		}
		if (this.containerBelt != null)
		{
			this.containerBelt.Kill();
			this.containerBelt = null;
		}
		if (this.containerWear != null)
		{
			this.containerWear.Kill();
			this.containerWear = null;
		}
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x00009B09 File Offset: 0x00007D09
	public void ClientInit(BasePlayer owner)
	{
		this.Initialize();
		this.containerMain.playerOwner = owner;
		this.containerBelt.playerOwner = owner;
		this.containerWear.playerOwner = owner;
		this.loot.ClientInit(owner);
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x00051F28 File Offset: 0x00050128
	[BaseEntity.RPC_Client]
	private void UpdatedItemContainer(BaseEntity.RPCMessage packet)
	{
		using (UpdateItemContainer updateItemContainer = UpdateItemContainer.Deserialize(packet.read))
		{
			ItemContainer container = this.GetContainer((PlayerInventory.Type)updateItemContainer.type);
			if (container != null)
			{
				if (updateItemContainer.container.Count > 0)
				{
					container.Load(updateItemContainer.container[0]);
					container.OnChanged();
				}
				if (container == this.containerWear)
				{
					base.baseEntity.CL_ClothingChanged();
				}
				if (base.baseEntity.IsLocalPlayer())
				{
					LocalPlayer.OnInventoryChanged();
				}
			}
		}
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x00051FBC File Offset: 0x000501BC
	public Item FindItemUID(uint id)
	{
		if (id == 0U)
		{
			return null;
		}
		if (this.containerMain != null)
		{
			Item item = this.containerMain.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (this.containerBelt != null)
		{
			Item item2 = this.containerBelt.FindItemByUID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (this.containerWear != null)
		{
			Item item3 = this.containerWear.FindItemByUID(id);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return this.loot.FindItem(id);
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x00052040 File Offset: 0x00050240
	public Item FindItemID(string itemName)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemName);
		if (itemDefinition == null)
		{
			return null;
		}
		return this.FindItemID(itemDefinition.itemid);
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x0005206C File Offset: 0x0005026C
	public Item FindItemID(int id)
	{
		if (this.containerMain != null)
		{
			Item item = this.containerMain.FindItemByItemID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (this.containerBelt != null)
		{
			Item item2 = this.containerBelt.FindItemByItemID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (this.containerWear != null)
		{
			Item item3 = this.containerWear.FindItemByItemID(id);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return null;
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x000520E0 File Offset: 0x000502E0
	public List<Item> FindItemIDs(int id)
	{
		List<Item> list = new List<Item>();
		if (this.containerMain != null)
		{
			list.AddRange(this.containerMain.FindItemsByItemID(id));
		}
		if (this.containerBelt != null)
		{
			list.AddRange(this.containerBelt.FindItemsByItemID(id));
		}
		if (this.containerWear != null)
		{
			list.AddRange(this.containerWear.FindItemsByItemID(id));
		}
		return list;
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00052144 File Offset: 0x00050344
	public ItemContainer FindContainer(uint id)
	{
		ItemContainer result;
		using (TimeWarning.New("FindContainer", 0.1f))
		{
			ItemContainer itemContainer = this.containerMain.FindContainer(id);
			if (itemContainer != null)
			{
				result = itemContainer;
			}
			else
			{
				itemContainer = this.containerBelt.FindContainer(id);
				if (itemContainer != null)
				{
					result = itemContainer;
				}
				else
				{
					itemContainer = this.containerWear.FindContainer(id);
					if (itemContainer != null)
					{
						result = itemContainer;
					}
					else
					{
						result = this.loot.FindContainer(id);
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00009B41 File Offset: 0x00007D41
	public ItemContainer GetContainer(PlayerInventory.Type id)
	{
		if (id == PlayerInventory.Type.Main)
		{
			return this.containerMain;
		}
		if (PlayerInventory.Type.Belt == id)
		{
			return this.containerBelt;
		}
		if (PlayerInventory.Type.Wear == id)
		{
			return this.containerWear;
		}
		return null;
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x000521C8 File Offset: 0x000503C8
	public void Load(PlayerInventory msg)
	{
		if (msg.invMain != null)
		{
			this.containerMain.Load(msg.invMain);
		}
		if (msg.invBelt != null)
		{
			this.containerBelt.Load(msg.invBelt);
		}
		if (msg.invWear != null)
		{
			this.containerWear.Load(msg.invWear);
		}
		if (base.baseEntity && base.baseEntity.isClient && base.baseEntity.IsLocalPlayer())
		{
			LocalPlayer.OnInventoryChanged();
		}
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x0005224C File Offset: 0x0005044C
	public int GetAmount(int itemid)
	{
		if (itemid == 0)
		{
			return 0;
		}
		int num = 0;
		if (this.containerMain != null)
		{
			num += this.containerMain.GetAmount(itemid, true);
		}
		if (this.containerBelt != null)
		{
			num += this.containerBelt.GetAmount(itemid, true);
		}
		if (this.containerWear != null)
		{
			num += this.containerWear.GetAmount(itemid, true);
		}
		return num;
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x000522AC File Offset: 0x000504AC
	public Item[] AllItems()
	{
		List<Item> list = new List<Item>();
		if (this.containerMain != null)
		{
			list.AddRange(this.containerMain.itemList);
		}
		if (this.containerBelt != null)
		{
			list.AddRange(this.containerBelt.itemList);
		}
		if (this.containerWear != null)
		{
			list.AddRange(this.containerWear.itemList);
		}
		return list.ToArray();
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x00052310 File Offset: 0x00050510
	public int AllItemsNoAlloc(ref List<Item> items)
	{
		items.Clear();
		if (this.containerMain != null)
		{
			items.AddRange(this.containerMain.itemList);
		}
		if (this.containerBelt != null)
		{
			items.AddRange(this.containerBelt.itemList);
		}
		if (this.containerWear != null)
		{
			items.AddRange(this.containerWear.itemList);
		}
		return items.Count;
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x00009B64 File Offset: 0x00007D64
	public void FindAmmo(List<Item> list, AmmoTypes ammoType)
	{
		if (this.containerMain != null)
		{
			this.containerMain.FindAmmo(list, ammoType);
		}
		if (this.containerBelt != null)
		{
			this.containerBelt.FindAmmo(list, ammoType);
		}
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00009B90 File Offset: 0x00007D90
	public bool HasAmmo(AmmoTypes ammoType)
	{
		return this.containerMain.HasAmmo(ammoType) || this.containerBelt.HasAmmo(ammoType);
	}

	// Token: 0x020000B1 RID: 177
	public enum Type
	{
		// Token: 0x04000643 RID: 1603
		Main,
		// Token: 0x04000644 RID: 1604
		Belt,
		// Token: 0x04000645 RID: 1605
		Wear
	}
}
