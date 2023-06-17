using System;
using UnityEngine;

// Token: 0x0200029F RID: 671
public class FlintStrikeWeapon : BaseProjectile
{
	// Token: 0x04000F85 RID: 3973
	public float successFraction = 0.5f;

	// Token: 0x04000F86 RID: 3974
	public RecoilProperties strikeRecoil;

	// Token: 0x04000F87 RID: 3975
	private bool _didSparkThisFrame;

	// Token: 0x04000F88 RID: 3976
	private bool _isStriking;

	// Token: 0x04000F89 RID: 3977
	private int strikes;

	// Token: 0x060012E3 RID: 4835 RVA: 0x000102B4 File Offset: 0x0000E4B4
	public override RecoilProperties GetRecoil()
	{
		return this.strikeRecoil;
	}

	// Token: 0x060012E4 RID: 4836 RVA: 0x0007A418 File Offset: 0x00078618
	public override void DoAttack()
	{
		if (base.HasAttackCooldown())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (this.primaryMagazine.contents <= 0 && ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY))
		{
			base.DryFire();
			return;
		}
		if (!ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY))
		{
			if (this._isStriking)
			{
				this._isStriking = false;
				base.StartAttackCooldown(0.5f);
				if (this.viewModel && this.viewModel.instance)
				{
					this.viewModel.instance.CrossFade("Idle", 0.1f);
				}
				return;
			}
		}
		else if (this._isStriking)
		{
			if (this._didSparkThisFrame)
			{
				base.DoAttack();
				this.strikes = 0;
				this._isStriking = false;
				this._didSparkThisFrame = false;
				return;
			}
		}
		else
		{
			if (!this._isStriking)
			{
				ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Alt_Attack, "");
			}
			this._isStriking = true;
			if (this.viewModel && this.viewModel.instance)
			{
				this.viewModel.instance.CrossFade("attack2", 0.1f);
			}
		}
	}

	// Token: 0x060012E5 RID: 4837 RVA: 0x0007A55C File Offset: 0x0007875C
	public override void OnFrame()
	{
		base.OnFrame();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if ((!ownerPlayer.CanAttack() || this.isReloading || !this.CanAttack()) && this._isStriking)
		{
			this._isStriking = false;
			base.StartAttackCooldown(0.5f);
			if (this.viewModel && this.viewModel.instance)
			{
				this.viewModel.instance.CrossFade("Idle", 0.1f);
			}
			return;
		}
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x0007A5F0 File Offset: 0x000787F0
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
		if (name == "SparkStrike" && this._isStriking)
		{
			Effect.client.Run("assets/prefabs/weapons/eoka pistol/effects/flint_spark.prefab", base.gameObject);
			this.AddPunch(new Vector3(Random.Range(this.strikeRecoil.recoilPitchMin, this.strikeRecoil.recoilPitchMax), Random.Range(this.strikeRecoil.recoilYawMin, this.strikeRecoil.recoilYawMax), 0f), Random.Range(this.strikeRecoil.timeToTakeMin, this.strikeRecoil.timeToTakeMax));
			if (Random.Range(0f, 1f) <= this.successFraction + (float)this.strikes * 0.05f)
			{
				this._didSparkThisFrame = true;
				return;
			}
			this.strikes++;
		}
	}
}
