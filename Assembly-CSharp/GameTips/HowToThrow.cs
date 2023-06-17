using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007F3 RID: 2035
	public class HowToThrow : BaseTip
	{
		// Token: 0x04002834 RID: 10292
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_howtothrow", "Press [attack] to strike. Throw by holding [attack2] to wind up, and [attack] to release.");

		// Token: 0x04002835 RID: 10293
		public static bool itemThrown = false;

		// Token: 0x04002836 RID: 10294
		public static float lastThrownTime = -1f;

		// Token: 0x06002C72 RID: 11378 RVA: 0x00022AAC File Offset: 0x00020CAC
		public override Translate.Phrase GetPhrase()
		{
			return HowToThrow.Phrase;
		}

		// Token: 0x06002C73 RID: 11379 RVA: 0x00022AB3 File Offset: 0x00020CB3
		public static void ItemThrown()
		{
			HowToThrow.lastThrownTime = Time.realtimeSinceStartup;
			HowToThrow.itemThrown = true;
			Analytics.MeleeThrows++;
		}

		// Token: 0x06002C74 RID: 11380 RVA: 0x00022AD1 File Offset: 0x00020CD1
		public static bool RecentlyThrown()
		{
			return HowToThrow.lastThrownTime != -1f && Time.realtimeSinceStartup < HowToThrow.lastThrownTime + 8f;
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06002C75 RID: 11381 RVA: 0x000E15D8 File Offset: 0x000DF7D8
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

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06002C76 RID: 11382 RVA: 0x00022AF3 File Offset: 0x00020CF3
		public override bool ShouldShow
		{
			get
			{
				return !HowToThrow.itemThrown && Analytics.MeleeThrows < 3 && Analytics.MeleeStrikes < 3 && HowToThrow.HasThrowableItemEquipped;
			}
		}
	}
}
