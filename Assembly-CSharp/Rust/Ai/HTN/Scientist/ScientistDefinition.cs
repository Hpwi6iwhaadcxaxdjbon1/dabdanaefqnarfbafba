using System;
using System.Collections;
using UnityEngine;

namespace Rust.Ai.HTN.Scientist
{
	// Token: 0x020008EF RID: 2287
	[CreateAssetMenu(menuName = "Rust/AI/Scientist Definition")]
	public class ScientistDefinition : BaseNpcDefinition
	{
		// Token: 0x04002BD3 RID: 11219
		[Header("Aim")]
		public AnimationCurve MissFunction = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x04002BD4 RID: 11220
		[Header("Equipment")]
		public PlayerInventoryProperties[] loadouts;

		// Token: 0x04002BD5 RID: 11221
		public LootContainer.LootSpawnSlot[] Loot;

		// Token: 0x04002BD6 RID: 11222
		[Header("Audio")]
		public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

		// Token: 0x04002BD7 RID: 11223
		public GameObjectRef RadioEffect;

		// Token: 0x04002BD8 RID: 11224
		public GameObjectRef DeathEffect;

		// Token: 0x04002BD9 RID: 11225
		private bool _isRadioEffectRunning;

		// Token: 0x060030D7 RID: 12503 RVA: 0x000255F9 File Offset: 0x000237F9
		public override void StartVoices(HTNPlayer target)
		{
			if (this._isRadioEffectRunning)
			{
				return;
			}
			this._isRadioEffectRunning = true;
			target.StartCoroutine(this.RadioChatter(target));
		}

		// Token: 0x060030D8 RID: 12504 RVA: 0x00025619 File Offset: 0x00023819
		public override void StopVoices(HTNPlayer target)
		{
			if (!this._isRadioEffectRunning)
			{
				return;
			}
			this._isRadioEffectRunning = false;
		}

		// Token: 0x060030D9 RID: 12505 RVA: 0x0002562B File Offset: 0x0002382B
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

		// Token: 0x060030DA RID: 12506 RVA: 0x00002ECE File Offset: 0x000010CE
		public override void Loadout(HTNPlayer target)
		{
		}

		// Token: 0x060030DB RID: 12507 RVA: 0x00025641 File Offset: 0x00023841
		private IEnumerator EquipWeapon(HTNPlayer target)
		{
			yield return CoroutineEx.waitForSeconds(0.25f);
			yield break;
		}
	}
}
