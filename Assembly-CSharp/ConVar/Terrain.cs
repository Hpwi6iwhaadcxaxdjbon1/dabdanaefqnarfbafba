using System;

namespace ConVar
{
	// Token: 0x02000885 RID: 2181
	[ConsoleSystem.Factory("terrain")]
	public class Terrain : ConsoleSystem
	{
		// Token: 0x04002A2A RID: 10794
		[ClientVar(Saved = true)]
		public static float quality = 100f;
	}
}
