using System;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class BowWeapon : BaseProjectile
{
	// Token: 0x040004B4 RID: 1204
	protected bool attackReady;

	// Token: 0x040004B5 RID: 1205
	private float arrowBack;

	// Token: 0x040004B6 RID: 1206
	private SwapArrows swapArrows;

	// Token: 0x060007D5 RID: 2005 RVA: 0x000490A8 File Offset: 0x000472A8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BowWeapon.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x00008649 File Offset: 0x00006849
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.swapArrows = base.GetComponent<SwapArrows>();
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x000490EC File Offset: 0x000472EC
	public override void OnInput()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		bool flag = ownerPlayer.CanAttack();
		this.aiming = this.IsAiming();
		if (this.viewModel)
		{
			this.viewModel.ironSights = (flag && this.aiming);
			this.viewModel.SetBool("aiming", this.IsAiming() && flag);
		}
		ownerPlayer.modelState.aiming = this.aiming;
		if (this.worldModelAnimator != null)
		{
			if (ownerPlayer.IsLocalPlayer())
			{
				this.arrowBack = ((this.IsAiming() && flag) ? 1f : 0f);
			}
			else
			{
				this.arrowBack = (ownerPlayer.IsAiming ? 1f : 0f);
			}
			this.worldModelAnimator.SetFloat("arrowPullBack", this.arrowBack, 0.12f, Time.deltaTime);
		}
		if (this.aiming && this.attackReady)
		{
			this.DoAttack();
		}
		if (ownerPlayer.input.state.IsDown(BUTTON.RELOAD) && ownerPlayer.CanAttack())
		{
			if (this.reloadPressTime == 0f)
			{
				this.reloadPressTime = Time.time;
			}
			if (Time.time - this.reloadPressTime > 0.15f && !ContextMenuUI.IsOpen() && base.HasMoreThanOneAmmoType(this.primaryMagazine.definition.ammoTypes))
			{
				ContextMenuUI.Open(base.GetReloadMenu(ownerPlayer), ContextMenuUI.MenuType.Reload);
			}
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0000865E File Offset: 0x0000685E
	public override void OnHolstered()
	{
		base.OnHolstered();
		this.attackReady = false;
		base.SetFlag(BaseEntity.Flags.OnFire, false, false, true);
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x00049264 File Offset: 0x00047464
	public override void DoAttack()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (!ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY))
		{
			return;
		}
		this.attackReady = false;
		if (this.primaryMagazine.contents <= 0)
		{
			this.TryReload();
			return;
		}
		ItemDefinition ammoType = this.primaryMagazine.ammoType;
		this.primaryMagazine.contents--;
		base.StartAttackCooldown(this.repeatDelay);
		base.SendSignalBroadcast(BaseEntity.Signal.Attack, "");
		if (this.viewModel)
		{
			this.viewModel.Play("attack");
		}
		if (this.worldModelAnimator != null)
		{
			this.worldModelAnimator.SetTrigger("fire");
		}
		ItemModProjectile component = ammoType.GetComponent<ItemModProjectile>();
		Debug.Assert(component != null, "Missing ItemModProjectile on " + ammoType);
		base.LaunchProjectileClientside(ammoType, component.numProjectiles, this.aimCone);
		Analytics.ShotArrows++;
		this.TryReload();
		this.UpdateAmmoDisplay();
		this.DidAttackClientside();
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x00049378 File Offset: 0x00047578
	public bool IsAiming()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		if (!ownerPlayer.input.state.IsDown(BUTTON.FIRE_SECONDARY))
		{
			return false;
		}
		if (base.HasAttackCooldown())
		{
			return false;
		}
		if (this.primaryMagazine.contents <= 0)
		{
			this.TryReload();
			return false;
		}
		return true;
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x00008677 File Offset: 0x00006877
	public override void OnViewmodelEvent(string name)
	{
		if (name == "attack_ready")
		{
			this.attackReady = true;
		}
		if (name == "attack_cancel")
		{
			this.attackReady = false;
		}
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x000493D0 File Offset: 0x000475D0
	public override void OnFrame()
	{
		base.OnFrame();
		if (this.viewModel && this.viewModel.instance)
		{
			SwapArrows component = this.viewModel.instance.GetComponent<SwapArrows>();
			if (component)
			{
				component.UpdateAmmoType(this.primaryMagazine.ammoType);
			}
		}
		if (this.swapArrows)
		{
			this.swapArrows.UpdateAmmoType(this.primaryMagazine.ammoType);
		}
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x000086A1 File Offset: 0x000068A1
	private void TryReload()
	{
		if (base.HasReloadCooldown())
		{
			return;
		}
		base.StartReloadCooldown(0.1f);
		base.ServerRPC("BowReload");
	}
}
