using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using ProtoBuf;
using Rust;
using Rust.Workshop;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200042D RID: 1069
public class Item
{
	// Token: 0x0400165C RID: 5724
	private float _condition;

	// Token: 0x0400165D RID: 5725
	private float _maxCondition = 100f;

	// Token: 0x0400165E RID: 5726
	public ItemDefinition info;

	// Token: 0x0400165F RID: 5727
	public uint uid;

	// Token: 0x04001660 RID: 5728
	public bool dirty;

	// Token: 0x04001661 RID: 5729
	public int amount = 1;

	// Token: 0x04001662 RID: 5730
	public int position;

	// Token: 0x04001663 RID: 5731
	public float busyTime;

	// Token: 0x04001664 RID: 5732
	public float removeTime;

	// Token: 0x04001665 RID: 5733
	public float fuel;

	// Token: 0x04001666 RID: 5734
	public bool isServer;

	// Token: 0x04001667 RID: 5735
	public Item.InstanceData instanceData;

	// Token: 0x04001668 RID: 5736
	public ulong skin;

	// Token: 0x04001669 RID: 5737
	public string name;

	// Token: 0x0400166A RID: 5738
	public string text;

	// Token: 0x0400166C RID: 5740
	public Item.Flag flags;

	// Token: 0x0400166D RID: 5741
	public ItemContainer contents;

	// Token: 0x0400166E RID: 5742
	public ItemContainer parent;

	// Token: 0x0400166F RID: 5743
	private EntityRef worldEnt;

	// Token: 0x04001670 RID: 5744
	private EntityRef heldEntity;

	// Token: 0x04001671 RID: 5745
	public string amountTextOverride;

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x060019B4 RID: 6580 RVA: 0x0001545D File Offset: 0x0001365D
	// (set) Token: 0x060019B3 RID: 6579 RVA: 0x00015444 File Offset: 0x00013644
	public float condition
	{
		get
		{
			return this._condition;
		}
		set
		{
			this._condition = Mathf.Clamp(value, 0f, this.maxCondition);
		}
	}

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x060019B6 RID: 6582 RVA: 0x00015488 File Offset: 0x00013688
	// (set) Token: 0x060019B5 RID: 6581 RVA: 0x00015465 File Offset: 0x00013665
	public float maxCondition
	{
		get
		{
			return this._maxCondition;
		}
		set
		{
			this._maxCondition = Mathf.Clamp(value, 0f, this.info.condition.max);
		}
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x060019B7 RID: 6583 RVA: 0x00015490 File Offset: 0x00013690
	public float maxConditionNormalized
	{
		get
		{
			return this._maxCondition / this.info.condition.max;
		}
	}

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x060019B8 RID: 6584 RVA: 0x000154A9 File Offset: 0x000136A9
	// (set) Token: 0x060019B9 RID: 6585 RVA: 0x000154C6 File Offset: 0x000136C6
	public float conditionNormalized
	{
		get
		{
			if (!this.hasCondition)
			{
				return 1f;
			}
			return this.condition / this.maxCondition;
		}
		set
		{
			if (!this.hasCondition)
			{
				return;
			}
			this.condition = value * this.maxCondition;
		}
	}

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x060019BA RID: 6586 RVA: 0x000154DF File Offset: 0x000136DF
	public bool hasCondition
	{
		get
		{
			return this.info != null && this.info.condition.enabled && this.info.condition.max > 0f;
		}
	}

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x060019BB RID: 6587 RVA: 0x0001551A File Offset: 0x0001371A
	public bool isBroken
	{
		get
		{
			return this.hasCondition && this.condition <= 0f;
		}
	}

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x060019BC RID: 6588 RVA: 0x00015536 File Offset: 0x00013736
	public int despawnMultiplier
	{
		get
		{
			if (!(this.info != null))
			{
				return 1;
			}
			return Mathf.Clamp((this.info.rarity - 1) * 4, 1, 100);
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x060019BD RID: 6589 RVA: 0x0001555F File Offset: 0x0001375F
	public ItemDefinition blueprintTargetDef
	{
		get
		{
			if (!this.IsBlueprint())
			{
				return null;
			}
			return ItemManager.FindItemDefinition(this.blueprintTarget);
		}
	}

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x060019BE RID: 6590 RVA: 0x00015576 File Offset: 0x00013776
	// (set) Token: 0x060019BF RID: 6591 RVA: 0x0001558D File Offset: 0x0001378D
	public int blueprintTarget
	{
		get
		{
			if (this.instanceData == null)
			{
				return 0;
			}
			return this.instanceData.blueprintTarget;
		}
		set
		{
			if (this.instanceData == null)
			{
				this.instanceData = new Item.InstanceData();
			}
			this.instanceData.ShouldPool = false;
			this.instanceData.blueprintTarget = value;
		}
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x060019C0 RID: 6592 RVA: 0x000155BA File Offset: 0x000137BA
	// (set) Token: 0x060019C1 RID: 6593 RVA: 0x000155C2 File Offset: 0x000137C2
	public int blueprintAmount
	{
		get
		{
			return this.amount;
		}
		set
		{
			this.amount = value;
		}
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x000155CB File Offset: 0x000137CB
	public bool IsBlueprint()
	{
		return this.blueprintTarget != 0;
	}

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060019C3 RID: 6595 RVA: 0x00091230 File Offset: 0x0008F430
	// (remove) Token: 0x060019C4 RID: 6596 RVA: 0x00091268 File Offset: 0x0008F468
	public event Action<Item> OnDirty;

	// Token: 0x060019C5 RID: 6597 RVA: 0x000155D6 File Offset: 0x000137D6
	public bool HasFlag(Item.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x060019C6 RID: 6598 RVA: 0x000155E3 File Offset: 0x000137E3
	public void SetFlag(Item.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x00015606 File Offset: 0x00013806
	public bool IsOn()
	{
		return this.HasFlag(Item.Flag.IsOn);
	}

	// Token: 0x060019C8 RID: 6600 RVA: 0x0001560F File Offset: 0x0001380F
	public bool IsOnFire()
	{
		return this.HasFlag(Item.Flag.OnFire);
	}

	// Token: 0x060019C9 RID: 6601 RVA: 0x00015618 File Offset: 0x00013818
	public bool IsCooking()
	{
		return this.HasFlag(Item.Flag.Cooking);
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x00015622 File Offset: 0x00013822
	public bool IsLocked()
	{
		return this.HasFlag(Item.Flag.IsLocked) || (this.parent != null && this.parent.IsLocked());
	}

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x060019CB RID: 6603 RVA: 0x00015644 File Offset: 0x00013844
	public Item parentItem
	{
		get
		{
			if (this.parent == null)
			{
				return null;
			}
			return this.parent.parent;
		}
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x0001565B File Offset: 0x0001385B
	public void MarkDirty()
	{
		this.OnChanged();
		this.dirty = true;
		if (this.OnDirty != null)
		{
			this.OnDirty.Invoke(this);
		}
	}

	// Token: 0x060019CD RID: 6605 RVA: 0x000912A0 File Offset: 0x0008F4A0
	public void OnChanged()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnChanged(this);
		}
		if (this.contents != null)
		{
			this.contents.OnChanged();
		}
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x00002ECE File Offset: 0x000010CE
	public void CollectedForCrafting(BasePlayer crafter)
	{
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x00002ECE File Offset: 0x000010CE
	public void ReturnedFromCancelledCraft(BasePlayer crafter)
	{
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x000912E4 File Offset: 0x0008F4E4
	public bool IsChildContainer(ItemContainer c)
	{
		if (this.contents == null)
		{
			return false;
		}
		if (this.contents == c)
		{
			return true;
		}
		using (List<Item>.Enumerator enumerator = this.contents.itemList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsChildContainer(c))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x0001567E File Offset: 0x0001387E
	public bool IsBusy()
	{
		return this.busyTime > Time.time;
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x00015690 File Offset: 0x00013890
	public void BusyFor(float fTime)
	{
		this.busyTime = Time.time + fTime;
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x0001569F File Offset: 0x0001389F
	public void Remove(float fTime = 0f)
	{
		if (this.removeTime > 0f)
		{
			return;
		}
		this.removeTime = Time.time + fTime;
		this.OnDirty = null;
		this.position = -1;
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x000156CA File Offset: 0x000138CA
	internal void DoRemove()
	{
		this.OnDirty = null;
		if (this.contents != null)
		{
			this.contents.Kill();
			this.contents = null;
		}
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x000156ED File Offset: 0x000138ED
	public void SwitchOnOff(bool bNewState, BasePlayer player)
	{
		if (this.HasFlag(Item.Flag.IsOn) == bNewState)
		{
			return;
		}
		this.SetFlag(Item.Flag.IsOn, bNewState);
		this.MarkDirty();
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x00015708 File Offset: 0x00013908
	public void LockUnlock(bool bNewState, BasePlayer player)
	{
		if (this.HasFlag(Item.Flag.IsLocked) == bNewState)
		{
			return;
		}
		this.SetFlag(Item.Flag.IsLocked, bNewState);
		this.MarkDirty();
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x060019D7 RID: 6615 RVA: 0x00015723 File Offset: 0x00013923
	public float temperature
	{
		get
		{
			if (this.parent != null)
			{
				return this.parent.temperature;
			}
			return 15f;
		}
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x0001573E File Offset: 0x0001393E
	public BasePlayer GetOwnerPlayer()
	{
		if (this.parent == null)
		{
			return null;
		}
		return this.parent.GetOwnerPlayer();
	}

	// Token: 0x060019D9 RID: 6617 RVA: 0x00015755 File Offset: 0x00013955
	public bool CanBeHeld()
	{
		return !this.isBroken;
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x00091358 File Offset: 0x0008F558
	public bool CanStack(Item item)
	{
		return item != this && this.info.stackable > 1 && item.info.stackable > 1 && item.info.itemid == this.info.itemid && (!this.hasCondition || this.condition == this.maxCondition) && (!item.hasCondition || item.condition == item.maxCondition) && this.IsValid() && (!this.IsBlueprint() || this.blueprintTarget == item.blueprintTarget);
	}

	// Token: 0x060019DB RID: 6619 RVA: 0x00015762 File Offset: 0x00013962
	public bool IsValid()
	{
		return this.removeTime <= 0f;
	}

	// Token: 0x060019DC RID: 6620 RVA: 0x000913F8 File Offset: 0x0008F5F8
	public void SetWorldEntity(BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			this.worldEnt.Set(null);
			this.MarkDirty();
			return;
		}
		if (this.worldEnt.uid == ent.net.ID)
		{
			return;
		}
		this.worldEnt.Set(ent);
		this.MarkDirty();
		this.OnMovedToWorld();
	}

	// Token: 0x060019DD RID: 6621 RVA: 0x00091454 File Offset: 0x0008F654
	public void OnMovedToWorld()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnMovedToWorld(this);
		}
	}

	// Token: 0x060019DE RID: 6622 RVA: 0x00015774 File Offset: 0x00013974
	public BaseEntity GetWorldEntity()
	{
		return this.worldEnt.Get(this.isServer);
	}

	// Token: 0x060019DF RID: 6623 RVA: 0x00015787 File Offset: 0x00013987
	public BaseEntity GetHeldEntity()
	{
		return this.heldEntity.Get(this.isServer);
	}

	// Token: 0x060019E0 RID: 6624 RVA: 0x00091484 File Offset: 0x0008F684
	public bool BeltBarSelect(BasePlayer player)
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			if (itemMods[i].BeltSelect(this, player))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019E1 RID: 6625 RVA: 0x000914BC File Offset: 0x0008F6BC
	public bool HasAmmo(AmmoTypes ammoType)
	{
		ItemModProjectile component = this.info.GetComponent<ItemModProjectile>();
		return (component && component.IsAmmo(ammoType)) || (this.contents != null && this.contents.HasAmmo(ammoType));
	}

	// Token: 0x060019E2 RID: 6626 RVA: 0x00091500 File Offset: 0x0008F700
	public void FindAmmo(List<Item> list, AmmoTypes ammoType)
	{
		ItemModProjectile component = this.info.GetComponent<ItemModProjectile>();
		if (component && component.IsAmmo(ammoType))
		{
			list.Add(this);
			return;
		}
		if (this.contents != null)
		{
			this.contents.FindAmmo(list, ammoType);
		}
	}

	// Token: 0x060019E3 RID: 6627 RVA: 0x00091548 File Offset: 0x0008F748
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"Item.",
			this.info.shortname,
			"x",
			this.amount,
			".",
			this.uid
		});
	}

	// Token: 0x060019E4 RID: 6628 RVA: 0x0001579A File Offset: 0x0001399A
	public Item FindItem(uint iUID)
	{
		if (this.uid == iUID)
		{
			return this;
		}
		if (this.contents == null)
		{
			return null;
		}
		return this.contents.FindItemByUID(iUID);
	}

	// Token: 0x060019E5 RID: 6629 RVA: 0x000915A4 File Offset: 0x0008F7A4
	public int MaxStackable()
	{
		int num = this.info.stackable;
		if (this.parent != null && this.parent.maxStackSize > 0)
		{
			num = Mathf.Min(this.parent.maxStackSize, num);
		}
		return num;
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x060019E6 RID: 6630 RVA: 0x000915E8 File Offset: 0x0008F7E8
	internal Sprite iconSprite
	{
		get
		{
			if (this.skin != 0UL)
			{
				ItemSkinDirectory.Skin skin = Enumerable.FirstOrDefault<ItemSkinDirectory.Skin>(this.info.skins, (ItemSkinDirectory.Skin x) => (long)x.id == (long)this.skin);
				if ((long)skin.id == (long)this.skin)
				{
					return skin.invItem.icon;
				}
				Sprite sprite = WorkshopIconLoader.Find(this.skin, this.info.iconSprite, GlobalMessages.OnItemIconChangedAction);
				if (sprite != null)
				{
					return sprite;
				}
			}
			return this.info.iconSprite;
		}
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x060019E7 RID: 6631 RVA: 0x00091668 File Offset: 0x0008F868
	internal bool isLoadingIconSprite
	{
		get
		{
			if (this.skin != 0UL)
			{
				if ((long)Enumerable.FirstOrDefault<ItemSkinDirectory.Skin>(this.info.skins, (ItemSkinDirectory.Skin x) => (long)x.id == (long)this.skin).id == (long)this.skin)
				{
					return false;
				}
				Skin skin = WorkshopSkin.GetSkin(this.skin);
				if (skin != null)
				{
					return !skin.IconLoaded;
				}
			}
			return false;
		}
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x060019E8 RID: 6632 RVA: 0x000157BD File Offset: 0x000139BD
	public BaseEntity.TraitFlag Traits
	{
		get
		{
			return this.info.Traits;
		}
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x000157CA File Offset: 0x000139CA
	public void UpdateAmountDisplay(Text text)
	{
		Item.UpdateAmountDisplay(text, this, 1, null);
	}

	// Token: 0x060019EA RID: 6634 RVA: 0x000916C4 File Offset: 0x0008F8C4
	public static void UpdateAmountDisplay(Text text, Item item, int amountOverride = 1, ItemDefinition infoOverride = null)
	{
		float num = (float)((item == null) ? amountOverride : item.amount);
		ItemDefinition itemDefinition = (item == null) ? infoOverride : item.info;
		if (item != null && item.amountTextOverride != null)
		{
			text.gameObject.SetActive(true);
			text.text = item.amountTextOverride;
			return;
		}
		if (item != null && itemDefinition.amountType == ItemDefinition.AmountType.OxygenSeconds)
		{
			text.gameObject.SetActive(true);
			text.text = string.Format("{0:N0}s", item.condition);
			return;
		}
		if (itemDefinition.amountType == ItemDefinition.AmountType.Millilitre)
		{
			text.gameObject.SetActive(true);
			text.text = string.Format("{0:N0}ml", num);
			return;
		}
		if (itemDefinition.amountType == ItemDefinition.AmountType.Feet)
		{
			text.gameObject.SetActive(true);
			text.text = string.Format("{0:N0}ft", num);
			return;
		}
		if (item != null && itemDefinition.amountType == ItemDefinition.AmountType.Genetics && item.instanceData != null)
		{
			text.gameObject.SetActive(true);
			text.text = string.Format("G-{0:G}", item.instanceData.dataInt);
			return;
		}
		if (item != null && itemDefinition.amountType == ItemDefinition.AmountType.Frequency)
		{
			int num2 = 0;
			if (item.instanceData != null)
			{
				if (item.instanceData.subEntity != 0U)
				{
					BaseNetworkable baseNetworkable = BaseNetworkable.clientEntities.Find(item.instanceData.subEntity);
					if (baseNetworkable)
					{
						IRFObject component = baseNetworkable.GetComponent<IRFObject>();
						if (component != null)
						{
							num2 = component.GetFrequency();
						}
					}
				}
				if (num2 == 0)
				{
					num2 = item.instanceData.dataInt;
				}
			}
			else
			{
				BaseEntity baseEntity = item.GetHeldEntity();
				if (baseEntity)
				{
					IRFObject component2 = baseEntity.GetComponent<IRFObject>();
					if (component2 != null)
					{
						num2 = component2.GetFrequency();
					}
				}
			}
			text.text = string.Format("{0:G}MHz", num2);
			text.gameObject.SetActive(true);
			return;
		}
		if (num > 1f)
		{
			text.gameObject.SetActive(true);
			text.text = string.Format("x{0:N0}", num);
			return;
		}
		if (item != null && item.contents != null && item.contents.capacity == 1 && item.contents.itemList.Count > 0)
		{
			text.gameObject.SetActive(true);
			item.contents.itemList[0].UpdateAmountDisplay(text);
			return;
		}
		text.gameObject.SetActive(false);
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x0009191C File Offset: 0x0008FB1C
	public virtual Item Save(bool bIncludeContainer = false, bool bIncludeOwners = true)
	{
		this.dirty = false;
		Item item = Pool.Get<Item>();
		item.UID = this.uid;
		item.itemid = this.info.itemid;
		item.slot = this.position;
		item.amount = this.amount;
		item.flags = (int)this.flags;
		item.removetime = this.removeTime;
		item.locktime = this.busyTime;
		item.instanceData = this.instanceData;
		item.worldEntity = this.worldEnt.uid;
		item.heldEntity = this.heldEntity.uid;
		item.skinid = this.skin;
		item.name = this.name;
		item.text = this.text;
		if (this.hasCondition)
		{
			item.conditionData = Pool.Get<Item.ConditionData>();
			item.conditionData.maxCondition = this._maxCondition;
			item.conditionData.condition = this._condition;
		}
		if (this.contents != null && bIncludeContainer)
		{
			item.contents = this.contents.Save();
		}
		return item;
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x00091A38 File Offset: 0x0008FC38
	public virtual void Load(Item load)
	{
		if (this.info == null || this.info.itemid != load.itemid)
		{
			this.info = ItemManager.FindItemDefinition(load.itemid);
		}
		this.uid = load.UID;
		this.name = load.name;
		this.text = load.text;
		this.amount = load.amount;
		this.position = load.slot;
		this.busyTime = load.locktime;
		this.removeTime = load.removetime;
		this.flags = (Item.Flag)load.flags;
		this.worldEnt.uid = load.worldEntity;
		this.heldEntity.uid = load.heldEntity;
		if (this.instanceData != null)
		{
			this.instanceData.ShouldPool = true;
			this.instanceData.ResetToPool();
			this.instanceData = null;
		}
		this.instanceData = load.instanceData;
		if (this.instanceData != null)
		{
			this.instanceData.ShouldPool = false;
		}
		this.skin = load.skinid;
		if (this.info == null || this.info.itemid != load.itemid)
		{
			this.info = ItemManager.FindItemDefinition(load.itemid);
		}
		if (this.info == null)
		{
			return;
		}
		this._condition = 0f;
		this._maxCondition = 0f;
		if (load.conditionData != null)
		{
			this._condition = load.conditionData.condition;
			this._maxCondition = load.conditionData.maxCondition;
		}
		else if (this.info.condition.enabled)
		{
			this._condition = this.info.condition.max;
			this._maxCondition = this.info.condition.max;
		}
		if (load.contents != null)
		{
			if (this.contents == null)
			{
				this.contents = new ItemContainer();
				if (!this.isServer)
				{
					this.contents.ClientInitialize(this, load.contents.slots);
				}
			}
			this.contents.Load(load.contents);
		}
	}

	// Token: 0x0200042E RID: 1070
	[Flags]
	public enum Flag
	{
		// Token: 0x04001673 RID: 5747
		None = 0,
		// Token: 0x04001674 RID: 5748
		Placeholder = 1,
		// Token: 0x04001675 RID: 5749
		IsOn = 2,
		// Token: 0x04001676 RID: 5750
		OnFire = 4,
		// Token: 0x04001677 RID: 5751
		IsLocked = 8,
		// Token: 0x04001678 RID: 5752
		Cooking = 16
	}
}
