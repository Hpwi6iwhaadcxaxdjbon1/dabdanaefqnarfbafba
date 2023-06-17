using System;

namespace ConVar
{
	// Token: 0x0200087F RID: 2175
	[ConsoleSystem.Factory("spawn")]
	public class Spawn : ConsoleSystem
	{
		// Token: 0x04002A11 RID: 10769
		[ServerVar]
		public static float min_rate = 0.5f;

		// Token: 0x04002A12 RID: 10770
		[ServerVar]
		public static float max_rate = 1f;

		// Token: 0x04002A13 RID: 10771
		[ServerVar]
		public static float min_density = 0.5f;

		// Token: 0x04002A14 RID: 10772
		[ServerVar]
		public static float max_density = 1f;

		// Token: 0x04002A15 RID: 10773
		[ServerVar]
		public static float player_base = 100f;

		// Token: 0x04002A16 RID: 10774
		[ServerVar]
		public static float player_scale = 2f;

		// Token: 0x04002A17 RID: 10775
		[ServerVar]
		public static bool respawn_populations = true;

		// Token: 0x04002A18 RID: 10776
		[ServerVar]
		public static bool respawn_groups = true;

		// Token: 0x04002A19 RID: 10777
		[ServerVar]
		public static bool respawn_individuals = true;

		// Token: 0x04002A1A RID: 10778
		[ServerVar]
		public static float tick_populations = 60f;

		// Token: 0x04002A1B RID: 10779
		[ServerVar]
		public static float tick_individuals = 300f;

		// Token: 0x06002F50 RID: 12112 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void fill_populations(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002F51 RID: 12113 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void fill_groups(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002F52 RID: 12114 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void fill_individuals(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002F53 RID: 12115 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06002F54 RID: 12116 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void scalars(ConsoleSystem.Arg args)
		{
		}
	}
}
