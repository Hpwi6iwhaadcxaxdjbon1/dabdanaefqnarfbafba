using System;

namespace ConVar
{
	// Token: 0x02000871 RID: 2161
	[ConsoleSystem.Factory("net")]
	public class Net : ConsoleSystem
	{
		// Token: 0x040029B7 RID: 10679
		[ServerVar]
		public static bool visdebug;

		// Token: 0x040029B8 RID: 10680
		[ClientVar]
		public static bool debug;
	}
}
