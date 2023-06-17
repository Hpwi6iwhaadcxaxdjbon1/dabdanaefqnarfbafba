using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007F4 RID: 2036
	public class HowToTreeMinigame : BaseTip
	{
		// Token: 0x04002837 RID: 10295
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_treeminigame", "Hit the red X mark to harvest the tree faster");

		// Token: 0x04002838 RID: 10296
		public static float lastTreeHitTime;

		// Token: 0x06002C79 RID: 11385 RVA: 0x00022B42 File Offset: 0x00020D42
		public override Translate.Phrase GetPhrase()
		{
			return HowToTreeMinigame.Phrase;
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06002C7A RID: 11386 RVA: 0x00022B49 File Offset: 0x00020D49
		public int TreeSpotsHit
		{
			get
			{
				return Analytics.TreeHit;
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06002C7B RID: 11387 RVA: 0x00022B50 File Offset: 0x00020D50
		public float TreeHitSecondsAgo
		{
			get
			{
				return Time.realtimeSinceStartup - HowToTreeMinigame.lastTreeHitTime;
			}
		}

		// Token: 0x06002C7C RID: 11388 RVA: 0x00022B5D File Offset: 0x00020D5D
		public static void TreeHit()
		{
			HowToTreeMinigame.lastTreeHitTime = Time.realtimeSinceStartup;
			Analytics.TreeHit++;
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06002C7D RID: 11389 RVA: 0x00022B75 File Offset: 0x00020D75
		public override bool ShouldShow
		{
			get
			{
				return this.TreeSpotsHit <= 15 && this.TreeHitSecondsAgo <= 5f;
			}
		}
	}
}
