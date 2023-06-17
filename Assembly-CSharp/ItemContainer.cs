using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000431 RID: 1073
public sealed class ItemContainer
{
	// Token: 0x04001679 RID: 5753
	public ItemContainer.Flag flags;

	// Token: 0x0400167A RID: 5754
	public ItemContainer.ContentsType allowedContents;

	// Token: 0x0400167B RID: 5755
	public ItemDefinition onlyAllowedItem;

	// Token: 0x0400167C RID: 5756
	public List<ItemSlot> availableSlots = new List<ItemSlot>();

	// Token: 0x0400167D RID: 5757
	public int capacity = 2;

	// Token: 0x0400167E RID: 5758
	public uint uid;

	// Token: 0x0400167F RID: 5759
	public bool dirty;

	// Token: 0x04001680 RID: 5760
	public List<Item> itemList = new List<Item>();

	// Token: 0x04001681 RID: 5761
	public float temperature = 15f;

	// Token: 0x04001682 RID: 5762
	public Item parent;

	// Token: 0x04001683 RID: 5763
	public BasePlayer playerOwner;

	// Token: 0x04001684 RID: 5764
	public BaseEntity entityOwner;

	// Token: 0x04001685 RID: 5765
	public bool isServer;

	// Token: 0x04001686 RID: 5766
	public int maxStackSize;

	// Token: 0x060019F2 RID: 6642 RVA: 0x00015800 File Offset: 0x00013A00
	public bool HasFlag(ItemContainer.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x060019F3 RID: 6643 RVA: 0x0001580D File Offset: 0x00013A0D
	public void SetFlag(ItemContainer.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x060019F4 RID: 6644 RVA: 0x00015830 File Offset: 0x00013A30
	public bool IsLocked()
	{
		return this.HasFlag(ItemContainer.Flag.IsLocked);
	}

	// Token: 0x060019F5 RID: 6645 RVA: 0x0001583A File Offset: 0x00013A3A
	public bool PlayerItemInputBlocked()
	{
		return this.HasFlag(ItemContainer.Flag.NoItemInput);
	}

	// Token: 0x060019F6 RID: 6646 RVA: 0x00091C58 File Offset: 0x0008FE58
	public void OnChanged()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnChanged();
		}
	}

	// Token: 0x060019F7 RID: 6647 RVA: 0x00015847 File Offset: 0x00013A47
	public void ClientInitialize(Item parentItem, int iMaxCapacity)
	{
		this.parent = parentItem;
		this.capacity = iMaxCapacity;
		this.uid = 0U;
		this.isServer = false;
	}

	// Token: 0x060019F8 RID: 6648 RVA: 0x00091C8C File Offset: 0x0008FE8C
	public Item FindItemByUID(uint iUID)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			Item item = this.itemList[i];
			if (item.IsValid())
			{
				Item item2 = item.FindItem(iUID);
				if (item2 != null)
				{
					return item2;
				}
			}
		}
		return null;
	}

	// Token: 0x060019F9 RID: 6649 RVA: 0x00015865 File Offset: 0x00013A65
	public bool IsFull()
	{
		return this.itemList.Count >= this.capacity;
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x0001587D File Offset: 0x00013A7D
	public bool CanTake(Item item)
	{
		return !this.IsFull();
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x0001588A File Offset: 0x00013A8A
	internal bool Insert(Item item)
	{
		if (this.itemList.Contains(item))
		{
			return false;
		}
		if (this.IsFull())
		{
			return false;
		}
		this.itemList.Add(item);
		item.parent = this;
		return this.FindPosition(item);
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x000158C5 File Offset: 0x00013AC5
	public bool SlotTaken(int i)
	{
		return this.GetSlot(i) != null;
	}

	// Token: 0x060019FD RID: 6653 RVA: 0x00091CD4 File Offset: 0x0008FED4
	public Item GetSlot(int slot)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].position == slot)
			{
				return this.itemList[i];
			}
		}
		return null;
	}

	// Token: 0x060019FE RID: 6654 RVA: 0x00091D1C File Offset: 0x0008FF1C
	internal bool FindPosition(Item item)
	{
		int position = item.position;
		item.position = -1;
		if (position >= 0 && !this.SlotTaken(position))
		{
			item.position = position;
			return true;
		}
		for (int i = 0; i < this.capacity; i++)
		{
			if (!this.SlotTaken(i))
			{
				item.position = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019FF RID: 6655 RVA: 0x00091D74 File Offset: 0x0008FF74
	internal void Clear()
	{
		Item[] array = this.itemList.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Remove(0f);
		}
	}

	// Token: 0x06001A00 RID: 6656 RVA: 0x00091DA8 File Offset: 0x0008FFA8
	public void Kill()
	{
		foreach (Item item in Enumerable.ToList<Item>(this.itemList))
		{
			item.Remove(0f);
		}
		this.itemList.Clear();
	}

	// Token: 0x06001A01 RID: 6657 RVA: 0x00091E10 File Offset: 0x00090010
	public int GetAmount(int itemid, bool onlyUsableAmounts)
	{
		int num = 0;
		foreach (Item item in this.itemList)
		{
			if (item.info.itemid == itemid && (!onlyUsableAmounts || !item.IsBusy()))
			{
				num += item.amount;
			}
		}
		return num;
	}

	// Token: 0x06001A02 RID: 6658 RVA: 0x00091E84 File Offset: 0x00090084
	public Item FindItemByItemID(int itemid)
	{
		return Enumerable.FirstOrDefault<Item>(this.itemList, (Item x) => x.info.itemid == itemid);
	}

	// Token: 0x06001A03 RID: 6659 RVA: 0x00091EB8 File Offset: 0x000900B8
	public Item FindItemsByItemName(string name)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(name);
		if (itemDefinition == null)
		{
			return null;
		}
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].info == itemDefinition)
			{
				return this.itemList[i];
			}
		}
		return null;
	}

	// Token: 0x06001A04 RID: 6660 RVA: 0x00091F14 File Offset: 0x00090114
	public List<Item> FindItemsByItemID(int itemid)
	{
		return this.itemList.FindAll((Item x) => x.info.itemid == itemid);
	}

	// Token: 0x06001A05 RID: 6661 RVA: 0x00091F48 File Offset: 0x00090148
	public ItemContainer Save()
	{
		ItemContainer itemContainer = Pool.Get<ItemContainer>();
		itemContainer.contents = Pool.GetList<Item>();
		itemContainer.UID = this.uid;
		itemContainer.slots = this.capacity;
		itemContainer.temperature = this.temperature;
		itemContainer.allowedContents = (int)this.allowedContents;
		itemContainer.allowedItem = ((this.onlyAllowedItem != null) ? this.onlyAllowedItem.itemid : 0);
		itemContainer.flags = (int)this.flags;
		itemContainer.maxStackSize = this.maxStackSize;
		if (this.availableSlots != null && this.availableSlots.Count > 0)
		{
			itemContainer.availableSlots = Pool.GetList<int>();
			for (int i = 0; i < this.availableSlots.Count; i++)
			{
				itemContainer.availableSlots.Add((int)this.availableSlots[i]);
			}
		}
		for (int j = 0; j < this.itemList.Count; j++)
		{
			Item item = this.itemList[j];
			if (item.IsValid())
			{
				itemContainer.contents.Add(item.Save(true, true));
			}
		}
		return itemContainer;
	}

	// Token: 0x06001A06 RID: 6662 RVA: 0x00092060 File Offset: 0x00090260
	public void Load(ItemContainer container)
	{
		using (TimeWarning.New("ItemContainer.Load", 0.1f))
		{
			this.uid = container.UID;
			this.capacity = container.slots;
			List<Item> list = this.itemList;
			this.itemList = Pool.GetList<Item>();
			this.temperature = container.temperature;
			this.flags = (ItemContainer.Flag)container.flags;
			this.allowedContents = (ItemContainer.ContentsType)((container.allowedContents == 0) ? 1 : container.allowedContents);
			this.onlyAllowedItem = ((container.allowedItem != 0) ? ItemManager.FindItemDefinition(container.allowedItem) : null);
			this.maxStackSize = container.maxStackSize;
			this.availableSlots.Clear();
			for (int i = 0; i < container.availableSlots.Count; i++)
			{
				this.availableSlots.Add((ItemSlot)container.availableSlots[i]);
			}
			using (TimeWarning.New("container.contents", 0.1f))
			{
				foreach (Item item in container.contents)
				{
					Item item2 = null;
					foreach (Item item3 in list)
					{
						if (item3.uid == item.UID)
						{
							item2 = item3;
							break;
						}
					}
					item2 = ItemManager.Load(item, item2, this.isServer);
					if (item2 != null)
					{
						item2.parent = this;
						item2.position = item.slot;
						this.Insert(item2);
					}
				}
			}
			using (TimeWarning.New("Delete old items", 0.1f))
			{
				foreach (Item item4 in list)
				{
					if (!this.itemList.Contains(item4))
					{
						item4.Remove(0f);
					}
				}
			}
			this.dirty = true;
			Pool.FreeList<Item>(ref list);
		}
	}

	// Token: 0x06001A07 RID: 6663 RVA: 0x000158D1 File Offset: 0x00013AD1
	public BasePlayer GetOwnerPlayer()
	{
		return this.playerOwner;
	}

	// Token: 0x06001A08 RID: 6664 RVA: 0x00092310 File Offset: 0x00090510
	public void FindAmmo(List<Item> list, AmmoTypes ammoType)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].FindAmmo(list, ammoType);
		}
	}

	// Token: 0x06001A09 RID: 6665 RVA: 0x00092348 File Offset: 0x00090548
	public bool HasAmmo(AmmoTypes ammoType)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].HasAmmo(ammoType))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001A0A RID: 6666 RVA: 0x00092384 File Offset: 0x00090584
	public uint ContentsHash()
	{
		uint num = 0U;
		for (int i = 0; i < this.capacity; i++)
		{
			Item slot = this.GetSlot(i);
			if (slot != null)
			{
				num = CRC.Compute32(num, slot.info.itemid);
				num = CRC.Compute32(num, slot.skin);
			}
		}
		return num;
	}

	// Token: 0x06001A0B RID: 6667 RVA: 0x000923D0 File Offset: 0x000905D0
	internal ItemContainer FindContainer(uint id)
	{
		if (id == this.uid)
		{
			return this;
		}
		for (int i = 0; i < this.itemList.Count; i++)
		{
			Item item = this.itemList[i];
			if (item.contents != null)
			{
				ItemContainer itemContainer = item.contents.FindContainer(id);
				if (itemContainer != null)
				{
					return itemContainer;
				}
			}
		}
		return null;
	}

	// Token: 0x06001A0C RID: 6668 RVA: 0x00092428 File Offset: 0x00090628
	public ItemContainer.CanAcceptResult CanAcceptItem(Item item, int targetPos)
	{
		if ((this.allowedContents & item.info.itemType) != item.info.itemType)
		{
			return ItemContainer.CanAcceptResult.CannotAccept;
		}
		if (this.onlyAllowedItem != null && this.onlyAllowedItem != item.info)
		{
			return ItemContainer.CanAcceptResult.CannotAccept;
		}
		if (this.availableSlots != null && this.availableSlots.Count > 0)
		{
			if (item.info.occupySlots == (ItemSlot)0 || item.info.occupySlots == ItemSlot.None)
			{
				return ItemContainer.CanAcceptResult.CannotAccept;
			}
			int[] array = new int[32];
			foreach (ItemSlot itemSlot in this.availableSlots)
			{
				array[(int)Mathf.Log((float)itemSlot, 2f)]++;
			}
			foreach (Item item2 in this.itemList)
			{
				for (int i = 0; i < 32; i++)
				{
					if ((item2.info.occupySlots & (ItemSlot)(1 << i)) != (ItemSlot)0)
					{
						array[i]--;
					}
				}
			}
			for (int j = 0; j < 32; j++)
			{
				if ((item.info.occupySlots & (ItemSlot)(1 << j)) != (ItemSlot)0 && array[j] <= 0)
				{
					return ItemContainer.CanAcceptResult.CannotAcceptRightNow;
				}
			}
		}
		return ItemContainer.CanAcceptResult.CanAccept;
	}

	// Token: 0x02000432 RID: 1074
	[Flags]
	public enum Flag
	{
		// Token: 0x04001688 RID: 5768
		IsPlayer = 1,
		// Token: 0x04001689 RID: 5769
		Clothing = 2,
		// Token: 0x0400168A RID: 5770
		Belt = 4,
		// Token: 0x0400168B RID: 5771
		SingleType = 8,
		// Token: 0x0400168C RID: 5772
		IsLocked = 16,
		// Token: 0x0400168D RID: 5773
		ShowSlotsOnIcon = 32,
		// Token: 0x0400168E RID: 5774
		NoBrokenItems = 64,
		// Token: 0x0400168F RID: 5775
		NoItemInput = 128
	}

	// Token: 0x02000433 RID: 1075
	[Flags]
	public enum ContentsType
	{
		// Token: 0x04001691 RID: 5777
		Generic = 1,
		// Token: 0x04001692 RID: 5778
		Liquid = 2
	}

	// Token: 0x02000434 RID: 1076
	public enum CanAcceptResult
	{
		// Token: 0x04001694 RID: 5780
		CanAccept,
		// Token: 0x04001695 RID: 5781
		CannotAccept,
		// Token: 0x04001696 RID: 5782
		CannotAcceptRightNow
	}
}
