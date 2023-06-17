using System;
using Facepunch.Steamworks;

namespace Facepunch.Rust
{
	// Token: 0x020008A8 RID: 2216
	public static class Analytics
	{
		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06002FD3 RID: 12243 RVA: 0x00024B33 File Offset: 0x00022D33
		// (set) Token: 0x06002FD4 RID: 12244 RVA: 0x00024B49 File Offset: 0x00022D49
		public static int InventoryOpened
		{
			get
			{
				return Client.Instance.Stats.GetInt("INVENTORY_OPENED");
			}
			set
			{
				Client.Instance.Stats.Set("INVENTORY_OPENED", value, false);
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06002FD5 RID: 12245 RVA: 0x00024B62 File Offset: 0x00022D62
		// (set) Token: 0x06002FD6 RID: 12246 RVA: 0x00024B78 File Offset: 0x00022D78
		public static int CraftingOpened
		{
			get
			{
				return Client.Instance.Stats.GetInt("CRAFTING_OPENED");
			}
			set
			{
				Client.Instance.Stats.Set("CRAFTING_OPENED", value, false);
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06002FD7 RID: 12247 RVA: 0x00024B91 File Offset: 0x00022D91
		// (set) Token: 0x06002FD8 RID: 12248 RVA: 0x00024BA7 File Offset: 0x00022DA7
		public static int MapOpened
		{
			get
			{
				return Client.Instance.Stats.GetInt("MAP_OPENED");
			}
			set
			{
				Client.Instance.Stats.Set("MAP_OPENED", value, false);
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06002FD9 RID: 12249 RVA: 0x00024BC0 File Offset: 0x00022DC0
		// (set) Token: 0x06002FDA RID: 12250 RVA: 0x00024BD6 File Offset: 0x00022DD6
		public static int TimesExamined
		{
			get
			{
				return Client.Instance.Stats.GetInt("ITEM_EXAMINED");
			}
			set
			{
				Client.Instance.Stats.Set("ITEM_EXAMINED", value, false);
			}
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06002FDB RID: 12251 RVA: 0x00024BEF File Offset: 0x00022DEF
		// (set) Token: 0x06002FDC RID: 12252 RVA: 0x00024C05 File Offset: 0x00022E05
		public static int CupboardOpened
		{
			get
			{
				return Client.Instance.Stats.GetInt("CUPBOARD_OPENED");
			}
			set
			{
				Client.Instance.Stats.Set("CUPBOARD_OPENED", value, false);
			}
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06002FDD RID: 12253 RVA: 0x00024C1E File Offset: 0x00022E1E
		// (set) Token: 0x06002FDE RID: 12254 RVA: 0x00024C34 File Offset: 0x00022E34
		public static int OreHit
		{
			get
			{
				return Client.Instance.Stats.GetInt("ORE_HIT");
			}
			set
			{
				Client.Instance.Stats.Set("ORE_HIT", value, true);
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06002FDF RID: 12255 RVA: 0x00024C4D File Offset: 0x00022E4D
		// (set) Token: 0x06002FE0 RID: 12256 RVA: 0x00024C63 File Offset: 0x00022E63
		public static int TreeHit
		{
			get
			{
				return Client.Instance.Stats.GetInt("TREE_HIT");
			}
			set
			{
				Client.Instance.Stats.Set("TREE_HIT", value, true);
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06002FE1 RID: 12257 RVA: 0x00024C7C File Offset: 0x00022E7C
		// (set) Token: 0x06002FE2 RID: 12258 RVA: 0x00024C92 File Offset: 0x00022E92
		public static float ComfortDuration
		{
			get
			{
				return Client.Instance.Stats.GetFloat("comfort_duration");
			}
			set
			{
				Client.Instance.Stats.Set("comfort_duration", value, false);
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06002FE3 RID: 12259 RVA: 0x00024CAB File Offset: 0x00022EAB
		// (set) Token: 0x06002FE4 RID: 12260 RVA: 0x00024CC1 File Offset: 0x00022EC1
		public static float RadiationExposureDuration
		{
			get
			{
				return Client.Instance.Stats.GetFloat("radiation_exposure_duration");
			}
			set
			{
				Client.Instance.Stats.Set("radiation_exposure_duration", value, false);
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06002FE5 RID: 12261 RVA: 0x00024CDA File Offset: 0x00022EDA
		// (set) Token: 0x06002FE6 RID: 12262 RVA: 0x00024CF0 File Offset: 0x00022EF0
		public static float ColdExposureDuration
		{
			get
			{
				return Client.Instance.Stats.GetFloat("cold_exposure_duration");
			}
			set
			{
				Client.Instance.Stats.Set("cold_exposure_duration", value, false);
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06002FE7 RID: 12263 RVA: 0x00024D09 File Offset: 0x00022F09
		// (set) Token: 0x06002FE8 RID: 12264 RVA: 0x00024D1F File Offset: 0x00022F1F
		public static float HotExposureDuration
		{
			get
			{
				return Client.Instance.Stats.GetFloat("hot_exposure_duration");
			}
			set
			{
				Client.Instance.Stats.Set("hot_exposure_duration", value, false);
			}
		}

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06002FE9 RID: 12265 RVA: 0x00024D38 File Offset: 0x00022F38
		// (set) Token: 0x06002FEA RID: 12266 RVA: 0x00024D4E File Offset: 0x00022F4E
		public static float ConsumedFood
		{
			get
			{
				return Client.Instance.Stats.GetFloat("calories_consumed");
			}
			set
			{
				Client.Instance.Stats.Set("calories_consumed", value, false);
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06002FEB RID: 12267 RVA: 0x00024D67 File Offset: 0x00022F67
		// (set) Token: 0x06002FEC RID: 12268 RVA: 0x00024D7D File Offset: 0x00022F7D
		public static float ConsumedWater
		{
			get
			{
				return Client.Instance.Stats.GetFloat("water_consumed");
			}
			set
			{
				Client.Instance.Stats.Set("water_consumed", value, false);
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06002FED RID: 12269 RVA: 0x00024D96 File Offset: 0x00022F96
		// (set) Token: 0x06002FEE RID: 12270 RVA: 0x00024DAC File Offset: 0x00022FAC
		public static int MeleeStrikes
		{
			get
			{
				return Client.Instance.Stats.GetInt("melee_strikes");
			}
			set
			{
				Client.Instance.Stats.Set("melee_strikes", value, false);
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06002FEF RID: 12271 RVA: 0x00024DC5 File Offset: 0x00022FC5
		// (set) Token: 0x06002FF0 RID: 12272 RVA: 0x00024DDB File Offset: 0x00022FDB
		public static int MeleeThrows
		{
			get
			{
				return Client.Instance.Stats.GetInt("melee_thrown");
			}
			set
			{
				Client.Instance.Stats.Set("melee_thrown", value, false);
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06002FF1 RID: 12273 RVA: 0x00024DF4 File Offset: 0x00022FF4
		// (set) Token: 0x06002FF2 RID: 12274 RVA: 0x00024E0A File Offset: 0x0002300A
		public static int ShotArrows
		{
			get
			{
				return Client.Instance.Stats.GetInt("arrows_shot");
			}
			set
			{
				Client.Instance.Stats.Set("arrows_shot", value, false);
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06002FF3 RID: 12275 RVA: 0x00024E23 File Offset: 0x00023023
		// (set) Token: 0x06002FF4 RID: 12276 RVA: 0x00024E39 File Offset: 0x00023039
		public static int PlacedBlocks
		{
			get
			{
				return Client.Instance.Stats.GetInt("placed_blocks");
			}
			set
			{
				Client.Instance.Stats.Set("placed_blocks", value, true);
			}
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06002FF5 RID: 12277 RVA: 0x00024E52 File Offset: 0x00023052
		// (set) Token: 0x06002FF6 RID: 12278 RVA: 0x00024E68 File Offset: 0x00023068
		public static int UpgradedBlocks
		{
			get
			{
				return Client.Instance.Stats.GetInt("upgraded_blocks");
			}
			set
			{
				Client.Instance.Stats.Set("upgraded_blocks", value, true);
			}
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06002FF7 RID: 12279 RVA: 0x00024E81 File Offset: 0x00023081
		// (set) Token: 0x06002FF8 RID: 12280 RVA: 0x00024E97 File Offset: 0x00023097
		public static float SecondsSpeaking
		{
			get
			{
				return Client.Instance.Stats.GetFloat("seconds_speaking");
			}
			set
			{
				Client.Instance.Stats.Set("seconds_speaking", value, false);
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06002FF9 RID: 12281 RVA: 0x00024EB0 File Offset: 0x000230B0
		// (set) Token: 0x06002FFA RID: 12282 RVA: 0x00024EC6 File Offset: 0x000230C6
		public static float TimeOnRoad
		{
			get
			{
				return Client.Instance.Stats.GetFloat("topology_road_duration");
			}
			set
			{
				Client.Instance.Stats.Set("topology_road_duration", value, false);
			}
		}

		// Token: 0x06002FFB RID: 12283 RVA: 0x00024EDF File Offset: 0x000230DF
		public static void IncrementLootOpened(string panelType)
		{
			if (panelType == "toolcupboard")
			{
				Analytics.CupboardOpened++;
			}
		}
	}
}
