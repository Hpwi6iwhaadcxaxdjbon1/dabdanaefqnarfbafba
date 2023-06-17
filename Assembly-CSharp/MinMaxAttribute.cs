using System;
using UnityEngine;

// Token: 0x020006FD RID: 1789
public class MinMaxAttribute : PropertyAttribute
{
	// Token: 0x0400233E RID: 9022
	public float min;

	// Token: 0x0400233F RID: 9023
	public float max;

	// Token: 0x06002759 RID: 10073 RVA: 0x0001EACC File Offset: 0x0001CCCC
	public MinMaxAttribute(float min, float max)
	{
		this.min = min;
		this.max = max;
	}
}
