using System;
using System.Collections.Generic;
using GameMenu;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000673 RID: 1651
public class SelectedItem : SingletonComponent<SelectedItem>, IInventoryChanged
{
	// Token: 0x040020C2 RID: 8386
	public Image icon;

	// Token: 0x040020C3 RID: 8387
	public Image iconSplitter;

	// Token: 0x040020C4 RID: 8388
	public Text title;

	// Token: 0x040020C5 RID: 8389
	public Text description;

	// Token: 0x040020C6 RID: 8390
	public GameObject splitPanel;

	// Token: 0x040020C7 RID: 8391
	public GameObject itemProtection;

	// Token: 0x040020C8 RID: 8392
	public GameObject menuOption;

	// Token: 0x040020C9 RID: 8393
	public GameObject optionsParent;

	// Token: 0x040020CA RID: 8394
	public GameObject innerPanelContainer;

	// Token: 0x040020CB RID: 8395
	private Animator animator;

	// Token: 0x040020CC RID: 8396
	private ProtectionValue[] protectionValues;

	// Token: 0x040020CD RID: 8397
	private ItemInformationPanel[] informationPanels;

	// Token: 0x040020CE RID: 8398
	private List<Option> previousOptions;

	// Token: 0x040020CF RID: 8399
	private Item lastItem;

	// Token: 0x040020D0 RID: 8400
	internal bool wasOpen;

	// Token: 0x040020D1 RID: 8401
	public static ItemIcon selectedItem;

	// Token: 0x040020D2 RID: 8402
	public static ItemIcon hoveredItem;

	// Token: 0x060024C9 RID: 9417 RVA: 0x0001CE46 File Offset: 0x0001B046
	public static void UpdateItem()
	{
		if (SingletonComponent<SelectedItem>.Instance == null)
		{
			return;
		}
		SingletonComponent<SelectedItem>.Instance.OnInventoryChanged();
	}

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x060024CA RID: 9418 RVA: 0x000C2788 File Offset: 0x000C0988
	public static Item item
	{
		get
		{
			if (SelectedItem.hoveredItem != null && SelectedItem.hoveredItem.item != null)
			{
				return SelectedItem.hoveredItem.item;
			}
			if (SelectedItem.selectedItem != null && SelectedItem.selectedItem.item != null)
			{
				return SelectedItem.selectedItem.item;
			}
			return null;
		}
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x000C27E0 File Offset: 0x000C09E0
	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.protectionValues = this.itemProtection.transform.GetComponentsInChildren<ProtectionValue>(true);
		ProtectionValue[] array = this.protectionValues;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].selectedItem = true;
		}
		this.informationPanels = base.GetComponentsInChildren<ItemInformationPanel>(true);
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x0001CE60 File Offset: 0x0001B060
	private void OnEnable()
	{
		if (this.animator)
		{
			this.animator.SetTrigger("close");
		}
		this.wasOpen = false;
		GlobalMessages.onInventoryChanged.Add(this);
		this.OnInventoryChanged();
	}

	// Token: 0x060024CD RID: 9421 RVA: 0x0001C384 File Offset: 0x0001A584
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onInventoryChanged.Remove(this);
	}

	// Token: 0x060024CE RID: 9422 RVA: 0x000C283C File Offset: 0x000C0A3C
	public bool NeedsOptionsUpdate(List<Option> oldOpts, List<Option> newOpts, Item item)
	{
		if (item != this.lastItem)
		{
			return true;
		}
		if (oldOpts == null)
		{
			return true;
		}
		if (oldOpts.Count != newOpts.Count)
		{
			return true;
		}
		for (int i = 0; i < oldOpts.Count; i++)
		{
			if (oldOpts[i].desc != newOpts[i].desc)
			{
				return true;
			}
			if (oldOpts[i].title != newOpts[i].title)
			{
				return true;
			}
			if (oldOpts[i].showDisabled != newOpts[i].showDisabled)
			{
				return true;
			}
			if (oldOpts[i].command != newOpts[i].command)
			{
				return true;
			}
			if (oldOpts[i].requirements != newOpts[i].requirements)
			{
				return true;
			}
			if (oldOpts[i].show != newOpts[i].show)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060024CF RID: 9423 RVA: 0x0001CE97 File Offset: 0x0001B097
	public void OnInventoryChanged()
	{
		this.RefreshItem(SelectedItem.item);
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x000C2940 File Offset: 0x000C0B40
	private void RefreshItem(Item item)
	{
		if (item == null || item.info == null)
		{
			return;
		}
		List<Option> list = SelectedItem.ItemMenuOptions(item);
		if (this.NeedsOptionsUpdate(this.previousOptions, list, item))
		{
			this.optionsParent.transform.DestroyChildren();
			this.previousOptions = list;
			this.lastItem = item;
			foreach (Option option in list)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.menuOption);
				gameObject.transform.SetParent(this.optionsParent.transform, false);
				gameObject.name = option.title;
				gameObject.GetComponent<ItemOptionButton>().Setup(item, option);
			}
		}
		bool flag = false;
		foreach (ItemInformationPanel itemInformationPanel in this.informationPanels)
		{
			if (itemInformationPanel.EligableForDisplay(item.info))
			{
				this.innerPanelContainer.transform.ActiveChild("empty", true);
				flag = true;
				itemInformationPanel.gameObject.SetActive(true);
				itemInformationPanel.SetupForItem(item.info, item);
			}
			else
			{
				itemInformationPanel.gameObject.SetActive(false);
			}
		}
		if (!flag)
		{
			this.innerPanelContainer.SetActive(false);
			if (item.info.selectionPanel != ItemSelectionPanel.None)
			{
				this.innerPanelContainer.transform.ActiveChild(item.info.selectionPanel.ToString().ToLower(), true);
			}
			else
			{
				this.innerPanelContainer.transform.ActiveChild("empty", true);
			}
			this.innerPanelContainer.SetActive(true);
		}
		this.itemProtection.SetActive(false);
		this.splitPanel.SetActive(item.amount > 1);
		this.icon.sprite = item.iconSprite;
		this.iconSplitter.sprite = this.icon.sprite;
		this.title.text = item.info.GetDisplayName(item);
		this.description.text = "";
		Text text = this.description;
		text.text += item.info.GetDescriptionText(item);
		base.BroadcastMessage("RefreshValue", SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x060024D1 RID: 9425 RVA: 0x000C2B8C File Offset: 0x000C0D8C
	public static List<Option> ItemMenuOptions(Item item)
	{
		List<Option> list = new List<Option>();
		if (item.IsLocked())
		{
			return list;
		}
		ItemMod[] itemMods = item.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].GetMenuOptions(item, list, LocalPlayer.Entity);
		}
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity)
		{
			heldEntity.GetItemOptions(list);
		}
		if (!item.info.HasFlag(ItemDefinition.Flag.NoDropping))
		{
			list.Add(new Option
			{
				icon = "drop",
				title = "drop",
				desc = "drop_desc",
				command = "drop",
				order = 255,
				show = true
			});
		}
		return list;
	}

	// Token: 0x060024D2 RID: 9426 RVA: 0x000C2C4C File Offset: 0x000C0E4C
	public void Update()
	{
		bool flag = SelectedItem.item != null;
		if (flag == this.wasOpen)
		{
			return;
		}
		this.wasOpen = flag;
		this.animator.SetTrigger(this.wasOpen ? "open" : "close");
	}

	// Token: 0x060024D3 RID: 9427 RVA: 0x0001CEA4 File Offset: 0x0001B0A4
	public static void TrySelect(ItemIcon newSelect)
	{
		if (SelectedItem.selectedItem == newSelect)
		{
			return;
		}
		SelectedItem.ClearSelection();
		SelectedItem.selectedItem = newSelect;
		if (SelectedItem.selectedItem != null)
		{
			SelectedItem.selectedItem.Select();
			SelectedItem.UpdateItem();
		}
	}

	// Token: 0x060024D4 RID: 9428 RVA: 0x0001CEDB File Offset: 0x0001B0DB
	public static void ClearIfSelected(ItemIcon newSelect)
	{
		if (SelectedItem.selectedItem != newSelect)
		{
			return;
		}
		SelectedItem.ClearSelection();
	}

	// Token: 0x060024D5 RID: 9429 RVA: 0x0001CEF0 File Offset: 0x0001B0F0
	public static void ClearSelection()
	{
		if (SelectedItem.selectedItem != null)
		{
			SelectedItem.selectedItem.Deselect();
			SelectedItem.selectedItem = null;
			SelectedItem.UpdateItem();
		}
	}

	// Token: 0x060024D6 RID: 9430 RVA: 0x0001CF14 File Offset: 0x0001B114
	internal static void SetHovered(ItemIcon item)
	{
		SelectedItem.hoveredItem = item;
		if (SingletonComponent<SelectedItem>.Instance)
		{
			SingletonComponent<SelectedItem>.Instance.OnInventoryChanged();
		}
	}
}
