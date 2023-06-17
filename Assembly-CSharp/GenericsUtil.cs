using System;

// Token: 0x0200073C RID: 1852
public static class GenericsUtil
{
	// Token: 0x0600284D RID: 10317 RVA: 0x0001F6BA File Offset: 0x0001D8BA
	public static TDst Cast<TSrc, TDst>(TSrc obj)
	{
		GenericsUtil.CastImpl<TSrc, TDst>.Value = obj;
		return GenericsUtil.CastImpl<TDst, TSrc>.Value;
	}

	// Token: 0x0600284E RID: 10318 RVA: 0x000CF6C4 File Offset: 0x000CD8C4
	public static void Swap<T>(ref T a, ref T b)
	{
		T t = a;
		a = b;
		b = t;
	}

	// Token: 0x0200073D RID: 1853
	private static class CastImpl<TSrc, TDst>
	{
		// Token: 0x040023E8 RID: 9192
		[ThreadStatic]
		public static TSrc Value;

		// Token: 0x0600284F RID: 10319 RVA: 0x0001F6C7 File Offset: 0x0001D8C7
		static CastImpl()
		{
			if (typeof(TSrc) != typeof(TDst))
			{
				throw new InvalidCastException();
			}
		}
	}
}
