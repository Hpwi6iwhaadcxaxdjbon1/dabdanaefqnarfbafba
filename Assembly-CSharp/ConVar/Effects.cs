using System;

namespace ConVar
{
	// Token: 0x02000855 RID: 2133
	[ConsoleSystem.Factory("effects")]
	public class Effects : ConsoleSystem
	{
		// Token: 0x04002968 RID: 10600
		[ClientVar(Saved = true)]
		public static int antialiasing = 3;

		// Token: 0x04002969 RID: 10601
		[ClientVar(Saved = true)]
		public static bool ao = true;

		// Token: 0x0400296A RID: 10602
		[ClientVar(Saved = true)]
		public static bool bloom = true;

		// Token: 0x0400296B RID: 10603
		[ClientVar(Saved = true)]
		public static bool lensdirt = true;

		// Token: 0x0400296C RID: 10604
		[ClientVar(Saved = true)]
		public static bool motionblur = true;

		// Token: 0x0400296D RID: 10605
		[ClientVar(Saved = true)]
		public static bool sharpen = true;

		// Token: 0x0400296E RID: 10606
		[ClientVar(Saved = true)]
		public static bool shafts = true;

		// Token: 0x0400296F RID: 10607
		[ClientVar(Saved = true)]
		public static bool vignet = true;

		// Token: 0x04002970 RID: 10608
		public const bool color = true;

		// Token: 0x04002971 RID: 10609
		[ClientVar]
		public static bool footsteps = true;

		// Token: 0x04002972 RID: 10610
		[ClientVar(Saved = true)]
		public static int maxgibs = 1000;

		// Token: 0x04002973 RID: 10611
		[ClientVar(Saved = true)]
		public static bool otherplayerslightflares = true;

		// Token: 0x04002974 RID: 10612
		[ClientVar(Saved = true, Help = "Show outlines of objects when applicable i.e. dropped items")]
		public static bool showoutlines = true;
	}
}
