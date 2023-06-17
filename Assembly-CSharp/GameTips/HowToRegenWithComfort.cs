using System;
using Facepunch.Rust;

namespace GameTips
{
	// Token: 0x020007F1 RID: 2033
	public class HowToRegenWithComfort : BaseTip
	{
		// Token: 0x04002832 RID: 10290
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_comfortregen", "You will regenerate a portion of your health when comfortable and not hungry or thirsty.");

		// Token: 0x06002C67 RID: 11367 RVA: 0x00022A06 File Offset: 0x00020C06
		public override Translate.Phrase GetPhrase()
		{
			return HowToRegenWithComfort.Phrase;
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06002C68 RID: 11368 RVA: 0x00022A0D File Offset: 0x00020C0D
		public static bool IsComfortable
		{
			get
			{
				return LocalPlayer.Entity != null && LocalPlayer.Entity.metabolism.comfort.value > 0f;
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06002C69 RID: 11369 RVA: 0x00022A39 File Offset: 0x00020C39
		public static float TimeComfortableTotal
		{
			get
			{
				return Analytics.ComfortDuration;
			}
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06002C6A RID: 11370 RVA: 0x00022A40 File Offset: 0x00020C40
		public override bool ShouldShow
		{
			get
			{
				return HowToRegenWithComfort.IsComfortable && HowToRegenWithComfort.TimeComfortableTotal <= 20f;
			}
		}
	}
}
