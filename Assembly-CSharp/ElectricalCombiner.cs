using System;

// Token: 0x020000CD RID: 205
public class ElectricalCombiner : IOEntity
{
	// Token: 0x040006CB RID: 1739
	public int input1Amount;

	// Token: 0x040006CC RID: 1740
	public int input2Amount;

	// Token: 0x06000A6B RID: 2667 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool IsRootEntity()
	{
		return true;
	}
}
