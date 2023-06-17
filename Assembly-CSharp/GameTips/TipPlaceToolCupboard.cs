using System;
using Facepunch.Rust;

namespace GameTips
{
	// Token: 0x020007FF RID: 2047
	public class TipPlaceToolCupboard : BaseTip
	{
		// Token: 0x04002849 RID: 10313
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_placecupboard", "Create a Tool Cupboard to prevent your base from decaying");

		// Token: 0x06002CBF RID: 11455 RVA: 0x00022EBC File Offset: 0x000210BC
		public override Translate.Phrase GetPhrase()
		{
			return TipPlaceToolCupboard.Phrase;
		}

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06002CC0 RID: 11456 RVA: 0x00022DA9 File Offset: 0x00020FA9
		public int CupboardOpenedTimes
		{
			get
			{
				return Analytics.CupboardOpened;
			}
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06002CC1 RID: 11457 RVA: 0x00022EC3 File Offset: 0x000210C3
		public int NumBlocksPlaced
		{
			get
			{
				return Analytics.PlacedBlocks;
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06002CC2 RID: 11458 RVA: 0x00022ECA File Offset: 0x000210CA
		public int NumBlocksUpgraded
		{
			get
			{
				return Analytics.UpgradedBlocks;
			}
		}

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06002CC3 RID: 11459 RVA: 0x000E1B08 File Offset: 0x000DFD08
		public override bool ShouldShow
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
				return !(component == null) && this.NumBlocksUpgraded >= 5 && !component.isTypeDeployable && this.CupboardOpenedTimes <= 10 && !LocalPlayer.Entity.IsBuildingAuthed() && !TipNoBuild.AimingInNoBuild();
			}
		}
	}
}
