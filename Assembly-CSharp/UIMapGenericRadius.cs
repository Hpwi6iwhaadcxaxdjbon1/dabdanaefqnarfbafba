using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000637 RID: 1591
public class UIMapGenericRadius : MonoBehaviour
{
	// Token: 0x04001F98 RID: 8088
	public Image radialImage;

	// Token: 0x04001F99 RID: 8089
	public Image outlineImage;

	// Token: 0x04001F9A RID: 8090
	public float radius;

	// Token: 0x04001F9B RID: 8091
	public CanvasGroup fade;

	// Token: 0x04001F9C RID: 8092
	public RectTransform rect;

	// Token: 0x06002387 RID: 9095 RVA: 0x0001C1A2 File Offset: 0x0001A3A2
	public void UpdateColors(Color col, Color col2, float totalAlpha)
	{
		this.radialImage.color = col;
		this.outlineImage.color = col2;
		this.fade.alpha = totalAlpha;
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x0001C1C8 File Offset: 0x0001A3C8
	public void SetRadius(float newRadius, bool force = false)
	{
		this.radius = newRadius;
		if (force)
		{
			this.rect.localScale = Vector3.one * this.radius;
		}
	}

	// Token: 0x06002389 RID: 9097 RVA: 0x0001C1EF File Offset: 0x0001A3EF
	public void Update()
	{
		this.rect.localScale = Vector3.Lerp(this.rect.localScale, Vector3.one * this.radius, Time.deltaTime);
	}
}
