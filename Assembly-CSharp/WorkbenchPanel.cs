using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000679 RID: 1657
public class WorkbenchPanel : LootPanel, IInventoryChanged
{
	// Token: 0x040020EC RID: 8428
	public Button experimentButton;

	// Token: 0x040020ED RID: 8429
	public Text timerText;

	// Token: 0x040020EE RID: 8430
	public Text costText;

	// Token: 0x040020EF RID: 8431
	public GameObject expermentCostParent;

	// Token: 0x040020F0 RID: 8432
	public GameObject controlsParent;

	// Token: 0x040020F1 RID: 8433
	public GameObject allUnlockedNotification;

	// Token: 0x040020F2 RID: 8434
	public GameObject informationParent;

	// Token: 0x040020F3 RID: 8435
	public GameObject cycleIcon;

	// Token: 0x040020F4 RID: 8436
	private bool wasWorking;

	// Token: 0x060024F8 RID: 9464 RVA: 0x000C34A0 File Offset: 0x000C16A0
	public override void Update()
	{
		base.Update();
		if (base.Container_00 == null)
		{
			return;
		}
		Workbench workbench = this.GetWorkbench();
		if (workbench == null)
		{
			return;
		}
		if (workbench.IsWorking() != this.wasWorking)
		{
			this.Refresh();
		}
		this.wasWorking = workbench.IsWorking();
	}

	// Token: 0x060024F9 RID: 9465 RVA: 0x0001D069 File Offset: 0x0001B269
	private void OnEnable()
	{
		this.Refresh();
		GlobalMessages.onInventoryChanged.Add(this);
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x0001C384 File Offset: 0x0001A584
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onInventoryChanged.Remove(this);
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x000C34F0 File Offset: 0x000C16F0
	public void ExperimentButtonClicked()
	{
		BaseEntity containerEntity = base.GetContainerEntity();
		if (containerEntity == null)
		{
			return;
		}
		containerEntity.SendMessage("TryExperiment");
	}

	// Token: 0x060024FC RID: 9468 RVA: 0x0001D07C File Offset: 0x0001B27C
	public Workbench GetWorkbench()
	{
		return base.GetContainerEntity() as Workbench;
	}

	// Token: 0x060024FD RID: 9469 RVA: 0x000C351C File Offset: 0x000C171C
	public bool CanAffordExperiment()
	{
		if (!this.GetWorkbench())
		{
			return false;
		}
		int scrapForExperiment = this.GetWorkbench().GetScrapForExperiment();
		if (base.Container_00 == null)
		{
			return false;
		}
		Item slot = base.Container_00.GetSlot(1);
		return slot != null && !(slot.info != this.GetWorkbench().experimentResource) && slot.amount >= scrapForExperiment;
	}

	// Token: 0x060024FE RID: 9470 RVA: 0x000C3584 File Offset: 0x000C1784
	public void Refresh()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		Workbench workbench = this.GetWorkbench();
		if (workbench == null)
		{
			return;
		}
		bool flag = workbench.PlayerUnlockedThisTier();
		this.allUnlockedNotification.SetActive(flag);
		this.informationParent.SetActive(!flag);
		this.expermentCostParent.SetActive(!flag);
		this.controlsParent.SetActive(!flag);
		this.costText.text = this.GetWorkbench().GetScrapForExperiment().ToString("N0");
		bool flag2 = this.CanAffordExperiment();
		bool flag3 = this.IsWorking();
		this.experimentButton.gameObject.SetActive(flag2 && !flag3);
		this.cycleIcon.gameObject.SetActive(flag3);
		base.GetComponent<VerticalLayoutGroup>().enabled = false;
		base.GetComponent<VerticalLayoutGroup>().enabled = true;
	}

	// Token: 0x060024FF RID: 9471 RVA: 0x0001D089 File Offset: 0x0001B289
	public void OnInventoryChanged()
	{
		this.Refresh();
	}

	// Token: 0x06002500 RID: 9472 RVA: 0x000C3668 File Offset: 0x000C1868
	public bool IsWorking()
	{
		Workbench workbench = base.GetContainerEntity() as Workbench;
		return !(workbench == null) && workbench.HasFlag(BaseEntity.Flags.On);
	}
}
