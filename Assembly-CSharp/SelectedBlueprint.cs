using System;
using System.Linq;
using Facepunch.Steamworks;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200064B RID: 1611
public class SelectedBlueprint : SingletonComponent<SelectedBlueprint>, IInventoryChanged
{
	// Token: 0x04002003 RID: 8195
	public ItemBlueprint blueprint;

	// Token: 0x04002004 RID: 8196
	public InputField craftAmountText;

	// Token: 0x04002005 RID: 8197
	public GameObject ingredientGrid;

	// Token: 0x04002006 RID: 8198
	public IconSkinPicker skinPicker;

	// Token: 0x04002007 RID: 8199
	public Image iconImage;

	// Token: 0x04002008 RID: 8200
	public Text titleText;

	// Token: 0x04002009 RID: 8201
	public Text descriptionText;

	// Token: 0x0400200A RID: 8202
	public CanvasGroup CraftArea;

	// Token: 0x0400200B RID: 8203
	public Button CraftButton;

	// Token: 0x0400200C RID: 8204
	public Text CraftTime;

	// Token: 0x0400200D RID: 8205
	public Text CraftAmount;

	// Token: 0x0400200E RID: 8206
	public GameObject[] workbenchReqs;

	// Token: 0x0400200F RID: 8207
	private ItemInformationPanel[] informationPanels;

	// Token: 0x04002010 RID: 8208
	private int craftAmount = 1;

	// Token: 0x04002011 RID: 8209
	private BlueprintCraftGridRow[] ingredientRows;

	// Token: 0x060023E2 RID: 9186 RVA: 0x000BE10C File Offset: 0x000BC30C
	public static void UpdateBlueprint(ItemBlueprint blueprint)
	{
		if (SingletonComponent<SelectedBlueprint>.Instance == null)
		{
			return;
		}
		if (SingletonComponent<SelectedBlueprint>.Instance.blueprint == blueprint)
		{
			return;
		}
		SingletonComponent<SelectedBlueprint>.Instance.blueprint = blueprint;
		if (SingletonComponent<SelectedBlueprint>.Instance.blueprint != null)
		{
			SelectedItem.ClearSelection();
		}
		SingletonComponent<SelectedBlueprint>.Instance.ChangeEffects();
		SingletonComponent<SelectedBlueprint>.Instance.SetCraft(1);
		SingletonComponent<SelectedBlueprint>.Instance.OnInventoryChanged();
	}

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x060023E3 RID: 9187 RVA: 0x0001C5E5 File Offset: 0x0001A7E5
	public static bool isOpen
	{
		get
		{
			return !(SingletonComponent<SelectedBlueprint>.Instance == null) && SingletonComponent<SelectedBlueprint>.Instance.blueprint != null;
		}
	}

	// Token: 0x060023E4 RID: 9188 RVA: 0x0001C606 File Offset: 0x0001A806
	private void Start()
	{
		this.ingredientRows = base.GetComponentsInChildren<BlueprintCraftGridRow>();
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x0001C614 File Offset: 0x0001A814
	private void OnEnable()
	{
		GlobalMessages.onInventoryChanged.Add(this);
		this.informationPanels = base.GetComponentsInChildren<ItemInformationPanel>(true);
		this.OnInventoryChanged();
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x0001C384 File Offset: 0x0001A584
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onInventoryChanged.Remove(this);
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x000BE17C File Offset: 0x000BC37C
	public void ChangeEffects()
	{
		base.GetComponent<CanvasGroup>().alpha = 0.4f;
		base.transform.localScale = Vector3.one * 0.95f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.2f);
		LeanTween.scale(base.gameObject, Vector3.one, 0.2f).setEase(27);
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x0001C634 File Offset: 0x0001A834
	public void OnInventoryChanged()
	{
		this.RefreshBlueprint(this.blueprint);
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000BE1E8 File Offset: 0x000BC3E8
	private void RefreshBlueprint(ItemBlueprint blueprint)
	{
		if (blueprint == null)
		{
			return;
		}
		this.SetCraft(this.craftAmount);
		this.UpdateIngredients();
		this.skinPicker.Refresh(blueprint);
		this.titleText.text = blueprint.targetItem.displayName.translated;
		this.descriptionText.text = blueprint.targetItem.displayDescription.translated;
		this.iconImage.sprite = blueprint.targetItem.iconSprite;
		this.CraftAmount.text = blueprint.amountToCreate.ToString("N0");
		float scaledDuration = ItemCrafter.GetScaledDuration(blueprint, LocalPlayer.GetCraftLevel());
		this.CraftTime.text = scaledDuration.ToString("N1");
		foreach (ItemInformationPanel itemInformationPanel in this.informationPanels)
		{
			if (itemInformationPanel.EligableForDisplay(blueprint.targetItem))
			{
				itemInformationPanel.gameObject.SetActive(true);
				itemInformationPanel.SetupForItem(blueprint.targetItem, null);
			}
			else
			{
				itemInformationPanel.gameObject.SetActive(false);
			}
		}
		int @int = PlayerPrefs.GetInt(string.Format("skin.{0}", blueprint.targetItem.shortname), 0);
		if (@int != 0)
		{
			this.iconImage.sprite = blueprint.targetItem.FindIconSprite(@int);
		}
		GameObject[] array2 = this.workbenchReqs;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetActive(false);
		}
		if (blueprint.workbenchLevelRequired > 0 && BasePlayer.craftMode == 0)
		{
			int num = Mathf.Clamp(blueprint.workbenchLevelRequired - 1, 0, this.workbenchReqs.Length - 1);
			this.workbenchReqs[num].SetActive(true);
			base.GetComponent<VerticalLayoutGroup>().enabled = false;
			base.GetComponent<VerticalLayoutGroup>().enabled = true;
		}
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x000BE3A4 File Offset: 0x000BC5A4
	public void Max()
	{
		int num = 999;
		for (int i = 0; i < this.blueprint.ingredients.Count; i++)
		{
			ItemAmount itemAmount = this.blueprint.ingredients[i];
			num = Mathf.Min(Mathf.FloorToInt((float)LocalPlayer.GetItemAmount(itemAmount.itemDef) / itemAmount.amount), num);
		}
		this.SetCraft(num);
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x000BE40C File Offset: 0x000BC60C
	public void StartCrafting()
	{
		if (!this.CanCraft())
		{
			return;
		}
		if (this.blueprint != null && !LocalPlayer.HasCraftLevel(this.blueprint.workbenchLevelRequired))
		{
			foreach (GameObject gameObject in this.workbenchReqs)
			{
				if (!LeanTween.isTweening(gameObject.gameObject))
				{
					LeanTween.moveLocalX(gameObject.gameObject, 15f, 0.1f).setEase(16).setLoopPingPong(2);
				}
			}
			return;
		}
		int inventoryId = PlayerPrefs.GetInt("skin." + this.blueprint.targetItem.shortname, 0);
		if (inventoryId != 0 && Enumerable.FirstOrDefault<ItemSkinDirectory.Skin>(this.blueprint.targetItem.skins, (ItemSkinDirectory.Skin x) => x.id == inventoryId).id != inventoryId && (this.blueprint.targetItem.skins2 == null || Enumerable.FirstOrDefault<Inventory.Definition>(this.blueprint.targetItem.skins2, (Inventory.Definition x) => x.Id == inventoryId) == null))
		{
			inventoryId = 0;
		}
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "craft.add", new object[]
		{
			this.blueprint.targetItem.itemid,
			this.craftAmount,
			inventoryId
		});
		this.SetCraft(1);
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x0001C642 File Offset: 0x0001A842
	public void AddToCraft(int i)
	{
		this.SetCraft(this.craftAmount + i);
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000BE57C File Offset: 0x000BC77C
	public void SetCraftFromTextbox(string i)
	{
		int craft = 0;
		if (!int.TryParse(i, ref craft))
		{
			return;
		}
		this.SetCraft(craft);
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x000BE5A0 File Offset: 0x000BC7A0
	public void SetCraft(int i)
	{
		i = Mathf.Clamp(i, 1, Mathf.Max(10, this.blueprint.targetItem.stackable));
		this.craftAmount = i;
		this.craftAmountText.text = i.ToString();
		this.UpdateIngredients();
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x000BE5EC File Offset: 0x000BC7EC
	public void UpdateIngredients()
	{
		if (this.ingredientRows == null)
		{
			return;
		}
		BlueprintCraftGridRow[] array = this.ingredientRows;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		if (this.blueprint == null)
		{
			return;
		}
		for (int j = 0; j < this.blueprint.ingredients.Count; j++)
		{
			ItemAmount itemAmount = this.blueprint.ingredients[j];
			this.ingredientRows[j].Setup(itemAmount.itemDef, (int)itemAmount.amount, this.craftAmount);
		}
		this.CraftButton.interactable = this.CanCraftAmount(this.craftAmount);
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x000BE694 File Offset: 0x000BC894
	private void Update()
	{
		bool flag = this.CanCraft();
		if (!UICrafting.isOpen)
		{
			return;
		}
		if (this.CraftArea.interactable != flag)
		{
			this.CraftArea.alpha = (flag ? 1f : 0.25f);
			this.CraftArea.interactable = flag;
			this.UpdateIngredients();
		}
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x0001C652 File Offset: 0x0001A852
	public bool CanCraft()
	{
		return !(this.blueprint == null) && LocalPlayer.HasUnlocked(this.blueprint.targetItem);
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x0001C679 File Offset: 0x0001A879
	public bool CanCraftAmount(int amount)
	{
		return this.CanCraft() && LocalPlayer.HasItems(this.blueprint.ingredients, this.craftAmount);
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x000BE6EC File Offset: 0x000BC8EC
	public void Unlock()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client.Quiet(), "unlockblueprint", new object[]
		{
			this.blueprint.targetItem.itemid
		});
	}
}
