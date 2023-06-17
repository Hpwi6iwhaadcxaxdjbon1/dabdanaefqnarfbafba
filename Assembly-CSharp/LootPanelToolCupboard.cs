using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000D5 RID: 213
public class LootPanelToolCupboard : LootPanel
{
	// Token: 0x040006DD RID: 1757
	public List<VirtualItemIcon> costIcons;

	// Token: 0x040006DE RID: 1758
	public Text costPerTimeText;

	// Token: 0x040006DF RID: 1759
	public Text protectedText;

	// Token: 0x040006E0 RID: 1760
	public GameObject baseNotProtectedObj;

	// Token: 0x040006E1 RID: 1761
	public GameObject baseProtectedObj;

	// Token: 0x040006E2 RID: 1762
	public Translate.Phrase protectedPrefix;

	// Token: 0x040006E3 RID: 1763
	public Tooltip costToolTip;

	// Token: 0x040006E4 RID: 1764
	public Translate.Phrase blocksPhrase;

	// Token: 0x040006E5 RID: 1765
	private float nextUpdateTime = -1f;

	// Token: 0x06000A7B RID: 2683 RVA: 0x0000A53B File Offset: 0x0000873B
	public BuildingPrivlidge GetTC()
	{
		return base.GetContainerEntity() as BuildingPrivlidge;
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0000A548 File Offset: 0x00008748
	public new void Update()
	{
		if (this.nextUpdateTime < Time.realtimeSinceStartup)
		{
			this.UpdateCosts();
			this.nextUpdateTime = Time.realtimeSinceStartup + 1f;
		}
		base.Update();
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x000554CC File Offset: 0x000536CC
	public void UpdateCosts()
	{
		BuildingPrivlidge tc = this.GetTC();
		if (tc == null)
		{
			return;
		}
		List<ItemAmount> list = Pool.GetList<ItemAmount>();
		tc.CalculateUpkeepCostAmounts(list);
		float num = tc.CalculateUpkeepPeriodMinutes();
		float num2 = 60f / num * 24f;
		foreach (VirtualItemIcon virtualItemIcon in this.costIcons)
		{
			virtualItemIcon.gameObject.SetActive(false);
		}
		for (int i = 0; i < list.Count; i++)
		{
			ItemAmount itemAmount = list[i];
			if (itemAmount != null)
			{
				if (i >= this.costIcons.Count)
				{
					Debug.LogWarning("WARNING! CONTACT DEVS Too many items for UpdateCosts, ingredient : " + itemAmount.itemDef.shortname);
				}
				else
				{
					this.costIcons[i].SetVirtualItem(itemAmount.itemDef, Mathf.CeilToInt(itemAmount.amount * num2), 0UL, false);
					this.costIcons[i].gameObject.SetActive(true);
				}
			}
		}
		if (base.Container_00 != null)
		{
			float num3 = -1f;
			foreach (ItemAmount itemAmount2 in list)
			{
				int num4 = Enumerable.Sum<Item>(base.Container_00.FindItemsByItemID(itemAmount2.itemid), (Item x) => x.amount);
				if (num4 > 0 && itemAmount2.amount > 0f)
				{
					float num5 = (float)num4 / itemAmount2.amount * num;
					if (num3 == -1f || num5 < num3)
					{
						num3 = num5;
					}
				}
				else
				{
					num3 = 0f;
				}
			}
			if (num3 == -1f)
			{
				num3 = 0f;
			}
			this.baseNotProtectedObj.SetActive(num3 == 0f);
			this.baseProtectedObj.SetActive(num3 > 0f);
			TimeSpan timeSpan = TimeSpan.FromMinutes((double)num3);
			string text = string.Format("{0:N0}d {1:N0}h {2:N0}m {3:N0}s ", new object[]
			{
				timeSpan.Days,
				timeSpan.Hours,
				timeSpan.Minutes,
				timeSpan.Seconds
			});
			this.protectedText.text = this.protectedPrefix.translated + " " + text;
		}
		if (tc.GetBuilding() != null && tc.GetBuilding().buildingBlocks != null)
		{
			string text2 = string.Concat(new object[]
			{
				tc.GetBuilding().buildingBlocks.Count,
				" ",
				this.blocksPhrase.translated,
				" ",
				(tc.CalculateUpkeepCostFraction() * 100f).ToString("N1"),
				"%"
			});
			this.costToolTip.Text = text2;
		}
		this.costIcons[0].transform.parent.gameObject.SetActive(false);
		this.costIcons[0].transform.parent.gameObject.SetActive(true);
		Pool.FreeList<ItemAmount>(ref list);
	}
}
