using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000670 RID: 1648
public class RepairBenchPanel : LootPanel
{
	// Token: 0x040020A6 RID: 8358
	public Text infoText;

	// Token: 0x040020A7 RID: 8359
	public Button repairButton;

	// Token: 0x040020A8 RID: 8360
	public Color gotColor;

	// Token: 0x040020A9 RID: 8361
	public Color notGotColor;

	// Token: 0x040020AA RID: 8362
	public Translate.Phrase phraseEmpty;

	// Token: 0x040020AB RID: 8363
	public Translate.Phrase phraseNotRepairable;

	// Token: 0x040020AC RID: 8364
	public Translate.Phrase phraseRepairNotNeeded;

	// Token: 0x040020AD RID: 8365
	public Translate.Phrase phraseNoBlueprint;

	// Token: 0x040020AE RID: 8366
	public GameObject skinsPanel;

	// Token: 0x040020AF RID: 8367
	public GameObject changeSkinDialog;

	// Token: 0x040020B0 RID: 8368
	public IconSkinPicker picker;

	// Token: 0x040020B1 RID: 8369
	private Item _displayItem;

	// Token: 0x060024B1 RID: 9393 RVA: 0x0001CE05 File Offset: 0x0001B005
	public void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x060024B2 RID: 9394 RVA: 0x0001CE0D File Offset: 0x0001B00D
	public void OnDisable()
	{
		this._displayItem = null;
		this.picker.Refresh(null);
		this.picker.skinChangedEvent = null;
	}

	// Token: 0x060024B3 RID: 9395 RVA: 0x000C1E0C File Offset: 0x000C000C
	public override void Update()
	{
		base.Update();
		ItemContainer container_ = base.Container_00;
		if (container_ == null)
		{
			return;
		}
		Item slot = container_.GetSlot(0);
		if (slot != this._displayItem)
		{
			this._displayItem = slot;
			this.Refresh();
		}
	}

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x060024B4 RID: 9396 RVA: 0x0001CE2E File Offset: 0x0001B02E
	public Item item
	{
		get
		{
			return this._displayItem;
		}
	}

	// Token: 0x060024B5 RID: 9397 RVA: 0x000C1E48 File Offset: 0x000C0048
	public void UpdateAvailableSkins()
	{
		bool flag = this.item != null && this.item.info.HasSkins;
		this.skinsPanel.SetActive(flag);
		if (!flag)
		{
			return;
		}
		this.picker.Refresh(this.item.info.Blueprint);
		this.picker.skinChangedEvent = new Action(this.SkinClicked);
		this.picker.GetComponentInChildren<ScrollRect>().horizontalNormalizedPosition = 0.5f;
	}

	// Token: 0x060024B6 RID: 9398 RVA: 0x000C1EC8 File Offset: 0x000C00C8
	public void SkinClicked()
	{
		if (this.item == null)
		{
			return;
		}
		int @int = PlayerPrefs.GetInt(string.Format("skin.{0}", this.item.info.shortname), 0);
		base.GetContainerEntity<RepairBench>().ChangeSkinTo(@int);
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x000C1F0C File Offset: 0x000C010C
	public void RepairClicked()
	{
		BaseEntity containerEntity = base.GetContainerEntity();
		if (containerEntity == null)
		{
			return;
		}
		containerEntity.SendMessage("TryRepair");
	}

	// Token: 0x060024B8 RID: 9400 RVA: 0x000C1F38 File Offset: 0x000C0138
	private string GetCostText()
	{
		string text = "";
		RepairBench repairBench = base.GetContainerEntity() as RepairBench;
		if (repairBench == null)
		{
			return "!!NO BENCH!!";
		}
		float num = repairBench.RepairCostFraction(this.item);
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(this._displayItem.info);
		if (itemBlueprint == null || LocalPlayer.Entity == null)
		{
			return this.phraseNotRepairable.translated;
		}
		if (!LocalPlayer.HasUnlocked(this.item.info) && this.item.info.Blueprint != null && this.item.info.Blueprint.isResearchable)
		{
			return this.phraseNoBlueprint.translated;
		}
		List<ItemAmount> list = Pool.GetList<ItemAmount>();
		repairBench.GetRepairCostList(itemBlueprint, list);
		foreach (ItemAmount itemAmount in list)
		{
			if (itemAmount.itemDef == null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Blueprint missing ingredient: ",
					itemBlueprint.targetItem.displayName.english,
					" / ",
					itemAmount.itemid
				}));
			}
			else if (itemAmount.itemDef.category != ItemCategory.Component)
			{
				int num2 = Mathf.CeilToInt(itemAmount.amount * num);
				bool flag = LocalPlayer.Entity.inventory.GetAmount(itemAmount.itemDef.itemid) >= num2;
				text = text + string.Format("<color=#{2}>{0:N0} {1}</color> ", num2, itemAmount.itemDef.displayName.translated, flag ? this.gotColor.ToHex() : this.notGotColor.ToHex()) + "\n";
			}
		}
		Pool.FreeList<ItemAmount>(ref list);
		return text;
	}

	// Token: 0x060024B9 RID: 9401 RVA: 0x000C214C File Offset: 0x000C034C
	private void Refresh()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		bool active = false;
		string text;
		if (this.item != null)
		{
			if (this.item.hasCondition && this.item.info.condition.repairable)
			{
				if (LocalPlayer.HasUnlocked(this.item.info) || (this.item.info.Blueprint != null && !this.item.info.Blueprint.isResearchable))
				{
					if (this.item.conditionNormalized < 1f)
					{
						text = this.GetCostText();
						active = true;
					}
					else
					{
						text = this.phraseRepairNotNeeded.translated;
					}
				}
				else
				{
					text = this.phraseNoBlueprint.translated;
				}
			}
			else
			{
				text = this.phraseNotRepairable.translated;
			}
		}
		else
		{
			text = this.phraseEmpty.translated;
		}
		this.UpdateAvailableSkins();
		this.infoText.text = text;
		this.repairButton.gameObject.SetActive(active);
	}

	// Token: 0x060024BA RID: 9402 RVA: 0x00002ECE File Offset: 0x000010CE
	public void UpdateSkinPanel()
	{
	}
}
