using System;
using Ionic.Zlib;

namespace Facepunch.Utility
{
	// Token: 0x020008A2 RID: 2210
	public class Compression
	{
		// Token: 0x06002FC2 RID: 12226 RVA: 0x000EB628 File Offset: 0x000E9828
		public static byte[] Compress(byte[] data)
		{
			byte[] result;
			try
			{
				result = GZipStream.CompressBuffer(data);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06002FC3 RID: 12227 RVA: 0x00024A60 File Offset: 0x00022C60
		public static byte[] Uncompress(byte[] data)
		{
			return GZipStream.UncompressBuffer(data);
		}
	}
}
