using System;
using UnityEngine;

// Token: 0x020000D1 RID: 209
public class SolarPanel : IOEntity
{
	// Token: 0x040006CE RID: 1742
	public Transform sunSampler;

	// Token: 0x040006CF RID: 1743
	private const int tickrateSeconds = 60;

	// Token: 0x040006D0 RID: 1744
	public int maximalPowerOutput = 10;

	// Token: 0x040006D1 RID: 1745
	public float dot_minimum = 0.1f;

	// Token: 0x040006D2 RID: 1746
	public float dot_maximum = 0.6f;

	// Token: 0x06000A70 RID: 2672 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool IsRootEntity()
	{
		return true;
	}
}
