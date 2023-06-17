using System;
using Facepunch.Rust;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000674 RID: 1652
public class UICrafting : SingletonComponent<UICrafting>
{
	// Token: 0x040020D3 RID: 8403
	public static bool isOpen;

	// Token: 0x040020D4 RID: 8404
	public static float LastOpened;

	// Token: 0x040020D5 RID: 8405
	private GameObject Inset;

	// Token: 0x040020D6 RID: 8406
	private float returnX;

	// Token: 0x060024D8 RID: 9432 RVA: 0x000C2C94 File Offset: 0x000C0E94
	protected override void Awake()
	{
		base.Awake();
		SingletonComponent<UICrafting>.Instance.GetComponent<GraphicRaycaster>().enabled = false;
		SingletonComponent<UICrafting>.Instance.GetComponent<CanvasGroup>().alpha = 0f;
		this.Inset = base.transform.GetChild(0).gameObject;
		Vector3 localPosition = this.Inset.transform.localPosition;
		this.returnX = localPosition.x;
		localPosition.x = 256f;
		this.Inset.transform.localPosition = localPosition;
	}

	// Token: 0x060024D9 RID: 9433 RVA: 0x000C2D1C File Offset: 0x000C0F1C
	public static void Open()
	{
		if (SingletonComponent<UICrafting>.Instance)
		{
			UIInventory.Close();
			if (UICrafting.isOpen)
			{
				return;
			}
			UICrafting.isOpen = true;
			LeanTween.cancel(SingletonComponent<UICrafting>.Instance.gameObject);
			LeanTween.cancel(SingletonComponent<UICrafting>.Instance.Inset);
			SingletonComponent<UICrafting>.Instance.GetComponent<GraphicRaycaster>().enabled = true;
			LeanTween.alphaCanvas(SingletonComponent<UICrafting>.Instance.GetComponent<CanvasGroup>(), 1f, 0.1f).setEase(27);
			LeanTween.moveLocalX(SingletonComponent<UICrafting>.Instance.Inset, SingletonComponent<UICrafting>.Instance.returnX, 0.1f).setEase(15);
			SelectedItem.ClearSelection();
			UICrafting.LastOpened = Time.realtimeSinceStartup;
			Analytics.CraftingOpened++;
		}
	}

	// Token: 0x060024DA RID: 9434 RVA: 0x0001CF3A File Offset: 0x0001B13A
	public static void Toggle()
	{
		if (UICrafting.isOpen)
		{
			UICrafting.Close();
			return;
		}
		UICrafting.Open();
	}

	// Token: 0x060024DB RID: 9435 RVA: 0x000C2DDC File Offset: 0x000C0FDC
	public static void Close()
	{
		if (SingletonComponent<UICrafting>.Instance)
		{
			if (!UICrafting.isOpen)
			{
				return;
			}
			UICrafting.isOpen = false;
			SingletonComponent<UICrafting>.Instance.GetComponent<GraphicRaycaster>().enabled = false;
			LeanTween.cancel(SingletonComponent<UICrafting>.Instance.gameObject);
			LeanTween.cancel(SingletonComponent<UICrafting>.Instance.Inset);
			LeanTween.alphaCanvas(SingletonComponent<UICrafting>.Instance.GetComponent<CanvasGroup>(), 0f, 0.2f).setEase(21);
			LeanTween.moveLocalX(SingletonComponent<UICrafting>.Instance.Inset, 256f, 0.2f).setEase(15);
			SelectedItem.ClearSelection();
			Client.EventSystem.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(null);
		}
		LocalPlayer.EndLooting();
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x0001CF4E File Offset: 0x0001B14E
	private void Update()
	{
		if (!this.ShouldShow())
		{
			UICrafting.Close();
		}
		if (UICrafting.isOpen)
		{
			IngameMenuBackground.Enabled = true;
		}
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x0001CF6A File Offset: 0x0001B16A
	private bool ShouldShow()
	{
		return !(LocalPlayer.Entity == null) && !LocalPlayer.Entity.IsSleeping() && !LocalPlayer.Entity.IsDead() && !LocalPlayer.Entity.IsSpectating();
	}
}
