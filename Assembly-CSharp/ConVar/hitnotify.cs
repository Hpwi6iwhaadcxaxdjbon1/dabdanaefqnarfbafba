using System;

namespace ConVar
{
	// Token: 0x02000891 RID: 2193
	[ConsoleSystem.Factory("hitnotify")]
	public class hitnotify : ConsoleSystem
	{
		// Token: 0x04002A3E RID: 10814
		[ClientVar]
		[Help("0 == off, 1 == clientside, 2 == serverside")]
		public static int notification_level = 1;
	}
}
