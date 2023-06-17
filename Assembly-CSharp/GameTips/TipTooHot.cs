using System;
using Facepunch.Rust;

namespace GameTips
{
	// Token: 0x02000803 RID: 2051
	public class TipTooHot : BaseTip
	{
		// Token: 0x0400284D RID: 10317
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_toohot", "You are extremely hot. Wear lighter clothing or use water to cool down.");

		// Token: 0x06002CD9 RID: 11481 RVA: 0x00023027 File Offset: 0x00021227
		public override Translate.Phrase GetPhrase()
		{
			return TipTooHot.Phrase;
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06002CDA RID: 11482 RVA: 0x0002302E File Offset: 0x0002122E
		public float HotDuration
		{
			get
			{
				return Analytics.HotExposureDuration;
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06002CDB RID: 11483 RVA: 0x00023035 File Offset: 0x00021235
		public bool IsHot
		{
			get
			{
				return LocalPlayer.Entity != null && LocalPlayer.Entity.metabolism.temperature.value > 40f;
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06002CDC RID: 11484 RVA: 0x00023061 File Offset: 0x00021261
		public override bool ShouldShow
		{
			get
			{
				return this.IsHot && this.HotDuration <= 120f;
			}
		}
	}
}
