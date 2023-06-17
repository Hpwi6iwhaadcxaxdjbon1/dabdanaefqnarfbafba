using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000644 RID: 1604
public class BlueprintCraftGridRow : MonoBehaviour
{
	// Token: 0x04001FE7 RID: 8167
	public GameObject amount;

	// Token: 0x04001FE8 RID: 8168
	public GameObject itemName;

	// Token: 0x04001FE9 RID: 8169
	public GameObject total;

	// Token: 0x04001FEA RID: 8170
	public GameObject have;

	// Token: 0x04001FEB RID: 8171
	public Color colorOK;

	// Token: 0x04001FEC RID: 8172
	public Color colorBad;

	// Token: 0x060023C4 RID: 9156 RVA: 0x000BDB98 File Offset: 0x000BBD98
	public void Clear()
	{
		base.GetComponent<CanvasGroup>().alpha = 0.5f;
		this.SetText(this.amount, "", this.colorOK);
		this.SetText(this.itemName, "", this.colorOK);
		this.SetText(this.total, "", this.colorOK);
		this.SetText(this.have, "", this.colorOK);
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x000BDC14 File Offset: 0x000BBE14
	private void SetText(GameObject obj, string text, Color color)
	{
		Text componentInChildren = obj.GetComponentInChildren<Text>();
		if (componentInChildren == null)
		{
			return;
		}
		componentInChildren.text = text;
		componentInChildren.color = color;
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x000BDC40 File Offset: 0x000BBE40
	public void Setup(ItemDefinition itemdef, int amountNeeded, int amountToCraft)
	{
		base.GetComponent<CanvasGroup>().alpha = 1f;
		int num = amountNeeded * amountToCraft;
		int itemAmount = LocalPlayer.GetItemAmount(itemdef);
		Color color = (num <= itemAmount) ? this.colorOK : this.colorBad;
		this.SetText(this.amount, amountNeeded.ToString("N0"), color);
		this.SetText(this.itemName, itemdef.displayName.translated, color);
		this.SetText(this.total, num.ToString("N0"), color);
		this.SetText(this.have, itemAmount.ToString("N0"), color);
	}
}
