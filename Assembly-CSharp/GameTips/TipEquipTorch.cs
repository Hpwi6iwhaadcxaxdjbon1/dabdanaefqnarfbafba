using System;
using Facepunch.Rust;
using UnityEngine;

namespace GameTips
{
	// Token: 0x020007FA RID: 2042
	public class TipEquipTorch : BaseTip
	{
		// Token: 0x04002841 RID: 10305
		public static Translate.Phrase Phrase = new Translate.TokenisedPhrase("gametip_equiptorch", "Equip your torch [slot2] and press [attack2] to ignite it");

		// Token: 0x04002842 RID: 10306
		public static float nextTorchTipTime;

		// Token: 0x06002CA0 RID: 11424 RVA: 0x00022D2A File Offset: 0x00020F2A
		public override Translate.Phrase GetPhrase()
		{
			return TipEquipTorch.Phrase;
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06002CA1 RID: 11425 RVA: 0x00022D31 File Offset: 0x00020F31
		public static bool IsDark
		{
			get
			{
				return TOD_Sky.Instance != null && TOD_Sky.Instance.IsNight;
			}
		}

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06002CA2 RID: 11426 RVA: 0x00022A39 File Offset: 0x00020C39
		public static float TimeComfortableTotal
		{
			get
			{
				return Analytics.ComfortDuration;
			}
		}

		// Token: 0x06002CA3 RID: 11427 RVA: 0x00022D4C File Offset: 0x00020F4C
		public static void TorchLit()
		{
			TipEquipTorch.nextTorchTipTime = Time.realtimeSinceStartup + 1800f;
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06002CA4 RID: 11428 RVA: 0x000E187C File Offset: 0x000DFA7C
		public static bool HasTorchThatNeedsLighting
		{
			get
			{
				if (LocalPlayer.Entity == null)
				{
					return false;
				}
				Item slot = LocalPlayer.Entity.inventory.containerBelt.GetSlot(1);
				if (slot == null || slot.info.shortname != "torch")
				{
					return false;
				}
				HeldEntity heldEntity = LocalPlayer.Entity.GetHeldEntity();
				return !(heldEntity != null) || !heldEntity.HasFlag(BaseEntity.Flags.On);
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06002CA5 RID: 11429 RVA: 0x00022D5E File Offset: 0x00020F5E
		public override bool ShouldShow
		{
			get
			{
				return Time.realtimeSinceStartup >= TipEquipTorch.nextTorchTipTime && TipEquipTorch.IsDark && Analytics.InventoryOpened < 30 && TipEquipTorch.HasTorchThatNeedsLighting;
			}
		}
	}
}
