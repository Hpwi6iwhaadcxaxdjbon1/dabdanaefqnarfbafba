using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200024C RID: 588
public class ItemListTools : MonoBehaviour
{
	// Token: 0x04000E4B RID: 3659
	public GameObject categoryButton;

	// Token: 0x04000E4C RID: 3660
	public GameObject itemButton;

	// Token: 0x04000E4D RID: 3661
	internal Button lastCategory;

	// Token: 0x06001194 RID: 4500 RVA: 0x0000F6AA File Offset: 0x0000D8AA
	public void OnPanelOpened()
	{
		this.Refresh();
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x0000F6B2 File Offset: 0x0000D8B2
	public void Refresh()
	{
		this.RebuildCategories();
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x00074BCC File Offset: 0x00072DCC
	private void RebuildCategories()
	{
		for (int i = 0; i < this.categoryButton.transform.parent.childCount; i++)
		{
			Transform child = this.categoryButton.transform.parent.GetChild(i);
			if (!(child == this.categoryButton.transform))
			{
				GameManager.Destroy(child.gameObject, 0f);
			}
		}
		this.categoryButton.SetActive(true);
		foreach (IGrouping<ItemCategory, ItemDefinition> grouping in Enumerable.OrderBy<IGrouping<ItemCategory, ItemDefinition>, ItemCategory>(Enumerable.GroupBy<ItemDefinition, ItemCategory>(ItemManager.GetItemDefinitions(), (ItemDefinition x) => x.category), (IGrouping<ItemCategory, ItemDefinition> x) => Enumerable.First<ItemDefinition>(x).category))
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.categoryButton);
			gameObject.transform.SetParent(this.categoryButton.transform.parent, false);
			gameObject.GetComponentInChildren<Text>().text = Enumerable.First<ItemDefinition>(grouping).category.ToString();
			Button btn = gameObject.GetComponentInChildren<Button>();
			ItemDefinition[] itemArray = Enumerable.ToArray<ItemDefinition>(grouping);
			btn.onClick.AddListener(delegate()
			{
				if (this.lastCategory)
				{
					this.lastCategory.interactable = true;
				}
				this.lastCategory = btn;
				this.lastCategory.interactable = false;
				this.SwitchItemCategory(itemArray);
			});
			if (this.lastCategory == null)
			{
				this.lastCategory = btn;
				this.lastCategory.interactable = false;
				this.SwitchItemCategory(itemArray);
			}
		}
		this.categoryButton.SetActive(false);
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x00074D9C File Offset: 0x00072F9C
	private void SwitchItemCategory(ItemDefinition[] defs)
	{
		for (int i = 0; i < this.itemButton.transform.parent.childCount; i++)
		{
			Transform child = this.itemButton.transform.parent.GetChild(i);
			if (!(child == this.itemButton.transform))
			{
				GameManager.Destroy(child.gameObject, 0f);
			}
		}
		this.itemButton.SetActive(true);
		foreach (ItemDefinition itemDefinition in Enumerable.OrderBy<ItemDefinition, string>(defs, (ItemDefinition x) => x.displayName.translated))
		{
			if (!itemDefinition.hidden)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.itemButton);
				gameObject.transform.SetParent(this.itemButton.transform.parent, false);
				gameObject.GetComponentInChildren<Text>().text = itemDefinition.displayName.translated;
				gameObject.GetComponentInChildren<ItemButtonTools>().itemDef = itemDefinition;
				gameObject.GetComponentInChildren<ItemButtonTools>().image.sprite = itemDefinition.iconSprite;
			}
		}
		this.itemButton.SetActive(false);
	}
}
