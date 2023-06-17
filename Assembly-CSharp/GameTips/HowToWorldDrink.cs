using System;
using Facepunch.Rust;

namespace GameTips
{
	// Token: 0x020007F7 RID: 2039
	public class HowToWorldDrink : BaseTip
	{
		// Token: 0x0400283B RID: 10299
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_worlddrink", "Approach the water and press [+use] to drink");

		// Token: 0x06002C8A RID: 11402 RVA: 0x00022C00 File Offset: 0x00020E00
		public override Translate.Phrase GetPhrase()
		{
			return HowToWorldDrink.Phrase;
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06002C8B RID: 11403 RVA: 0x00022C07 File Offset: 0x00020E07
		public bool NearFreshWater
		{
			get
			{
				return !(LocalPlayer.Entity == null) && WaterResource.IsFreshWater(LocalPlayer.Entity.transform.position);
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06002C8C RID: 11404 RVA: 0x00022C2C File Offset: 0x00020E2C
		public float ConsumedWater
		{
			get
			{
				return Analytics.ConsumedWater;
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06002C8D RID: 11405 RVA: 0x000E178C File Offset: 0x000DF98C
		public override bool ShouldShow
		{
			get
			{
				return !(LocalPlayer.Entity == null) && this.ConsumedWater <= 300f && this.NearFreshWater && LocalPlayer.Entity.metabolism.hydration.Fraction() <= 0.75f;
			}
		}
	}
}
