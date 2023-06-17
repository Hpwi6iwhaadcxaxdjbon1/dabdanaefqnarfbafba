using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200066F RID: 1647
public class QuickCraftButton : MonoBehaviour
{
	// Token: 0x040020A0 RID: 8352
	public Image icon;

	// Token: 0x040020A1 RID: 8353
	public Tooltip tooltip;

	// Token: 0x040020A2 RID: 8354
	public Text CraftCount;

	// Token: 0x040020A3 RID: 8355
	private ItemBlueprint bp;

	// Token: 0x040020A4 RID: 8356
	private int skinId;

	// Token: 0x040020A5 RID: 8357
	private int oldCount = -1;

	// Token: 0x060024AD RID: 9389 RVA: 0x000C1CD0 File Offset: 0x000BFED0
	internal void Setup(ItemBlueprint item)
	{
		this.skinId = PlayerPrefs.GetInt("skin." + item.targetItem.shortname, 0);
		this.bp = item;
		this.icon.sprite = item.targetItem.iconSprite;
		this.tooltip.Text = item.targetItem.displayName.translated.ToUpper();
		this.oldCount = -1;
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x000C1D44 File Offset: 0x000BFF44
	private void Update()
	{
		int num = CraftingQueue.Count(this.bp);
		if (this.oldCount == num)
		{
			return;
		}
		this.oldCount = num;
		this.CraftCount.transform.parent.gameObject.SetActive(num > 0);
		if (num == 0)
		{
			return;
		}
		this.CraftCount.text = num.ToString("N0");
	}

	// Token: 0x060024AF RID: 9391 RVA: 0x000C1DA8 File Offset: 0x000BFFA8
	public void OnClicked()
	{
		if (this.bp == null)
		{
			return;
		}
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "craft.add", new object[]
		{
			this.bp.targetItem.itemid,
			1,
			(ulong)((long)this.skinId)
		});
	}
}
