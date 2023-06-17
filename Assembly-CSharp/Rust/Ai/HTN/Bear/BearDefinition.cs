using System;
using System.Collections;
using UnityEngine;

namespace Rust.Ai.HTN.Bear
{
	// Token: 0x02000900 RID: 2304
	[CreateAssetMenu(menuName = "Rust/AI/Animals/Bear Definition")]
	public class BearDefinition : BaseNpcDefinition
	{
		// Token: 0x04002C11 RID: 11281
		[Header("Sensory Extensions")]
		public float StandingAggroRange = 40f;

		// Token: 0x04002C12 RID: 11282
		[Header("Corpse")]
		public GameObjectRef CorpsePrefab;

		// Token: 0x04002C13 RID: 11283
		[Header("Equipment")]
		public LootContainer.LootSpawnSlot[] Loot;

		// Token: 0x04002C14 RID: 11284
		[Header("Audio")]
		public Vector2 IdleEffectRepeatRange = new Vector2(10f, 15f);

		// Token: 0x04002C15 RID: 11285
		public GameObjectRef IdleEffect;

		// Token: 0x04002C16 RID: 11286
		public GameObjectRef DeathEffect;

		// Token: 0x04002C17 RID: 11287
		private bool _isEffectRunning;

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06003110 RID: 12560 RVA: 0x000257CF File Offset: 0x000239CF
		public float SqrStandingAggroRange
		{
			get
			{
				return this.StandingAggroRange * this.StandingAggroRange;
			}
		}

		// Token: 0x06003111 RID: 12561 RVA: 0x000257DE File Offset: 0x000239DE
		public float AggroRange(bool isStanding)
		{
			if (!isStanding)
			{
				return this.Engagement.AggroRange;
			}
			return this.StandingAggroRange;
		}

		// Token: 0x06003112 RID: 12562 RVA: 0x000257F5 File Offset: 0x000239F5
		public float SqrAggroRange(bool isStanding)
		{
			if (!isStanding)
			{
				return this.Engagement.SqrAggroRange;
			}
			return this.SqrStandingAggroRange;
		}

		// Token: 0x06003113 RID: 12563 RVA: 0x0002580C File Offset: 0x00023A0C
		public override void StartVoices(HTNAnimal target)
		{
			if (this._isEffectRunning)
			{
				return;
			}
			this._isEffectRunning = true;
			target.StartCoroutine(this.PlayEffects(target));
		}

		// Token: 0x06003114 RID: 12564 RVA: 0x0002582C File Offset: 0x00023A2C
		public override void StopVoices(HTNAnimal target)
		{
			if (!this._isEffectRunning)
			{
				return;
			}
			this._isEffectRunning = false;
		}

		// Token: 0x06003115 RID: 12565 RVA: 0x0002583E File Offset: 0x00023A3E
		private IEnumerator PlayEffects(HTNAnimal target)
		{
			while (this._isEffectRunning && target != null && target.transform != null && !target.IsDestroyed && !target.IsDead())
			{
				if (this.IdleEffect.isValid)
				{
					Effect.server.Run(this.IdleEffect.resourcePath, target, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
				}
				float seconds = Random.Range(this.IdleEffectRepeatRange.x, this.IdleEffectRepeatRange.y + 1f);
				yield return CoroutineEx.waitForSeconds(seconds);
			}
			yield break;
		}
	}
}
