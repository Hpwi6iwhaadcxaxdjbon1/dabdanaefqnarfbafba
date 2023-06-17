using System;

namespace ConVar
{
	// Token: 0x0200087B RID: 2171
	[ConsoleSystem.Factory("profile")]
	public class Profile : ConsoleSystem
	{
		// Token: 0x06002F45 RID: 12101 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		[ClientVar]
		public static void start(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002F46 RID: 12102 RVA: 0x00002ECE File Offset: 0x000010CE
		[ClientVar]
		[ServerVar]
		public static void stop(ConsoleSystem.Arg arg)
		{
		}
	}
}
