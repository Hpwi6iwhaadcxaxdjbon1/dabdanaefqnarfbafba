using System;

namespace ConVar
{
	// Token: 0x02000860 RID: 2144
	[ConsoleSystem.Factory("file")]
	public class FileConVar : ConsoleSystem
	{
		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06002E9D RID: 11933 RVA: 0x00023EA0 File Offset: 0x000220A0
		// (set) Token: 0x06002E9E RID: 11934 RVA: 0x00023EA7 File Offset: 0x000220A7
		[ClientVar]
		public static bool debug
		{
			get
			{
				return FileSystem.LogDebug;
			}
			set
			{
				FileSystem.LogDebug = value;
			}
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06002E9F RID: 11935 RVA: 0x00023EAF File Offset: 0x000220AF
		// (set) Token: 0x06002EA0 RID: 11936 RVA: 0x00023EB6 File Offset: 0x000220B6
		[ClientVar]
		public static bool time
		{
			get
			{
				return FileSystem.LogTime;
			}
			set
			{
				FileSystem.LogTime = value;
			}
		}
	}
}
