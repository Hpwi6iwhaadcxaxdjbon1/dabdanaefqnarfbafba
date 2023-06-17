using System;
using System.Text;
using Rust;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000642 RID: 1602
public class BlueprintButton : MonoBehaviour, IClientComponent, IInventoryChanged, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x04001FCE RID: 8142
	public new Text name;

	// Token: 0x04001FCF RID: 8143
	public Text subtitle;

	// Token: 0x04001FD0 RID: 8144
	public Image image;

	// Token: 0x04001FD1 RID: 8145
	public Button button;

	// Token: 0x04001FD2 RID: 8146
	public CanvasGroup group;

	// Token: 0x04001FD3 RID: 8147
	public GameObject newNotification;

	// Token: 0x04001FD4 RID: 8148
	public string gotColor = "#ffffff";

	// Token: 0x04001FD5 RID: 8149
	public string notGotColor = "#ff0000";

	// Token: 0x04001FD6 RID: 8150
	public float craftableFraction;

	// Token: 0x04001FD7 RID: 8151
	public GameObject lockedOverlay;

	// Token: 0x04001FD8 RID: 8152
	[Header("Locked")]
	public CanvasGroup LockedGroup;

	// Token: 0x04001FD9 RID: 8153
	public Text LockedPrice;

	// Token: 0x04001FDA RID: 8154
	public Image LockedImageBackground;

	// Token: 0x04001FDB RID: 8155
	public Color LockedCannotUnlockColor;

	// Token: 0x04001FDC RID: 8156
	public Color LockedCanUnlockColor;

	// Token: 0x04001FDD RID: 8157
	[Header("Unlock Level")]
	public GameObject LockedLevel;

	// Token: 0x04001FDE RID: 8158
	internal bool needsRefresh;

	// Token: 0x04001FDF RID: 8159
	internal ItemBlueprint blueprint;

	// Token: 0x060023B3 RID: 9139 RVA: 0x000BD64C File Offset: 0x000BB84C
	public void Setup(ItemBlueprint bp)
	{
		this.blueprint = bp;
		string translated = bp.targetItem.displayName.translated;
		if (this.name.text != translated)
		{
			this.name.text = translated;
		}
		if (this.image.sprite != bp.targetItem.iconSprite)
		{
			this.image.sprite = bp.targetItem.iconSprite;
		}
		this.subtitle.text = "...";
		Tooltip component = base.GetComponent<Tooltip>();
		if (component != null)
		{
			component.Text = translated.ToUpper();
		}
		this.needsRefresh = false;
		this.Refresh();
		this.UpdateSelection();
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x0001C377 File Offset: 0x0001A577
	public void Awake()
	{
		GlobalMessages.onInventoryChanged.Add(this);
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x0001C384 File Offset: 0x0001A584
	public void OnDestroy()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onInventoryChanged.Remove(this);
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x0001C39A File Offset: 0x0001A59A
	private void Update()
	{
		if (!UICrafting.isOpen)
		{
			return;
		}
		if (this.needsRefresh)
		{
			this.needsRefresh = false;
			this.Refresh();
		}
		this.UpdateSelection();
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x000BD704 File Offset: 0x000BB904
	private void UpdateSelection()
	{
		bool flag = false;
		if (this.button.interactable != !flag)
		{
			this.button.interactable = !flag;
		}
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x000BD734 File Offset: 0x000BB934
	private void Refresh()
	{
		if (this.blueprint == null || LocalPlayer.Entity == null)
		{
			return;
		}
		this.needsRefresh = false;
		float num = 0f;
		float num2 = 0f;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ItemAmount itemAmount in this.blueprint.ingredients)
		{
			if (itemAmount.itemDef == null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Blueprint missing ingredient: ",
					this.blueprint.targetItem.displayName.english,
					" / ",
					itemAmount.itemid
				}));
			}
			else
			{
				int amount = LocalPlayer.Entity.inventory.GetAmount(itemAmount.itemDef.itemid);
				bool flag = (float)amount >= itemAmount.amount;
				num += Mathf.Min((float)amount, itemAmount.amount);
				num2 += itemAmount.amount;
				stringBuilder.AppendFormat("<color={2}>{0:N0} {1}</color>, ", itemAmount.amount, itemAmount.itemDef.displayName.translated, flag ? this.gotColor : this.notGotColor);
			}
		}
		bool flag2 = LocalPlayer.HasUnlocked(this.blueprint.targetItem);
		string text = stringBuilder.ToString();
		this.subtitle.text = text.TrimEnd(new char[]
		{
			',',
			' '
		});
		this.craftableFraction = 0f;
		if (num > 0f)
		{
			this.craftableFraction = num / num2;
		}
		this.group.alpha = ((this.craftableFraction >= 1f && flag2) ? 1f : 0.5f);
		this.lockedOverlay.SetActive(!this.blueprint.defaultBlueprint && !flag2);
		this.newNotification.SetActive(BasePlayer.craftMode == 0 && LocalPlayer.GetCraftCount(this.blueprint) < 2 && !this.blueprint.defaultBlueprint && flag2);
		base.GetComponentInParent<UIBlueprints>().needsResort = true;
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x0001C3BF File Offset: 0x0001A5BF
	public void OnInventoryChanged()
	{
		this.needsRefresh = true;
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x0001C3C8 File Offset: 0x0001A5C8
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == null)
		{
			if (eventData.clickCount > 1)
			{
				SelectedBlueprint.UpdateBlueprint(this.blueprint);
				SingletonComponent<SelectedBlueprint>.Instance.StartCrafting();
				return;
			}
			SelectedBlueprint.UpdateBlueprint(this.blueprint);
		}
	}
}
