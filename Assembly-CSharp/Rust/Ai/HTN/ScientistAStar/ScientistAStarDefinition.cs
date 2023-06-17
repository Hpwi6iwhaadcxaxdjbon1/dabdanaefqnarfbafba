using System;
using System.Collections;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar
{
	// Token: 0x020008E9 RID: 2281
	[CreateAssetMenu(menuName = "Rust/AI/Scientist AStar Definition")]
	public class ScientistAStarDefinition : BaseNpcDefinition
	{
		// Token: 0x04002BBE RID: 11198
		[Header("Aim")]
		public AnimationCurve MissFunction = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x04002BBF RID: 11199
		[Header("Equipment")]
		public PlayerInventoryProperties[] loadouts;

		// Token: 0x04002BC0 RID: 11200
		public LootContainer.LootSpawnSlot[] Loot;

		// Token: 0x04002BC1 RID: 11201
		[Header("Audio")]
		public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

		// Token: 0x04002BC2 RID: 11202
		public GameObjectRef RadioEffect;

		// Token: 0x04002BC3 RID: 11203
		public GameObjectRef DeathEffect;

		// Token: 0x04002BC4 RID: 11204
		private bool _isRadioEffectRunning;

		// Token: 0x060030C2 RID: 12482 RVA: 0x0002550F File Offset: 0x0002370F
		public override void StartVoices(HTNPlayer target)
		{
			if (this._isRadioEffectRunning)
			{
				return;
			}
			this._isRadioEffectRunning = true;
			target.StartCoroutine(this.RadioChatter(target));
		}

		// Token: 0x060030C3 RID: 12483 RVA: 0x0002552F File Offset: 0x0002372F
		public override void StopVoices(HTNPlayer target)
		{
			if (!this._isRadioEffectRunning)
			{
				return;
			}
			this._isRadioEffectRunning = false;
		}

		// Token: 0x060030C4 RID: 12484 RVA: 0x00025541 File Offset: 0x00023741
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

		// Token: 0x060030C5 RID: 12485 RVA: 0x00002ECE File Offset: 0x000010CE
		public override void Loadout(HTNPlayer target)
		{
		}

		// Token: 0x060030C6 RID: 12486 RVA: 0x00025557 File Offset: 0x00023757
		private IEnumerator EquipWeapon(HTNPlayer target)
		{
			yield return CoroutineEx.waitForSeconds(0.25f);
			yield break;
		}
	}
}
