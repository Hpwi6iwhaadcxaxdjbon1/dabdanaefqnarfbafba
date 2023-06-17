using System;
using Facepunch.Rust;

namespace GameTips
{
	// Token: 0x020007F5 RID: 2037
	public class HowToUseBow : BaseTip
	{
		// Token: 0x04002839 RID: 10297
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_howtouseBow", "Hold [attack2] to aim. Press [attack] to fire");

		// Token: 0x06002C80 RID: 11392 RVA: 0x00022BA9 File Offset: 0x00020DA9
		public override Translate.Phrase GetPhrase()
		{
			return HowToUseBow.Phrase;
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06002C81 RID: 11393 RVA: 0x000E1624 File Offset: 0x000DF824
		public static bool HasBowItemEquipped
		{
			get
			{
				if (LocalPlayer.Entity == null)
				{
					return false;
				}
				HeldEntity heldEntity = LocalPlayer.Entity.GetHeldEntity();
				return !(heldEntity == null) && !(heldEntity.GetComponent<BowWeapon>() == null);
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06002C82 RID: 11394 RVA: 0x00022BB0 File Offset: 0x00020DB0
		public override bool ShouldShow
		{
			get
			{
				return Analytics.ShotArrows <= 0 && HowToUseBow.HasBowItemEquipped;
			}
		}
	}
}
