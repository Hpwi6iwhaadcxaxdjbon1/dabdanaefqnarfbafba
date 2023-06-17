using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000667 RID: 1639
public class LootPanelWaterCatcher : LootPanel
{
	// Token: 0x0400207B RID: 8315
	public ItemIcon sourceItem;

	// Token: 0x0400207C RID: 8316
	public Image capacityImage;

	// Token: 0x0400207D RID: 8317
	public CanvasGroup helpCanvas;

	// Token: 0x0400207E RID: 8318
	public CanvasGroup buttonsCanvas;

	// Token: 0x0400207F RID: 8319
	public Button fromButton;

	// Token: 0x04002080 RID: 8320
	public Button toButton;

	// Token: 0x04002081 RID: 8321
	public Button drinkButton;

	// Token: 0x06002481 RID: 9345 RVA: 0x000C0D64 File Offset: 0x000BEF64
	public bool CanCopyFrom()
	{
		if (!this.SelectedValidTarget())
		{
			return false;
		}
		if (base.Container_00.itemList.Count == 0)
		{
			return false;
		}
		Item item = base.Container_00.itemList[0];
		if (item.amount == 0)
		{
			return false;
		}
		if (SelectedItem.selectedItem.item.contents.itemList.Count > 0)
		{
			if (SelectedItem.selectedItem.item.contents.itemList[0].info != item.info)
			{
				return false;
			}
			if (SelectedItem.selectedItem.item.contents.itemList[0].amount >= SelectedItem.selectedItem.item.contents.itemList[0].MaxStackable())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x000C0E38 File Offset: 0x000BF038
	public void CopyFrom()
	{
		if (!this.CanCopyFrom())
		{
			return;
		}
		Item item = base.Container_00.itemList[0];
		LocalPlayer.MoveItem(item.uid, SelectedItem.selectedItem.item.contents.uid, 0, Mathf.Min((item.info.maxDraggable > 0) ? item.info.maxDraggable : item.amount, item.amount));
	}

	// Token: 0x06002483 RID: 9347 RVA: 0x000C0EAC File Offset: 0x000BF0AC
	public bool CanCopyTo()
	{
		if (!this.SelectedValidTarget())
		{
			return false;
		}
		if (SelectedItem.selectedItem.item.contents.itemList.Count == 0)
		{
			return false;
		}
		Item item = SelectedItem.selectedItem.item.contents.itemList[0];
		if (base.Container_00.itemList.Count > 0)
		{
			Item item2 = base.Container_00.itemList[0];
			if (item.info != item2.info)
			{
				return false;
			}
			if (item2.amount >= item2.MaxStackable())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002484 RID: 9348 RVA: 0x000C0F48 File Offset: 0x000BF148
	public void CopyTo()
	{
		if (!this.CanCopyTo())
		{
			return;
		}
		Item item = SelectedItem.selectedItem.item.contents.itemList[0];
		LocalPlayer.MoveItem(item.uid, base.Container_00.uid, 0, Mathf.Min((item.info.maxDraggable > 0) ? item.info.maxDraggable : item.amount, item.amount));
	}

	// Token: 0x06002485 RID: 9349 RVA: 0x000C0FBC File Offset: 0x000BF1BC
	public override void Update()
	{
		base.Update();
		if (base.Container_00 == null)
		{
			return;
		}
		LiquidContainer liquidContainer = this.GetLiquidContainer();
		bool flag = this.SelectedValidTarget();
		this.buttonsCanvas.alpha = (flag ? 1f : 0f);
		this.helpCanvas.alpha = (flag ? 0f : 1f);
		this.fromButton.interactable = this.CanCopyFrom();
		this.toButton.interactable = this.CanCopyTo();
		if (base.Container_00.itemList.Count > 0)
		{
			this.capacityImage.fillAmount = (float)base.Container_00.itemList[0].amount / (float)liquidContainer.maxStackSize;
		}
		else
		{
			this.capacityImage.fillAmount = 0f;
		}
		this.drinkButton.gameObject.SetActive(this.CanDrink());
	}

	// Token: 0x06002486 RID: 9350 RVA: 0x0001CCB6 File Offset: 0x0001AEB6
	public bool SelectedValidTarget()
	{
		return !(SelectedItem.selectedItem == null) && SelectedItem.selectedItem.item != null && SelectedItem.selectedItem.item.contents != null;
	}

	// Token: 0x06002487 RID: 9351 RVA: 0x0001CCE9 File Offset: 0x0001AEE9
	public LiquidContainer GetLiquidContainer()
	{
		return base.GetContainerEntity() as LiquidContainer;
	}

	// Token: 0x06002488 RID: 9352 RVA: 0x0001CCF6 File Offset: 0x0001AEF6
	public bool CanDrink()
	{
		return !(this.GetLiquidContainer() == null) && this.GetLiquidContainer().MenuDrink_ShowIf(LocalPlayer.Entity);
	}

	// Token: 0x06002489 RID: 9353 RVA: 0x0001CD18 File Offset: 0x0001AF18
	public void Drink()
	{
		if (this.GetLiquidContainer() == null)
		{
			return;
		}
		this.GetLiquidContainer().MenuDrink(LocalPlayer.Entity);
	}
}
