using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000108 RID: 264
public class BasePathNode : MonoBehaviour
{
	// Token: 0x040007C0 RID: 1984
	public List<BasePathNode> linked;

	// Token: 0x040007C1 RID: 1985
	public float maxVelocityOnApproach = -1f;

	// Token: 0x040007C2 RID: 1986
	public bool straightaway;

	// Token: 0x06000B56 RID: 2902 RVA: 0x00002ECE File Offset: 0x000010CE
	public void OnDrawGizmosSelected()
	{
	}
}
