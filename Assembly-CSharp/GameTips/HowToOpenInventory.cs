using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007EE RID: 2030
	public class HowToOpenInventory : BaseTip
	{
		// Token: 0x0400282E RID: 10286
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_openinventory", "Press [inventory.toggle] to open the INVENTORY MENU");

		// Token: 0x06002C54 RID: 11348 RVA: 0x00022903 File Offset: 0x00020B03
		public override Translate.Phrase GetPhrase()
		{
			return HowToOpenInventory.Phrase;
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06002C55 RID: 11349 RVA: 0x0002290A File Offset: 0x00020B0A
		public int InventoryOpenedTimes
		{
			get
			{
				return Analytics.InventoryOpened;
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06002C56 RID: 11350 RVA: 0x00022911 File Offset: 0x00020B11
		public float IntentoryOpenedSecondsAgo
		{
			get
			{
				return Time.realtimeSinceStartup - UIInventory.LastOpened;
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06002C57 RID: 11351 RVA: 0x0002291E File Offset: 0x00020B1E
		public override bool ShouldShow
		{
			get
			{
				return this.InventoryOpenedTimes <= 3 && this.IntentoryOpenedSecondsAgo >= 120f;
			}
		}
	}
}
