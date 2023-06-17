using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Steamworks;
using Rust;
using Rust.Workshop.Game;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006A2 RID: 1698
public class SteamInventoryInfo : SingletonComponent<SteamInventoryInfo>
{
	// Token: 0x040021D6 RID: 8662
	public GameObject inventoryItemPrefab;

	// Token: 0x040021D7 RID: 8663
	public GameObject inventoryCanvas;

	// Token: 0x040021D8 RID: 8664
	public GameObject missingItems;

	// Token: 0x040021D9 RID: 8665
	public WorkshopInventoryCraftingControls CraftControl;

	// Token: 0x040021DA RID: 8666
	private Inventory.Item[] PreviousItems;

	// Token: 0x040021DB RID: 8667
	public bool FirstUpdate;

	// Token: 0x040021DC RID: 8668
	private float LastStackTime;

	// Token: 0x060025D8 RID: 9688 RVA: 0x0001D87B File Offset: 0x0001BA7B
	private void OnEnable()
	{
		if (Global.SteamClient != null)
		{
			Global.SteamClient.Inventory.OnUpdate += new Action(this.Refresh);
		}
		this.Refresh();
	}

	// Token: 0x060025D9 RID: 9689 RVA: 0x0001D8A5 File Offset: 0x0001BAA5
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (Global.SteamClient != null)
		{
			Global.SteamClient.Inventory.OnUpdate -= new Action(this.Refresh);
		}
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x000C6EFC File Offset: 0x000C50FC
	public void ManualRefresh()
	{
		Enumerable.All<Transform>(Enumerable.ToArray<Transform>(Enumerable.Where<Transform>(Enumerable.Cast<Transform>(this.inventoryCanvas.transform), (Transform x) => x.gameObject.activeSelf)), delegate(Transform x)
		{
			GameManager.DestroyImmediate(x.gameObject, false);
			return true;
		});
		this.inventoryItemPrefab.SetActive(false);
		this.missingItems.SetActive(true);
		this.PreviousItems = null;
		this.CraftControl.OnManualRefresh();
		Global.SteamClient.Inventory.Items = null;
		global::Client.Steam.Inventory.Refresh();
	}

	// Token: 0x060025DB RID: 9691 RVA: 0x0001D8D1 File Offset: 0x0001BAD1
	public void Update()
	{
		this.FixStacks();
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x000C6FB0 File Offset: 0x000C51B0
	public void Refresh()
	{
		if (global::Client.Steam == null)
		{
			return;
		}
		if (!this.NeedsUpdate(global::Client.Steam.Inventory.Items))
		{
			return;
		}
		if (global::Client.Steam.Inventory.Items == null || global::Client.Steam.Inventory.Items.Length == 0)
		{
			Enumerable.All<Transform>(Enumerable.ToArray<Transform>(Enumerable.Where<Transform>(Enumerable.Cast<Transform>(this.inventoryCanvas.transform), (Transform x) => x.gameObject.activeSelf)), delegate(Transform x)
			{
				GameManager.DestroyImmediate(x.gameObject, false);
				return true;
			});
			this.inventoryItemPrefab.SetActive(false);
			this.missingItems.SetActive(true);
			return;
		}
		this.missingItems.SetActive(false);
		WorkshopInventoryItem[] componentsInChildren = this.inventoryCanvas.GetComponentsInChildren<WorkshopInventoryItem>();
		int siblingIndex = 0;
		foreach (WorkshopInventoryItem workshopInventoryItem in Enumerable.ToArray<WorkshopInventoryItem>(Enumerable.Where<WorkshopInventoryItem>(componentsInChildren, (WorkshopInventoryItem x) => !Enumerable.Contains<Inventory.Item>(global::Client.Steam.Inventory.Items, x.Item))))
		{
			siblingIndex = workshopInventoryItem.transform.GetSiblingIndex();
			GameManager.DestroyImmediate(workshopInventoryItem.gameObject, false);
		}
		componentsInChildren = this.inventoryCanvas.GetComponentsInChildren<WorkshopInventoryItem>();
		using (IEnumerator<Inventory.Item> enumerator = Enumerable.OrderBy<Inventory.Item, int>(global::Client.Steam.Inventory.Items, (Inventory.Item x) => x.DefinitionId).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Inventory.Item item = enumerator.Current;
				if (item.DefinitionId != 10031 && item.DefinitionId != 14182 && item.DefinitionId != 14183 && !(Enumerable.FirstOrDefault<WorkshopInventoryItem>(componentsInChildren, (WorkshopInventoryItem x) => x.Item.Id == item.Id) != null))
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.inventoryItemPrefab);
					gameObject.transform.SetParent(this.inventoryCanvas.transform, false);
					gameObject.SetActive(true);
					gameObject.transform.SetSiblingIndex(siblingIndex);
					siblingIndex = 0;
					gameObject.GetComponent<WorkshopInventoryItem>().Setup(item, !this.FirstUpdate);
					gameObject.GetComponent<Toggle>().group = this.inventoryCanvas.GetComponent<ToggleGroup>();
				}
			}
		}
		this.CraftControl.OnRefreshed(global::Client.Steam.Inventory.Items);
		this.FirstUpdate = false;
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x000C7254 File Offset: 0x000C5454
	private void FixStacks()
	{
		if (this.LastStackTime > Time.realtimeSinceStartup - 10f)
		{
			return;
		}
		if (global::Client.Steam.Inventory.Items == null)
		{
			return;
		}
		this.LastStackTime = Time.realtimeSinceStartup;
		bool flag = false;
		foreach (Inventory.Item item in global::Client.Steam.Inventory.Items)
		{
			if (item.Quantity > 1 && item.Definition != null && !(item.Definition.Type == "Resource"))
			{
				global::Client.Steam.Inventory.SplitStack(item, 1);
				flag = true;
			}
		}
		if (flag)
		{
			return;
		}
		foreach (IGrouping<int, Inventory.Item> grouping in Enumerable.GroupBy<Inventory.Item, int>(Enumerable.Where<Inventory.Item>(Enumerable.Where<Inventory.Item>(Enumerable.Where<Inventory.Item>(global::Client.Steam.Inventory.Items, (Inventory.Item x) => x.Definition != null), (Inventory.Item x) => x.Definition.Type != null), (Inventory.Item x) => x.Definition.Type == "Resource"), (Inventory.Item x) => x.DefinitionId))
		{
			if (Enumerable.Count<Inventory.Item>(grouping) > 1)
			{
				foreach (Inventory.Item item2 in grouping)
				{
					if (!(item2 == Enumerable.First<Inventory.Item>(grouping)))
					{
						global::Client.Steam.Inventory.Stack(item2, Enumerable.First<Inventory.Item>(grouping), item2.Quantity);
					}
				}
			}
		}
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x000C7440 File Offset: 0x000C5640
	private bool NeedsUpdate(Inventory.Item[] items)
	{
		if (items == null)
		{
			return true;
		}
		if (this.PreviousItems == null)
		{
			this.PreviousItems = Enumerable.ToArray<Inventory.Item>(items);
			return true;
		}
		if (this.PreviousItems.Length != items.Length)
		{
			this.PreviousItems = Enumerable.ToArray<Inventory.Item>(items);
			return true;
		}
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].Id != this.PreviousItems[i].Id)
			{
				this.PreviousItems = Enumerable.ToArray<Inventory.Item>(items);
				return true;
			}
		}
		this.PreviousItems = Enumerable.ToArray<Inventory.Item>(items);
		return false;
	}
}
