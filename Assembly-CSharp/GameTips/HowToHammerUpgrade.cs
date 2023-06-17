using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007EB RID: 2027
	public class HowToHammerUpgrade : BaseTip
	{
		// Token: 0x04002829 RID: 10281
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_howtohammerUpgrade", "Aim at a building block and hold [attack2] to select different upgrades");

		// Token: 0x0400282A RID: 10282
		public static float lastBuildChangeTime = float.NegativeInfinity;

		// Token: 0x06002C40 RID: 11328 RVA: 0x000227D3 File Offset: 0x000209D3
		public override Translate.Phrase GetPhrase()
		{
			return HowToHammerUpgrade.Phrase;
		}

		// Token: 0x06002C41 RID: 11329 RVA: 0x000227DA File Offset: 0x000209DA
		public static void UpgradeHappened()
		{
			Analytics.UpgradedBlocks++;
			HowToHammerUpgrade.lastBuildChangeTime = Time.realtimeSinceStartup;
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06002C42 RID: 11330 RVA: 0x000227F2 File Offset: 0x000209F2
		public float TimeSinceBuildChanged
		{
			get
			{
				return Time.realtimeSinceStartup - HowToHammerUpgrade.lastBuildChangeTime;
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06002C43 RID: 11331 RVA: 0x000E1544 File Offset: 0x000DF744
		public static bool HasHammerEquipped
		{
			get
			{
				if (LocalPlayer.Entity == null)
				{
					return false;
				}
				HeldEntity heldEntity = LocalPlayer.Entity.GetHeldEntity();
				return !(heldEntity == null) && !(heldEntity.GetComponent<Hammer>() == null);
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06002C44 RID: 11332 RVA: 0x000227FF File Offset: 0x000209FF
		public override bool ShouldShow
		{
			get
			{
				return this.TimeSinceBuildChanged >= 30f && Analytics.UpgradedBlocks <= 4 && HowToHammerUpgrade.HasHammerEquipped;
			}
		}
	}
}
