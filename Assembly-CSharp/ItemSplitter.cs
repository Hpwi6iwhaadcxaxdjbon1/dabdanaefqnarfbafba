using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200065D RID: 1629
public class ItemSplitter : MonoBehaviour
{
	// Token: 0x0400205E RID: 8286
	public Slider slider;

	// Token: 0x0400205F RID: 8287
	public Text textValue;

	// Token: 0x04002060 RID: 8288
	public Text splitAmountText;

	// Token: 0x04002061 RID: 8289
	private float oldAmount;

	// Token: 0x1700025E RID: 606
	// (get) Token: 0x06002455 RID: 9301 RVA: 0x0001CAE4 File Offset: 0x0001ACE4
	public ItemIcon.DragInfo dragInfo
	{
		get
		{
			return new ItemIcon.DragInfo
			{
				amount = (int)this.slider.value,
				item = SelectedItem.item
			};
		}
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x000C06DC File Offset: 0x000BE8DC
	public void RefreshValue()
	{
		Item item = SelectedItem.item;
		if (item == null)
		{
			return;
		}
		if (this.slider.maxValue == (float)item.amount)
		{
			return;
		}
		this.slider.maxValue = (float)item.amount;
		this.slider.minValue = 1f;
		this.slider.value = (float)item.amount / 2f;
	}

	// Token: 0x06002457 RID: 9303 RVA: 0x000C0744 File Offset: 0x000BE944
	public void Update()
	{
		if (SelectedItem.item == null)
		{
			return;
		}
		if (this.oldAmount == this.slider.value)
		{
			return;
		}
		this.oldAmount = this.slider.value;
		string text = this.oldAmount.ToString("N0");
		this.textValue.text = text;
		this.splitAmountText.text = "x" + text;
	}
}
