using System;

namespace ConVar
{
	// Token: 0x0200087D RID: 2173
	[ConsoleSystem.Factory("sentry")]
	public class Sentry : ConsoleSystem
	{
		// Token: 0x040029CF RID: 10703
		[ServerVar(Help = "target everyone regardless of authorization")]
		public static bool targetall = false;

		// Token: 0x040029D0 RID: 10704
		[ServerVar(Help = "how long until something is considered hostile after it attacked")]
		public static float hostileduration = 120f;
	}
}
