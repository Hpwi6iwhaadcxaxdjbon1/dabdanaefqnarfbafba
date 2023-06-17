using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200065F RID: 1631
public class ItemStatValue : MonoBehaviour
{
	// Token: 0x04002063 RID: 8291
	public Text text;

	// Token: 0x04002064 RID: 8292
	public Slider slider;

	// Token: 0x04002065 RID: 8293
	public bool selectedItem;

	// Token: 0x04002066 RID: 8294
	public bool smallerIsBetter;

	// Token: 0x04002067 RID: 8295
	public bool asPercentage;

	// Token: 0x0600245D RID: 9309 RVA: 0x00002ECE File Offset: 0x000010CE
	private void OnEnable()
	{
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x0001CB2F File Offset: 0x0001AD2F
	private void OnDisable()
	{
		bool isQuitting = Application.isQuitting;
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x000C07B4 File Offset: 0x000BE9B4
	public void SetValue(float val, int numDecimals = 0, string overrideText = "")
	{
		this.text.text = ((overrideText == "") ? string.Format("{0:n" + numDecimals + "}", val) : overrideText);
		if (this.asPercentage)
		{
			Text text = this.text;
			text.text += " %";
		}
		if (this.smallerIsBetter)
		{
			val = this.slider.maxValue - val;
		}
		this.slider.value = val;
	}
}
