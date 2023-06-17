using System;
using Facepunch.Rust;
using Facepunch.Steamworks;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007F9 RID: 2041
	public class TipConsumeFood : BaseTip
	{
		// Token: 0x0400283E RID: 10302
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_consumefood", "You can heal, and eliminate thirst and hunger by consuming food items.");

		// Token: 0x0400283F RID: 10303
		public float lastFoodPickupTime;

		// Token: 0x04002840 RID: 10304
		public int lastFoodPickupCount;

		// Token: 0x06002C97 RID: 11415 RVA: 0x00022CB8 File Offset: 0x00020EB8
		public override Translate.Phrase GetPhrase()
		{
			return TipConsumeFood.Phrase;
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06002C98 RID: 11416 RVA: 0x00022CBF File Offset: 0x00020EBF
		public float ConsumedFood
		{
			get
			{
				return Analytics.ConsumedFood;
			}
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06002C99 RID: 11417 RVA: 0x00022C2C File Offset: 0x00020E2C
		public float ConsumedWater
		{
			get
			{
				return Analytics.ConsumedWater;
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06002C9A RID: 11418 RVA: 0x00022CC6 File Offset: 0x00020EC6
		public int PickedUpFood
		{
			get
			{
				return Client.Instance.Stats.GetInt("pickup_category_food");
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06002C9B RID: 11419 RVA: 0x00022CDC File Offset: 0x00020EDC
		public bool HasConsumedEnough
		{
			get
			{
				return this.ConsumedFood >= 100f && this.ConsumedWater >= 100f;
			}
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06002C9C RID: 11420 RVA: 0x000E1830 File Offset: 0x000DFA30
		public bool RecentlyPickedUpFood
		{
			get
			{
				int pickedUpFood = this.PickedUpFood;
				if (pickedUpFood >= 20)
				{
					return false;
				}
				if (pickedUpFood > this.lastFoodPickupCount)
				{
					this.lastFoodPickupTime = Time.realtimeSinceStartup;
				}
				this.lastFoodPickupCount = pickedUpFood;
				return Time.realtimeSinceStartup < this.lastFoodPickupTime + 10f;
			}
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06002C9D RID: 11421 RVA: 0x00022CFD File Offset: 0x00020EFD
		public override bool ShouldShow
		{
			get
			{
				return !this.HasConsumedEnough && this.RecentlyPickedUpFood;
			}
		}
	}
}
