using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x02000758 RID: 1880
public static class RawWriter
{
	// Token: 0x06002907 RID: 10503 RVA: 0x000D1EE4 File Offset: 0x000D00E4
	public static void Write(IEnumerable<byte> data, string path)
	{
		using (FileStream fileStream = File.Open(path, 2))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (byte b in data)
				{
					binaryWriter.Write(b);
				}
			}
		}
	}

	// Token: 0x06002908 RID: 10504 RVA: 0x000D1F68 File Offset: 0x000D0168
	public static void Write(IEnumerable<int> data, string path)
	{
		using (FileStream fileStream = File.Open(path, 2))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (int num in data)
				{
					binaryWriter.Write(num);
				}
			}
		}
	}

	// Token: 0x06002909 RID: 10505 RVA: 0x000D1FEC File Offset: 0x000D01EC
	public static void Write(IEnumerable<short> data, string path)
	{
		using (FileStream fileStream = File.Open(path, 2))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (short num in data)
				{
					binaryWriter.Write(num);
				}
			}
		}
	}

	// Token: 0x0600290A RID: 10506 RVA: 0x000D2070 File Offset: 0x000D0270
	public static void Write(IEnumerable<float> data, string path)
	{
		using (FileStream fileStream = File.Open(path, 2))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (float num in data)
				{
					binaryWriter.Write(num);
				}
			}
		}
	}
}
