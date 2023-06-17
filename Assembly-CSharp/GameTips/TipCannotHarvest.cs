using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007F8 RID: 2040
	public class TipCannotHarvest : BaseTip
	{
		// Token: 0x0400283C RID: 10300
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_cannotharvest", "You cannot gather anything here.");

		// Token: 0x0400283D RID: 10301
		public static float nonEntityHitTime = float.NegativeInfinity;

		// Token: 0x06002C90 RID: 11408 RVA: 0x00022C49 File Offset: 0x00020E49
		public override Translate.Phrase GetPhrase()
		{
			return TipCannotHarvest.Phrase;
		}

		// Token: 0x06002C91 RID: 11409 RVA: 0x00022C50 File Offset: 0x00020E50
		public static void HitNonEntity()
		{
			TipCannotHarvest.nonEntityHitTime = Time.realtimeSinceStartup;
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06002C92 RID: 11410 RVA: 0x00022C5C File Offset: 0x00020E5C
		public float TimeSinceHitNonEntity
		{
			get
			{
				return Time.realtimeSinceStartup - TipCannotHarvest.nonEntityHitTime;
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06002C93 RID: 11411 RVA: 0x000E17E0 File Offset: 0x000DF9E0
		public static bool HasToolItemEquipped
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
				return !(component == null) && component.gathering.Any();
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06002C94 RID: 11412 RVA: 0x00022C69 File Offset: 0x00020E69
		public override bool ShouldShow
		{
			get
			{
				return this.TimeSinceHitNonEntity <= 3f && (Analytics.OreHit <= 50 || Analytics.TreeHit <= 50) && TipCannotHarvest.HasToolItemEquipped;
			}
		}
	}
}
