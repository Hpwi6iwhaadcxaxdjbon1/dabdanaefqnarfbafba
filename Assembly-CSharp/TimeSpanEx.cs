using System;

// Token: 0x0200072B RID: 1835
public static class TimeSpanEx
{
	// Token: 0x06002815 RID: 10261 RVA: 0x0001F364 File Offset: 0x0001D564
	public static string ToShortString(this TimeSpan timeSpan)
	{
		return string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
	}
}
