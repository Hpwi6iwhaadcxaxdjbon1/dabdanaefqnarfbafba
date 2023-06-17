using System;
using UnityEngine;

// Token: 0x02000723 RID: 1827
public static class ArrayEx
{
	// Token: 0x060027FD RID: 10237 RVA: 0x000CE328 File Offset: 0x000CC528
	public static T GetRandom<T>(this T[] array)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[Random.Range(0, array.Length)];
	}

	// Token: 0x060027FE RID: 10238 RVA: 0x000CE358 File Offset: 0x000CC558
	public static T GetRandom<T>(this T[] array, uint seed)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[SeedRandom.Range(ref seed, 0, array.Length)];
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000CE388 File Offset: 0x000CC588
	public static T GetRandom<T>(this T[] array, ref uint seed)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[SeedRandom.Range(ref seed, 0, array.Length)];
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x0001F318 File Offset: 0x0001D518
	public static void Shuffle<T>(this T[] array, uint seed)
	{
		array.Shuffle(ref seed);
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x000CE3B8 File Offset: 0x000CC5B8
	public static void Shuffle<T>(this T[] array, ref uint seed)
	{
		for (int i = 0; i < array.Length; i++)
		{
			int num = SeedRandom.Range(ref seed, 0, array.Length);
			int num2 = SeedRandom.Range(ref seed, 0, array.Length);
			T t = array[num];
			array[num] = array[num2];
			array[num2] = t;
		}
	}

	// Token: 0x06002802 RID: 10242 RVA: 0x000CE408 File Offset: 0x000CC608
	public static void BubbleSort<T>(this T[] array) where T : IComparable<T>
	{
		for (int i = 1; i < array.Length; i++)
		{
			T t = array[i];
			for (int j = i - 1; j >= 0; j--)
			{
				T t2 = array[j];
				if (t.CompareTo(t2) >= 0)
				{
					break;
				}
				array[j + 1] = t2;
				array[j] = t;
			}
		}
	}
}
