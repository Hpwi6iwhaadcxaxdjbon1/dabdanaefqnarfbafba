using System;
using ConVar;
using Facepunch;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000606 RID: 1542
public class Branding : BaseMonoBehaviour
{
	// Token: 0x04001EE5 RID: 7909
	public Text versionText;

	// Token: 0x04001EE6 RID: 7910
	public CanvasGroup canvasGroup;

	// Token: 0x04001EE7 RID: 7911
	private string oldChangeId;

	// Token: 0x06002297 RID: 8855 RVA: 0x000B92D0 File Offset: 0x000B74D0
	private void Update()
	{
		this.canvasGroup.alpha = (ConVar.Graphics.branding ? 1f : 0f);
		if (this.oldChangeId != BuildInfo.Current.Scm.ChangeId)
		{
			this.versionText.text = BuildInfo.Current.Scm.ChangeId;
			this.oldChangeId = BuildInfo.Current.Scm.ChangeId;
		}
	}
}
