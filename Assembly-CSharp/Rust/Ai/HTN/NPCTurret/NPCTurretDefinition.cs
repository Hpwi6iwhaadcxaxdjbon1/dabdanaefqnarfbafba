using System;
using System.Collections;
using UnityEngine;

namespace Rust.Ai.HTN.NPCTurret
{
	// Token: 0x020008F5 RID: 2293
	[CreateAssetMenu(menuName = "Rust/AI/NPC Turret Definition")]
	public class NPCTurretDefinition : BaseNpcDefinition
	{
		// Token: 0x04002BEA RID: 11242
		[Header("Equipment")]
		public PlayerInventoryProperties[] loadouts;

		// Token: 0x04002BEB RID: 11243
		public LootContainer.LootSpawnSlot[] Loot;

		// Token: 0x04002BEC RID: 11244
		[Header("Audio")]
		public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

		// Token: 0x04002BED RID: 11245
		public GameObjectRef RadioEffect;

		// Token: 0x04002BEE RID: 11246
		public GameObjectRef DeathEffect;

		// Token: 0x04002BEF RID: 11247
		private bool _isRadioEffectRunning;

		// Token: 0x04002BF0 RID: 11248
		[Header("Corpse")]
		public string CorpsePath = "assets/prefabs/npc/scientist/scientist_corpse.prefab";

		// Token: 0x060030EC RID: 12524 RVA: 0x000256B3 File Offset: 0x000238B3
		public override void StartVoices(HTNPlayer target)
		{
			if (this._isRadioEffectRunning)
			{
				return;
			}
			this._isRadioEffectRunning = true;
			target.StartCoroutine(this.RadioChatter(target));
		}

		// Token: 0x060030ED RID: 12525 RVA: 0x000256D3 File Offset: 0x000238D3
		public override void StopVoices(HTNPlayer target)
		{
			if (!this._isRadioEffectRunning)
			{
				return;
			}
			this._isRadioEffectRunning = false;
		}

		// Token: 0x060030EE RID: 12526 RVA: 0x000256E5 File Offset: 0x000238E5
		private IEnumerator RadioChatter(HTNPlayer target)
		{
			while (this._isRadioEffectRunning && target != null && target.transform != null && !target.IsDestroyed && !target.IsDead())
			{
				if (this.RadioEffect.isValid)
				{
					Effect.server.Run(this.RadioEffect.resourcePath, target, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
				}
				float seconds = Random.Range(this.RadioEffectRepeatRange.x, this.RadioEffectRepeatRange.y + 1f);
				yield return CoroutineEx.waitForSeconds(seconds);
			}
			yield break;
		}

		// Token: 0x060030EF RID: 12527 RVA: 0x00002ECE File Offset: 0x000010CE
		public override void Loadout(HTNPlayer target)
		{
		}

		// Token: 0x060030F0 RID: 12528 RVA: 0x000256FB File Offset: 0x000238FB
		private IEnumerator EquipWeapon(HTNPlayer target)
		{
			yield return CoroutineEx.waitForSeconds(0.25f);
			yield break;
		}
	}
}
