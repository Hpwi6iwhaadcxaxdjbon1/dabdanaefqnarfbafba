using System;
using UnityEngine;

// Token: 0x020006E9 RID: 1769
public class UIWounded : MonoBehaviour
{
	// Token: 0x04002305 RID: 8965
	public CanvasGroup group;

	// Token: 0x06002715 RID: 10005 RVA: 0x0001E821 File Offset: 0x0001CA21
	private void Update()
	{
		this.group.alpha = (float)(this.ShouldShow() ? 1 : 0);
	}

	// Token: 0x06002716 RID: 10006 RVA: 0x0001E83B File Offset: 0x0001CA3B
	private bool ShouldShow()
	{
		return !(LocalPlayer.Entity == null) && !LocalPlayer.Entity.IsDead() && LocalPlayer.Entity.IsWounded();
	}
}
