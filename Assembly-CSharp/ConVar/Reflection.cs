using System;

namespace ConVar
{
	// Token: 0x0200087C RID: 2172
	[ConsoleSystem.Factory("reflection")]
	public class Reflection : ConsoleSystem
	{
		// Token: 0x040029CE RID: 10702
		[ClientVar(Saved = true)]
		public static int quality = 1;
	}
}
