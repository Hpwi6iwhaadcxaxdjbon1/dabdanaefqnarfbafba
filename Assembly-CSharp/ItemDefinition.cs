using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Steamworks;
using Rust;
using Rust.Workshop;
using UnityEngine;

// Token: 0x02000438 RID: 1080
public class ItemDefinition : MonoBehaviour
{
	// Token: 0x040016A6 RID: 5798
	[Header("Item")]
	[ReadOnly]
	public int itemid;

	// Token: 0x040016A7 RID: 5799
	[Tooltip("The shortname should be unique. A hash will be generated from it to identify the item type. If this name changes at any point it will make all saves incompatible")]
	public string shortname;

	// Token: 0x040016A8 RID: 5800
	[Header("Appearance")]
	public Translate.Phrase displayName;

	// Token: 0x040016A9 RID: 5801
	public Translate.Phrase displayDescription;

	// Token: 0x040016AA RID: 5802
	public Sprite iconSprite;

	// Token: 0x040016AB RID: 5803
	public ItemCategory category;

	// Token: 0x040016AC RID: 5804
	public ItemSelectionPanel selectionPanel;

	// Token: 0x040016AD RID: 5805
	[Header("Containment")]
	public int maxDraggable;

	// Token: 0x040016AE RID: 5806
	public ItemContainer.ContentsType itemType = ItemContainer.ContentsType.Generic;

	// Token: 0x040016AF RID: 5807
	public ItemDefinition.AmountType amountType;

	// Token: 0x040016B0 RID: 5808
	[InspectorFlags]
	public ItemSlot occupySlots = ItemSlot.None;

	// Token: 0x040016B1 RID: 5809
	public int stackable;

	// Token: 0x040016B2 RID: 5810
	public bool quickDespawn;

	// Token: 0x040016B3 RID: 5811
	[Header("Spawn Tables")]
	public Rarity rarity;

	// Token: 0x040016B4 RID: 5812
	public bool spawnAsBlueprint;

	// Token: 0x040016B5 RID: 5813
	[Header("Sounds")]
	public SoundDefinition inventorySelectSound;

	// Token: 0x040016B6 RID: 5814
	public SoundDefinition inventoryGrabSound;

	// Token: 0x040016B7 RID: 5815
	public SoundDefinition inventoryDropSound;

	// Token: 0x040016B8 RID: 5816
	public SoundDefinition physImpactSoundDef;

	// Token: 0x040016B9 RID: 5817
	public ItemDefinition.Condition condition;

	// Token: 0x040016BA RID: 5818
	[Header("Misc")]
	public bool hidden;

	// Token: 0x040016BB RID: 5819
	[InspectorFlags]
	public ItemDefinition.Flag flags;

	// Token: 0x040016BC RID: 5820
	[Tooltip("User can craft this item on any server if they have this steam item")]
	public SteamInventoryItem steamItem;

	// Token: 0x040016BD RID: 5821
	[Tooltip("Can only craft this item if the parent is craftable (tech tree)")]
	public ItemDefinition Parent;

	// Token: 0x040016BE RID: 5822
	public GameObjectRef worldModelPrefab;

	// Token: 0x040016BF RID: 5823
	[NonSerialized]
	public ItemMod[] itemMods;

	// Token: 0x040016C0 RID: 5824
	public BaseEntity.TraitFlag Traits;

	// Token: 0x040016C1 RID: 5825
	[NonSerialized]
	public ItemSkinDirectory.Skin[] skins;

	// Token: 0x040016C2 RID: 5826
	[NonSerialized]
	private Inventory.Definition[] _skins2;

	// Token: 0x040016C3 RID: 5827
	[Tooltip("Panel to show in the inventory menu when selected")]
	public GameObject panel;

	// Token: 0x040016C8 RID: 5832
	[NonSerialized]
	public ItemDefinition[] Children = new ItemDefinition[0];

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06001A15 RID: 6677 RVA: 0x000926C0 File Offset: 0x000908C0
	public Inventory.Definition[] skins2
	{
		get
		{
			if (this._skins2 != null)
			{
				return this._skins2;
			}
			if (Global.SteamClient != null && Global.SteamClient.Inventory.Definitions != null)
			{
				string prefabname = base.name;
				this._skins2 = Enumerable.ToArray<Inventory.Definition>(Enumerable.Where<Inventory.Definition>(Global.SteamClient.Inventory.Definitions, (Inventory.Definition x) => (x.GetStringProperty("itemshortname") == this.shortname || x.GetStringProperty("itemshortname") == prefabname) && !string.IsNullOrEmpty(x.GetStringProperty("workshopdownload"))));
			}
			return this._skins2;
		}
	}

	// Token: 0x06001A16 RID: 6678 RVA: 0x00015975 File Offset: 0x00013B75
	public void InvalidateWorkshopSkinCache()
	{
		this._skins2 = null;
	}

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06001A17 RID: 6679 RVA: 0x0001597E File Offset: 0x00013B7E
	public ItemBlueprint Blueprint
	{
		get
		{
			return base.GetComponent<ItemBlueprint>();
		}
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06001A18 RID: 6680 RVA: 0x00015986 File Offset: 0x00013B86
	public int craftingStackable
	{
		get
		{
			return Mathf.Max(10, this.stackable);
		}
	}

	// Token: 0x06001A19 RID: 6681 RVA: 0x00015995 File Offset: 0x00013B95
	public bool HasFlag(ItemDefinition.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06001A1A RID: 6682 RVA: 0x00092740 File Offset: 0x00090940
	public void Initialize(List<ItemDefinition> itemList)
	{
		if (this.itemMods != null)
		{
			Debug.LogError("Item Definition Initializing twice: " + base.name);
		}
		this.skins = ItemSkinDirectory.ForItem(this);
		this.itemMods = base.GetComponentsInChildren<ItemMod>(true);
		ItemMod[] array = this.itemMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ModInit();
		}
		this.Children = Enumerable.ToArray<ItemDefinition>(Enumerable.Where<ItemDefinition>(itemList, (ItemDefinition x) => x.Parent == this));
		this.ItemModWearable = base.GetComponent<ItemModWearable>();
		this.isHoldable = (base.GetComponent<ItemModEntity>() != null);
		this.isUsable = (base.GetComponent<ItemModEntity>() != null || base.GetComponent<ItemModConsume>() != null);
	}

	// Token: 0x06001A1B RID: 6683 RVA: 0x00092800 File Offset: 0x00090A00
	public virtual string GetDisplayName(Item item)
	{
		if (!string.IsNullOrEmpty(item.name))
		{
			return item.name;
		}
		if (item.IsBlueprint())
		{
			return item.blueprintTargetDef.displayName.translated + " " + this.displayName.translated;
		}
		return this.displayName.translated;
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x000159A2 File Offset: 0x00013BA2
	public virtual string GetDescriptionText(Item item)
	{
		if (item.IsBlueprint())
		{
			return item.blueprintTargetDef.Blueprint.GetIngredientString(true);
		}
		return this.displayDescription.translated;
	}

	// Token: 0x06001A1D RID: 6685 RVA: 0x0009285C File Offset: 0x00090A5C
	public Sprite FindIconSprite(int skinid)
	{
		if (this.HasSkins && skinid > 0)
		{
			if (this.skins != null)
			{
				for (int i = 0; i < this.skins.Length; i++)
				{
					if (this.skins[i].id == skinid)
					{
						ItemSkin itemSkin = FileSystem.Load<ItemSkin>(this.skins[i].name, false);
						if (!(itemSkin == null))
						{
							return itemSkin.icon;
						}
					}
				}
			}
			if (this.skins2 != null)
			{
				for (int j = 0; j < this.skins2.Length; j++)
				{
					if (this.skins2[j].Id == skinid)
					{
						ulong property = this.skins2[j].GetProperty<ulong>("workshopdownload");
						if (property > 0UL)
						{
							return WorkshopIconLoader.Find(property, this.iconSprite, GlobalMessages.OnItemIconChangedAction);
						}
					}
				}
			}
		}
		return this.iconSprite;
	}

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06001A1E RID: 6686 RVA: 0x000159C9 File Offset: 0x00013BC9
	public bool isWearable
	{
		get
		{
			return this.ItemModWearable != null;
		}
	}

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06001A1F RID: 6687 RVA: 0x000159D7 File Offset: 0x00013BD7
	// (set) Token: 0x06001A20 RID: 6688 RVA: 0x000159DF File Offset: 0x00013BDF
	public ItemModWearable ItemModWearable { get; private set; }

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06001A21 RID: 6689 RVA: 0x000159E8 File Offset: 0x00013BE8
	// (set) Token: 0x06001A22 RID: 6690 RVA: 0x000159F0 File Offset: 0x00013BF0
	public bool isHoldable { get; private set; }

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06001A23 RID: 6691 RVA: 0x000159F9 File Offset: 0x00013BF9
	// (set) Token: 0x06001A24 RID: 6692 RVA: 0x00015A01 File Offset: 0x00013C01
	public bool isUsable { get; private set; }

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06001A25 RID: 6693 RVA: 0x00015A0A File Offset: 0x00013C0A
	public bool HasSkins
	{
		get
		{
			return (this.skins2 != null && this.skins2.Length != 0) || (this.skins != null && this.skins.Length != 0);
		}
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06001A26 RID: 6694 RVA: 0x00015A33 File Offset: 0x00013C33
	// (set) Token: 0x06001A27 RID: 6695 RVA: 0x00015A3B File Offset: 0x00013C3B
	public bool CraftableWithSkin { get; private set; }

	// Token: 0x02000439 RID: 1081
	[Serializable]
	public struct Condition
	{
		// Token: 0x040016C9 RID: 5833
		public bool enabled;

		// Token: 0x040016CA RID: 5834
		[Tooltip("The maximum condition this item type can have, new items will start with this value")]
		public float max;

		// Token: 0x040016CB RID: 5835
		[Tooltip("If false then item will destroy when condition reaches 0")]
		public bool repairable;

		// Token: 0x040016CC RID: 5836
		[Tooltip("If true, never lose max condition when repaired")]
		public bool maintainMaxCondition;

		// Token: 0x040016CD RID: 5837
		public ItemDefinition.Condition.WorldSpawnCondition foundCondition;

		// Token: 0x0200043A RID: 1082
		[Serializable]
		public class WorldSpawnCondition
		{
			// Token: 0x040016CE RID: 5838
			public float fractionMin = 1f;

			// Token: 0x040016CF RID: 5839
			public float fractionMax = 1f;
		}
	}

	// Token: 0x0200043B RID: 1083
	[Flags]
	public enum Flag
	{
		// Token: 0x040016D1 RID: 5841
		NoDropping = 1,
		// Token: 0x040016D2 RID: 5842
		NotStraightToBelt = 2
	}

	// Token: 0x0200043C RID: 1084
	public enum AmountType
	{
		// Token: 0x040016D4 RID: 5844
		Count,
		// Token: 0x040016D5 RID: 5845
		Millilitre,
		// Token: 0x040016D6 RID: 5846
		Feet,
		// Token: 0x040016D7 RID: 5847
		Genetics,
		// Token: 0x040016D8 RID: 5848
		OxygenSeconds,
		// Token: 0x040016D9 RID: 5849
		Frequency
	}
}
