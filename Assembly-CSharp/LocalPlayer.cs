using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch.Steamworks;
using Rust;
using UnityEngine;

// Token: 0x0200031A RID: 794
public static class LocalPlayer
{
	// Token: 0x0400120F RID: 4623
	public static float LastDeathTime = float.NegativeInfinity;

	// Token: 0x04001210 RID: 4624
	private static Dictionary<int, int> CraftCounts = new Dictionary<int, int>();

	// Token: 0x04001211 RID: 4625
	public static LocalPlayer.ItemBlueprintDescendingComparer ItemBlueprintDescendingOrder = new LocalPlayer.ItemBlueprintDescendingComparer();

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x0600151A RID: 5402 RVA: 0x00011E9E File Offset: 0x0001009E
	// (set) Token: 0x0600151B RID: 5403 RVA: 0x00011EA5 File Offset: 0x000100A5
	public static BasePlayer Entity { get; set; }

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x0600151C RID: 5404 RVA: 0x00011EAD File Offset: 0x000100AD
	// (set) Token: 0x0600151D RID: 5405 RVA: 0x00011EB4 File Offset: 0x000100B4
	public static ulong LastAttackerSteamID { get; set; }

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x0600151E RID: 5406 RVA: 0x00011EBC File Offset: 0x000100BC
	// (set) Token: 0x0600151F RID: 5407 RVA: 0x00011EC3 File Offset: 0x000100C3
	public static string LastAttackerName { get; set; }

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06001520 RID: 5408 RVA: 0x00011ECB File Offset: 0x000100CB
	public static float TimeSinceLastDeath
	{
		get
		{
			return UnityEngine.Time.realtimeSinceStartup - LocalPlayer.LastDeathTime;
		}
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x00082950 File Offset: 0x00080B50
	public static void EndLooting()
	{
		if (!LocalPlayer.Entity.IsValid())
		{
			return;
		}
		if (!LocalPlayer.Entity.inventory.loot.IsLooting())
		{
			return;
		}
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.endloot", Array.Empty<object>());
		LocalPlayer.Entity.inventory.loot.Clear();
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x00011ED8 File Offset: 0x000100D8
	internal static ItemContainer GetContainer(PlayerInventory.Type type)
	{
		if (!LocalPlayer.Entity.IsValid())
		{
			return null;
		}
		return LocalPlayer.Entity.inventory.GetContainer(type);
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x000829AC File Offset: 0x00080BAC
	internal static ItemContainer GetLootContainer(int i)
	{
		if (!LocalPlayer.Entity.IsValid())
		{
			return null;
		}
		if (!LocalPlayer.Entity.inventory.loot.IsLooting())
		{
			return null;
		}
		if (LocalPlayer.Entity.inventory.loot.containers.Count <= i)
		{
			return null;
		}
		return LocalPlayer.Entity.inventory.loot.containers[i];
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x00011EF8 File Offset: 0x000100F8
	public static void OnInventoryChanged()
	{
		if (LocalPlayer.Entity.IsValid() && LocalPlayer.Entity.IsDeveloper && !Debugging.oninventorychanged)
		{
			return;
		}
		GlobalMessages.OnInventoryChanged();
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x00011F1F File Offset: 0x0001011F
	public static void OnItemAmountChanged()
	{
		GlobalMessages.OnItemAmountChanged();
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x00011F26 File Offset: 0x00010126
	internal static bool HasUnlocked(ItemDefinition targetItem)
	{
		return LocalPlayer.Entity.IsValid() && !(targetItem.Blueprint == null) && LocalPlayer.Entity.blueprints.HasUnlocked(targetItem);
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x00082A18 File Offset: 0x00080C18
	internal static float GetCraftLevel()
	{
		if (LocalPlayer.Entity == null)
		{
			return 0f;
		}
		if (BasePlayer.craftMode != 0)
		{
			return 5f;
		}
		return (float)(LocalPlayer.Entity.HasPlayerFlag(BasePlayer.PlayerFlags.Workbench3) ? 3 : (LocalPlayer.Entity.HasPlayerFlag(BasePlayer.PlayerFlags.Workbench2) ? 2 : (LocalPlayer.Entity.HasPlayerFlag(BasePlayer.PlayerFlags.Workbench1) ? 1 : 0)));
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x00011F56 File Offset: 0x00010156
	internal static bool HasCraftLevel(int levelReq)
	{
		return LocalPlayer.GetCraftLevel() >= (float)levelReq;
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x00011F64 File Offset: 0x00010164
	public static void MoveItem(uint itemid, uint targetContainer, int targetSlot, int amount)
	{
		if (!LocalPlayer.Entity.IsValid())
		{
			return;
		}
		LocalPlayer.Entity.ServerRPC<uint, uint, sbyte, ushort>("MoveItem", itemid, targetContainer, (sbyte)targetSlot, (ushort)amount);
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x00011F88 File Offset: 0x00010188
	public static void ItemCommand(uint itemid, string command)
	{
		if (!LocalPlayer.Entity.IsValid())
		{
			return;
		}
		if (string.IsNullOrEmpty(command))
		{
			Debug.LogError("ItemCommand: command is null or empty!");
		}
		LocalPlayer.Entity.ServerRPC<uint, string>("ItemCmd", itemid, command);
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x00011FBA File Offset: 0x000101BA
	public static void ItemCommandEx<T1>(uint itemid, string command, T1 arg1)
	{
		if (!LocalPlayer.Entity.IsValid())
		{
			return;
		}
		if (string.IsNullOrEmpty(command))
		{
			Debug.LogError("ItemCommand: command is null or empty!");
		}
		LocalPlayer.Entity.ServerRPC<uint, string, T1>("ItemCmd", itemid, command, arg1);
	}

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x0600152C RID: 5420 RVA: 0x00011FED File Offset: 0x000101ED
	public static bool isSleeping
	{
		get
		{
			return LocalPlayer.Entity.IsValid() && LocalPlayer.Entity.IsSleeping();
		}
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x0600152D RID: 5421 RVA: 0x00012007 File Offset: 0x00010207
	public static bool isAdmin
	{
		get
		{
			return LocalPlayer.Entity.IsValid() && LocalPlayer.Entity.IsAdmin;
		}
	}

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x0600152E RID: 5422 RVA: 0x00012021 File Offset: 0x00010221
	public static bool isDeveloper
	{
		get
		{
			return LocalPlayer.Entity.IsValid() && LocalPlayer.Entity.IsDeveloper;
		}
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x0001203B File Offset: 0x0001023B
	public static void FindAmmo(List<Item> list, AmmoTypes ammo)
	{
		if (!LocalPlayer.Entity)
		{
			return;
		}
		LocalPlayer.Entity.inventory.FindAmmo(list, ammo);
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x0001205B File Offset: 0x0001025B
	public static void ModifyCamera()
	{
		if (!LocalPlayer.Entity)
		{
			return;
		}
		LocalPlayer.Entity.ModifyCamera();
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x00082A84 File Offset: 0x00080C84
	public static bool HasItems(List<ItemAmount> list, int amount = 1)
	{
		if (!LocalPlayer.Entity)
		{
			return false;
		}
		foreach (ItemAmount itemAmount in list)
		{
			if ((float)LocalPlayer.GetItemAmount(itemAmount.itemDef) < itemAmount.amount * (float)amount)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x00082AF8 File Offset: 0x00080CF8
	public static int GetItemAmount(ItemDefinition item)
	{
		if (item == null)
		{
			return 0;
		}
		if (LocalPlayer.Entity == null)
		{
			return 0;
		}
		if (LocalPlayer.Entity.inventory == null)
		{
			return 0;
		}
		return LocalPlayer.Entity.inventory.GetAmount(item.itemid);
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x00082B48 File Offset: 0x00080D48
	public static string BuildItemRequiredString(List<ItemAmount> list)
	{
		if (!LocalPlayer.Entity)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ItemAmount itemAmount in list)
		{
			int itemAmount2 = LocalPlayer.GetItemAmount(itemAmount.itemDef);
			if ((float)itemAmount2 < itemAmount.amount)
			{
				stringBuilder.AppendFormat("[e]{0:N0}[/e] x {1} [e]({2:N0})[/e]\n", new object[]
				{
					itemAmount.amount,
					itemAmount.itemDef.displayName.translated,
					itemAmount2,
					(float)itemAmount2 - itemAmount.amount
				});
			}
			else
			{
				stringBuilder.AppendFormat("{0:N0} x {1} ({2:N0})\n", new object[]
				{
					itemAmount.amount,
					itemAmount.itemDef.displayName.translated,
					itemAmount2,
					(float)itemAmount2 - itemAmount.amount
				});
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x00082C68 File Offset: 0x00080E68
	public static bool HasInventoryItem(int id)
	{
		return global::Client.Steam != null && global::Client.Steam.Inventory.Items != null && Enumerable.Any<Inventory.Item>(global::Client.Steam.Inventory.Items, (Inventory.Item x) => id == x.DefinitionId);
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x00082CC0 File Offset: 0x00080EC0
	public static void ResetCraftCounts()
	{
		foreach (ItemDefinition itemDefinition in ItemManager.itemList)
		{
			string key = string.Format("craftcount.{0}", itemDefinition.shortname);
			if (PlayerPrefs.HasKey(key))
			{
				PlayerPrefs.DeleteKey(key);
			}
		}
		Debug.Log("Craft counts reset to zero.");
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x00082D34 File Offset: 0x00080F34
	public static void ListCraftCounts()
	{
		foreach (ItemDefinition itemDefinition in ItemManager.itemList)
		{
			string text = string.Format("craftcount.{0}", itemDefinition.shortname);
			if (PlayerPrefs.HasKey(text))
			{
				Debug.Log(text + " : " + PlayerPrefs.GetInt(text, -1));
			}
		}
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x00082DB4 File Offset: 0x00080FB4
	public static int GetCraftCount(ItemBlueprint x)
	{
		int num = 0;
		if (LocalPlayer.CraftCounts.TryGetValue(x.targetItem.itemid, ref num))
		{
			return num;
		}
		num = PlayerPrefs.GetInt(string.Format("craftcount.{0}", x.targetItem.shortname), 0);
		LocalPlayer.CraftCounts.Add(x.targetItem.itemid, num);
		return num;
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x00082E14 File Offset: 0x00081014
	public static void AddCraftCount(ItemBlueprint x)
	{
		int num = LocalPlayer.GetCraftCount(x) + 1;
		LocalPlayer.CraftCounts[x.targetItem.itemid] = num;
		PlayerPrefs.SetInt(string.Format("craftcount.{0}", x.targetItem.shortname), num);
	}

	// Token: 0x0200031B RID: 795
	public class ItemBlueprintDescendingComparer : IComparer<ItemBlueprint>
	{
		// Token: 0x0600153A RID: 5434 RVA: 0x00082E5C File Offset: 0x0008105C
		public int Compare(ItemBlueprint a, ItemBlueprint b)
		{
			return LocalPlayer.GetCraftCount(b).CompareTo(LocalPlayer.GetCraftCount(a));
		}
	}
}
