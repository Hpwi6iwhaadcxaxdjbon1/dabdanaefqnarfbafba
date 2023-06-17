using System;
using System.Text;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000870 RID: 2160
	[ConsoleSystem.Factory("music")]
	public class Music : ConsoleSystem
	{
		// Token: 0x040029B4 RID: 10676
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x040029B5 RID: 10677
		[ClientVar]
		public static int songGapMin = 240;

		// Token: 0x040029B6 RID: 10678
		[ClientVar]
		public static int songGapMax = 480;

		// Token: 0x06002F18 RID: 12056 RVA: 0x000E9288 File Offset: 0x000E7488
		[ClientVar]
		public static void info(ConsoleSystem.Arg arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (SingletonComponent<MusicManager>.Instance == null)
			{
				stringBuilder.Append("No music manager was found");
			}
			else
			{
				stringBuilder.Append("Current music info: ");
				stringBuilder.AppendLine();
				stringBuilder.Append("  theme: " + SingletonComponent<MusicManager>.Instance.currentTheme);
				stringBuilder.AppendLine();
				stringBuilder.Append("  intensity: " + SingletonComponent<MusicManager>.Instance.intensity);
				stringBuilder.AppendLine();
				stringBuilder.Append("  next music: " + SingletonComponent<MusicManager>.Instance.nextMusic);
				stringBuilder.AppendLine();
				stringBuilder.Append("  current time: " + Time.time);
				stringBuilder.AppendLine();
			}
			arg.ReplyWith(stringBuilder.ToString());
		}
	}
}
