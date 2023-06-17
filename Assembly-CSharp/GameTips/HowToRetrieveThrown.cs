using System;
using Facepunch.Rust;

namespace GameTips
{
	// Token: 0x020007F2 RID: 2034
	public class HowToRetrieveThrown : BaseTip
	{
		// Token: 0x04002833 RID: 10291
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_howtoretrievethrow", "You can retrieve thrown objects by approaching them and pressing [+use]");

		// Token: 0x06002C6D RID: 11373 RVA: 0x00022A70 File Offset: 0x00020C70
		public override Translate.Phrase GetPhrase()
		{
			return HowToRetrieveThrown.Phrase;
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06002C6E RID: 11374 RVA: 0x000E15D8 File Offset: 0x000DF7D8
		public static bool HasThrowableItemEquipped
		{
			get
			{
				if (LocalPlayer.Entity == null)
				{
					return false;
				}
				HeldEntity heldEntity = LocalPlayer.Entity.GetHeldEntity();
				if (heldEntity == null)
				{
					return false;
				}
				BaseMelee component = heldEntity.GetComponent<BaseMelee>();
				return !(component == null) && component.canThrowAsProjectile;
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06002C6F RID: 11375 RVA: 0x00022A77 File Offset: 0x00020C77
		public override bool ShouldShow
		{
			get
			{
				return HowToThrow.itemThrown && Analytics.MeleeThrows <= 1 && HowToThrow.RecentlyThrown();
			}
		}
	}
}
