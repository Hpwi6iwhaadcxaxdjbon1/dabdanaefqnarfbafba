using System;
using Facepunch.Steamworks;

namespace GameTips
{
	// Token: 0x020007FE RID: 2046
	public class TipPlaceSleepingBag : BaseTip
	{
		// Token: 0x04002848 RID: 10312
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_placebag", "Craft and place a Sleeping Bag to create a respawn point.");

		// Token: 0x06002CBB RID: 11451 RVA: 0x00022E9F File Offset: 0x0002109F
		public override Translate.Phrase GetPhrase()
		{
			return TipPlaceSleepingBag.Phrase;
		}

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06002CBC RID: 11452 RVA: 0x000E1AA8 File Offset: 0x000DFCA8
		public override bool ShouldShow
		{
			get
			{
				if (LocalPlayer.Entity == null)
				{
					return false;
				}
				if (LocalPlayer.Entity.TimeAwake > 10f)
				{
					return false;
				}
				if (LocalPlayer.TimeSinceLastDeath > 60f)
				{
					return false;
				}
				Achievement achievement = Client.Instance.Achievements.Find("PLACE_SLEEPINGBAG");
				return achievement != null && !achievement.State;
			}
		}
	}
}
