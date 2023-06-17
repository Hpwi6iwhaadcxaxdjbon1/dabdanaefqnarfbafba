using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007ED RID: 2029
	public class HowToOpenCrafting : BaseTip
	{
		// Token: 0x0400282D RID: 10285
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_opencrafting", "Press [inventory.togglecrafting] to open the CRAFTING MENU");

		// Token: 0x06002C4E RID: 11342 RVA: 0x000228B5 File Offset: 0x00020AB5
		public override Translate.Phrase GetPhrase()
		{
			return HowToOpenCrafting.Phrase;
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06002C4F RID: 11343 RVA: 0x000228BC File Offset: 0x00020ABC
		public int CraftingOpenedTimes
		{
			get
			{
				return Analytics.CraftingOpened;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06002C50 RID: 11344 RVA: 0x000228C3 File Offset: 0x00020AC3
		public float CraftingOpenedSecondsAgo
		{
			get
			{
				return Time.realtimeSinceStartup - UICrafting.LastOpened;
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06002C51 RID: 11345 RVA: 0x000228D0 File Offset: 0x00020AD0
		public override bool ShouldShow
		{
			get
			{
				return this.CraftingOpenedTimes <= 2 && this.CraftingOpenedSecondsAgo >= 120f;
			}
		}
	}
}
