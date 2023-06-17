using System;

// Token: 0x020002CC RID: 716
public class Upkeep : PrefabAttribute
{
	// Token: 0x04000FF5 RID: 4085
	public float upkeepMultiplier = 1f;

	// Token: 0x06001396 RID: 5014 RVA: 0x000109BA File Offset: 0x0000EBBA
	protected override Type GetIndexedType()
	{
		return typeof(Upkeep);
	}
}
