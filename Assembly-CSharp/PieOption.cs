using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006B0 RID: 1712
public class PieOption : MonoBehaviour
{
	// Token: 0x0400223D RID: 8765
	public PieShape background;

	// Token: 0x0400223E RID: 8766
	public Image imageIcon;

	// Token: 0x06002622 RID: 9762 RVA: 0x000C906C File Offset: 0x000C726C
	public void UpdateOption(float startSlice, float sliceSize, float border, string optionTitle, float outerSize, float innerSize, float imageSize, Sprite sprite)
	{
		if (this.background == null)
		{
			return;
		}
		float num = this.background.rectTransform.rect.height * 0.5f;
		float num2 = num * (innerSize + (outerSize - innerSize) * 0.5f);
		float num3 = num * (outerSize - innerSize);
		this.background.startRadius = startSlice;
		this.background.endRadius = startSlice + sliceSize;
		this.background.border = border;
		this.background.outerSize = outerSize;
		this.background.innerSize = innerSize;
		this.background.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);
		float num4 = startSlice + sliceSize * 0.5f;
		float x = Mathf.Sin(num4 * 0.017453292f) * num2;
		float y = Mathf.Cos(num4 * 0.017453292f) * num2;
		this.imageIcon.rectTransform.localPosition = new Vector3(x, y);
		this.imageIcon.rectTransform.sizeDelta = new Vector2(num3 * imageSize, num3 * imageSize);
		this.imageIcon.sprite = sprite;
	}
}
