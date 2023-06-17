using System;

namespace ConVar
{
	// Token: 0x0200088C RID: 2188
	[ConsoleSystem.Factory("water")]
	public class Water : ConsoleSystem
	{
		// Token: 0x04002A3B RID: 10811
		[ClientVar(Saved = true)]
		public static int quality = 1;

		// Token: 0x04002A3C RID: 10812
		[ClientVar(Saved = true)]
		public static int reflections = 1;
	}
}
