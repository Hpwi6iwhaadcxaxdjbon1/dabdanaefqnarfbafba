using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007FD RID: 2045
	public class TipNoBuild : BaseTip
	{
		// Token: 0x04002847 RID: 10311
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_nobuild", "You cannot build this close to other structures, objects, or monuments");

		// Token: 0x06002CB5 RID: 11445 RVA: 0x00022E48 File Offset: 0x00021048
		public override Translate.Phrase GetPhrase()
		{
			return TipNoBuild.Phrase;
		}

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06002CB6 RID: 11446 RVA: 0x000E1A24 File Offset: 0x000DFC24
		public static bool HasBuildingPlanEquipped
		{
			get
			{
				if (LocalPlayer.Entity == null)
				{
					return false;
				}
				HeldEntity heldEntity = LocalPlayer.Entity.GetHeldEntity();
				return !(heldEntity == null) && !(heldEntity.GetComponent<Planner>() == null);
			}
		}

		// Token: 0x06002CB7 RID: 11447 RVA: 0x000E1A68 File Offset: 0x000DFC68
		public static bool AimingInNoBuild()
		{
			List<Collider> list = Pool.GetList<Collider>();
			GamePhysics.OverlapSphere(LocalPlayer.Entity.lookingAtPoint, 0.1f, list, 537001984, 2);
			bool result = list.Count > 0;
			Pool.FreeList<Collider>(ref list);
			return result;
		}

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06002CB8 RID: 11448 RVA: 0x00022E4F File Offset: 0x0002104F
		public override bool ShouldShow
		{
			get
			{
				return !(LocalPlayer.Entity == null) && Analytics.PlacedBlocks <= 50 && Analytics.UpgradedBlocks <= 10 && TipNoBuild.HasBuildingPlanEquipped && TipNoBuild.AimingInNoBuild();
			}
		}
	}
}
