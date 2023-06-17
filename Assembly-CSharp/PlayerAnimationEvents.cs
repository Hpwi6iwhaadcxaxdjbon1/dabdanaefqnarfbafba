using System;
using UnityEngine;

// Token: 0x0200030C RID: 780
public class PlayerAnimationEvents : MonoBehaviour
{
	// Token: 0x04001157 RID: 4439
	private BasePlayer player;

	// Token: 0x06001456 RID: 5206 RVA: 0x0001158F File Offset: 0x0000F78F
	protected void OnEnable()
	{
		this.player = (base.gameObject.ToBaseEntity() as BasePlayer);
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x0007E118 File Offset: 0x0007C318
	public void ThirdPersonReloadSound(AnimationEvent animEvent)
	{
		if (!this.ShouldPlayThirdPersonSounds())
		{
			return;
		}
		AttackEntity attackEntity = this.player.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return;
		}
		Debug.Assert(attackEntity.reloadSounds != null, "ThirdPersonReloadSound: projectile.reloadSounds is null", base.gameObject);
		if (animEvent.intParameter > attackEntity.reloadSounds.Length - 1)
		{
			return;
		}
		Debug.Assert(attackEntity.reloadSounds[animEvent.intParameter] != null, "ThirdPersonReloadSound: null reload sound", base.gameObject);
		Debug.Assert(animEvent.intParameter < attackEntity.reloadSounds.Length, "ThirdPersonReloadSound: intparam out of bounds", base.gameObject);
		SoundManager.PlayOneshot(attackEntity.reloadSounds[animEvent.intParameter], attackEntity.gameObject, false, default(Vector3));
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x0007E1DC File Offset: 0x0007C3DC
	public void ThirdPersonMeleeAttackSound(AnimationEvent animEvent)
	{
		if (!this.ShouldPlayThirdPersonSounds())
		{
			return;
		}
		AttackEntity attackEntity = this.player.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return;
		}
		if (attackEntity.thirdPersonMeleeSound == null)
		{
			return;
		}
		SoundManager.PlayOneshot(attackEntity.thirdPersonMeleeSound, attackEntity.gameObject, false, default(Vector3));
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x0007E238 File Offset: 0x0007C438
	public void ThirdPersonDeploySound(AnimationEvent animEvent)
	{
		if (!this.ShouldPlayThirdPersonSounds())
		{
			return;
		}
		HeldEntity heldEntity = this.player.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		if (heldEntity.thirdPersonDeploySound == null)
		{
			return;
		}
		SoundManager.PlayOneshot(heldEntity.thirdPersonDeploySound, heldEntity.gameObject, false, default(Vector3));
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x0007E290 File Offset: 0x0007C490
	public void ThirdPersonLiquidThrowSound(AnimationEvent animEvent)
	{
		if (!this.ShouldPlayThirdPersonSounds())
		{
			return;
		}
		BaseLiquidVessel baseLiquidVessel = this.player.GetHeldEntity() as BaseLiquidVessel;
		if (baseLiquidVessel == null)
		{
			return;
		}
		if (baseLiquidVessel.throwSound3P == null)
		{
			return;
		}
		SoundManager.PlayOneshot(baseLiquidVessel.throwSound3P, baseLiquidVessel.gameObject, false, default(Vector3));
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x0007E2EC File Offset: 0x0007C4EC
	public void PlayThirdPersonSound(SoundDefinition def)
	{
		if (def == null)
		{
			return;
		}
		if (!this.ShouldPlayThirdPersonSounds())
		{
			return;
		}
		SoundManager.PlayOneshot(def, base.gameObject, false, default(Vector3));
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x000115A7 File Offset: 0x0000F7A7
	public bool ShouldPlayThirdPersonSounds()
	{
		return this.player != null && (!this.player.IsLocalPlayer() || !this.player.InFirstPersonMode());
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x00002ECE File Offset: 0x000010CE
	public void SleepingEvent(AnimationEvent animEvent)
	{
	}
}
