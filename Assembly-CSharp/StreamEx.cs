using System;
using System.IO;

// Token: 0x0200072A RID: 1834
public static class StreamEx
{
	// Token: 0x040023CC RID: 9164
	private static readonly byte[] StaticBuffer = new byte[16384];

	// Token: 0x06002813 RID: 10259 RVA: 0x000CED60 File Offset: 0x000CCF60
	public static void WriteToOtherStream(this Stream self, Stream target)
	{
		int num;
		while ((num = self.Read(StreamEx.StaticBuffer, 0, StreamEx.StaticBuffer.Length)) > 0)
		{
			target.Write(StreamEx.StaticBuffer, 0, num);
		}
	}
}
