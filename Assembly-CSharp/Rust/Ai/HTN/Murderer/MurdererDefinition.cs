using System;
using System.Collections;
using UnityEngine;

namespace Rust.Ai.HTN.Murderer
{
	// Token: 0x020008FB RID: 2299
	[CreateAssetMenu(menuName = "Rust/AI/Murderer Definition")]
	public class MurdererDefinition : BaseNpcDefinition
	{
		// Token: 0x04002C01 RID: 11265
		[Header("Aim")]
		public AnimationCurve MissFunction = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x04002C02 RID: 11266
		[Header("Equipment")]
		public PlayerInventoryProperties[] loadouts;

		// Token: 0x04002C03 RID: 11267
		public LootContainer.LootSpawnSlot[] Loot;

		// Token: 0x04002C04 RID: 11268
		[Header("Audio")]
		public GameObjectRef DeathEffect;

		// Token: 0x06003101 RID: 12545 RVA: 0x00002ECE File Offset: 0x000010CE
		public override void StartVoices(HTNPlayer target)
		{
		}

		// Token: 0x06003102 RID: 12546 RVA: 0x00002ECE File Offset: 0x000010CE
		public override void StopVoices(HTNPlayer target)
		{
		}

		// Token: 0x06003103 RID: 12547 RVA: 0x00002ECE File Offset: 0x000010CE
		public override void Loadout(HTNPlayer target)
		{
		}

		// Token: 0x06003104 RID: 12548 RVA: 0x00002ECE File Offset: 0x000010CE
		public override void OnlyLoadoutWeapons(HTNPlayer target)
		{
		}

		// Token: 0x06003105 RID: 12549 RVA: 0x00025789 File Offset: 0x00023989
		private IEnumerator EquipWeapon(HTNPlayer target)
		{
			yield return CoroutineEx.waitForSeconds(0.25f);
			yield break;
		}
	}
}
