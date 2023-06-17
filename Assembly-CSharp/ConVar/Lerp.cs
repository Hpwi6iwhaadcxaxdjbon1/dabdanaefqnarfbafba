using System;

namespace ConVar
{
	// Token: 0x0200086D RID: 2157
	[ConsoleSystem.Factory("lerp")]
	public class Lerp : ConsoleSystem
	{
		// Token: 0x040029AF RID: 10671
		[ClientVar(Help = "Enables interpolation and extrapolation of network positions")]
		public static bool enabled = true;

		// Token: 0x040029B0 RID: 10672
		[ClientVar(Help = "How many seconds to smoothen velocity")]
		public static float smoothing = 0.1f;

		// Token: 0x040029B1 RID: 10673
		[ClientVar(Help = "How many seconds behind to lerp")]
		public static float interpolation = 0.1f;

		// Token: 0x040029B2 RID: 10674
		[ClientVar(Help = "How many seconds ahead to lerp")]
		public static float extrapolation = 1f;

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06002F0B RID: 12043 RVA: 0x000242C8 File Offset: 0x000224C8
		// (set) Token: 0x06002F0C RID: 12044 RVA: 0x000242CF File Offset: 0x000224CF
		[ClientVar]
		public static bool debug
		{
			get
			{
				return PositionLerp.DebugLog;
			}
			set
			{
				PositionLerp.DebugLog = value;
			}
		}
	}
}
