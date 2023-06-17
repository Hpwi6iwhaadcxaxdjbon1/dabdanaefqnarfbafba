using System;
using UnityEngine;

// Token: 0x0200063F RID: 1599
public class IngameMenuBackground : MonoBehaviour
{
	// Token: 0x04001FBD RID: 8125
	public static bool Enabled;

	// Token: 0x04001FBE RID: 8126
	public CanvasGroup canvasGroup;

	// Token: 0x060023A9 RID: 9129 RVA: 0x000BD2CC File Offset: 0x000BB4CC
	private void LateUpdate()
	{
		if (IngameMenuBackground.Enabled)
		{
			this.canvasGroup.alpha += 10f * Time.unscaledDeltaTime;
			CursorManager.HoldOpen(false);
		}
		else
		{
			this.canvasGroup.alpha -= 10f * Time.unscaledDeltaTime;
		}
		IngameMenuBackground.Enabled = false;
	}
}
