using System;
using Facepunch.Rust;

namespace GameTips
{
	// Token: 0x02000802 RID: 2050
	public class TipTooCold : BaseTip
	{
		// Token: 0x0400284C RID: 10316
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_toocold", "You are very cold. Wear heavier clothing or light a campfire.");

		// Token: 0x06002CD3 RID: 11475 RVA: 0x00022FC2 File Offset: 0x000211C2
		public override Translate.Phrase GetPhrase()
		{
			return TipTooCold.Phrase;
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06002CD4 RID: 11476 RVA: 0x00022EEE File Offset: 0x000210EE
		public float ColdDuration
		{
			get
			{
				return Analytics.ColdExposureDuration;
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06002CD5 RID: 11477 RVA: 0x00022FC9 File Offset: 0x000211C9
		public bool IsCold
		{
			get
			{
				return LocalPlayer.Entity != null && LocalPlayer.Entity.metabolism.temperature.value < 5f;
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06002CD6 RID: 11478 RVA: 0x00022FF5 File Offset: 0x000211F5
		public override bool ShouldShow
		{
			get
			{
				return this.IsCold && this.ColdDuration <= 120f;
			}
		}
	}
}
