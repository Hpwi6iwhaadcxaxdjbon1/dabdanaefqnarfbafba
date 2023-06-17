using System;
using Facepunch.Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000676 RID: 1654
public class UIInventory : SingletonComponent<UIInventory>
{
	// Token: 0x040020D9 RID: 8409
	public Text PlayerName;

	// Token: 0x040020DA RID: 8410
	public static bool isOpen;

	// Token: 0x040020DB RID: 8411
	public static float LastOpened;

	// Token: 0x040020DC RID: 8412
	public VerticalLayoutGroup rightContents;

	// Token: 0x040020DD RID: 8413
	public GameObject QuickCraft;

	// Token: 0x040020DE RID: 8414
	private GameObject Inset;

	// Token: 0x040020DF RID: 8415
	private float returnX;

	// Token: 0x060024E6 RID: 9446 RVA: 0x000C2F2C File Offset: 0x000C112C
	protected override void Awake()
	{
		base.Awake();
		SingletonComponent<UIInventory>.Instance.GetComponent<GraphicRaycaster>().enabled = false;
		SingletonComponent<UIInventory>.Instance.GetComponent<CanvasGroup>().alpha = 0f;
		this.Inset = base.transform.GetChild(0).gameObject;
		Vector3 localPosition = this.Inset.transform.localPosition;
		this.returnX = localPosition.x;
		localPosition.x = -256f;
		this.Inset.transform.localPosition = localPosition;
	}

	// Token: 0x060024E7 RID: 9447 RVA: 0x000C2FB4 File Offset: 0x000C11B4
	public static void Open()
	{
		if (SingletonComponent<UIInventory>.Instance)
		{
			UICrafting.Close();
			if (UIInventory.isOpen)
			{
				return;
			}
			UIInventory.isOpen = true;
			LeanTween.cancel(SingletonComponent<UIInventory>.Instance.gameObject);
			LeanTween.cancel(SingletonComponent<UIInventory>.Instance.Inset);
			SingletonComponent<UIInventory>.Instance.GetComponent<GraphicRaycaster>().enabled = true;
			LeanTween.alphaCanvas(SingletonComponent<UIInventory>.Instance.GetComponent<CanvasGroup>(), 1f, 0.1f).setEase(27);
			LeanTween.moveLocalX(SingletonComponent<UIInventory>.Instance.Inset, SingletonComponent<UIInventory>.Instance.returnX, 0.1f).setEase(15);
			SelectedItem.ClearSelection();
			SingletonComponent<UIInventory>.Instance.PlayerName.text = (LocalPlayer.Entity ? LocalPlayer.Entity.displayName : "");
			Analytics.InventoryOpened++;
			UIInventory.LastOpened = Time.realtimeSinceStartup;
		}
	}

	// Token: 0x060024E8 RID: 9448 RVA: 0x0001CFE9 File Offset: 0x0001B1E9
	public static void ToggleInventory()
	{
		if (UIInventory.isOpen)
		{
			UIInventory.Close();
			return;
		}
		UIInventory.Open();
	}

	// Token: 0x060024E9 RID: 9449 RVA: 0x000C30A0 File Offset: 0x000C12A0
	public static void Close()
	{
		if (SingletonComponent<UIInventory>.Instance)
		{
			if (!UIInventory.isOpen)
			{
				return;
			}
			UIInventory.isOpen = false;
			SingletonComponent<UIInventory>.Instance.GetComponent<GraphicRaycaster>().enabled = false;
			LeanTween.cancel(SingletonComponent<UIInventory>.Instance.gameObject);
			LeanTween.cancel(SingletonComponent<UIInventory>.Instance.Inset);
			LeanTween.alphaCanvas(SingletonComponent<UIInventory>.Instance.GetComponent<CanvasGroup>(), 0f, 0.2f).setEase(6);
			LeanTween.moveLocalX(SingletonComponent<UIInventory>.Instance.Inset, -256f, 0.2f).setEase(15);
			SelectedItem.ClearSelection();
			Client.EventSystem.SetSelectedGameObject(null);
		}
		UIInventory.PlayCloseSound();
		LocalPlayer.EndLooting();
	}

	// Token: 0x060024EA RID: 9450 RVA: 0x000C3154 File Offset: 0x000C1354
	private static void PlayCloseSound()
	{
		if (!LocalPlayer.Entity)
		{
			return;
		}
		if (!LocalPlayer.Entity.inventory.loot.IsLooting())
		{
			return;
		}
		StorageContainer storageContainer = LocalPlayer.Entity.inventory.loot.GetClientEntity() as StorageContainer;
		if (storageContainer != null && storageContainer.closeSound != null)
		{
			SoundManager.PlayOneshot(storageContainer.closeSound, storageContainer.gameObject, false, default(Vector3));
		}
	}

	// Token: 0x060024EB RID: 9451 RVA: 0x0001CFFD File Offset: 0x0001B1FD
	public static void OpenLoot(string lootType)
	{
		LootPanelContainer.containerName = lootType;
		Analytics.IncrementLootOpened(lootType);
		UIInventory.Open();
	}

	// Token: 0x060024EC RID: 9452 RVA: 0x000C31D4 File Offset: 0x000C13D4
	private void Update()
	{
		if (!this.ShouldShow())
		{
			UIInventory.Close();
		}
		if (UIInventory.isOpen)
		{
			IngameMenuBackground.Enabled = true;
		}
		bool flag = LocalPlayer.Entity != null && !LocalPlayer.Entity.inventory.loot.IsLooting();
		if (this.QuickCraft.activeSelf != flag)
		{
			this.QuickCraft.SetActive(flag);
			this.rightContents.gameObject.SetActive(false);
			this.rightContents.gameObject.SetActive(true);
		}
	}

	// Token: 0x060024ED RID: 9453 RVA: 0x0001CF6A File Offset: 0x0001B16A
	private bool ShouldShow()
	{
		return !(LocalPlayer.Entity == null) && !LocalPlayer.Entity.IsSleeping() && !LocalPlayer.Entity.IsDead() && !LocalPlayer.Entity.IsSpectating();
	}
}
