using System;

namespace ConVar
{
	// Token: 0x0200084A RID: 2122
	[ConsoleSystem.Factory("construct")]
	public class Construct : ConsoleSystem
	{
		// Token: 0x04002953 RID: 10579
		[ServerVar]
		[Help("How many minutes before a placed frame gets destroyed")]
		public static float frameminutes = 30f;
	}
}
