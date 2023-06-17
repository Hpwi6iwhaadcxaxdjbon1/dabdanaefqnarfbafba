using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007EA RID: 2026
	public class HowToExamineHeld : BaseTip
	{
		// Token: 0x04002828 RID: 10280
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_examineheld", "Press [inventory.examineheld] to examine your held item");

		// Token: 0x06002C3A RID: 11322 RVA: 0x0002279A File Offset: 0x0002099A
		public override Translate.Phrase GetPhrase()
		{
			return HowToExamineHeld.Phrase;
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06002C3B RID: 11323 RVA: 0x000227A1 File Offset: 0x000209A1
		public int ExaminedTimes
		{
			get
			{
				return Analytics.TimesExamined;
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06002C3C RID: 11324 RVA: 0x000227A8 File Offset: 0x000209A8
		public float SecondsLastExamined
		{
			get
			{
				return Time.realtimeSinceStartup - HeldEntity.lastExamineTime;
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06002C3D RID: 11325 RVA: 0x000E14EC File Offset: 0x000DF6EC
		public override bool ShouldShow
		{
			get
			{
				if (this.ExaminedTimes > 3)
				{
					return false;
				}
				if (this.SecondsLastExamined < 120f)
				{
					return false;
				}
				if (LocalPlayer.Entity == null)
				{
					return false;
				}
				HeldEntity heldEntity = LocalPlayer.Entity.GetHeldEntity();
				return !(heldEntity == null) && heldEntity.skinID != 0UL;
			}
		}
	}
}
