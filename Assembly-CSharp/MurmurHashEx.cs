using System;
using System.IO;
using System.Text;

// Token: 0x02000750 RID: 1872
public static class MurmurHashEx
{
	// Token: 0x060028C7 RID: 10439 RVA: 0x0001FB18 File Offset: 0x0001DD18
	public static int MurmurHashSigned(this string str)
	{
		return MurmurHash.Signed(MurmurHashEx.StringToStream(str));
	}

	// Token: 0x060028C8 RID: 10440 RVA: 0x0001FB25 File Offset: 0x0001DD25
	public static uint MurmurHashUnsigned(this string str)
	{
		return MurmurHash.Unsigned(MurmurHashEx.StringToStream(str));
	}

	// Token: 0x060028C9 RID: 10441 RVA: 0x0001FB32 File Offset: 0x0001DD32
	private static MemoryStream StringToStream(string str)
	{
		return new MemoryStream(Encoding.UTF8.GetBytes(str ?? string.Empty));
	}
}
