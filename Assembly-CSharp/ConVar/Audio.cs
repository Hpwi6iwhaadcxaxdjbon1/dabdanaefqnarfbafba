using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000843 RID: 2115
	[ConsoleSystem.Factory("audio")]
	public class Audio : ConsoleSystem
	{
		// Token: 0x04002929 RID: 10537
		[ClientVar(Help = "Volume", Saved = true)]
		public static float master = 1f;

		// Token: 0x0400292A RID: 10538
		[ClientVar(Help = "Volume", Saved = true)]
		public static float musicvolume = 1f;

		// Token: 0x0400292B RID: 10539
		[ClientVar(Help = "Volume", Saved = true)]
		public static float musicvolumemenu = 1f;

		// Token: 0x0400292C RID: 10540
		[ClientVar(Help = "Volume", Saved = true)]
		public static float game = 1f;

		// Token: 0x0400292D RID: 10541
		[ClientVar(Help = "Volume", Saved = true)]
		public static float voices = 1f;

		// Token: 0x0400292E RID: 10542
		[ClientVar(Help = "Ambience System")]
		public static bool ambience = true;

		// Token: 0x0400292F RID: 10543
		[ClientVar(Help = "Max ms per frame to spend updating sounds")]
		public static float framebudget = 0.3f;

		// Token: 0x04002930 RID: 10544
		[ClientVar(Help = "Use more advanced sound occlusion", Saved = true)]
		public static bool advancedocclusion = false;

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06002E1E RID: 11806 RVA: 0x00023B2A File Offset: 0x00021D2A
		// (set) Token: 0x06002E1F RID: 11807 RVA: 0x000E70E8 File Offset: 0x000E52E8
		[ClientVar(Help = "Volume", Saved = true)]
		public static int speakers
		{
			get
			{
				return AudioSettings.speakerMode;
			}
			set
			{
				value = Mathf.Clamp(value, 2, 7);
				AudioConfiguration configuration = AudioSettings.GetConfiguration();
				configuration.speakerMode = value;
				using (TimeWarning.New("Audio Settings Reset", 0.25f))
				{
					AudioSettings.Reset(configuration);
				}
			}
		}
	}
}
