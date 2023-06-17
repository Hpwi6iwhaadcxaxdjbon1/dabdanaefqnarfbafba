using System;
using System.Collections.Generic;

// Token: 0x02000727 RID: 1831
public static class LinqEx
{
	// Token: 0x0600280D RID: 10253 RVA: 0x000CE8F0 File Offset: 0x000CCAF0
	public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
	{
		int num = -1;
		T t = default(T);
		int num2 = 0;
		foreach (T t2 in sequence)
		{
			if (t2.CompareTo(t) > 0 || num == -1)
			{
				num = num2;
				t = t2;
			}
			num2++;
		}
		return num;
	}
}
