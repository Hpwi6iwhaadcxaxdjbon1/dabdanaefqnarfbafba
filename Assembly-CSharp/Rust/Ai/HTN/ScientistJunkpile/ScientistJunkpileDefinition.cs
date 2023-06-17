using System;
using System.Collections;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile
{
	// Token: 0x020008E2 RID: 2274
	[CreateAssetMenu(menuName = "Rust/AI/Scientist Junkpile Definition")]
	public class ScientistJunkpileDefinition : BaseNpcDefinition
	{
		// Token: 0x04002B9D RID: 11165
		[Header("Aim")]
		public AnimationCurve MissFunction = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x04002B9E RID: 11166
		[Header("Equipment")]
		public PlayerInventoryProperties[] loadouts;

		// Token: 0x04002B9F RID: 11167
		public LootContainer.LootSpawnSlot[] Loot;

		// Token: 0x04002BA0 RID: 11168
		[Header("Audio")]
		public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

		// Token: 0x04002BA1 RID: 11169
		public GameObjectRef RadioEffect;

		// Token: 0x04002BA2 RID: 11170
		public GameObjectRef DeathEffect;

		// Token: 0x04002BA3 RID: 11171
		private bool _isRadioEffectRunning;

		// Token: 0x060030AD RID: 12461 RVA: 0x00025445 File Offset: 0x00023645
		public override void StartVoices(HTNPlayer target)
		{
			if (this._isRadioEffectRunning)
			{
				return;
			}
			this._isRadioEffectRunning = true;
			target.StartCoroutine(this.RadioChatter(target));
		}

		// Token: 0x060030AE RID: 12462 RVA: 0x00025465 File Offset: 0x00023665
		public override void StopVoices(HTNPlayer target)
		{
			if (!this._isRadioEffectRunning)
			{
				return;
			}
			this._isRadioEffectRunning = false;
		}

		// Token: 0x060030AF RID: 12463 RVA: 0x00025477 File Offset: 0x00023677
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

		// Token: 0x060030B0 RID: 12464 RVA: 0x00002ECE File Offset: 0x000010CE
		public override void Loadout(HTNPlayer target)
		{
		}

		// Token: 0x060030B1 RID: 12465 RVA: 0x0002548D File Offset: 0x0002368D
		private IEnumerator EquipTool(HTNPlayer target)
		{
			yield return CoroutineEx.waitForSeconds(0.25f);
			yield break;
		}
	}
}
