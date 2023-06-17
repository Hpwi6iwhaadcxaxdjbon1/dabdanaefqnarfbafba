using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000480 RID: 1152
public static class ImageProcessing
{
	// Token: 0x040017B6 RID: 6070
	private static byte[] signature = new byte[]
	{
		137,
		80,
		78,
		71,
		13,
		10,
		26,
		10
	};

	// Token: 0x06001ADF RID: 6879 RVA: 0x00093E0C File Offset: 0x0009200C
	public static void GaussianBlur2D(float[] data, int len1, int len2, int iterations = 1)
	{
		float[] array = data;
		float[] array2 = new float[len1 * len2];
		for (int i = 0; i < iterations; i++)
		{
			for (int j = 0; j < len1; j++)
			{
				int num = Mathf.Max(0, j - 1);
				int num2 = Mathf.Min(len1 - 1, j + 1);
				for (int k = 0; k < len2; k++)
				{
					int num3 = Mathf.Max(0, k - 1);
					int num4 = Mathf.Min(len2 - 1, k + 1);
					float num5 = array[j * len2 + k] * 4f + array[j * len2 + num3] + array[j * len2 + num4] + array[num * len2 + k] + array[num2 * len2 + k];
					array2[j * len2 + k] = num5 * 0.125f;
				}
			}
			GenericsUtil.Swap<float[]>(ref array, ref array2);
		}
		if (array != data)
		{
			Buffer.BlockCopy(array, 0, data, 0, data.Length * 4);
		}
	}

	// Token: 0x06001AE0 RID: 6880 RVA: 0x00093EEC File Offset: 0x000920EC
	public static void GaussianBlur2D(float[] data, int len1, int len2, int len3, int iterations = 1)
	{
		float[] src = data;
		float[] dst = new float[len1 * len2 * len3];
		Action<int> <>9__0;
		for (int i = 0; i < iterations; i++)
		{
			int num = 0;
			int len4 = len1;
			Action<int> action;
			if ((action = <>9__0) == null)
			{
				action = (<>9__0 = delegate(int x)
				{
					int num2 = Mathf.Max(0, x - 1);
					int num3 = Mathf.Min(len1 - 1, x + 1);
					for (int j = 0; j < len2; j++)
					{
						int num4 = Mathf.Max(0, j - 1);
						int num5 = Mathf.Min(len2 - 1, j + 1);
						for (int k = 0; k < len3; k++)
						{
							float num6 = src[(x * len2 + j) * len3 + k] * 4f + src[(x * len2 + num4) * len3 + k] + src[(x * len2 + num5) * len3 + k] + src[(num2 * len2 + j) * len3 + k] + src[(num3 * len2 + j) * len3 + k];
							dst[(x * len2 + j) * len3 + k] = num6 * 0.125f;
						}
					}
				});
			}
			Parallel.For(num, len4, action);
			GenericsUtil.Swap<float[]>(ref src, ref dst);
		}
		if (src != data)
		{
			Buffer.BlockCopy(src, 0, data, 0, data.Length * 4);
		}
	}

	// Token: 0x06001AE1 RID: 6881 RVA: 0x00093FA0 File Offset: 0x000921A0
	public static void Average2D(float[] data, int len1, int len2, int iterations = 1)
	{
		float[] src = data;
		float[] dst = new float[len1 * len2];
		Action<int> <>9__0;
		for (int i = 0; i < iterations; i++)
		{
			int num = 0;
			int len3 = len1;
			Action<int> action;
			if ((action = <>9__0) == null)
			{
				action = (<>9__0 = delegate(int x)
				{
					int num2 = Mathf.Max(0, x - 1);
					int num3 = Mathf.Min(len1 - 1, x + 1);
					for (int j = 0; j < len2; j++)
					{
						int num4 = Mathf.Max(0, j - 1);
						int num5 = Mathf.Min(len2 - 1, j + 1);
						float num6 = src[x * len2 + j] + src[x * len2 + num4] + src[x * len2 + num5] + src[num2 * len2 + j] + src[num3 * len2 + j];
						dst[x * len2 + j] = num6 * 0.2f;
					}
				});
			}
			Parallel.For(num, len3, action);
			GenericsUtil.Swap<float[]>(ref src, ref dst);
		}
		if (src != data)
		{
			Buffer.BlockCopy(src, 0, data, 0, data.Length * 4);
		}
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x00094044 File Offset: 0x00092244
	public static void Average2D(float[] data, int len1, int len2, int len3, int iterations = 1)
	{
		float[] src = data;
		float[] dst = new float[len1 * len2 * len3];
		Action<int> <>9__0;
		for (int i = 0; i < iterations; i++)
		{
			int num = 0;
			int len4 = len1;
			Action<int> action;
			if ((action = <>9__0) == null)
			{
				action = (<>9__0 = delegate(int x)
				{
					int num2 = Mathf.Max(0, x - 1);
					int num3 = Mathf.Min(len1 - 1, x + 1);
					for (int j = 0; j < len2; j++)
					{
						int num4 = Mathf.Max(0, j - 1);
						int num5 = Mathf.Min(len2 - 1, j + 1);
						for (int k = 0; k < len3; k++)
						{
							float num6 = src[(x * len2 + j) * len3 + k] + src[(x * len2 + num4) * len3 + k] + src[(x * len2 + num5) * len3 + k] + src[(num2 * len2 + j) * len3 + k] + src[(num3 * len2 + j) * len3 + k];
							dst[(x * len2 + j) * len3 + k] = num6 * 0.2f;
						}
					}
				});
			}
			Parallel.For(num, len4, action);
			GenericsUtil.Swap<float[]>(ref src, ref dst);
		}
		if (src != data)
		{
			Buffer.BlockCopy(src, 0, data, 0, data.Length * 4);
		}
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x000940F8 File Offset: 0x000922F8
	public static void Upsample2D(float[] src, int srclen1, int srclen2, float[] dst, int dstlen1, int dstlen2)
	{
		if (2 * srclen1 != dstlen1 || 2 * srclen2 != dstlen2)
		{
			return;
		}
		Parallel.For(0, srclen1, delegate(int x)
		{
			int num = Mathf.Max(0, x - 1);
			int num2 = Mathf.Min(srclen1 - 1, x + 1);
			for (int i = 0; i < srclen2; i++)
			{
				int num3 = Mathf.Max(0, i - 1);
				int num4 = Mathf.Min(srclen2 - 1, i + 1);
				float num5 = src[x * srclen2 + i] * 6f;
				float num6 = num5 + src[num * srclen2 + i] + src[x * srclen2 + num3];
				dst[2 * x * dstlen2 + 2 * i] = num6 * 0.125f;
				float num7 = num5 + src[num2 * srclen2 + i] + src[x * srclen2 + num3];
				dst[(2 * x + 1) * dstlen2 + 2 * i] = num7 * 0.125f;
				float num8 = num5 + src[num * srclen2 + i] + src[x * srclen2 + num4];
				dst[2 * x * dstlen2 + (2 * i + 1)] = num8 * 0.125f;
				float num9 = num5 + src[num2 * srclen2 + i] + src[x * srclen2 + num4];
				dst[(2 * x + 1) * dstlen2 + (2 * i + 1)] = num9 * 0.125f;
			}
		});
	}

	// Token: 0x06001AE4 RID: 6884 RVA: 0x00094164 File Offset: 0x00092364
	public static void Upsample2D(float[] src, int srclen1, int srclen2, int srclen3, float[] dst, int dstlen1, int dstlen2, int dstlen3)
	{
		if (2 * srclen1 != dstlen1 || 2 * srclen2 != dstlen2 || srclen3 != dstlen3)
		{
			return;
		}
		Parallel.For(0, srclen1, delegate(int x)
		{
			int num = Mathf.Max(0, x - 1);
			int num2 = Mathf.Min(srclen1 - 1, x + 1);
			for (int i = 0; i < srclen2; i++)
			{
				int num3 = Mathf.Max(0, i - 1);
				int num4 = Mathf.Min(srclen2 - 1, i + 1);
				for (int j = 0; j < srclen3; j++)
				{
					float num5 = src[(x * srclen2 + i) * srclen3 + j] * 6f;
					float num6 = num5 + src[(num * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num3) * srclen3 + j];
					dst[(2 * x * dstlen2 + 2 * i) * dstlen3 + j] = num6 * 0.125f;
					float num7 = num5 + src[(num2 * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num3) * srclen3 + j];
					dst[((2 * x + 1) * dstlen2 + 2 * i) * dstlen3 + j] = num7 * 0.125f;
					float num8 = num5 + src[(num * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num4) * srclen3 + j];
					dst[(2 * x * dstlen2 + (2 * i + 1)) * dstlen3 + j] = num8 * 0.125f;
					float num9 = num5 + src[(num2 * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num4) * srclen3 + j];
					dst[((2 * x + 1) * dstlen2 + (2 * i + 1)) * dstlen3 + j] = num9 * 0.125f;
				}
			}
		});
	}

	// Token: 0x06001AE5 RID: 6885 RVA: 0x000941F0 File Offset: 0x000923F0
	public static void Dilate2D(int[] src, int len1, int len2, int srcmask, int radius, Action<int, int> action)
	{
		Parallel.For(0, len1, delegate(int x)
		{
			MaxQueue maxQueue = new MaxQueue(radius * 2 + 1);
			for (int i = 0; i < radius; i++)
			{
				maxQueue.Push(src[x * len2 + i] & srcmask);
			}
			for (int j = 0; j < len2; j++)
			{
				if (j > radius)
				{
					maxQueue.Pop();
				}
				if (j < len2 - radius)
				{
					maxQueue.Push(src[x * len2 + j + radius] & srcmask);
				}
				if (maxQueue.Max != 0)
				{
					action.Invoke(x, j);
				}
			}
		});
		Parallel.For(0, len2, delegate(int y)
		{
			MaxQueue maxQueue = new MaxQueue(radius * 2 + 1);
			for (int i = 0; i < radius; i++)
			{
				maxQueue.Push(src[i * len2 + y] & srcmask);
			}
			for (int j = 0; j < len1; j++)
			{
				if (j > radius)
				{
					maxQueue.Pop();
				}
				if (j < len1 - radius)
				{
					maxQueue.Push(src[(j + radius) * len2 + y] & srcmask);
				}
				if (maxQueue.Max != 0)
				{
					action.Invoke(j, y);
				}
			}
		});
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x00094260 File Offset: 0x00092460
	public static void FloodFill2D(int x, int y, int[] data, int len1, int len2, int mask_any, int mask_not, Func<int, int> action)
	{
		Stack<KeyValuePair<int, int>> stack = new Stack<KeyValuePair<int, int>>();
		stack.Push(new KeyValuePair<int, int>(x, y));
		while (stack.Count > 0)
		{
			KeyValuePair<int, int> keyValuePair = stack.Pop();
			x = keyValuePair.Key;
			y = keyValuePair.Value;
			int i;
			for (i = y; i >= 0; i--)
			{
				int num = data[x * len2 + i];
				if ((num & mask_any) == 0 || (num & mask_not) != 0)
				{
					break;
				}
			}
			i++;
			bool flag2;
			bool flag = flag2 = false;
			while (i < len2)
			{
				int num2 = data[x * len2 + i];
				if ((num2 & mask_any) == 0 || (num2 & mask_not) != 0)
				{
					break;
				}
				data[x * len2 + i] = action.Invoke(num2);
				if (x > 0)
				{
					int num3 = data[(x - 1) * len2 + i];
					bool flag3 = (num3 & mask_any) != 0 && (num3 & mask_not) == 0;
					if (!flag2 && flag3)
					{
						stack.Push(new KeyValuePair<int, int>(x - 1, i));
						flag2 = true;
					}
					else if (flag2 && !flag3)
					{
						flag2 = false;
					}
				}
				if (x < len1 - 1)
				{
					int num4 = data[(x + 1) * len2 + i];
					bool flag4 = (num4 & mask_any) != 0 && (num4 & mask_not) == 0;
					if (!flag && flag4)
					{
						stack.Push(new KeyValuePair<int, int>(x + 1, i));
						flag = true;
					}
					else if (flag && !flag4)
					{
						flag = false;
					}
				}
				i++;
			}
		}
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x000943C4 File Offset: 0x000925C4
	public static bool IsValidPNG(byte[] data, int maxWidth, int maxHeight)
	{
		if (data.Length < 24)
		{
			return false;
		}
		if (data.Length > 24 + maxWidth * maxHeight * 4)
		{
			return false;
		}
		for (int i = 0; i < 8; i++)
		{
			if (data[i] != ImageProcessing.signature[i])
			{
				return false;
			}
		}
		Union32 union = default(Union32);
		union.b4 = data[16];
		union.b3 = data[17];
		union.b2 = data[18];
		union.b1 = data[19];
		if (union.i < 1 || union.i > maxWidth)
		{
			return false;
		}
		Union32 union2 = default(Union32);
		union2.b4 = data[20];
		union2.b3 = data[21];
		union2.b2 = data[22];
		union2.b1 = data[23];
		return union2.i >= 1 && union2.i <= maxHeight;
	}
}
