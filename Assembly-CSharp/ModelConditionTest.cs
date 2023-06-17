using System;

// Token: 0x020001A9 RID: 425
public abstract class ModelConditionTest : PrefabAttribute
{
	// Token: 0x06000E5E RID: 3678
	public abstract bool DoTest(BaseEntity ent);

	// Token: 0x06000E5F RID: 3679 RVA: 0x0000D2C2 File Offset: 0x0000B4C2
	protected override Type GetIndexedType()
	{
		return typeof(ModelConditionTest);
	}
}
