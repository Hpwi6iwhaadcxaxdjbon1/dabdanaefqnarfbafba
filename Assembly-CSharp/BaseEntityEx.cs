using System;

// Token: 0x0200029A RID: 666
public static class BaseEntityEx
{
	// Token: 0x060012C3 RID: 4803 RVA: 0x00010100 File Offset: 0x0000E300
	public static bool IsValid(this BaseEntity ent)
	{
		return !(ent == null) && ent.net != null;
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x00010118 File Offset: 0x0000E318
	public static bool IsValidEntityReference<T>(this T obj) where T : class
	{
		return obj as BaseEntity != null;
	}
}
