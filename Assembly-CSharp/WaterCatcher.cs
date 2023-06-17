using System;
using UnityEngine;

// Token: 0x020002C4 RID: 708
public class WaterCatcher : LiquidContainer
{
	// Token: 0x04000FE5 RID: 4069
	[Header("Water Catcher")]
	public ItemDefinition itemToCreate;

	// Token: 0x04000FE6 RID: 4070
	public float maxItemToCreate = 10f;

	// Token: 0x04000FE7 RID: 4071
	[Header("Outside Test")]
	public Vector3 rainTestPosition = new Vector3(0f, 1f, 0f);

	// Token: 0x04000FE8 RID: 4072
	public float rainTestSize = 1f;

	// Token: 0x04000FE9 RID: 4073
	private const float collectInterval = 60f;
}
