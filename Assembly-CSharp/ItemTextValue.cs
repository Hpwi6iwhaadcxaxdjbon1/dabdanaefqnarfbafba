using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000661 RID: 1633
public class ItemTextValue : MonoBehaviour
{
	// Token: 0x04002068 RID: 8296
	public Text text;

	// Token: 0x04002069 RID: 8297
	public Color bad;

	// Token: 0x0400206A RID: 8298
	public Color good;

	// Token: 0x0400206B RID: 8299
	public bool negativestat;

	// Token: 0x0400206C RID: 8300
	public bool asPercentage;

	// Token: 0x0400206D RID: 8301
	public bool useColors = true;

	// Token: 0x0400206E RID: 8302
	public bool signed = true;

	// Token: 0x0400206F RID: 8303
	public string suffix;

	// Token: 0x06002463 RID: 9315 RVA: 0x000C0870 File Offset: 0x000BEA70
	public void SetValue(float val, int numDecimals = 0, string overrideText = "")
	{
		this.text.text = ((overrideText == "") ? string.Format("{0}{1:n" + numDecimals + "}", (val > 0f && this.signed) ? "+" : "", val) : overrideText);
		if (this.asPercentage)
		{
			Text text = this.text;
			text.text += " %";
		}
		if (this.suffix != "")
		{
			Text text2 = this.text;
			text2.text += this.suffix;
		}
		bool flag = val > 0f;
		if (this.negativestat)
		{
			flag = !flag;
		}
		if (this.useColors)
		{
			this.text.color = (flag ? this.good : this.bad);
		}
	}
}
