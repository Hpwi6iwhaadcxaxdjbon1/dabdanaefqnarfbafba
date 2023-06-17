using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000671 RID: 1649
public class ResearchTablePanel : LootPanel
{
	// Token: 0x040020B2 RID: 8370
	public Button researchButton;

	// Token: 0x040020B3 RID: 8371
	public Text timerText;

	// Token: 0x040020B4 RID: 8372
	public GameObject itemDescNoItem;

	// Token: 0x040020B5 RID: 8373
	public GameObject itemDescTooBroken;

	// Token: 0x040020B6 RID: 8374
	public GameObject itemDescNotResearchable;

	// Token: 0x040020B7 RID: 8375
	public GameObject itemDescTooMany;

	// Token: 0x040020B8 RID: 8376
	public GameObject itemTakeBlueprint;

	// Token: 0x040020B9 RID: 8377
	public Text successChanceText;

	// Token: 0x040020BA RID: 8378
	public ItemIcon scrapIcon;

	// Token: 0x040020BB RID: 8379
	[NonSerialized]
	public bool wasResearching;

	// Token: 0x040020BC RID: 8380
	public GameObject[] workbenchReqs;

	// Token: 0x040020BD RID: 8381
	private Item _researchItem;

	// Token: 0x060024BC RID: 9404 RVA: 0x0001CE36 File Offset: 0x0001B036
	public void Awake()
	{
		this.Refresh();
	}

	// Token: 0x060024BD RID: 9405 RVA: 0x000C2260 File Offset: 0x000C0460
	public override void Update()
	{
		base.Update();
		ItemContainer container_ = base.Container_00;
		if (container_ == null)
		{
			return;
		}
		Item slot = container_.GetSlot(0);
		if (slot != this._researchItem || this.wasResearching != this.IsResearching())
		{
			this._researchItem = slot;
			this.wasResearching = this.IsResearching();
			this.Refresh();
		}
		if (this.wasResearching)
		{
			this.UpdateResearchProgress();
		}
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x000C22C4 File Offset: 0x000C04C4
	public void ResearchClicked()
	{
		BaseEntity containerEntity = base.GetContainerEntity();
		if (containerEntity == null)
		{
			return;
		}
		containerEntity.SendMessage("TryResearch");
	}

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x060024BF RID: 9407 RVA: 0x0001CE3E File Offset: 0x0001B03E
	public Item researchItem
	{
		get
		{
			return this._researchItem;
		}
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x000C22F0 File Offset: 0x000C04F0
	public bool IsResearching()
	{
		ResearchTable researchTable = base.GetContainerEntity() as ResearchTable;
		return !(researchTable == null) && researchTable.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x000C231C File Offset: 0x000C051C
	public float ResearchTimeLeft()
	{
		ResearchTable researchTable = base.GetContainerEntity() as ResearchTable;
		if (researchTable == null)
		{
			return 0f;
		}
		if (researchTable.researchFinishedTime > 0f)
		{
			return Mathf.Clamp(researchTable.researchFinishedTime - Time.realtimeSinceStartup, 0f, float.PositiveInfinity);
		}
		return 0f;
	}

	// Token: 0x060024C2 RID: 9410 RVA: 0x000C2374 File Offset: 0x000C0574
	private void Refresh()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		ResearchTable researchTable = base.GetContainerEntity() as ResearchTable;
		if (!researchTable)
		{
			return;
		}
		this.itemDescNoItem.SetActive(false);
		this.itemDescTooBroken.SetActive(false);
		this.itemDescNotResearchable.SetActive(false);
		this.itemDescTooMany.SetActive(false);
		this.itemTakeBlueprint.SetActive(false);
		this.successChanceText.text = "N/A";
		bool flag = false;
		if (this.researchItem != null)
		{
			if (this.researchItem.IsBlueprint())
			{
				this.itemTakeBlueprint.SetActive(true);
			}
			else if (this.researchItem.amount > 1)
			{
				this.itemDescTooMany.SetActive(true);
			}
			else if (researchTable.IsItemResearchable(this.researchItem))
			{
				if (this.researchItem.conditionNormalized > 0f)
				{
					Item item = null;
					if (base.Container_00 != null)
					{
						item = base.Container_00.GetSlot(1);
					}
					int num = researchTable.ScrapForResearch(this.researchItem);
					this.successChanceText.text = string.Format("{0:N0}", num);
					flag = (item != null && item.amount >= num);
				}
				else
				{
					this.itemDescTooBroken.SetActive(true);
				}
			}
			else
			{
				this.itemDescNotResearchable.SetActive(true);
			}
		}
		else
		{
			this.itemDescNoItem.SetActive(true);
		}
		this.UpdateWorkbenchNotifications();
		bool flag2 = this.IsResearching();
		this.researchButton.gameObject.SetActive(flag && !flag2);
		this.timerText.gameObject.SetActive(flag2);
	}

	// Token: 0x060024C3 RID: 9411 RVA: 0x000C2514 File Offset: 0x000C0714
	private void UpdateResearchProgress()
	{
		this.timerText.text = this.ResearchTimeLeft().ToString("0.00");
	}

	// Token: 0x060024C4 RID: 9412 RVA: 0x000C2540 File Offset: 0x000C0740
	public void UpdateWorkbenchNotifications()
	{
		GameObject[] array = this.workbenchReqs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		ResearchTable researchTable = base.GetContainerEntity() as ResearchTable;
		if (!researchTable)
		{
			return;
		}
		if (BasePlayer.craftMode != 0)
		{
			return;
		}
		if (this.researchItem == null || !researchTable.IsItemResearchable(this.researchItem))
		{
			return;
		}
		ItemBlueprint blueprint = this.researchItem.info.Blueprint;
		if (blueprint.workbenchLevelRequired > 0)
		{
			int num = Mathf.Clamp(blueprint.workbenchLevelRequired - 1, 0, this.workbenchReqs.Length - 1);
			this.workbenchReqs[num].SetActive(true);
		}
	}
}
