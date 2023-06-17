using System;
using Network;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class CompoundBowWeapon : BowWeapon
{
	// Token: 0x04000560 RID: 1376
	public float stringHoldDurationMax = 3f;

	// Token: 0x04000561 RID: 1377
	public float stringBonusDamage = 1f;

	// Token: 0x04000562 RID: 1378
	public float stringBonusDistance = 0.5f;

	// Token: 0x04000563 RID: 1379
	public float stringBonusVelocity = 1f;

	// Token: 0x04000564 RID: 1380
	public float movementPenaltyRampUpTime = 0.5f;

	// Token: 0x04000565 RID: 1381
	public SoundDefinition chargeUpSoundDef;

	// Token: 0x04000566 RID: 1382
	public SoundDefinition stringHeldSoundDef;

	// Token: 0x04000567 RID: 1383
	public SoundDefinition drawFinishSoundDef;

	// Token: 0x04000568 RID: 1384
	private Sound chargeUpSound;

	// Token: 0x04000569 RID: 1385
	private Sound stringHeldSound;

	// Token: 0x0400056A RID: 1386
	protected float movementPenalty;

	// Token: 0x0400056B RID: 1387
	private float lastMoveTime;

	// Token: 0x0400056C RID: 1388
	private float currentHoldProgress;

	// Token: 0x0400056D RID: 1389
	internal float stringHoldTimeStart;

	// Token: 0x0400056E RID: 1390
	private bool drawFinishPlayed;

	// Token: 0x06000888 RID: 2184 RVA: 0x0004CEE4 File Offset: 0x0004B0E4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CompoundBowWeapon.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0004CF28 File Offset: 0x0004B128
	public void UpdateMovementPenalty(float delta)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		bool flag = false;
		if (base.isClient)
		{
			if (ownerPlayer == null || ownerPlayer.movement == null)
			{
				return;
			}
			flag = (ownerPlayer.movement.CurrentMoveSpeed() > 0.1f);
		}
		if (flag)
		{
			this.movementPenalty += delta * (1f / this.movementPenaltyRampUpTime);
		}
		else
		{
			this.movementPenalty -= delta * (1f / this.stringHoldDurationMax);
		}
		this.movementPenalty = Mathf.Clamp01(this.movementPenalty);
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x00008E85 File Offset: 0x00007085
	public override void DidAttackClientside()
	{
		base.DidAttackClientside();
		this.stringHoldTimeStart = 0f;
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x0004CFC0 File Offset: 0x0004B1C0
	public override void OnInput()
	{
		base.OnInput();
		this.UpdateMovementPenalty(Time.deltaTime);
		float stringBonusScale = this.GetStringBonusScale();
		this.currentHoldProgress = Mathf.MoveTowards(this.currentHoldProgress, stringBonusScale, Time.deltaTime * 8f);
		PowerBar.Instance.SetProgress(this.currentHoldProgress);
		PowerBar.Instance.SetVisible(this.attackReady && this.stringHoldTimeStart != 0f && this.currentHoldProgress > 0f);
		if (this.viewModel && this.viewModel.instance)
		{
			this.viewModel.instance.SetFloat("drawstrength", this.currentHoldProgress);
		}
		this.UpdateDrawSounds();
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x00008E98 File Offset: 0x00007098
	public override void OnHolstered()
	{
		base.OnHolstered();
		PowerBar.Instance.SetVisible(false);
		this.SetStringHeld(false);
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00008EB2 File Offset: 0x000070B2
	public void SetStringHeld(bool isHeld)
	{
		if (isHeld)
		{
			this.stringHoldTimeStart = Time.time;
		}
		else
		{
			this.stringHoldTimeStart = 0f;
		}
		base.ServerRPC<bool>("RPC_StringHoldStatus", isHeld);
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x00008EDB File Offset: 0x000070DB
	public override void OnViewmodelEvent(string name)
	{
		if (name == "attack_ready")
		{
			this.attackReady = true;
			this.SetStringHeld(true);
		}
		if (name == "attack_cancel")
		{
			this.attackReady = false;
			this.SetStringHeld(false);
		}
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x00008F13 File Offset: 0x00007113
	public float GetLastPlayerMovementTime()
	{
		if (base.isClient)
		{
			return this.lastMoveTime;
		}
		return 0f;
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x00008F29 File Offset: 0x00007129
	public float GetStringBonusScale()
	{
		if (this.stringHoldTimeStart == 0f)
		{
			return 0f;
		}
		return Mathf.Clamp01(Mathf.Clamp01((Time.time - this.stringHoldTimeStart) / this.stringHoldDurationMax) - this.movementPenalty);
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x0004D084 File Offset: 0x0004B284
	public override float GetDamageScale(bool getMax = false)
	{
		float num = getMax ? 1f : this.GetStringBonusScale();
		return this.damageScale + this.stringBonusDamage * num;
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x0004D0B4 File Offset: 0x0004B2B4
	public override float GetDistanceScale(bool getMax = false)
	{
		float num = getMax ? 1f : this.GetStringBonusScale();
		return this.distanceScale + this.stringBonusDistance * num;
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0004D0E4 File Offset: 0x0004B2E4
	public override float GetProjectileVelocityScale(bool getMax = false)
	{
		float num = getMax ? 1f : this.GetStringBonusScale();
		return this.projectileVelocityScale + this.stringBonusVelocity * num;
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x0004D114 File Offset: 0x0004B314
	private void StopDrawSounds()
	{
		if (this.stringHeldSound != null)
		{
			this.stringHeldSound.FadeOutAndRecycle(0.1f);
			this.stringHeldSound = null;
		}
		if (this.chargeUpSound != null)
		{
			this.chargeUpSound.FadeOutAndRecycle(0.1f);
			this.chargeUpSound = null;
		}
		this.drawFinishPlayed = false;
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x0004D174 File Offset: 0x0004B374
	private void UpdateDrawSounds()
	{
		if (this.currentHoldProgress > 0f)
		{
			if (this.currentHoldProgress < 1f)
			{
				if (this.chargeUpSound == null)
				{
					this.chargeUpSound = SoundManager.RequestSoundInstance(this.chargeUpSoundDef, base.gameObject, default(Vector3), false);
					if (this.chargeUpSound != null)
					{
						this.chargeUpSound.Play();
					}
				}
				this.drawFinishPlayed = false;
				return;
			}
			if (!this.drawFinishPlayed)
			{
				SoundManager.PlayOneshot(this.drawFinishSoundDef, base.gameObject, false, default(Vector3));
				this.drawFinishPlayed = true;
			}
			if (this.chargeUpSound != null)
			{
				this.chargeUpSound.FadeOutAndRecycle(0.3f);
				this.chargeUpSound = null;
			}
			if (this.stringHeldSound == null)
			{
				this.stringHeldSound = SoundManager.RequestSoundInstance(this.stringHeldSoundDef, base.gameObject, default(Vector3), false);
				if (this.stringHeldSound != null)
				{
					this.stringHeldSound.FadeInAndPlay(0.1f);
					return;
				}
			}
		}
		else
		{
			this.StopDrawSounds();
		}
	}
}
