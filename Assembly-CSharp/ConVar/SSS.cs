using System;

namespace ConVar
{
	// Token: 0x02000880 RID: 2176
	[ConsoleSystem.Factory("SSS")]
	public class SSS : ConsoleSystem
	{
		// Token: 0x04002A1C RID: 10780
		[ClientVar(Saved = true)]
		public static bool enabled = true;

		// Token: 0x04002A1D RID: 10781
		[ClientVar(Saved = true)]
		public static int quality = 0;

		// Token: 0x04002A1E RID: 10782
		[ClientVar(Saved = true)]
		public static bool halfres = true;

		// Token: 0x04002A1F RID: 10783
		[ClientVar(Saved = true)]
		public static float scale = 1f;
	}
}
