using System;
using System.Collections;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000655 RID: 1621
public class ItemIcon : BaseMonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IDraggable, IInventoryChanged, IItemAmountChanged, IItemIconChanged
{
	// Token: 0x04002033 RID: 8243
	public static Color defaultBackgroundColor = new Color(0.96862745f, 0.92156863f, 0.88235295f, 0.03529412f);

	// Token: 0x04002034 RID: 8244
	public static Color selectedBackgroundColor = new Color(0.12156863f, 0.41960785f, 0.627451f, 0.78431374f);

	// Token: 0x04002035 RID: 8245
	public ItemContainerSource containerSource;

	// Token: 0x04002036 RID: 8246
	public int slotOffset;

	// Token: 0x04002037 RID: 8247
	[Range(0f, 64f)]
	public int slot;

	// Token: 0x04002038 RID: 8248
	public bool setSlotFromSiblingIndex = true;

	// Token: 0x04002039 RID: 8249
	public GameObject slots;

	// Token: 0x0400203A RID: 8250
	public CanvasGroup iconContents;

	// Token: 0x0400203B RID: 8251
	public Image iconImage;

	// Token: 0x0400203C RID: 8252
	public Image underlayImage;

	// Token: 0x0400203D RID: 8253
	public Text amountText;

	// Token: 0x0400203E RID: 8254
	public Image hoverOutline;

	// Token: 0x0400203F RID: 8255
	public Image cornerIcon;

	// Token: 0x04002040 RID: 8256
	public Image lockedImage;

	// Token: 0x04002041 RID: 8257
	public Image progressImage;

	// Token: 0x04002042 RID: 8258
	public Image backgroundImage;

	// Token: 0x04002043 RID: 8259
	public CanvasGroup conditionObject;

	// Token: 0x04002044 RID: 8260
	public Image conditionFill;

	// Token: 0x04002045 RID: 8261
	public Image maxConditionFill;

	// Token: 0x04002046 RID: 8262
	public bool allowSelection = true;

	// Token: 0x04002047 RID: 8263
	public bool allowDropping = true;

	// Token: 0x04002048 RID: 8264
	[NonSerialized]
	public Item item;

	// Token: 0x04002049 RID: 8265
	[NonSerialized]
	public bool invalidSlot;

	// Token: 0x0400204A RID: 8266
	public SoundDefinition hoverSound;

	// Token: 0x0400204B RID: 8267
	internal Image[] slotImages;

	// Token: 0x0400204C RID: 8268
	public Action timedAction;

	// Token: 0x0400204D RID: 8269
	public Vector2? timedActionTime;

	// Token: 0x0600241E RID: 9246 RVA: 0x0001C821 File Offset: 0x0001AA21
	protected void Awake()
	{
		this.slotImages = this.slots.GetComponentsInChildren<Image>(true);
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x000BF538 File Offset: 0x000BD738
	protected void OnEnable()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.item = null;
		GlobalMessages.onInventoryChanged.Add(this);
		GlobalMessages.onItemAmountChanged.Add(this);
		GlobalMessages.onItemIconChanged.Add(this);
		this.Deselect();
		this.backgroundImage.color = ItemIcon.defaultBackgroundColor;
		if (this.progressImage)
		{
			this.progressImage.fillAmount = 0f;
			if (this.progressImage.gameObject.activeSelf)
			{
				this.progressImage.gameObject.SetActive(false);
			}
		}
		base.Invoke(new Action(this.OnInventoryChanged), 0f);
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x000BF5E4 File Offset: 0x000BD7E4
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onInventoryChanged.Remove(this);
		GlobalMessages.onItemAmountChanged.Remove(this);
		GlobalMessages.onItemIconChanged.Remove(this);
		this.ClearTimedAction();
		if (this.hoverOutline)
		{
			this.hoverOutline.enabled = false;
		}
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x000BF63C File Offset: 0x000BD83C
	public Sprite GetItemSprite()
	{
		if (this.item.IsOnFire())
		{
			return FileSystem.Load<Sprite>("Assets/Icons/isonfire.png", true);
		}
		if (this.item.IsCooking())
		{
			return FileSystem.Load<Sprite>("Assets/Icons/iscooking.png", true);
		}
		if (this.item.isBroken)
		{
			return FileSystem.Load<Sprite>("Assets/Icons/isbroken.png", true);
		}
		if (this.item.isLoadingIconSprite)
		{
			return FileSystem.Load<Sprite>("Assets/Icons/isloading.png", true);
		}
		if (this.item.instanceData != null && this.item.instanceData.subEntity != 0U)
		{
			BaseEntity baseEntity = BaseNetworkable.clientEntities.Find(this.item.instanceData.subEntity) as BaseEntity;
			if (baseEntity && baseEntity.IsOn())
			{
				return FileSystem.Load<Sprite>("Assets/Icons/iscooking.png", true);
			}
		}
		if (this.item.contents != null && this.item.contents.capacity == 1 && this.item.contents.allowedContents == ItemContainer.ContentsType.Liquid && this.item.contents.itemList.Count > 0)
		{
			return this.item.contents.itemList[0].info.iconSprite;
		}
		return null;
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x0001C835 File Offset: 0x0001AA35
	public void OnItemAmountChanged()
	{
		if (!this.invalidSlot && this.item != null)
		{
			this.item.UpdateAmountDisplay(this.amountText);
		}
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x0001C858 File Offset: 0x0001AA58
	public void OnItemIconChanged()
	{
		if (!this.invalidSlot && this.item != null && this.iconImage.sprite != this.item.iconSprite)
		{
			this.UpdateItemIcon();
			this.UpdateCornerIcon();
		}
	}

	// Token: 0x06002424 RID: 9252 RVA: 0x000BF778 File Offset: 0x000BD978
	public void OnInventoryChanged()
	{
		Item item = this.item;
		this.item = this.iconValue;
		if (this.invalidSlot)
		{
			this.iconContents.alpha = 0f;
			SelectedItem.ClearIfSelected(this);
			this.lockedImage.gameObject.SetActive(true);
			return;
		}
		if (this.item != null)
		{
			if (item != null && this.item.uid != item.uid)
			{
				SelectedItem.ClearIfSelected(this);
				this.ClearTimedAction();
			}
			this.iconContents.alpha = 1f;
			this.UpdateItemIcon();
			if (this.underlayImage != null)
			{
				if (this.item.IsBlueprint())
				{
					ItemDefinition itemDefinition = ItemManager.FindItemDefinition(this.item.blueprintTarget);
					if (itemDefinition != null)
					{
						this.underlayImage.sprite = this.item.iconSprite;
						this.underlayImage.enabled = true;
						this.iconImage.sprite = itemDefinition.iconSprite;
						Color color = this.iconImage.color;
						color.a = 1f;
						this.iconImage.color = color;
					}
				}
				else
				{
					Color color2 = this.iconImage.color;
					color2.a = 0.84313726f;
					this.iconImage.color = color2;
					this.underlayImage.enabled = false;
				}
			}
			else
			{
				Debug.Log("No underlay image on :" + base.name);
			}
			this.UpdateCornerIcon();
			this.lockedImage.gameObject.SetActive(this.item.IsLocked());
			if (this.item.hasCondition)
			{
				this.conditionObject.gameObject.SetActive(true);
				this.conditionObject.alpha = 1f;
				this.maxConditionFill.fillAmount = 1f - this.item.maxCondition / this.item.info.condition.max;
				this.conditionFill.fillAmount = this.item.conditionNormalized * (1f - this.maxConditionFill.fillAmount);
			}
			else
			{
				this.conditionObject.gameObject.SetActive(false);
			}
			this.item.UpdateAmountDisplay(this.amountText);
			this.UpdateSlots();
			return;
		}
		ItemContainer itemContainer = (this.containerSource != null) ? this.containerSource.GetItemContainer() : null;
		bool active = itemContainer != null && itemContainer.IsLocked();
		this.lockedImage.gameObject.SetActive(active);
		this.iconContents.alpha = 0f;
		SelectedItem.ClearIfSelected(this);
		this.slots.SetActive(false);
	}

	// Token: 0x06002425 RID: 9253 RVA: 0x0001C893 File Offset: 0x0001AA93
	private void UpdateItemIcon()
	{
		this.iconImage.sprite = this.item.iconSprite;
	}

	// Token: 0x06002426 RID: 9254 RVA: 0x000BFA1C File Offset: 0x000BDC1C
	private void UpdateCornerIcon()
	{
		Sprite itemSprite = this.GetItemSprite();
		if (itemSprite == null)
		{
			this.cornerIcon.gameObject.SetActive(false);
			return;
		}
		this.cornerIcon.sprite = itemSprite;
		this.cornerIcon.gameObject.SetActive(true);
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x000BFA68 File Offset: 0x000BDC68
	private void UpdateSlots()
	{
		if (this.item.contents == null || this.item.contents.capacity <= 0 || !this.item.contents.HasFlag(ItemContainer.Flag.ShowSlotsOnIcon))
		{
			this.slots.SetActive(false);
			return;
		}
		int capacity = this.item.contents.capacity;
		int count = this.item.contents.itemList.Count;
		this.slots.SetActive(true);
		for (int i = 0; i < this.slotImages.Length; i++)
		{
			if (i > capacity)
			{
				this.slotImages[i].gameObject.SetActive(false);
			}
			else
			{
				this.slotImages[i].gameObject.SetActive(true);
				this.slotImages[i].color = ((i > count) ? new Color(1f, 1f, 1f, 0.15f) : new Color(1f, 1f, 1f, 0.75f));
			}
		}
	}

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06002428 RID: 9256 RVA: 0x000BFB70 File Offset: 0x000BDD70
	private Item iconValue
	{
		get
		{
			Item result = null;
			if (this.containerSource != null)
			{
				ItemContainer itemContainer = this.containerSource.GetItemContainer();
				if (itemContainer != null)
				{
					result = itemContainer.GetSlot(this.slot);
					this.invalidSlot = (itemContainer.capacity <= this.slot);
				}
			}
			return result;
		}
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x000BFBC4 File Offset: 0x000BDDC4
	public void Select()
	{
		if (this.item != null && this.item.info.inventorySelectSound != null)
		{
			SoundManager.PlayOneshot(this.item.info.inventorySelectSound, null, true, default(Vector3));
		}
		this.backgroundImage.color = ItemIcon.selectedBackgroundColor;
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x0001C8AB File Offset: 0x0001AAAB
	public void Deselect()
	{
		this.backgroundImage.color = ItemIcon.defaultBackgroundColor;
	}

	// Token: 0x0600242B RID: 9259 RVA: 0x0001C8BD File Offset: 0x0001AABD
	public void SetActive(bool active)
	{
		if (active)
		{
			this.backgroundImage.color = ItemIcon.selectedBackgroundColor;
			return;
		}
		this.backgroundImage.color = ItemIcon.defaultBackgroundColor;
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x000BFC24 File Offset: 0x000BDE24
	public virtual void OnDroppedValue(ItemIcon.DragInfo dropInfo)
	{
		if (ConVar.Client.debugdragdrop)
		{
			Debug.Log("[dragdrop] ItemIcon.OnDroppedValue " + dropInfo.item);
		}
		if (dropInfo.item == null)
		{
			return;
		}
		ItemContainer itemContainer = this.containerSource.GetItemContainer();
		if (itemContainer == null)
		{
			return;
		}
		if (ConVar.Client.debugdragdrop)
		{
			Debug.Log("[dragdrop] ItemIcon.MoveItem");
		}
		if (dropInfo.item.info.inventoryDropSound != null)
		{
			SoundManager.PlayOneshot(dropInfo.item.info.inventoryDropSound, null, true, default(Vector3));
		}
		if (this.TryToDropInsideItem(dropInfo, this.item))
		{
			return;
		}
		LocalPlayer.MoveItem(dropInfo.item.uid, itemContainer.uid, this.slot, dropInfo.amount);
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x000BFCE4 File Offset: 0x000BDEE4
	private bool TryToDropInsideItem(ItemIcon.DragInfo dropInfo, Item item)
	{
		if (item == null)
		{
			return false;
		}
		if (item.contents == null)
		{
			return false;
		}
		if (item.contents.capacity == 0)
		{
			return false;
		}
		switch (item.contents.CanAcceptItem(dropInfo.item, -1))
		{
		case ItemContainer.CanAcceptResult.CannotAccept:
			return false;
		case ItemContainer.CanAcceptResult.CannotAcceptRightNow:
			return true;
		}
		LocalPlayer.MoveItem(dropInfo.item.uid, item.contents.uid, -1, dropInfo.amount);
		return true;
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x0001C8E3 File Offset: 0x0001AAE3
	public void ClearTimedAction()
	{
		this.timedAction = null;
		this.timedActionTime = default(Vector2?);
		base.CancelInvoke(new Action(this.RunTimedAction));
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000BFD60 File Offset: 0x000BDF60
	public void SetTimedAction(float time, Action action)
	{
		this.timedAction = action;
		this.timedActionTime = new Vector2?(new Vector2(UnityEngine.Time.realtimeSinceStartup, UnityEngine.Time.realtimeSinceStartup + time));
		base.StartCoroutine(this.UpdateTimedActionIcon());
		base.Invoke(new Action(this.RunTimedAction), time);
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x0001C90A File Offset: 0x0001AB0A
	public void RunTimedAction()
	{
		if (this.timedAction != null)
		{
			this.timedAction.Invoke();
		}
		this.ClearTimedAction();
	}

	// Token: 0x06002431 RID: 9265 RVA: 0x0001C925 File Offset: 0x0001AB25
	private IEnumerator UpdateTimedActionIcon()
	{
		if (!this.progressImage)
		{
			yield break;
		}
		while (this.timedAction != null)
		{
			this.progressImage.gameObject.SetActive(true);
			if (this.timedActionTime != null)
			{
				this.progressImage.fillAmount = Mathf.InverseLerp(this.timedActionTime.Value.x, this.timedActionTime.Value.y, UnityEngine.Time.realtimeSinceStartup);
			}
			yield return null;
		}
		this.progressImage.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06002432 RID: 9266 RVA: 0x000BFDB0 File Offset: 0x000BDFB0
	public virtual void TryToMove()
	{
		if (this.item == null)
		{
			return;
		}
		ItemContainer itemContainer = this.containerSource.GetItemContainer();
		if (itemContainer == null)
		{
			return;
		}
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		PlayerLoot loot = LocalPlayer.Entity.inventory.loot;
		bool flag = loot.containers != null && loot.containers.Count > 0;
		if (!flag && (itemContainer.HasFlag(ItemContainer.Flag.Clothing) || itemContainer.HasFlag(ItemContainer.Flag.Belt)))
		{
			this.SetTimedAction(0.2f, delegate
			{
				if (this.item == null)
				{
					return;
				}
				if (LocalPlayer.Entity == null)
				{
					return;
				}
				if (LocalPlayer.Entity.inventory.containerMain == null)
				{
					return;
				}
				LocalPlayer.MoveItem(this.item.uid, LocalPlayer.Entity.inventory.containerMain.uid, -1, this.item.amount);
				if (this.item.info.inventoryDropSound != null)
				{
					SoundManager.PlayOneshot(this.item.info.inventoryDropSound, null, true, default(Vector3));
				}
			});
			if (this.item.info.inventoryGrabSound != null)
			{
				SoundManager.PlayOneshot(this.item.info.inventoryGrabSound, null, true, default(Vector3));
			}
			return;
		}
		if (!flag && !itemContainer.HasFlag(ItemContainer.Flag.Clothing) && this.item.info.isWearable)
		{
			this.SetTimedAction(0.2f, delegate
			{
				if (this.item == null)
				{
					return;
				}
				if (LocalPlayer.Entity == null)
				{
					return;
				}
				if (LocalPlayer.Entity.inventory.containerWear == null)
				{
					return;
				}
				LocalPlayer.MoveItem(this.item.uid, LocalPlayer.Entity.inventory.containerWear.uid, -1, this.item.amount);
				if (this.item.info.inventoryDropSound != null)
				{
					SoundManager.PlayOneshot(this.item.info.inventoryDropSound, null, true, default(Vector3));
				}
			});
			if (this.item.info.inventoryGrabSound != null)
			{
				SoundManager.PlayOneshot(this.item.info.inventoryGrabSound, null, true, default(Vector3));
			}
		}
		if (flag && loot.containers.Contains(itemContainer))
		{
			this.SetTimedAction(1f, delegate
			{
				if (this.item == null)
				{
					return;
				}
				if (LocalPlayer.Entity == null)
				{
					return;
				}
				if (LocalPlayer.Entity.inventory.containerMain == null)
				{
					return;
				}
				LocalPlayer.MoveItem(this.item.uid, 0U, -1, this.item.amount);
				if (this.item.info.inventoryDropSound != null)
				{
					SoundManager.PlayOneshot(this.item.info.inventoryDropSound, null, true, default(Vector3));
				}
			});
			if (this.item.info.inventoryGrabSound != null)
			{
				SoundManager.PlayOneshot(this.item.info.inventoryGrabSound, null, true, default(Vector3));
			}
			return;
		}
		if (flag)
		{
			this.SetTimedAction(1f, delegate
			{
				if (this.item == null)
				{
					return;
				}
				if (loot.containers == null)
				{
					return;
				}
				if (loot.containers.Count == 0)
				{
					return;
				}
				if (loot.GetClientEntity() == null)
				{
					return;
				}
				int targetSlot = -1;
				int num = 0;
				StorageContainer component = loot.GetClientEntity().GetComponent<StorageContainer>();
				if (component != null)
				{
					num = component.GetMoveToContainerIndex(LocalPlayer.Entity);
					targetSlot = component.GetMoveToSlotIndex(LocalPlayer.Entity);
					if (num > loot.containers.Count - 1)
					{
						Debug.LogWarning("Tried to move an item into a container with a higher index than exists!");
						return;
					}
				}
				LocalPlayer.MoveItem(this.item.uid, loot.containers[num].uid, targetSlot, this.item.amount);
				if (this.item.info.inventoryDropSound != null)
				{
					SoundManager.PlayOneshot(this.item.info.inventoryDropSound, null, true, default(Vector3));
				}
			});
			if (this.item.info.inventoryGrabSound != null)
			{
				SoundManager.PlayOneshot(this.item.info.inventoryGrabSound, null, true, default(Vector3));
			}
			return;
		}
	}

	// Token: 0x06002433 RID: 9267 RVA: 0x0001C934 File Offset: 0x0001AB34
	public virtual void OnPointerClick(PointerEventData eventData)
	{
		PointerEventData.InputButton button = eventData.button;
		if (button == null && this.allowSelection)
		{
			SelectedItem.TrySelect(this);
		}
		if (button == 1)
		{
			this.TryToMove();
		}
	}

	// Token: 0x06002434 RID: 9268 RVA: 0x000BFFBC File Offset: 0x000BE1BC
	public void OnPointerEnter(PointerEventData eventData)
	{
		base.transform.localScale = Vector3.one;
		LeanTween.scale(base.gameObject, Vector3.one * 1.1f, 0.3f).setEase(TweenMode.Punch);
		if (this.hoverSound != null)
		{
			SoundManager.PlayOneshot(this.hoverSound, null, true, default(Vector3));
		}
		if (SelectedBlueprint.isOpen)
		{
			return;
		}
		if (DragMe.dragging)
		{
			return;
		}
		if (this.item == null)
		{
			return;
		}
		if (this.hoverOutline)
		{
			this.hoverOutline.enabled = true;
		}
	}

	// Token: 0x06002435 RID: 9269 RVA: 0x0001C956 File Offset: 0x0001AB56
	public void OnPointerExit(PointerEventData eventData)
	{
		if (this.hoverOutline)
		{
			this.hoverOutline.enabled = false;
		}
	}

	// Token: 0x06002436 RID: 9270 RVA: 0x000C0060 File Offset: 0x000BE260
	public object GetDragData()
	{
		this.OnInventoryChanged();
		ItemIcon.DragInfo dragInfo = new ItemIcon.DragInfo();
		if (this.item == null)
		{
			return null;
		}
		dragInfo.item = this.item;
		dragInfo.amount = this.item.amount;
		dragInfo.canDrop = this.allowDropping;
		if (this.item.info.maxDraggable > 0)
		{
			dragInfo.amount = Mathf.Min(this.item.info.maxDraggable, this.item.amount);
		}
		if (UnityEngine.Input.GetMouseButton(1))
		{
			dragInfo.amount = 1;
		}
		if (UnityEngine.Input.GetMouseButton(2))
		{
			dragInfo.amount = Mathf.Max(dragInfo.amount / 2, 1);
		}
		if (this.item.info.inventoryGrabSound != null)
		{
			SoundManager.PlayOneshot(this.item.info.inventoryGrabSound, null, true, default(Vector3));
		}
		return dragInfo;
	}

	// Token: 0x06002437 RID: 9271 RVA: 0x0001C971 File Offset: 0x0001AB71
	public string GetDragType()
	{
		return "item";
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x0001C978 File Offset: 0x0001AB78
	public Sprite GetDragSprite()
	{
		if (this.item == null)
		{
			return null;
		}
		return this.iconImage.sprite;
	}

	// Token: 0x02000656 RID: 1622
	public class DragInfo
	{
		// Token: 0x0400204E RID: 8270
		public Item item;

		// Token: 0x0400204F RID: 8271
		public int amount;

		// Token: 0x04002050 RID: 8272
		public bool canDrop;
	}
}
