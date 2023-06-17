using System;

namespace ConVar
{
	// Token: 0x0200088A RID: 2186
	[ConsoleSystem.Factory("vis")]
	public class Vis : ConsoleSystem
	{
		// Token: 0x04002A2D RID: 10797
		[Help("Turns on debug display of lerp")]
		[ClientVar]
		public static bool lerp;

		// Token: 0x04002A2E RID: 10798
		[ServerVar]
		[Help("Turns on debug display of damages")]
		public static bool damage;

		// Token: 0x04002A2F RID: 10799
		[ServerVar]
		[ClientVar]
		[Help("Turns on debug display of attacks")]
		public static bool attack;

		// Token: 0x04002A30 RID: 10800
		[ClientVar]
		[ServerVar]
		[Help("Turns on debug display of protection")]
		public static bool protection;

		// Token: 0x04002A31 RID: 10801
		[Help("Turns on debug display of weakspots")]
		[ServerVar]
		public static bool weakspots;

		// Token: 0x04002A32 RID: 10802
		[ServerVar]
		[Help("Show trigger entries")]
		public static bool triggers;

		// Token: 0x04002A33 RID: 10803
		[Help("Turns on debug display of hitboxes")]
		[ServerVar]
		public static bool hitboxes;

		// Token: 0x04002A34 RID: 10804
		[Help("Turns on debug display of line of sight checks")]
		[ServerVar]
		public static bool lineofsight;

		// Token: 0x04002A35 RID: 10805
		[Help("Turns on debug display of senses, which are received by Ai")]
		[ServerVar]
		public static bool sense;
	}
}
