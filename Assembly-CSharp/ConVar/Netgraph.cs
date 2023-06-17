using System;

namespace ConVar
{
	// Token: 0x02000872 RID: 2162
	[ConsoleSystem.Factory("netgraph")]
	public class Netgraph : ConsoleSystem
	{
		// Token: 0x040029B9 RID: 10681
		[ClientVar(Saved = true)]
		public static bool enabled = false;

		// Token: 0x040029BA RID: 10682
		[ClientVar(Saved = true)]
		public static float updatespeed = 5f;

		// Token: 0x040029BB RID: 10683
		[ClientVar(Saved = false)]
		public static string typefilter = string.Empty;

		// Token: 0x040029BC RID: 10684
		[ClientVar(Saved = false)]
		public static string entityfilter = string.Empty;
	}
}
