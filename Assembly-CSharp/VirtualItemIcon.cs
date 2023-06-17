using System;
using System.Linq;
using Rust.Workshop;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000677 RID: 1655
public class VirtualItemIcon : MonoBehaviour
{
	// Token: 0x040020E0 RID: 8416
	public ItemDefinition itemDef;

	// Token: 0x040020E1 RID: 8417
	public int itemAmount;

	// Token: 0x040020E2 RID: 8418
	public bool asBlueprint;

	// Token: 0x040020E3 RID: 8419
	public Image iconImage;

	// Token: 0x040020E4 RID: 8420
	public Image bpUnderlay;

	// Token: 0x040020E5 RID: 8421
	public Text amountText;

	// Token: 0x040020E6 RID: 8422
	public CanvasGroup iconContents;

	// Token: 0x040020E7 RID: 8423
	public CanvasGroup conditionObject;

	// Token: 0x040020E8 RID: 8424
	public Image conditionFill;

	// Token: 0x040020E9 RID: 8425
	public Image maxConditionFill;

	// Token: 0x040020EA RID: 8426
	public Image cornerIcon;

	// Token: 0x060024EF RID: 9455 RVA: 0x000C3260 File Offset: 0x000C1460
	public void SetVirtualItem(ItemDefinition info, int amount = 0, ulong skinId = 0UL, bool asBP = false)
	{
		if (info == null)
		{
			this.iconContents.alpha = 0f;
			this.amountText.text = "";
			this.asBlueprint = false;
		}
		else
		{
			this.bpUnderlay.enabled = asBP;
			this.iconContents.alpha = 1f;
			this.UpdateIcon(info, skinId);
			this.UpdateAmount(amount);
			Item.UpdateAmountDisplay(this.amountText, null, amount, info);
		}
		this.itemDef = info;
		this.itemAmount = amount;
		this.asBlueprint = asBP;
		this.DisableCondition();
	}

	// Token: 0x060024F0 RID: 9456 RVA: 0x0001D018 File Offset: 0x0001B218
	public void UpdateAmount(int amount)
	{
		if (this.itemDef == null)
		{
			return;
		}
		Item.UpdateAmountDisplay(this.amountText, null, amount, this.itemDef);
		this.itemAmount = amount;
	}

	// Token: 0x060024F1 RID: 9457 RVA: 0x000C32F8 File Offset: 0x000C14F8
	public Sprite GetItemSkinSprite(ItemDefinition info, ulong skin)
	{
		if (skin != 0UL)
		{
			ItemSkinDirectory.Skin skin2 = Enumerable.FirstOrDefault<ItemSkinDirectory.Skin>(info.skins, (ItemSkinDirectory.Skin x) => (long)x.id == (long)skin);
			if ((long)skin2.id == (long)skin)
			{
				return skin2.invItem.icon;
			}
			Sprite sprite = WorkshopIconLoader.Find(skin, info.iconSprite, GlobalMessages.OnItemIconChangedAction);
			if (sprite != null)
			{
				return sprite;
			}
		}
		return info.iconSprite;
	}

	// Token: 0x060024F2 RID: 9458 RVA: 0x0001D043 File Offset: 0x0001B243
	public void UpdateIcon(ItemDefinition info, ulong skinId = 0UL)
	{
		this.iconImage.sprite = this.GetItemSkinSprite(info, skinId);
	}

	// Token: 0x060024F3 RID: 9459 RVA: 0x000C3378 File Offset: 0x000C1578
	public void UpdateCondition(float itemMaxCondition, float itemConditionNormalized)
	{
		if (this.itemDef.condition.enabled)
		{
			this.conditionObject.gameObject.SetActive(true);
			this.conditionObject.alpha = 1f;
			this.maxConditionFill.fillAmount = 1f - itemMaxCondition / this.itemDef.condition.max;
			this.conditionFill.fillAmount = itemConditionNormalized * (1f - this.maxConditionFill.fillAmount);
			if (this.cornerIcon)
			{
				if (itemConditionNormalized <= 0f)
				{
					this.cornerIcon.sprite = FileSystem.Load<Sprite>("Assets/Icons/isbroken.png", true);
					this.cornerIcon.gameObject.SetActive(true);
					return;
				}
				this.cornerIcon.gameObject.SetActive(false);
				return;
			}
		}
		else
		{
			this.DisableCondition();
		}
	}

	// Token: 0x060024F4 RID: 9460 RVA: 0x000C3454 File Offset: 0x000C1654
	public void DisableCondition()
	{
		this.conditionObject.gameObject.SetActive(false);
		this.conditionObject.alpha = 0f;
		if (this.cornerIcon)
		{
			this.cornerIcon.gameObject.SetActive(false);
		}
	}
}
