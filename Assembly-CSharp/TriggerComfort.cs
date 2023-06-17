using System;
using UnityEngine;

// Token: 0x02000405 RID: 1029
public class TriggerComfort : TriggerBase
{
	// Token: 0x040015B0 RID: 5552
	public float triggerSize;

	// Token: 0x040015B1 RID: 5553
	public float baseComfort = 0.5f;

	// Token: 0x040015B2 RID: 5554
	public float minComfortRange = 2.5f;

	// Token: 0x0600193D RID: 6461 RVA: 0x00014F0B File Offset: 0x0001310B
	private void OnValidate()
	{
		this.triggerSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
	}
}
