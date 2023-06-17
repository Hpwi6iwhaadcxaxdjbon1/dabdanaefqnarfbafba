using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200064D RID: 1613
public class UIBlueprints : ListComponent<UIBlueprints>
{
	// Token: 0x04002013 RID: 8211
	public GameObjectRef buttonPrefab;

	// Token: 0x04002014 RID: 8212
	public ScrollRect scrollRect;

	// Token: 0x04002015 RID: 8213
	public InputField searchField;

	// Token: 0x04002016 RID: 8214
	public GameObject listAvailable;

	// Token: 0x04002017 RID: 8215
	public GameObject listLocked;

	// Token: 0x04002018 RID: 8216
	public GameObject Categories;

	// Token: 0x04002019 RID: 8217
	[NonSerialized]
	public bool needsResort;

	// Token: 0x0400201A RID: 8218
	private ItemCategory category = ItemCategory.All;

	// Token: 0x0400201B RID: 8219
	private List<BlueprintButton> buttons = new List<BlueprintButton>();

	// Token: 0x0400201C RID: 8220
	private static ItemCategory[] miscInclusive = new ItemCategory[]
	{
		ItemCategory.Component
	};

	// Token: 0x060023F8 RID: 9208 RVA: 0x000BE730 File Offset: 0x000BC930
	public void ChangeCategory(string strCategory)
	{
		this.searchField.text = string.Empty;
		ItemCategory itemCategory = (ItemCategory)Enum.Parse(typeof(ItemCategory), strCategory);
		if (itemCategory == this.category)
		{
			return;
		}
		this.category = itemCategory;
		this.UpdateBlueprintList();
		this.UpdateFlash();
	}

	// Token: 0x060023F9 RID: 9209 RVA: 0x000BE780 File Offset: 0x000BC980
	private void UpdateFlash()
	{
		this.scrollRect.GetComponent<CanvasGroup>().alpha = 0.4f;
		this.scrollRect.transform.localScale = Vector3.one * 0.95f;
		LeanTween.alphaCanvas(this.scrollRect.GetComponent<CanvasGroup>(), 1f, 0.2f);
		LeanTween.scale(this.scrollRect.gameObject, Vector3.one, 0.2f).setEase(27);
	}

	// Token: 0x060023FA RID: 9210 RVA: 0x0001C6CA File Offset: 0x0001A8CA
	private void RetireChildren(GameObject go)
	{
		if (go != null && go.transform.childCount > 0)
		{
			go.transform.RetireAllChildren(GameManager.client);
		}
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x0001C6F3 File Offset: 0x0001A8F3
	protected override void OnEnable()
	{
		this.RetireChildren(this.listAvailable);
		this.RetireChildren(this.listLocked);
		base.OnEnable();
		this.UpdateBlueprintList();
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x0001C719 File Offset: 0x0001A919
	protected override void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		base.OnDisable();
		this.RetireChildren(this.listAvailable);
		this.RetireChildren(this.listLocked);
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x000BE800 File Offset: 0x000BCA00
	public static IEnumerable<ItemBlueprint> GetForCategory(ItemCategory category)
	{
		if (LocalPlayer.Entity == null)
		{
			return Enumerable.Empty<ItemBlueprint>();
		}
		switch (category)
		{
		case ItemCategory.Misc:
			return Enumerable.OrderBy<ItemBlueprint, Rarity>(Enumerable.Where<ItemBlueprint>(ItemManager.bpList, (ItemBlueprint x) => (x.targetItem.category == category || x.targetItem.category == UIBlueprints.miscInclusive[0]) && LocalPlayer.HasUnlocked(x.targetItem)), (ItemBlueprint x) => x.rarity);
		case ItemCategory.All:
			return ItemManager.bpList;
		case ItemCategory.Common:
			return Enumerable.OrderByDescending<ItemBlueprint, int>(Enumerable.Where<ItemBlueprint>(ItemManager.bpList, (ItemBlueprint x) => LocalPlayer.HasUnlocked(x.targetItem)), (ItemBlueprint y) => LocalPlayer.GetCraftCount(y));
		default:
			return Enumerable.OrderBy<ItemBlueprint, Rarity>(Enumerable.Where<ItemBlueprint>(ItemManager.bpList, (ItemBlueprint x) => x.targetItem.category == category && LocalPlayer.HasUnlocked(x.targetItem)), (ItemBlueprint x) => x.rarity);
		}
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x0001C741 File Offset: 0x0001A941
	public static IEnumerable<ItemBlueprint> GetCraftableForCategory(ItemCategory category)
	{
		return UIBlueprints.GetForCategory(category);
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x0001C749 File Offset: 0x0001A949
	public static int CountCraftableForCategory(ItemCategory category)
	{
		return UIBlueprints.CountForCategory(category, true);
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x000BE918 File Offset: 0x000BCB18
	public static int CountForCategory(ItemCategory category, bool onlyUnlocked = true)
	{
		if (LocalPlayer.Entity == null)
		{
			return 0;
		}
		if (category == ItemCategory.All)
		{
			return ItemManager.bpList.Count;
		}
		if (category == ItemCategory.Common)
		{
			return ItemManager.bpList.Count;
		}
		int num = 0;
		for (int i = 0; i < ItemManager.bpList.Count; i++)
		{
			if (ItemManager.bpList[i].targetItem.category == category && (!onlyUnlocked || LocalPlayer.HasUnlocked(ItemManager.bpList[i].targetItem)))
			{
				num++;
			}
		}
		if (category == ItemCategory.Misc)
		{
			foreach (ItemCategory itemCategory in UIBlueprints.miscInclusive)
			{
				num += UIBlueprints.CountForCategory(itemCategory, onlyUnlocked);
			}
		}
		return num;
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000BE9CC File Offset: 0x000BCBCC
	public void UpdateBlueprintList()
	{
		this.RetireChildren(this.listAvailable);
		this.RetireChildren(this.listLocked);
		this.buttons.Clear();
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		if (LocalPlayer.Entity.blueprints == null)
		{
			return;
		}
		this.UpdateBlueprints(UIBlueprints.GetCraftableForCategory(this.category));
		foreach (BlueprintCategoryButton blueprintCategoryButton in this.Categories.GetComponentsInChildren<BlueprintCategoryButton>(true))
		{
			if (blueprintCategoryButton.Category != ItemCategory.Common)
			{
				int num = UIBlueprints.CountCraftableForCategory(blueprintCategoryButton.Category);
				blueprintCategoryButton.gameObject.SetActive(num > 0);
			}
		}
		this.Categories.GetComponent<VerticalLayoutGroup>().enabled = false;
		this.Categories.GetComponent<VerticalLayoutGroup>().enabled = true;
	}

	// Token: 0x06002402 RID: 9218 RVA: 0x000BEA94 File Offset: 0x000BCC94
	private void UpdateBlueprints(IEnumerable<ItemBlueprint> list)
	{
		int num = 0;
		foreach (ItemBlueprint itemBlueprint in list)
		{
			if (!itemBlueprint.NeedsSteamItem || LocalPlayer.HasUnlocked(itemBlueprint.targetItem))
			{
				BlueprintButton component = GameManager.client.CreatePrefab(this.buttonPrefab.resourcePath, Vector3.zero, Quaternion.identity, true).GetComponent<BlueprintButton>();
				component.transform.SetParent(this.listAvailable.transform, false);
				if (LocalPlayer.HasItems(itemBlueprint.ingredients, 1))
				{
					component.transform.SetParent(this.listAvailable.transform, false);
				}
				else
				{
					component.transform.SetParent(this.listLocked.transform, false);
				}
				component.Setup(itemBlueprint);
				this.buttons.Add(component);
				num++;
				if (this.category == ItemCategory.Common && num > 60)
				{
					break;
				}
			}
		}
		this.listAvailable.SetActive(this.listAvailable.transform.childCount > 0);
		this.listLocked.SetActive(this.listLocked.transform.childCount > 0);
	}

	// Token: 0x06002403 RID: 9219 RVA: 0x000BEBD4 File Offset: 0x000BCDD4
	public static void Refresh()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		foreach (UIBlueprints uiblueprints in ListComponent<UIBlueprints>.InstanceList)
		{
			uiblueprints.UpdateBlueprintList();
		}
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x000BEC2C File Offset: 0x000BCE2C
	public void Search(string search)
	{
		if (search == "")
		{
			search = "BALLS BALLS BALLS";
		}
		this.RetireChildren(this.listAvailable);
		this.RetireChildren(this.listLocked);
		this.buttons.Clear();
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		if (LocalPlayer.Entity.blueprints == null)
		{
			return;
		}
		IEnumerable<ItemBlueprint> list = Enumerable.Take<ItemBlueprint>(Enumerable.Where<ItemBlueprint>(ItemManager.bpList, (ItemBlueprint x) => StringEx.Contains(x.targetItem.shortname, search, 1) || StringEx.Contains(x.targetItem.displayName.translated, search, 1) || StringEx.Contains(x.targetItem.displayDescription.translated, search, 1)), 60);
		this.UpdateBlueprints(list);
		this.UpdateFlash();
	}
}
