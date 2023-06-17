using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000470 RID: 1136
public class ItemManager
{
	// Token: 0x04001783 RID: 6019
	public static List<ItemDefinition> itemList;

	// Token: 0x04001784 RID: 6020
	public static Dictionary<int, ItemDefinition> itemDictionary;

	// Token: 0x04001785 RID: 6021
	public static List<ItemBlueprint> bpList;

	// Token: 0x04001786 RID: 6022
	public static int[] defaultBlueprints;

	// Token: 0x06001A9B RID: 6811 RVA: 0x00093258 File Offset: 0x00091458
	public static void InvalidateWorkshopSkinCache()
	{
		if (ItemManager.itemList == null)
		{
			return;
		}
		foreach (ItemDefinition itemDefinition in ItemManager.itemList)
		{
			itemDefinition.InvalidateWorkshopSkinCache();
		}
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x000932B0 File Offset: 0x000914B0
	public static void Initialize()
	{
		if (ItemManager.itemList != null)
		{
			return;
		}
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		IEnumerable<GameObject> enumerable = Enumerable.Cast<GameObject>(FileSystem.Load<ObjectList>("Assets/items.asset", true).objects);
		if (stopwatch.Elapsed.TotalSeconds > 1.0)
		{
			Debug.Log("Loading Items Took: " + (stopwatch.Elapsed.TotalMilliseconds / 1000.0).ToString() + " seconds");
		}
		List<ItemDefinition> list = Enumerable.ToList<ItemDefinition>(Enumerable.Where<ItemDefinition>(Enumerable.Select<GameObject, ItemDefinition>(enumerable, (GameObject x) => x.GetComponent<ItemDefinition>()), (ItemDefinition x) => x != null));
		List<ItemBlueprint> list2 = Enumerable.ToList<ItemBlueprint>(Enumerable.Where<ItemBlueprint>(Enumerable.Select<GameObject, ItemBlueprint>(enumerable, (GameObject x) => x.GetComponent<ItemBlueprint>()), (ItemBlueprint x) => x != null && x.userCraftable));
		Dictionary<int, ItemDefinition> dictionary = new Dictionary<int, ItemDefinition>();
		foreach (ItemDefinition itemDefinition in list)
		{
			itemDefinition.Initialize(list);
			if (dictionary.ContainsKey(itemDefinition.itemid))
			{
				ItemDefinition itemDefinition2 = dictionary[itemDefinition.itemid];
				Debug.LogWarning(string.Concat(new object[]
				{
					"Item ID duplicate ",
					itemDefinition.itemid,
					" (",
					itemDefinition.name,
					") - have you given your items unique shortnames?"
				}), itemDefinition.gameObject);
				Debug.LogWarning("Other item is " + itemDefinition2.name, itemDefinition2);
			}
			else
			{
				dictionary.Add(itemDefinition.itemid, itemDefinition);
			}
		}
		stopwatch.Stop();
		if (stopwatch.Elapsed.TotalSeconds > 1.0)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Building Items Took: ",
				(stopwatch.Elapsed.TotalMilliseconds / 1000.0).ToString(),
				" seconds / Items: ",
				list.Count.ToString(),
				" / Blueprints: ",
				list2.Count.ToString()
			}));
		}
		ItemManager.defaultBlueprints = Enumerable.ToArray<int>(Enumerable.Select<ItemBlueprint, int>(Enumerable.Where<ItemBlueprint>(list2, (ItemBlueprint x) => !x.NeedsSteamItem && x.defaultBlueprint), (ItemBlueprint x) => x.targetItem.itemid));
		ItemManager.itemList = list;
		ItemManager.bpList = list2;
		ItemManager.itemDictionary = dictionary;
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x00012C94 File Offset: 0x00010E94
	public static void UpdateUnlockedSkins()
	{
		ItemManager.Initialize();
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x0001611F File Offset: 0x0001431F
	public static Item Load(Item load, Item created, bool isServer)
	{
		if (created == null)
		{
			created = new Item();
		}
		created.isServer = isServer;
		created.Load(load);
		if (created.info == null)
		{
			Debug.LogWarning("Item loading failed - item is invalid");
			return null;
		}
		return created;
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x000935A8 File Offset: 0x000917A8
	public static ItemDefinition FindItemDefinition(int itemID)
	{
		ItemDefinition result = null;
		ItemManager.itemDictionary.TryGetValue(itemID, ref result);
		return result;
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x000935C8 File Offset: 0x000917C8
	public static ItemDefinition FindItemDefinition(string shortName)
	{
		ItemManager.Initialize();
		for (int i = 0; i < ItemManager.itemList.Count; i++)
		{
			if (ItemManager.itemList[i].shortname == shortName)
			{
				return ItemManager.itemList[i];
			}
		}
		return null;
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x00016154 File Offset: 0x00014354
	public static ItemBlueprint FindBlueprint(ItemDefinition item)
	{
		return item.GetComponent<ItemBlueprint>();
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x0001615C File Offset: 0x0001435C
	public static List<ItemDefinition> GetItemDefinitions()
	{
		ItemManager.Initialize();
		return ItemManager.itemList;
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x00016168 File Offset: 0x00014368
	public static List<ItemBlueprint> GetBlueprints()
	{
		ItemManager.Initialize();
		return ItemManager.bpList;
	}
}
