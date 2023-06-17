using System;
using UnityEngine;

// Token: 0x020002C5 RID: 709
public class WaterPurifier : LiquidContainer
{
	// Token: 0x04000FEA RID: 4074
	public GameObjectRef storagePrefab;

	// Token: 0x04000FEB RID: 4075
	public Transform storagePrefabAnchor;

	// Token: 0x04000FEC RID: 4076
	public ItemDefinition freshWater;

	// Token: 0x04000FED RID: 4077
	public int waterToProcessPerMinute = 120;

	// Token: 0x04000FEE RID: 4078
	public int freshWaterRatio = 4;

	// Token: 0x06001385 RID: 4997 RVA: 0x00004723 File Offset: 0x00002923
	public bool IsBoiling()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x020002C6 RID: 710
	public static class WaterPurifierFlags
	{
		// Token: 0x04000FEF RID: 4079
		public const BaseEntity.Flags Boiling = BaseEntity.Flags.Reserved1;
	}
}
