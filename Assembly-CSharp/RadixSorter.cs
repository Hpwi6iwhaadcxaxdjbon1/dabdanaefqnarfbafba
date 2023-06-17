using System;

// Token: 0x02000757 RID: 1879
public class RadixSorter
{
	// Token: 0x0400242B RID: 9259
	private uint[] histogram;

	// Token: 0x0400242C RID: 9260
	private uint[] offset;

	// Token: 0x06002904 RID: 10500 RVA: 0x0001FE90 File Offset: 0x0001E090
	public RadixSorter()
	{
		this.histogram = new uint[768];
		this.offset = new uint[768];
	}

	// Token: 0x06002905 RID: 10501 RVA: 0x000D1C24 File Offset: 0x000CFE24
	public void SortU8(uint[] values, uint[] remap, uint num)
	{
		for (int i = 0; i < 256; i++)
		{
			this.histogram[i] = 0U;
		}
		for (uint num2 = 0U; num2 < num; num2 += 1U)
		{
			this.histogram[(int)(values[(int)num2] & 255U)] += 1U;
		}
		this.offset[0] = 0U;
		for (uint num3 = 0U; num3 < 255U; num3 += 1U)
		{
			this.offset[(int)(num3 + 1U)] = this.offset[(int)num3] + this.histogram[(int)num3];
		}
		for (uint num4 = 0U; num4 < num; num4 += 1U)
		{
			uint[] array = this.offset;
			uint num5 = values[(int)num4] & 255U;
			uint num6 = array[(int)num5];
			array[(int)num5] = num6 + 1U;
			remap[(int)num6] = num4;
		}
	}

	// Token: 0x06002906 RID: 10502 RVA: 0x000D1CD4 File Offset: 0x000CFED4
	public void SortU24(uint[] values, uint[] remap, uint[] remapTemp, uint num)
	{
		for (int i = 0; i < 768; i++)
		{
			this.histogram[i] = 0U;
		}
		for (uint num2 = 0U; num2 < num; num2 += 1U)
		{
			uint num3 = values[(int)num2];
			this.histogram[(int)(num3 & 255U)] += 1U;
			this.histogram[(int)(256U + (num3 >> 8 & 255U))] += 1U;
			this.histogram[(int)(512U + (num3 >> 16 & 255U))] += 1U;
		}
		this.offset[0] = (this.offset[256] = (this.offset[512] = 0U));
		uint num4 = 0U;
		uint num5 = 256U;
		uint num6 = 512U;
		while (num4 < 255U)
		{
			this.offset[(int)(num4 + 1U)] = this.offset[(int)num4] + this.histogram[(int)num4];
			this.offset[(int)(num5 + 1U)] = this.offset[(int)num5] + this.histogram[(int)num5];
			this.offset[(int)(num6 + 1U)] = this.offset[(int)num6] + this.histogram[(int)num6];
			num4 += 1U;
			num5 += 1U;
			num6 += 1U;
		}
		for (uint num7 = 0U; num7 < num; num7 += 1U)
		{
			uint[] array = this.offset;
			uint num8 = values[(int)num7] & 255U;
			uint num9 = array[(int)num8];
			array[(int)num8] = num9 + 1U;
			remapTemp[(int)num9] = num7;
		}
		for (uint num10 = 0U; num10 < num; num10 += 1U)
		{
			uint num11 = remapTemp[(int)num10];
			uint[] array2 = this.offset;
			uint num12 = 256U + (values[(int)num11] >> 8 & 255U);
			uint num9 = array2[(int)num12];
			array2[(int)num12] = num9 + 1U;
			remap[(int)num9] = num11;
		}
		for (uint num13 = 0U; num13 < num; num13 += 1U)
		{
			uint num11 = remap[(int)num13];
			uint[] array3 = this.offset;
			uint num14 = 512U + (values[(int)num11] >> 16 & 255U);
			uint num9 = array3[(int)num14];
			array3[(int)num14] = num9 + 1U;
			remapTemp[(int)num9] = num11;
		}
		for (uint num15 = 0U; num15 < num; num15 += 1U)
		{
			remap[(int)num15] = remapTemp[(int)num15];
		}
	}
}
