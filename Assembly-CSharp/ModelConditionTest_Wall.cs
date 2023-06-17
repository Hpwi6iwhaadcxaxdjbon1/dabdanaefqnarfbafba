using System;

// Token: 0x020001AF RID: 431
public class ModelConditionTest_Wall : ModelConditionTest
{
	// Token: 0x06000E71 RID: 3697 RVA: 0x0000D2E9 File Offset: 0x0000B4E9
	public override bool DoTest(BaseEntity ent)
	{
		return !ModelConditionTest_WallTriangleLeft.CheckCondition(ent) && !ModelConditionTest_WallTriangleRight.CheckCondition(ent);
	}
}
