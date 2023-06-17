using System;
using UnityEngine;

// Token: 0x02000304 RID: 772
public class FruitScale : MonoBehaviour, IClientComponent
{
	// Token: 0x06001440 RID: 5184 RVA: 0x000113F8 File Offset: 0x0000F5F8
	public void SetProgress(float progress)
	{
		base.transform.localScale = Vector3.one * progress;
	}
}
