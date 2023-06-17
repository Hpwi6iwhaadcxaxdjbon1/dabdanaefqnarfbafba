using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000094 RID: 148
public class FlameThrower : AttackEntity
{
	// Token: 0x0400057C RID: 1404
	[Header("Flame Thrower")]
	public int maxAmmo = 100;

	// Token: 0x0400057D RID: 1405
	public int ammo = 100;

	// Token: 0x0400057E RID: 1406
	public ItemDefinition fuelType;

	// Token: 0x0400057F RID: 1407
	public float timeSinceLastAttack;

	// Token: 0x04000580 RID: 1408
	[FormerlySerializedAs("nextAttackTime")]
	public float nextReadyTime;

	// Token: 0x04000581 RID: 1409
	public float flameRange = 10f;

	// Token: 0x04000582 RID: 1410
	public float flameRadius = 2.5f;

	// Token: 0x04000583 RID: 1411
	public ParticleSystem[] flameEffects;

	// Token: 0x04000584 RID: 1412
	public FlameJet jet;

	// Token: 0x04000585 RID: 1413
	public GameObjectRef fireballPrefab;

	// Token: 0x04000586 RID: 1414
	public List<DamageTypeEntry> damagePerSec;

	// Token: 0x04000587 RID: 1415
	public SoundDefinition flameStart3P;

	// Token: 0x04000588 RID: 1416
	public SoundDefinition flameLoop3P;

	// Token: 0x04000589 RID: 1417
	public SoundDefinition flameStop3P;

	// Token: 0x0400058A RID: 1418
	public SoundDefinition pilotLoopSoundDef;

	// Token: 0x0400058B RID: 1419
	private float tickRate = 0.25f;

	// Token: 0x0400058C RID: 1420
	private float lastFlameTick;

	// Token: 0x0400058D RID: 1421
	public float fuelPerSec;

	// Token: 0x0400058E RID: 1422
	private float ammoRemainder;

	// Token: 0x0400058F RID: 1423
	public float reloadDuration = 3.5f;

	// Token: 0x04000590 RID: 1424
	private bool isReloading;

	// Token: 0x04000591 RID: 1425
	private Sound loopSound;

	// Token: 0x04000592 RID: 1426
	private Sound pilotLoopSound;

	// Token: 0x04000593 RID: 1427
	private float currentGaugeSetting;

	// Token: 0x04000594 RID: 1428
	private bool wasFlameOn;

	// Token: 0x04000595 RID: 1429
	private float nextUnfireTime;

	// Token: 0x04000596 RID: 1430
	private bool firing;

	// Token: 0x060008B5 RID: 2229 RVA: 0x0004DA44 File Offset: 0x0004BC44
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FlameThrower.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x000090C0 File Offset: 0x000072C0
	private bool IsWeaponBusy()
	{
		return Time.realtimeSinceStartup < this.nextReadyTime;
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x000090CF File Offset: 0x000072CF
	private void SetBusyFor(float dur)
	{
		this.nextReadyTime = Time.realtimeSinceStartup + dur;
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x000090DE File Offset: 0x000072DE
	private void ClearBusy()
	{
		this.nextReadyTime = Time.realtimeSinceStartup - 1f;
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0004DA88 File Offset: 0x0004BC88
	public void ReduceAmmo(float firingTime)
	{
		this.ammoRemainder += this.fuelPerSec * firingTime;
		if (this.ammoRemainder >= 1f)
		{
			int num = Mathf.FloorToInt(this.ammoRemainder);
			this.ammoRemainder -= (float)num;
			if (this.ammoRemainder >= 1f)
			{
				num++;
				this.ammoRemainder -= 1f;
			}
			this.ammo -= num;
			if (this.ammo <= 0)
			{
				this.ammo = 0;
			}
		}
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x000090F1 File Offset: 0x000072F1
	public void PilotLightToggle_Shared()
	{
		base.SetFlag(BaseEntity.Flags.On, !base.HasFlag(BaseEntity.Flags.On), false, true);
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x000056A3 File Offset: 0x000038A3
	public bool IsPilotOn()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x00007B4D File Offset: 0x00005D4D
	public bool IsFlameOn()
	{
		return base.HasFlag(BaseEntity.Flags.OnFire);
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x00009106 File Offset: 0x00007306
	public bool HasAmmo()
	{
		return this.GetAmmo() != null;
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x0004DB14 File Offset: 0x0004BD14
	public Item GetAmmo()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		Item item = ownerPlayer.inventory.containerMain.FindItemsByItemName(this.fuelType.shortname);
		if (item == null)
		{
			item = ownerPlayer.inventory.containerBelt.FindItemsByItemName(this.fuelType.shortname);
		}
		return item;
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x0004DB70 File Offset: 0x0004BD70
	public Sound GetFlameLoopSound()
	{
		if (this.loopSound == null)
		{
			this.loopSound = SoundManager.RequestSoundInstance(this.flameLoop3P, base.gameObject, default(Vector3), false);
		}
		return this.loopSound;
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x0004DBB4 File Offset: 0x0004BDB4
	public Sound GetPilotLoopSound()
	{
		if (this.pilotLoopSound == null)
		{
			this.pilotLoopSound = SoundManager.RequestSoundInstance(this.pilotLoopSoundDef, base.gameObject, default(Vector3), false);
		}
		return this.pilotLoopSound;
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x0004DBF8 File Offset: 0x0004BDF8
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		bool flag = ownerPlayer == LocalPlayer.Entity && ownerPlayer.InFirstPersonMode();
		if (!flag)
		{
			ParticleSystem[] array = this.flameEffects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = this.IsFlameOn();
			}
			this.jet.SetOn(this.IsFlameOn());
		}
		else if (this.viewModel != null)
		{
			float b = (float)this.ammo / (float)this.maxAmmo;
			this.currentGaugeSetting = Mathf.Lerp(this.currentGaugeSetting, b, Time.deltaTime * 3f);
			this.viewModel.SetFloat("gauge", this.currentGaugeSetting);
		}
		this.UpdateSounds(flag);
		this.wasFlameOn = this.IsFlameOn();
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0004DCD4 File Offset: 0x0004BED4
	private void UpdateSounds(bool isFirstPerson = false)
	{
		if (this.wasFlameOn != this.IsFlameOn())
		{
			if (this.IsFlameOn())
			{
				SoundManager.PlayOneshot(this.flameStart3P, base.gameObject, isFirstPerson, default(Vector3));
				this.GetFlameLoopSound().FadeInAndPlay(0.1f);
				if (isFirstPerson)
				{
					this.GetFlameLoopSound().MakeFirstPerson();
					this.GetPilotLoopSound().FadeOutAndRecycle(0.3f);
					this.pilotLoopSound = null;
					return;
				}
			}
			else
			{
				this.GetFlameLoopSound().FadeOutAndRecycle(0.3f);
				SoundManager.PlayOneshot(this.flameStop3P, base.gameObject, isFirstPerson, default(Vector3));
				this.loopSound = null;
				if (isFirstPerson)
				{
					this.GetPilotLoopSound().MakeFirstPerson();
					this.GetPilotLoopSound().FadeInAndPlay(0.1f);
				}
			}
		}
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x0004DDA0 File Offset: 0x0004BFA0
	public override void OnInput()
	{
		base.OnInput();
		if (!this.IsFullyDeployed())
		{
			return;
		}
		this.timeSinceLastAttack += Time.deltaTime;
		if (!this.IsWeaponBusy() && this.isReloading)
		{
			this.isReloading = false;
			base.ServerRPC("DoReload");
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		bool flag = Physics.Raycast(ownerPlayer.transform.position + new Vector3(0f, 1.5f, 0f), ownerPlayer.eyes.BodyForward(), 1.1f, 1218652417);
		bool flag2 = ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY);
		if (flag2)
		{
			this.nextUnfireTime = Time.realtimeSinceStartup + this.tickRate;
		}
		bool flag3 = flag2 || Time.realtimeSinceStartup < this.nextUnfireTime;
		bool flag4 = ownerPlayer.CanAttack() && !ownerPlayer.IsRunning() && flag3 && this.ammo > 0 && !this.IsWeaponBusy() && !flag;
		if (this.viewModel)
		{
			this.viewModel.SetBool("fire", flag4);
		}
		if (flag4 != this.firing)
		{
			base.ServerRPC<bool>("SetFiring", flag4);
		}
		this.firing = flag4;
		if (!flag4 && !this.isReloading && !this.IsWeaponBusy() && ownerPlayer.input.state.IsDown(BUTTON.RELOAD) && this.HasAmmo())
		{
			this.SetBusyFor(this.reloadDuration);
			if (this.viewModel)
			{
				this.viewModel.Play("reload");
			}
			if (this.worldModelAnimator != null)
			{
				this.worldModelAnimator.SetTrigger("reload");
			}
			this.isReloading = true;
			ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Reload, "");
		}
		if (flag4)
		{
			this.timeSinceLastAttack = 0f;
			this.ReduceAmmo(Time.deltaTime);
			this.UpdateAmmoDisplay();
		}
		this.UpdateFlameStateFirstPerson();
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0004DFA0 File Offset: 0x0004C1A0
	public void UpdateFlameStateFirstPerson()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (this.viewModel && this.viewModel.instance && ownerPlayer.InFirstPersonMode())
		{
			this.viewModel.instance.GetComponent<flamethrowerFire>().flameState = (this.firing ? flamethrowerState.FLAME_ON : (this.IsPilotOn() ? flamethrowerState.PILOT_LIGHT : flamethrowerState.OFF));
		}
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00009111 File Offset: 0x00007311
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
		this.viewModel;
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x00009126 File Offset: 0x00007326
	public override void OnHolstered()
	{
		this.isReloading = false;
		base.OnHolstered();
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0004E010 File Offset: 0x0004C210
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.ammo = info.msg.baseProjectile.primaryMagazine.contents;
			this.UpdateAmmoDisplay();
		}
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x0004E064 File Offset: 0x0004C264
	public override void GetItemOptions(List<Option> options)
	{
		base.GetItemOptions(options);
		if (this.ammo > 0)
		{
			options.Add(new Option
			{
				icon = "menu_dots",
				title = "unload_ammo",
				desc = "unload_ammo_desc",
				command = "unload_ammo",
				order = 10
			});
		}
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x0004E0CC File Offset: 0x0004C2CC
	protected virtual void UpdateAmmoDisplay()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.amountTextOverride = string.Format("{0}", this.ammo);
		LocalPlayer.OnItemAmountChanged();
	}
}
