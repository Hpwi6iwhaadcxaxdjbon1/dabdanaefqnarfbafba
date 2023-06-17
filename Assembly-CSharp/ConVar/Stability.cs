using System;

namespace ConVar
{
	// Token: 0x02000881 RID: 2177
	[ConsoleSystem.Factory("stability")]
	public class Stability : ConsoleSystem
	{
		// Token: 0x04002A20 RID: 10784
		[ServerVar]
		public static int verbose = 0;

		// Token: 0x04002A21 RID: 10785
		[ServerVar]
		public static int strikes = 10;

		// Token: 0x04002A22 RID: 10786
		[ServerVar]
		public static float collapse = 0.05f;

		// Token: 0x04002A23 RID: 10787
		[ServerVar]
		public static float accuracy = 0.001f;

		// Token: 0x04002A24 RID: 10788
		[ServerVar]
		public static float stabilityqueue = 9f;

		// Token: 0x04002A25 RID: 10789
		[ServerVar]
		public static float surroundingsqueue = 3f;
	}
}
