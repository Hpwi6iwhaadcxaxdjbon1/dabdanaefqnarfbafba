using System;

namespace ConVar
{
	// Token: 0x02000850 RID: 2128
	[ConsoleSystem.Factory("decal")]
	public class Decal : ConsoleSystem
	{
		// Token: 0x04002961 RID: 10593
		[ClientVar]
		public static bool cache = false;

		// Token: 0x04002962 RID: 10594
		[ClientVar]
		public static bool instancing = true;

		// Token: 0x04002963 RID: 10595
		[ClientVar]
		public static int capacity = 128;

		// Token: 0x04002964 RID: 10596
		[ClientVar]
		public static int limit = 256;

		// Token: 0x06002E6E RID: 11886 RVA: 0x00023CF7 File Offset: 0x00021EF7
		[ClientVar]
		public static void clear(ConsoleSystem.Arg args)
		{
			DeferredDecalSystem.Clear();
		}
	}
}
