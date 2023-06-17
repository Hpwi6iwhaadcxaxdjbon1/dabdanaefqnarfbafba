using System;
using System.IO;

// Token: 0x0200074F RID: 1871
public static class MurmurHash
{
	// Token: 0x04002416 RID: 9238
	private const uint seed = 1337U;

	// Token: 0x060028C3 RID: 10435 RVA: 0x0001FAD1 File Offset: 0x0001DCD1
	public static int Signed(Stream stream)
	{
		return (int)MurmurHash.Unsigned(stream);
	}

	// Token: 0x060028C4 RID: 10436 RVA: 0x000D0DAC File Offset: 0x000CEFAC
	public static uint Unsigned(Stream stream)
	{
		uint num = 1337U;
		uint num2 = 0U;
		using (BinaryReader binaryReader = new BinaryReader(stream))
		{
			byte[] array = binaryReader.ReadBytes(4);
			while (array.Length != 0)
			{
				num2 += (uint)array.Length;
				switch (array.Length)
				{
				case 1:
				{
					uint num3 = (uint)array[0];
					num3 *= 3432918353U;
					num3 = MurmurHash.rot(num3, 15);
					num3 *= 461845907U;
					num ^= num3;
					break;
				}
				case 2:
				{
					uint num3 = (uint)((int)array[0] | (int)array[1] << 8);
					num3 *= 3432918353U;
					num3 = MurmurHash.rot(num3, 15);
					num3 *= 461845907U;
					num ^= num3;
					break;
				}
				case 3:
				{
					uint num3 = (uint)((int)array[0] | (int)array[1] << 8 | (int)array[2] << 16);
					num3 *= 3432918353U;
					num3 = MurmurHash.rot(num3, 15);
					num3 *= 461845907U;
					num ^= num3;
					break;
				}
				case 4:
				{
					uint num3 = (uint)((int)array[0] | (int)array[1] << 8 | (int)array[2] << 16 | (int)array[3] << 24);
					num3 *= 3432918353U;
					num3 = MurmurHash.rot(num3, 15);
					num3 *= 461845907U;
					num ^= num3;
					num = MurmurHash.rot(num, 13);
					num = num * 5U + 3864292196U;
					break;
				}
				}
				array = binaryReader.ReadBytes(4);
			}
		}
		num ^= num2;
		num = MurmurHash.mix(num);
		return num;
	}

	// Token: 0x060028C5 RID: 10437 RVA: 0x0001FAD9 File Offset: 0x0001DCD9
	private static uint rot(uint x, byte r)
	{
		return x << (int)r | x >> (int)(32 - r);
	}

	// Token: 0x060028C6 RID: 10438 RVA: 0x0001FAEB File Offset: 0x0001DCEB
	private static uint mix(uint h)
	{
		h ^= h >> 16;
		h *= 2246822507U;
		h ^= h >> 13;
		h *= 3266489909U;
		h ^= h >> 16;
		return h;
	}
}
