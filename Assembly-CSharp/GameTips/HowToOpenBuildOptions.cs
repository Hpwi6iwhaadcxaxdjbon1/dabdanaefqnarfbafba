using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007EC RID: 2028
	public class HowToOpenBuildOptions : BaseTip
	{
		// Token: 0x0400282B RID: 10283
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_howtobuildoptions", "Hold [attack2] to select different building blocks");

		// Token: 0x0400282C RID: 10284
		public static float lastBuildChangeTime = float.NegativeInfinity;

		// Token: 0x06002C47 RID: 11335 RVA: 0x00022844 File Offset: 0x00020A44
		public override Translate.Phrase GetPhrase()
		{
			return HowToOpenBuildOptions.Phrase;
		}

		// Token: 0x06002C48 RID: 11336 RVA: 0x0002284B File Offset: 0x00020A4B
		public static void BuildOptionChanged()
		{
			HowToOpenBuildOptions.lastBuildChangeTime = Time.realtimeSinceStartup;
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06002C49 RID: 11337 RVA: 0x00022857 File Offset: 0x00020A57
		public float TimeSinceBuildChanged
		{
			get
			{
				return Time.realtimeSinceStartup - HowToOpenBuildOptions.lastBuildChangeTime;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06002C4A RID: 11338 RVA: 0x000E1588 File Offset: 0x000DF788
		public static bool HasBuildingPlanEquipped
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
				Planner component = heldEntity.GetComponent<Planner>();
				return !(component == null) && !component.isTypeDeployable;
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06002C4B RID: 11339 RVA: 0x00022864 File Offset: 0x00020A64
		public override bool ShouldShow
		{
			get
			{
				return this.TimeSinceBuildChanged >= 30f && Analytics.PlacedBlocks <= 30 && Analytics.UpgradedBlocks <= 10 && HowToOpenBuildOptions.HasBuildingPlanEquipped;
			}
		}
	}
}
