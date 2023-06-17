using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class BaseCollision : MonoBehaviour, IClientComponent
{
	// Token: 0x04000DDD RID: 3549
	public BaseEntity Owner;

	// Token: 0x04000DDE RID: 3550
	public Model model;

	// Token: 0x0600110A RID: 4362 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void TraceTest(HitTest test, List<TraceInfo> hits)
	{
	}
}
