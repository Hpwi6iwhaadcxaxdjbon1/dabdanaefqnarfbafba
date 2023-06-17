using System;
using UnityEngine;

// Token: 0x0200047D RID: 1149
[Serializable]
public class ByteMap
{
	// Token: 0x040017AC RID: 6060
	[SerializeField]
	private int size;

	// Token: 0x040017AD RID: 6061
	[SerializeField]
	private int bytes;

	// Token: 0x040017AE RID: 6062
	[SerializeField]
	private byte[] values;

	// Token: 0x06001AC8 RID: 6856 RVA: 0x000162FC File Offset: 0x000144FC
	public ByteMap(int size, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = new byte[bytes * size * size];
	}

	// Token: 0x06001AC9 RID: 6857 RVA: 0x00016322 File Offset: 0x00014522
	public ByteMap(int size, byte[] values, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = values;
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06001ACA RID: 6858 RVA: 0x0001633F File Offset: 0x0001453F
	public int Size
	{
		get
		{
			return this.size;
		}
	}

	// Token: 0x17000171 RID: 369
	public uint this[int x, int y]
	{
		get
		{
			int num = y * this.bytes * this.size + x * this.bytes;
			switch (this.bytes)
			{
			case 1:
				return (uint)this.values[num];
			case 2:
			{
				uint num2 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				return num2 << 8 | num3;
			}
			case 3:
			{
				uint num4 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				uint num5 = (uint)this.values[num + 2];
				return num4 << 16 | num3 << 8 | num5;
			}
			default:
			{
				uint num6 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				uint num5 = (uint)this.values[num + 2];
				uint num7 = (uint)this.values[num + 3];
				return num6 << 24 | num3 << 16 | num5 << 8 | num7;
			}
			}
		}
		set
		{
			int num = y * this.bytes * this.size + x * this.bytes;
			switch (this.bytes)
			{
			case 1:
				this.values[num] = (byte)(value & 255U);
				return;
			case 2:
				this.values[num] = (byte)(value >> 8 & 255U);
				this.values[num + 1] = (byte)(value & 255U);
				return;
			case 3:
				this.values[num] = (byte)(value >> 16 & 255U);
				this.values[num + 1] = (byte)(value >> 8 & 255U);
				this.values[num + 2] = (byte)(value & 255U);
				return;
			default:
				this.values[num] = (byte)(value >> 24 & 255U);
				this.values[num + 1] = (byte)(value >> 16 & 255U);
				this.values[num + 2] = (byte)(value >> 8 & 255U);
				this.values[num + 3] = (byte)(value & 255U);
				return;
			}
		}
	}
}
