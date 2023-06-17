using System;

// Token: 0x02000617 RID: 1559
public class NeedsMouseWheel : ListComponent<NeedsMouseWheel>
{
	// Token: 0x060022F5 RID: 8949 RVA: 0x0001BB55 File Offset: 0x00019D55
	public static bool AnyActive()
	{
		return ListComponent<NeedsMouseWheel>.InstanceList.Count > 0;
	}
}
