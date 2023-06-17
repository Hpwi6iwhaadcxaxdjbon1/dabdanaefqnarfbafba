using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000253 RID: 595
public static class SystemInfoEx
{
	// Token: 0x04000E5C RID: 3676
	private static bool[] supportedRenderTextureFormats;

	// Token: 0x060011A9 RID: 4521
	[DllImport("RustNative")]
	private static extern ulong System_GetMemoryUsage();

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x060011AA RID: 4522 RVA: 0x0000F74C File Offset: 0x0000D94C
	public static int systemMemoryUsed
	{
		get
		{
			return (int)(SystemInfoEx.System_GetMemoryUsage() / 1024UL / 1024UL);
		}
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x0007505C File Offset: 0x0007325C
	public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
	{
		if (SystemInfoEx.supportedRenderTextureFormats == null)
		{
			Array values = Enum.GetValues(typeof(RenderTextureFormat));
			int num = (int)values.GetValue(values.Length - 1);
			SystemInfoEx.supportedRenderTextureFormats = new bool[num + 1];
			for (int i = 0; i <= num; i++)
			{
				bool flag = Enum.IsDefined(typeof(RenderTextureFormat), i);
				SystemInfoEx.supportedRenderTextureFormats[i] = (flag && SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)i));
			}
		}
		return SystemInfoEx.supportedRenderTextureFormats[(int)format];
	}
}
