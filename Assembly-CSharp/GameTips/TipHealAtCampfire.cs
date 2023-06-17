using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007FC RID: 2044
	public class TipHealAtCampfire : BaseTip
	{
		// Token: 0x04002844 RID: 10308
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_healatcampfire", "You can heal by standing next to a lit campfire.");

		// Token: 0x04002845 RID: 10309
		public float oldHealth = -1f;

		// Token: 0x04002846 RID: 10310
		public float lastHurtTime;

		// Token: 0x06002CAD RID: 11437 RVA: 0x00022DC6 File Offset: 0x00020FC6
		public override Translate.Phrase GetPhrase()
		{
			return TipHealAtCampfire.Phrase;
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06002CAE RID: 11438 RVA: 0x00022DCD File Offset: 0x00020FCD
		public bool IsHurt
		{
			get
			{
				return LocalPlayer.Entity != null && LocalPlayer.Entity.healthFraction < 0.6f;
			}
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06002CAF RID: 11439 RVA: 0x00022A39 File Offset: 0x00020C39
		public float TimeComfortableTotal
		{
			get
			{
				return Analytics.ComfortDuration;
			}
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06002CB0 RID: 11440 RVA: 0x000E1954 File Offset: 0x000DFB54
		public bool HealEligable
		{
			get
			{
				return LocalPlayer.Entity != null && LocalPlayer.Entity.metabolism.calories.value > 100f && LocalPlayer.Entity.metabolism.hydration.value > 40f;
			}
		}

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06002CB1 RID: 11441 RVA: 0x000E19A8 File Offset: 0x000DFBA8
		public bool RecentlyHurt
		{
			get
			{
				if (LocalPlayer.Entity == null)
				{
					return false;
				}
				if (LocalPlayer.Entity.TimeAwake < 1f)
				{
					return false;
				}
				float health = LocalPlayer.Entity.health;
				if (this.oldHealth == -1f)
				{
					this.oldHealth = health;
				}
				if (health < this.oldHealth)
				{
					this.lastHurtTime = Time.realtimeSinceStartup;
				}
				this.oldHealth = health;
				return Time.realtimeSinceStartup < this.lastHurtTime + 10f;
			}
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06002CB2 RID: 11442 RVA: 0x00022DEF File Offset: 0x00020FEF
		public override bool ShouldShow
		{
			get
			{
				return this.RecentlyHurt && this.IsHurt && this.TimeComfortableTotal <= 20f && this.HealEligable;
			}
		}
	}
}
