﻿using System;
using System.Linq;
using Facepunch.Steamworks;

// Token: 0x020005FE RID: 1534
public class AchievementGroup
{
	// Token: 0x04001EC8 RID: 7880
	public Translate.Phrase groupTitle = new Translate.Phrase("", "");

	// Token: 0x04001EC9 RID: 7881
	public static AchievementGroup[] All = new AchievementGroup[]
	{
		new AchievementGroup("list_getting_started", "Getting Started")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("COLLECT_100_WOOD", "Harvest 100 Wood"),
				new AchievementGroup.AchievementItem("CRAFT_CAMPFIRE", "Craft a camp fire"),
				new AchievementGroup.AchievementItem("PLACE_CAMPFIRE", "Place a camp fire")
			}
		},
		new AchievementGroup("list_tools_weaps", "Craft Tools & Weapons")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("COLLECT_700_WOOD", "Harvest 700 Wood"),
				new AchievementGroup.AchievementItem("COLLECT_200_STONE", "Harvest 200 Stone"),
				new AchievementGroup.AchievementItem("CRAFT_STONE_HATCHET", "Craft a Stone Hatchet"),
				new AchievementGroup.AchievementItem("CRAFT_STONE_PICKAXE", "Craft a Stone Pickaxe"),
				new AchievementGroup.AchievementItem("CRAFT_SPEAR", "Craft a Wooden Spear")
			}
		},
		new AchievementGroup("list_respawn_point", "Create a respawn point")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("COLLECT_30_CLOTH", "Collect 30 Cloth"),
				new AchievementGroup.AchievementItem("CRAFT_SLEEPINGBAG", "Craft a sleeping bag"),
				new AchievementGroup.AchievementItem("PLACE_SLEEPINGBAG", "Place a sleeping bag")
			}
		},
		new AchievementGroup("list_base_building", "Build a Base")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("CRAFT_BUILDING_PLAN", "Craft a Building Plan"),
				new AchievementGroup.AchievementItem("CRAFT_HAMMER", "Craft a hammer"),
				new AchievementGroup.AchievementItem("CONSTRUCT_BASE", "Construct a Base"),
				new AchievementGroup.AchievementItem("UPGRADE_BASE", "Upgrade your base")
			}
		},
		new AchievementGroup("list_secure_base", "Secure your Base")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("CRAFT_WOODEN_DOOR", "Craft a Wooden Door"),
				new AchievementGroup.AchievementItem("CRAFT_LOCK", "Craft a lock"),
				new AchievementGroup.AchievementItem("PLACE_WOODEN_DOOR", "Place Wooden Door"),
				new AchievementGroup.AchievementItem("PLACE_LOCK", "Place lock on Door"),
				new AchievementGroup.AchievementItem("LOCK_LOCK", "Lock the Lock")
			}
		},
		new AchievementGroup("list_create_storage", "Create Storage")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("CRAFT_WOODEN_BOX", "Craft a Wooden Box"),
				new AchievementGroup.AchievementItem("PLACE_WOODEN_BOX", "Place Wooden Box in Base")
			}
		},
		new AchievementGroup("list_craft_toolcupboard", "Claim an Area")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("CRAFT_TOOL_CUPBOARD", "Craft a Tool Cupboard"),
				new AchievementGroup.AchievementItem("PLACE_TOOL_CUPBOARD", "Place tool cupboard in base")
			}
		},
		new AchievementGroup("list_hunt", "Going Hunting")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("COLLECT_50_CLOTH", "Gather 50 Cloth"),
				new AchievementGroup.AchievementItem("CRAFT_HUNTING_BOW", "Craft a Hunting Bow"),
				new AchievementGroup.AchievementItem("CRAFT_ARROWS", "Craft some Arrows"),
				new AchievementGroup.AchievementItem("KILL_ANIMAL", "Kill an Animal"),
				new AchievementGroup.AchievementItem("SKIN_ANIMAL", "Harvest an Animal")
			}
		},
		new AchievementGroup("list_gear_up", "Craft & Equip Clothing")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("CRAFT_BURLAP_HEADWRAP", "Craft a Burlap Headwrap"),
				new AchievementGroup.AchievementItem("CRAFT_BURLAP_SHIRT", "Craft a Burlap Shirt"),
				new AchievementGroup.AchievementItem("CRAFT_BURLAP_PANTS", "Craft Burlap Pants"),
				new AchievementGroup.AchievementItem("EQUIP_CLOTHING", "Equip Clothing")
			}
		},
		new AchievementGroup("list_furnace", "Create a Furnace")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("COLLECT_50_LGF", "Collect or Craft 50 Low Grade Fuel"),
				new AchievementGroup.AchievementItem("CRAFT_FURNACE", "Craft a Furnace"),
				new AchievementGroup.AchievementItem("PLACE_FURNACE", "Place a Furnace")
			}
		},
		new AchievementGroup("list_machete", "Craft a Metal Weapon")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("COLLECT_300_METAL_ORE", "Collect 300 Metal Ore"),
				new AchievementGroup.AchievementItem("CRAFT_MACHETE", "Craft a Machete")
			}
		},
		new AchievementGroup("list_explore_1", "Exploring")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("VISIT_ROAD", "Visit a Road"),
				new AchievementGroup.AchievementItem("DESTROY_10_BARRELS", "Destroy 10 Barrels"),
				new AchievementGroup.AchievementItem("COLLECT_65_SCRAP", "Collect 65 Scrap")
			}
		},
		new AchievementGroup("list_workbench", "Workbenches")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("CRAFT_WORKBENCH", "Craft a Workbench"),
				new AchievementGroup.AchievementItem("PLACE_WORKBENCH", "Place Workbench in base"),
				new AchievementGroup.AchievementItem("CRAFT_NAILGUN", "Craft a Nailgun"),
				new AchievementGroup.AchievementItem("CRAFT_NAILGUN_NAILS", "Craft Nailgun Nails")
			}
		},
		new AchievementGroup("list_research", "Researching")
		{
			Items = new AchievementGroup.AchievementItem[]
			{
				new AchievementGroup.AchievementItem("CRAFT_RESEARCH_TABLE", "Craft a Research Table"),
				new AchievementGroup.AchievementItem("PLACE_RESEARCH_TABLE", "Place Research Table in base"),
				new AchievementGroup.AchievementItem("RESEARCH_ITEM", "Research an Item")
			}
		}
	};

	// Token: 0x04001ECA RID: 7882
	public AchievementGroup.AchievementItem[] Items;

	// Token: 0x06002270 RID: 8816 RVA: 0x0001B5CE File Offset: 0x000197CE
	public AchievementGroup(string token = "", string english = "")
	{
		this.groupTitle.token = token;
		this.groupTitle.english = english;
	}

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x06002271 RID: 8817 RVA: 0x0001B603 File Offset: 0x00019803
	public bool Unlocked
	{
		get
		{
			return Enumerable.All<AchievementGroup.AchievementItem>(this.Items, (AchievementGroup.AchievementItem x) => x.Unlocked);
		}
	}

	// Token: 0x020005FF RID: 1535
	public class AchievementItem
	{
		// Token: 0x04001ECB RID: 7883
		public string Name;

		// Token: 0x04001ECC RID: 7884
		public Translate.Phrase Phrase;

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06002273 RID: 8819 RVA: 0x0001B62F File Offset: 0x0001982F
		public Achievement Achievement
		{
			get
			{
				if (Client.Instance != null)
				{
					return Client.Instance.Achievements.Find(this.Name);
				}
				return null;
			}
		}

		// Token: 0x06002274 RID: 8820 RVA: 0x0001B64F File Offset: 0x0001984F
		public AchievementItem(string name, string phrase)
		{
			this.Name = name;
			this.Phrase = new Translate.Phrase(("achievement_" + name).ToLower(), phrase);
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06002275 RID: 8821 RVA: 0x0001B67A File Offset: 0x0001987A
		public bool Unlocked
		{
			get
			{
				return this.Achievement != null && this.Achievement.State;
			}
		}
	}
}
