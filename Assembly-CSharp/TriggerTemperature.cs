using System;
using UnityEngine;

// Token: 0x0200040D RID: 1037
public class TriggerTemperature : TriggerBase
{
	// Token: 0x040015C3 RID: 5571
	public float Temperature = 50f;

	// Token: 0x040015C4 RID: 5572
	public float triggerSize;

	// Token: 0x040015C5 RID: 5573
	public float minSize;

	// Token: 0x0600194A RID: 6474 RVA: 0x00014FA3 File Offset: 0x000131A3
	private void OnValidate()
	{
		this.triggerSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
	}
}
