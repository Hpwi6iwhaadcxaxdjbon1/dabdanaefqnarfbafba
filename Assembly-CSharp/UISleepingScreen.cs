using System;
using UnityEngine;

// Token: 0x020006E6 RID: 1766
public class UISleepingScreen : SingletonComponent<UISleepingScreen>, IUIScreen
{
	// Token: 0x04002302 RID: 8962
	protected CanvasGroup canvasGroup;

	// Token: 0x0600270C RID: 9996 RVA: 0x0001E7AB File Offset: 0x0001C9AB
	protected override void Awake()
	{
		base.Awake();
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x0600270D RID: 9997 RVA: 0x0001E7BF File Offset: 0x0001C9BF
	public void SetVisible(bool b)
	{
		this.canvasGroup.alpha = (b ? 1f : 0f);
	}
}
