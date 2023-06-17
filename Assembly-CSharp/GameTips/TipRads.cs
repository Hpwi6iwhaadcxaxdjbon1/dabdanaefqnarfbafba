using System;
using Facepunch.Rust;

namespace GameTips
{
	// Token: 0x02000800 RID: 2048
	public class TipRads : BaseTip
	{
		// Token: 0x0400284A RID: 10314
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_radiation", "You are being exposed to lethal levels of radiation. Wear heavier clothing to protect yourself.");

		// Token: 0x06002CC6 RID: 11462 RVA: 0x00022EE7 File Offset: 0x000210E7
		public override Translate.Phrase GetPhrase()
		{
			return TipRads.Phrase;
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06002CC7 RID: 11463 RVA: 0x00022EEE File Offset: 0x000210EE
		public float RadDuration
		{
			get
			{
				return Analytics.ColdExposureDuration;
			}
		}

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06002CC8 RID: 11464 RVA: 0x00022EF5 File Offset: 0x000210F5
		public bool HasRads
		{
			get
			{
				return LocalPlayer.Entity != null && LocalPlayer.Entity.metabolism.radiation_level.value > 0f;
			}
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06002CC9 RID: 11465 RVA: 0x00022F21 File Offset: 0x00021121
		public override bool ShouldShow
		{
			get
			{
				return this.HasRads && this.RadDuration <= 60f;
			}
		}
	}
}
