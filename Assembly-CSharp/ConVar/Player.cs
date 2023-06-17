using System;

namespace ConVar
{
	// Token: 0x02000877 RID: 2167
	[ConsoleSystem.Factory("player")]
	public class Player : ConsoleSystem
	{
		// Token: 0x040029BF RID: 10687
		[ClientVar(Saved = true)]
		public static bool recoilcomp = true;

		// Token: 0x040029C0 RID: 10688
		[ClientVar(Saved = true)]
		public static bool footik = true;

		// Token: 0x040029C1 RID: 10689
		[ClientVar(Saved = true)]
		public static float footikdistance = 30f;
	}
}
