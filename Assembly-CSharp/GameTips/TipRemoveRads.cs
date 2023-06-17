using System;
using Facepunch.Rust;

namespace GameTips
{
	// Token: 0x02000801 RID: 2049
	public class TipRemoveRads : BaseTip
	{
		// Token: 0x0400284B RID: 10315
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_removeradiation", "You can reduce radiation levels by swimming, or consuming certain foods ");

		// Token: 0x06002CCC RID: 11468 RVA: 0x00022F53 File Offset: 0x00021153
		public override Translate.Phrase GetPhrase()
		{
			return TipRemoveRads.Phrase;
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06002CCD RID: 11469 RVA: 0x00022EEE File Offset: 0x000210EE
		public float RadDuration
		{
			get
			{
				return Analytics.ColdExposureDuration;
			}
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06002CCE RID: 11470 RVA: 0x00022F5A File Offset: 0x0002115A
		public bool HasRads
		{
			get
			{
				return LocalPlayer.Entity != null && LocalPlayer.Entity.metabolism.radiation_poison.value > 0f;
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06002CCF RID: 11471 RVA: 0x00022EF5 File Offset: 0x000210F5
		public bool HasRadExposure
		{
			get
			{
				return LocalPlayer.Entity != null && LocalPlayer.Entity.metabolism.radiation_level.value > 0f;
			}
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06002CD0 RID: 11472 RVA: 0x00022F86 File Offset: 0x00021186
		public override bool ShouldShow
		{
			get
			{
				return !this.HasRadExposure && this.HasRads && this.RadDuration <= 60f;
			}
		}
	}
}
