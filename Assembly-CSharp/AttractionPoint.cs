using System;

// Token: 0x02000199 RID: 409
public class AttractionPoint : PrefabAttribute
{
	// Token: 0x04000B2F RID: 2863
	public string groupName;

	// Token: 0x06000E22 RID: 3618 RVA: 0x0000D0C7 File Offset: 0x0000B2C7
	protected override Type GetIndexedType()
	{
		return typeof(AttractionPoint);
	}
}
