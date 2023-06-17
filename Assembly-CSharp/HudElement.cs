using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200062C RID: 1580
public class HudElement : MonoBehaviour
{
	// Token: 0x04001F64 RID: 8036
	public Text[] ValueText;

	// Token: 0x04001F65 RID: 8037
	public Image[] FilledImage;

	// Token: 0x04001F66 RID: 8038
	private float LastValue;

	// Token: 0x06002344 RID: 9028 RVA: 0x000BB6C4 File Offset: 0x000B98C4
	public void SetValue(float value, float max = 1f)
	{
		using (TimeWarning.New("HudElement.SetValue", 0.1f))
		{
			float num = value / max;
			if (num != this.LastValue)
			{
				this.LastValue = num;
				this.SetText(value.ToString("0"));
				this.SetImage(num);
			}
		}
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x000BB72C File Offset: 0x000B992C
	private void SetText(string v)
	{
		for (int i = 0; i < this.ValueText.Length; i++)
		{
			this.ValueText[i].text = v;
		}
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x000BB75C File Offset: 0x000B995C
	private void SetImage(float f)
	{
		for (int i = 0; i < this.FilledImage.Length; i++)
		{
			this.FilledImage[i].fillAmount = f;
		}
	}
}
