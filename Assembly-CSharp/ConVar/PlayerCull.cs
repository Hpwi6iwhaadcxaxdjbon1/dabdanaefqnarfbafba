using System;

namespace ConVar
{
	// Token: 0x02000878 RID: 2168
	public class PlayerCull : ConsoleSystem
	{
		// Token: 0x040029C2 RID: 10690
		private static bool _enabled = true;

		// Token: 0x040029C3 RID: 10691
		[ClientVar(Saved = true, Help = "How many times per second to check for visibility")]
		public static float updateRate = 5f;

		// Token: 0x040029C4 RID: 10692
		[ClientVar(Saved = true, Help = "Players of any kind will always be visible closer than this")]
		public static float minCullDist = 20f;

		// Token: 0x040029C5 RID: 10693
		[ClientVar(Saved = true, Help = "Maximum distance to show sleepers in meters")]
		public static float maxSleeperDist = 30f;

		// Token: 0x040029C6 RID: 10694
		[ClientVar(Saved = true, Help = "Maximum distance to show any players in meters")]
		public static float maxPlayerDist = 5000f;

		// Token: 0x040029C7 RID: 10695
		[ClientVar(Saved = true, Help = "Quality of Vis : 0 = Chest check, 1 = Chest+Head, 2 = Chest+Head+Arms, 3 = Chest+Head+Arms+Feet")]
		public static int visQuality = 2;

		// Token: 0x040029C8 RID: 10696
		[ClientVar(ClientAdmin = true)]
		public static bool debug = false;

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06002F34 RID: 12084 RVA: 0x000244F5 File Offset: 0x000226F5
		// (set) Token: 0x06002F35 RID: 12085 RVA: 0x000E94D4 File Offset: 0x000E76D4
		[ClientVar(Saved = true, Help = "Enable/Disable player culling entirely (always render even when hidden)")]
		public static bool enabled
		{
			get
			{
				return PlayerCull._enabled;
			}
			set
			{
				if (PlayerCull._enabled == value)
				{
					return;
				}
				PlayerCull._enabled = value;
				if (!PlayerCull._enabled)
				{
					foreach (BasePlayer basePlayer in BasePlayer.VisiblePlayerList)
					{
						basePlayer.MakeVisible();
					}
				}
			}
		}
	}
}
