using System;

namespace ConVar
{
	// Token: 0x0200088B RID: 2187
	[ConsoleSystem.Factory("voice")]
	public class Voice : ConsoleSystem
	{
		// Token: 0x04002A36 RID: 10806
		[ClientVar(Saved = true)]
		public static bool loopback = false;

		// Token: 0x04002A37 RID: 10807
		[ClientVar]
		public static float ui_scale = 1f;

		// Token: 0x04002A38 RID: 10808
		[ClientVar]
		public static float ui_cut = 0f;

		// Token: 0x04002A39 RID: 10809
		[ClientVar]
		public static int ui_samples = 20;

		// Token: 0x04002A3A RID: 10810
		[ClientVar]
		public static float ui_lerp = 0.2f;
	}
}
