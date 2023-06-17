using System;
using Facepunch.Steamworks;
using Rust;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000873 RID: 2163
	[ConsoleSystem.Factory("note")]
	public class Note : ConsoleSystem
	{
		// Token: 0x06002F1F RID: 12063 RVA: 0x000E936C File Offset: 0x000E756C
		[ClientVar(AllowRunFromServer = true)]
		public static void craft_add(ConsoleSystem.Arg args)
		{
			int @int = args.GetInt(0, 0);
			int int2 = args.GetInt(1, 0);
			int int3 = args.GetInt(2, 0);
			int int4 = args.GetInt(3, 0);
			CraftingQueue.TaskAdd(@int, int2, int3, int4);
		}

		// Token: 0x06002F20 RID: 12064 RVA: 0x000E93A4 File Offset: 0x000E75A4
		[ClientVar(AllowRunFromServer = true)]
		public static void craft_start(ConsoleSystem.Arg args)
		{
			int @int = args.GetInt(0, 0);
			float @float = args.GetFloat(1, 0f);
			CraftingQueue.TaskStarted(@int, Time.realtimeSinceStartup + @float);
		}

		// Token: 0x06002F21 RID: 12065 RVA: 0x000E93D4 File Offset: 0x000E75D4
		[ClientVar(AllowRunFromServer = true)]
		public static void craft_done(ConsoleSystem.Arg args)
		{
			int @int = args.GetInt(0, 0);
			bool success = args.GetInt(1, 0) == 1;
			int int2 = args.GetInt(2, 0);
			CraftingQueue.TaskFinished(@int, success, int2);
		}

		// Token: 0x06002F22 RID: 12066 RVA: 0x000E9408 File Offset: 0x000E7608
		[ClientVar(AllowRunFromServer = true)]
		public static void inv(ConsoleSystem.Arg args)
		{
			int @int = args.GetInt(0, 0);
			int int2 = args.GetInt(1, 0);
			string @string = args.GetString(2, null);
			BaseEntity.GiveItemReason int3 = (BaseEntity.GiveItemReason)args.GetInt(3, 0);
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(@int);
			if (itemDefinition != null)
			{
				NoticeArea.ItemPickUp(itemDefinition, int2, @string);
				if (int3 == BaseEntity.GiveItemReason.ResourceHarvested && GameInfo.HasAchievements)
				{
					Facepunch.Steamworks.Client.Instance.Stats.Add(string.Format("harvested_{0}", itemDefinition.shortname), int2, true);
				}
				if (int3 == BaseEntity.GiveItemReason.PickedUp && GameInfo.HasAchievements)
				{
					Facepunch.Steamworks.Client.Instance.Stats.Add(string.Format("pickup_category_{0}", itemDefinition.category), int2, false);
				}
				Facepunch.Steamworks.Client.Instance.Stats.Add(string.Format("acquired_{0}", itemDefinition.shortname), int2, true);
			}
		}
	}
}
