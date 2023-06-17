using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007F0 RID: 2032
	public class HowToOreMinigame : BaseTip
	{
		// Token: 0x04002830 RID: 10288
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_oreminigame", "Hit the bright areas of the ore node to harvest it faster.");

		// Token: 0x04002831 RID: 10289
		public static float lastOreHitTime;

		// Token: 0x06002C60 RID: 11360 RVA: 0x0002299F File Offset: 0x00020B9F
		public override Translate.Phrase GetPhrase()
		{
			return HowToOreMinigame.Phrase;
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06002C61 RID: 11361 RVA: 0x000229A6 File Offset: 0x00020BA6
		public int OreSpotsHit
		{
			get
			{
				return Analytics.OreHit;
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06002C62 RID: 11362 RVA: 0x000229AD File Offset: 0x00020BAD
		public float OreHitSecondsAgo
		{
			get
			{
				return Time.realtimeSinceStartup - HowToOreMinigame.lastOreHitTime;
			}
		}

		// Token: 0x06002C63 RID: 11363 RVA: 0x000229BA File Offset: 0x00020BBA
		public static void OreHit()
		{
			HowToOreMinigame.lastOreHitTime = Time.realtimeSinceStartup;
			Analytics.OreHit++;
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06002C64 RID: 11364 RVA: 0x000229D2 File Offset: 0x00020BD2
		public override bool ShouldShow
		{
			get
			{
				return this.OreSpotsHit <= 15 && this.OreHitSecondsAgo <= 5f;
			}
		}
	}
}
