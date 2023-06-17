using System;

// Token: 0x0200039E RID: 926
public static class HitAreaUtil
{
	// Token: 0x06001784 RID: 6020 RVA: 0x00013B54 File Offset: 0x00011D54
	public static string Format(HitArea area)
	{
		if (area == (HitArea)0)
		{
			return "None";
		}
		if (area == (HitArea)(-1))
		{
			return "Generic";
		}
		return area.ToString();
	}
}
