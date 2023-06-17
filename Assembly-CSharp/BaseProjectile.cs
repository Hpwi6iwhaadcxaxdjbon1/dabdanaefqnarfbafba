using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using GameMenu;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class BaseProjectile : AttackEntity
{
	// Token: 0x0400044C RID: 1100
	[Header("NPC Info")]
	public float NoiseRadius = 100f;

	// Token: 0x0400044D RID: 1101
	[Header("Projectile")]
	public float damageScale = 1f;

	// Token: 0x0400044E RID: 1102
	public float distanceScale = 1f;

	// Token: 0x0400044F RID: 1103
	public float projectileVelocityScale = 1f;

	// Token: 0x04000450 RID: 1104
	public bool automatic;

	// Token: 0x04000451 RID: 1105
	[Header("Effects")]
	public GameObjectRef attackFX;

	// Token: 0x04000452 RID: 1106
	public GameObjectRef silencedAttack;

	// Token: 0x04000453 RID: 1107
	public GameObjectRef muzzleBrakeAttack;

	// Token: 0x04000454 RID: 1108
	public Transform MuzzlePoint;

	// Token: 0x04000455 RID: 1109
	[Header("Reloading")]
	public float reloadTime = 1f;

	// Token: 0x04000456 RID: 1110
	public bool canUnloadAmmo = true;

	// Token: 0x04000457 RID: 1111
	public BaseProjectile.Magazine primaryMagazine;

	// Token: 0x04000458 RID: 1112
	[Header("Recoil")]
	public float aimSway = 3f;

	// Token: 0x04000459 RID: 1113
	public float aimSwaySpeed = 1f;

	// Token: 0x0400045A RID: 1114
	public RecoilProperties recoil;

	// Token: 0x0400045B RID: 1115
	[Header("Aim Cone")]
	public AnimationCurve aimconeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x0400045C RID: 1116
	public float aimCone;

	// Token: 0x0400045D RID: 1117
	public float hipAimCone = 1.8f;

	// Token: 0x0400045E RID: 1118
	public float aimconePenaltyPerShot;

	// Token: 0x0400045F RID: 1119
	public float aimConePenaltyMax;

	// Token: 0x04000460 RID: 1120
	public float aimconePenaltyRecoverTime = 0.1f;

	// Token: 0x04000461 RID: 1121
	public float aimconePenaltyRecoverDelay = 0.1f;

	// Token: 0x04000462 RID: 1122
	public float stancePenaltyScale = 1f;

	// Token: 0x04000463 RID: 1123
	[Header("Iconsights")]
	public bool hasADS = true;

	// Token: 0x04000464 RID: 1124
	public bool noAimingWhileCycling;

	// Token: 0x04000465 RID: 1125
	public bool manualCycle;

	// Token: 0x04000466 RID: 1126
	[NonSerialized]
	protected bool needsCycle;

	// Token: 0x04000467 RID: 1127
	[NonSerialized]
	protected bool isCycling;

	// Token: 0x04000468 RID: 1128
	[NonSerialized]
	public bool aiming;

	// Token: 0x04000469 RID: 1129
	private float nextReloadTime = float.NegativeInfinity;

	// Token: 0x0400046A RID: 1130
	private float stancePenalty;

	// Token: 0x0400046B RID: 1131
	private float aimconePenalty;

	// Token: 0x0400046C RID: 1132
	[NonSerialized]
	protected bool isReloading;

	// Token: 0x0400046D RID: 1133
	private float swaySampleTime;

	// Token: 0x0400046E RID: 1134
	public float resetDuration = 0.3f;

	// Token: 0x0400046F RID: 1135
	public int numShotsFired;

	// Token: 0x04000470 RID: 1136
	private float lastShotTime;

	// Token: 0x04000471 RID: 1137
	[NonSerialized]
	public float reloadPressTime;

	// Token: 0x04000472 RID: 1138
	private ItemDefinition ammoTypePreReload;

	// Token: 0x04000473 RID: 1139
	private bool triggerReady = true;

	// Token: 0x04000474 RID: 1140
	private List<Projectile> createdProjectiles = new List<Projectile>();

	// Token: 0x06000763 RID: 1891 RVA: 0x00047350 File Offset: 0x00045550
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseProjectile.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x0000828A File Offset: 0x0000648A
	public virtual float GetDamageScale(bool getMax = false)
	{
		return this.damageScale;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x00008292 File Offset: 0x00006492
	public virtual float GetDistanceScale(bool getMax = false)
	{
		return this.distanceScale;
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x0000829A File Offset: 0x0000649A
	public virtual float GetProjectileVelocityScale(bool getMax = false)
	{
		return this.projectileVelocityScale;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x000082A2 File Offset: 0x000064A2
	protected void StartReloadCooldown(float cooldown)
	{
		this.nextReloadTime = base.CalculateCooldownTime(this.nextReloadTime, cooldown, false);
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x000082B8 File Offset: 0x000064B8
	protected void ResetReloadCooldown()
	{
		this.nextReloadTime = float.NegativeInfinity;
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x000082C5 File Offset: 0x000064C5
	protected bool HasReloadCooldown()
	{
		return UnityEngine.Time.time < this.nextReloadTime;
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x000082D4 File Offset: 0x000064D4
	protected float GetReloadCooldown()
	{
		return Mathf.Max(this.nextReloadTime - UnityEngine.Time.time, 0f);
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x000082EC File Offset: 0x000064EC
	protected float GetReloadIdle()
	{
		return Mathf.Max(UnityEngine.Time.time - this.nextReloadTime, 0f);
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x00047394 File Offset: 0x00045594
	private void OnDrawGizmos()
	{
		if (!base.isClient)
		{
			return;
		}
		if (this.MuzzlePoint != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(this.MuzzlePoint.position, this.MuzzlePoint.position + this.MuzzlePoint.forward * 10f);
			BasePlayer ownerPlayer = base.GetOwnerPlayer();
			if (ownerPlayer)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(this.MuzzlePoint.position, this.MuzzlePoint.position + ownerPlayer.eyes.rotation * Vector3.forward * 10f);
			}
		}
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x00008304 File Offset: 0x00006504
	public virtual RecoilProperties GetRecoil()
	{
		return this.recoil;
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x00047454 File Offset: 0x00045654
	public virtual float GetAimCone()
	{
		float num = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.sightAimCone, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
		float num2 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.sightAimCone, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		float num3 = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.hipAimCone, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
		float num4 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.hipAimCone, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		if (this.aiming || base.isServer)
		{
			return (this.aimCone + this.aimconePenalty + this.stancePenalty * this.stancePenaltyScale) * num + num2;
		}
		return (this.aimCone + this.aimconePenalty + this.stancePenalty * this.stancePenaltyScale) * num + num2 + this.hipAimCone * num3 + num4;
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x000475E4 File Offset: 0x000457E4
	public float ScaleRepeatDelay(float delay)
	{
		float num = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.repeatDelay, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
		float num2 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.repeatDelay, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		return delay * num + num2;
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x0004768C File Offset: 0x0004588C
	public Projectile.Modifier GetProjectileModifier()
	{
		Projectile.Modifier result = default(Projectile.Modifier);
		result.damageOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileDamage, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		result.damageScale = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileDamage, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * this.GetDamageScale(false);
		result.distanceOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileDistance, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		result.distanceScale = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileDistance, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * this.GetDistanceScale(false);
		return result;
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x0000830C File Offset: 0x0000650C
	public virtual void DidAttackClientside()
	{
		Rust.GC.Pause(5f);
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x000477F4 File Offset: 0x000459F4
	public override void GetItemOptions(List<Option> options)
	{
		base.GetItemOptions(options);
		if (this.canUnloadAmmo && this.primaryMagazine.contents > 0)
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

	// Token: 0x06000773 RID: 1907 RVA: 0x00047868 File Offset: 0x00045A68
	public override void EditViewAngles()
	{
		base.EditViewAngles();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ownerPlayer.input.state.IsDown(BUTTON.FIRE_SECONDARY))
		{
			ownerPlayer.BlockSprint(0.2f);
			if (base.NextAttackTime < UnityEngine.Time.time - 3f)
			{
				float num = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.aimsway, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
				float num2 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.aimsway, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
				float num3 = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.aimswaySpeed, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
				float num4 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.aimswaySpeed, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
				bool flag = ownerPlayer.IsDucked();
				this.swaySampleTime += UnityEngine.Time.deltaTime * (this.aimSwaySpeed * num3 + num4);
				float num5 = Mathf.Sin(UnityEngine.Time.time * 2f);
				float num6 = (num5 < 0f) ? (1f - Mathf.Clamp(Mathf.Abs(num5) / 1f, 0f, 1f)) : 1f;
				float num7 = flag ? 0.6f : 1f;
				float num8 = 1f - Mathf.Clamp01(ownerPlayer.clothingAccuracyBonus);
				float num9 = (this.aimSway * num + num2) * num7 * num6 * num8;
				Vector3 viewVars = ownerPlayer.input.ClientLookVars();
				viewVars.y += (Mathf.PerlinNoise(this.swaySampleTime, this.swaySampleTime) - 0.5f) * num9 * UnityEngine.Time.deltaTime;
				viewVars.x += (Mathf.PerlinNoise(this.swaySampleTime + 0.1f, this.swaySampleTime + 0.2f) - 0.5f) * num9 * UnityEngine.Time.deltaTime;
				ownerPlayer.input.SetViewVars(viewVars);
			}
		}
		this.SimulateAimcone();
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00008318 File Offset: 0x00006518
	public override void OnFrame()
	{
		base.OnFrame();
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x00047B14 File Offset: 0x00045D14
	public virtual void ShotFired()
	{
		this.numShotsFired++;
		this.aimconePenalty += this.aimconePenaltyPerShot;
		if (this.aimconePenalty > this.aimConePenaltyMax)
		{
			this.aimconePenalty = this.aimConePenaltyMax;
		}
		this.lastShotTime = UnityEngine.Time.time;
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00047B68 File Offset: 0x00045D68
	public virtual void SimulateAimcone()
	{
		float num = UnityEngine.Time.time - this.lastShotTime;
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (num > this.repeatDelay * 2f)
		{
			int num2 = Mathf.CeilToInt(this.resetDuration * (float)this.primaryMagazine.capacity * num);
			this.numShotsFired = Mathf.Clamp(this.numShotsFired - num2, 0, this.primaryMagazine.capacity);
			this.numShotsFired = 0;
		}
		if (this.recoil != null && this.recoil.useCurves)
		{
			float y = this.recoil.yawCurve.Evaluate((float)this.numShotsFired / (float)this.primaryMagazine.capacity) * this.recoil.recoilYawMax;
			float x = this.recoil.pitchCurve.Evaluate((float)this.numShotsFired / (float)this.primaryMagazine.capacity) * this.recoil.recoilPitchMax;
			if ((float)this.numShotsFired == 0f && ownerPlayer.input.recoilAngles != Vector3.zero)
			{
				ownerPlayer.input.FinalizeRecoil();
			}
			Vector3 recoilAngles = Vector3.MoveTowards(ownerPlayer.input.recoilAngles, new Vector3(x, y, 0f), UnityEngine.Time.deltaTime * 20f);
			ownerPlayer.input.recoilAngles = recoilAngles;
		}
		float target = 0f;
		if (ownerPlayer.movement.IsDucked)
		{
			target = 0f;
		}
		else
		{
			if (this.numShotsFired > 0)
			{
				target = 0.5f;
			}
			if (ownerPlayer.movement.CurrentMoveSpeed() > 0.25f)
			{
				target = 1f;
			}
		}
		this.stancePenalty = Mathf.MoveTowards(this.stancePenalty, target, UnityEngine.Time.deltaTime * 2f);
		if (num > this.aimconePenaltyRecoverTime)
		{
			this.aimconePenalty = 0f;
		}
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x00008320 File Offset: 0x00006520
	public virtual bool CanAttack()
	{
		return !ProjectileWeaponMod.HasBrokenWeaponMod(this);
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x0000832D File Offset: 0x0000652D
	public virtual bool CanAim()
	{
		return this.hasADS && !ProjectileWeaponMod.HasBrokenWeaponMod(this);
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00047D3C File Offset: 0x00045F3C
	public override void OnInput()
	{
		base.OnInput();
		bool flag = this.IsFullyDeployed();
		if (this.isCycling && !base.HasAttackCooldown())
		{
			this.isCycling = false;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		bool flag2 = ownerPlayer.CanAttack() && !this.isReloading && this.CanAttack();
		bool flag3 = this.aiming;
		this.aiming = (this.CanAim() && flag && flag2 && ownerPlayer.input.state.IsDown(BUTTON.FIRE_SECONDARY) && (!this.noAimingWhileCycling || !this.isCycling) && !ownerPlayer.clothingBlocksAiming);
		if (this.needsCycle && flag3 && !this.aiming)
		{
			this.BeginCycle();
		}
		if (this.viewModel)
		{
			this.viewModel.ironSights = this.aiming;
			ownerPlayer.modelState.aiming = this.aiming;
		}
		if (this.children != null)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				ProjectileWeaponMod projectileWeaponMod = this.children[i] as ProjectileWeaponMod;
				if (projectileWeaponMod)
				{
					projectileWeaponMod.SetAiming(this.aiming);
				}
			}
		}
		if (!flag)
		{
			return;
		}
		if (ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY))
		{
			ownerPlayer.BlockSprint(0.2f);
		}
		if (flag2)
		{
			this.DoAttack();
		}
		this.DoReload();
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00008344 File Offset: 0x00006544
	public bool ReadyToAim()
	{
		return this.IsFullyDeployed() && !this.isReloading && (!this.manualCycle || !this.isCycling);
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x0000836B File Offset: 0x0000656B
	public bool ReadyToFire()
	{
		return this.ReadyToAim() && !base.HasAttackCooldown();
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00008380 File Offset: 0x00006580
	public void AmmoTypeClicked(ItemDefinition newAmmoType)
	{
		this.primaryMagazine.ammoType = newAmmoType;
		base.ServerRPC<int>("SwitchAmmoTo", newAmmoType.itemid);
		this.reloadPressTime = UnityEngine.Time.time + 1.5f;
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00047EAC File Offset: 0x000460AC
	private void AddAmmoOption(ItemDefinition ammo, List<Option> opts, int order = 0)
	{
		Option option = default(Option);
		option.title = ammo.displayName.token;
		option.desc = ammo.displayDescription.token;
		option.iconSprite = ammo.iconSprite;
		option.order = order;
		option.show = true;
		ItemDefinition ammoType = ammo;
		option.function = delegate(BasePlayer ply)
		{
			this.AmmoTypeClicked(ammoType);
		};
		opts.Add(option);
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00047F30 File Offset: 0x00046130
	public List<Option> GetReloadMenu(BasePlayer player)
	{
		List<Option> list = new List<Option>();
		List<ItemDefinition> list2 = new List<ItemDefinition>();
		List<Item> list3 = new List<Item>();
		player.inventory.FindAmmo(list3, this.primaryMagazine.definition.ammoTypes);
		int num = 0;
		this.AddAmmoOption(this.primaryMagazine.ammoType, list, 0);
		list2.Add(this.primaryMagazine.ammoType);
		foreach (Item item in list3)
		{
			if (!list2.Contains(item.info))
			{
				list2.Add(item.info);
				this.AddAmmoOption(item.info, list, num);
				num++;
			}
		}
		return list;
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00048000 File Offset: 0x00046200
	private void PredictAmmoType()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		List<Item> list = ownerPlayer.inventory.FindItemIDs(this.primaryMagazine.ammoType.itemid);
		if (list.Count == 0)
		{
			List<Item> list2 = Pool.GetList<Item>();
			ownerPlayer.inventory.FindAmmo(list2, this.primaryMagazine.definition.ammoTypes);
			if (list2.Count != 0)
			{
				list = ownerPlayer.inventory.FindItemIDs(list2[0].info.itemid);
				if (list.Count != 0)
				{
					this.primaryMagazine.ammoType = list[0].info;
				}
			}
			Pool.FreeList<Item>(ref list2);
		}
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x000480B0 File Offset: 0x000462B0
	public bool HasMoreThanOneAmmoType(AmmoTypes ammoType)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		List<Item> list = Pool.GetList<Item>();
		ownerPlayer.inventory.FindAmmo(list, ammoType);
		ItemDefinition itemDefinition = null;
		for (int i = 0; i < list.Count; i++)
		{
			Item item = list[i];
			if (itemDefinition == null)
			{
				itemDefinition = item.info;
			}
			else if (item.info != itemDefinition)
			{
				Pool.FreeList<Item>(ref list);
				return true;
			}
		}
		if (itemDefinition != null && itemDefinition != this.primaryMagazine.ammoType && this.primaryMagazine.contents > 0)
		{
			Pool.FreeList<Item>(ref list);
			return true;
		}
		Pool.FreeList<Item>(ref list);
		return false;
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x000083B0 File Offset: 0x000065B0
	public override void OnDeploy()
	{
		base.OnDeploy();
		this.isReloading = false;
		this.reloadPressTime = 0f;
		this.ResetReloadCooldown();
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x00048164 File Offset: 0x00046364
	private void DoReload()
	{
		if (this.HasReloadCooldown())
		{
			return;
		}
		if (this.isReloading)
		{
			this.isCycling = false;
			this.needsCycle = false;
			this.isReloading = false;
			base.ServerRPC("Reload");
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		bool flag = this.HasMoreThanOneAmmoType(this.primaryMagazine.definition.ammoTypes);
		bool flag2 = ownerPlayer.input.state.IsDown(BUTTON.RELOAD);
		if (flag2)
		{
			if (this.reloadPressTime == 0f)
			{
				this.reloadPressTime = UnityEngine.Time.time;
				if (flag)
				{
					this.ammoTypePreReload = this.primaryMagazine.ammoType;
				}
			}
			if (flag && UnityEngine.Time.time - this.reloadPressTime > 0.15f && !ContextMenuUI.IsOpen())
			{
				ContextMenuUI.Open(this.GetReloadMenu(ownerPlayer), ContextMenuUI.MenuType.Reload);
			}
		}
		if (!flag2 || !flag)
		{
			float num = this.reloadPressTime;
			float time = UnityEngine.Time.time;
			if (this.reloadPressTime != 0f)
			{
				bool flag3 = flag && this.primaryMagazine.ammoType != this.ammoTypePreReload;
				if (this.primaryMagazine.CanReload(ownerPlayer) || flag3)
				{
					this.PredictAmmoType();
					this.isReloading = true;
					this.StartReloadCooldown(this.reloadTime);
					base.ServerRPC("StartReload");
					if (this.viewModel)
					{
						this.viewModel.Play("reload");
					}
					if (this.worldModelAnimator != null)
					{
						this.worldModelAnimator.SetTrigger("reload");
					}
					ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Reload, "");
				}
				this.reloadPressTime = 0f;
			}
		}
	}

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000783 RID: 1923 RVA: 0x000083D0 File Offset: 0x000065D0
	public bool isSemiAuto
	{
		get
		{
			return !this.automatic;
		}
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00048300 File Offset: 0x00046500
	public virtual void DoAttack()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (this.automatic)
		{
			if (!ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY))
			{
				return;
			}
		}
		else
		{
			bool flag = ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY);
			if (!flag)
			{
				this.triggerReady = true;
				return;
			}
			if (!this.triggerReady || !flag)
			{
				return;
			}
		}
		if (this.manualCycle && this.needsCycle)
		{
			this.BeginCycle();
			return;
		}
		if (base.HasAttackCooldown())
		{
			return;
		}
		if (this.isSemiAuto)
		{
			this.triggerReady = false;
		}
		if (this.primaryMagazine.contents <= 0 && !this.UsingInfiniteAmmoCheat)
		{
			if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY))
			{
				this.DryFire();
			}
			return;
		}
		if (!this.UsingInfiniteAmmoCheat)
		{
			this.primaryMagazine.contents--;
		}
		if (ConVar.Client.prediction)
		{
			this.OnSignal(BaseEntity.Signal.Attack, string.Empty);
		}
		if (this.viewModel)
		{
			this.viewModel.Play("attack");
			if (this.recoil)
			{
				float num = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.recoil, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
				float num2 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.recoil, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
				float num3 = this.aiming ? this.recoil.ADSScale : 1f;
				float num4 = ownerPlayer.movement.IsDucked ? 0.5f : 1f;
				float speed = ownerPlayer.GetSpeed(0f, 0f);
				float num5 = Mathf.Clamp01(ownerPlayer.movement.CurrentMoveSpeed() / speed);
				float num6 = 1f + num5 * this.recoil.movementPenalty;
				float num7 = num3 * num4 * num * num6;
				if (!this.recoil.useCurves)
				{
					float x2 = Random.Range(this.recoil.recoilPitchMin, this.recoil.recoilPitchMax) * num7 + num2;
					float y2 = Random.Range(this.recoil.recoilYawMin, this.recoil.recoilYawMax) * num7 + num2;
					float duration = Random.Range(this.recoil.timeToTakeMin, this.recoil.timeToTakeMax);
					this.AddPunch(new Vector3(x2, y2, 0f), duration);
				}
			}
		}
		this.LaunchProjectile();
		this.UpdateAmmoDisplay();
		this.ShotFired();
		this.DidAttackClientside();
		if (!this.manualCycle)
		{
			this.BeginCycle();
			return;
		}
		this.needsCycle = true;
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x000083DB File Offset: 0x000065DB
	public void BeginCycle()
	{
		base.StartAttackCooldown(this.ScaleRepeatDelay(this.repeatDelay));
		this.isCycling = true;
		this.needsCycle = false;
		if (this.manualCycle)
		{
			this.viewModel.Play("manualcycle");
		}
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x000485F0 File Offset: 0x000467F0
	public virtual void LaunchProjectile()
	{
		ItemModProjectile component = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
		if (!component)
		{
			Debug.LogError("NO ITEMMODPROJECTILE FOR AMMO: " + this.primaryMagazine.ammoType.displayName.english);
			return;
		}
		this.LaunchProjectileClientside(this.primaryMagazine.ammoType, component.numProjectiles, this.GetAimCone());
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x00008415 File Offset: 0x00006615
	public void DryFire()
	{
		base.StartAttackCooldown(this.repeatDelay);
		base.SendSignalBroadcast(BaseEntity.Signal.DryFire, "");
		if (this.viewModel)
		{
			this.viewModel.Play("dryfire");
		}
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x00048658 File Offset: 0x00046858
	public bool IsSilenced()
	{
		if (this.children != null)
		{
			if (Enumerable.Any<ProjectileWeaponMod>(Enumerable.Where<ProjectileWeaponMod>(Enumerable.Cast<ProjectileWeaponMod>(this.children), (ProjectileWeaponMod x) => x != null && x.isSilencer && !x.IsBroken())))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x000486A8 File Offset: 0x000468A8
	public override void OnSignal(BaseEntity.Signal signal, string arg)
	{
		base.OnSignal(signal, arg);
		if (signal == BaseEntity.Signal.Attack)
		{
			string resourcePath = this.attackFX.resourcePath;
			if (this.children != null)
			{
				foreach (ProjectileWeaponMod projectileWeaponMod in Enumerable.Where<ProjectileWeaponMod>(Enumerable.Cast<ProjectileWeaponMod>(this.children), (ProjectileWeaponMod x) => x != null))
				{
					if (projectileWeaponMod.isSilencer)
					{
						resourcePath = projectileWeaponMod.defaultSilencerEffect.resourcePath;
						if (this.silencedAttack.isValid)
						{
							resourcePath = this.silencedAttack.resourcePath;
							break;
						}
						break;
					}
					else if (projectileWeaponMod.isMuzzleBoost || projectileWeaponMod.isMuzzleBrake)
					{
						if (this.muzzleBrakeAttack.isValid)
						{
							resourcePath = this.muzzleBrakeAttack.resourcePath;
							break;
						}
						break;
					}
				}
			}
			if (this.worldModelAnimator != null)
			{
				this.worldModelAnimator.SetTrigger("fire");
			}
			Effect.client.Run(resourcePath, base.gameObject);
			BasePlayer ownerPlayer = base.GetOwnerPlayer();
			if (ownerPlayer)
			{
				ownerPlayer.OnSignal(BaseEntity.Signal.Attack, arg);
			}
		}
		if (signal == BaseEntity.Signal.Reload && this.worldModelAnimator != null)
		{
			this.worldModelAnimator.SetTrigger("reload");
		}
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x00048800 File Offset: 0x00046A00
	internal void LaunchProjectileClientside(ItemDefinition ammo, int projectileCount, float projSpreadaimCone)
	{
		projSpreadaimCone = 2f;
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		ItemModProjectile component = ammo.GetComponent<ItemModProjectile>();
		if (component == null)
		{
			Debug.Log("Ammo doesn't have a Projectile module!");
			return;
		}
		this.createdProjectiles.Clear();
		float num = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileVelocity, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
		float num2 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileVelocity, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		using (ProjectileShoot projectileShoot = Pool.Get<ProjectileShoot>())
		{
			projectileShoot.projectiles = new List<ProjectileShoot.Projectile>();
			projectileShoot.ammoType = ammo.itemid;
			for (int i = 0; i < projectileCount; i++)
			{
				Vector3 position = ownerPlayer.eyes.position;
				Vector3 vector = ownerPlayer.eyes.BodyForward();
				if (projSpreadaimCone > 0f || component.projectileSpread > 0f)
				{
					Quaternion rotation = ownerPlayer.eyes.rotation;
					float num3 = this.aimconeCurve.Evaluate(Random.Range(0f, 1f));
					float num4 = (projectileCount > 1) ? component.GetIndexedSpreadScalar(i, projectileCount) : component.GetSpreadScalar();
					vector = AimConeUtil.GetModifiedAimConeDirection(num3 * projSpreadaimCone + component.projectileSpread * num4, rotation * Vector3.forward, projectileCount <= 1);
					if (Global.developer > 0)
					{
						UnityEngine.DDraw.Arrow(position, position + vector * 3f, 0.1f, Color.white, 20f);
					}
				}
				AttackEntity component2 = base.gameManager.FindPrefab(this).GetComponent<AttackEntity>();
				Projectile component3 = component.projectileObject.Get().GetComponent<Projectile>();
				Vector3 vector2 = vector * (component.GetRandomVelocity() * this.GetProjectileVelocityScale(false) * num + num2);
				int seed = ownerPlayer.NewProjectileSeed();
				int projectileID = ownerPlayer.NewProjectileID();
				Projectile projectile = this.CreateProjectile(component.projectileObject.resourcePath, position, vector, vector2);
				if (projectile != null)
				{
					projectile.mod = component;
					projectile.seed = seed;
					projectile.owner = ownerPlayer;
					projectile.sourceWeaponPrefab = component2;
					projectile.sourceProjectilePrefab = component3;
					projectile.projectileID = projectileID;
					projectile.invisible = this.IsSilenced();
					this.createdProjectiles.Add(projectile);
				}
				ProjectileShoot.Projectile projectile2 = new ProjectileShoot.Projectile();
				projectile2.projectileID = projectileID;
				projectile2.startPos = position;
				projectile2.startVel = vector2;
				projectile2.seed = seed;
				projectileShoot.projectiles.Add(projectile2);
			}
			base.ServerRPC<ProjectileShoot>("CLProject", projectileShoot);
			Vector3 inheritedProjectileVelocity = ownerPlayer.GetInheritedProjectileVelocity();
			foreach (Projectile projectile3 in this.createdProjectiles)
			{
				projectile3.Launch();
				projectile3.AdjustVelocity(inheritedProjectileVelocity);
			}
			this.createdProjectiles.Clear();
		}
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x00048B74 File Offset: 0x00046D74
	private Projectile CreateProjectile(string prefabPath, Vector3 pos, Vector3 forward, Vector3 velocity)
	{
		GameObject gameObject = base.gameManager.CreatePrefab(prefabPath, pos, Quaternion.LookRotation(forward), true);
		if (gameObject == null)
		{
			return null;
		}
		Projectile component = gameObject.GetComponent<Projectile>();
		component.InitializeVelocity(velocity);
		component.modifier = this.GetProjectileModifier();
		return component;
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x00048BBC File Offset: 0x00046DBC
	protected virtual void UpdateAmmoDisplay()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.amountTextOverride = string.Format("{0}", this.primaryMagazine.contents);
		LocalPlayer.OnItemAmountChanged();
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x0600078D RID: 1933 RVA: 0x0000508F File Offset: 0x0000328F
	private bool UsingInfiniteAmmoCheat
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x00048BFC File Offset: 0x00046DFC
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.primaryMagazine.Load(info.msg.baseProjectile.primaryMagazine);
		}
		this.UpdateAmmoDisplay();
	}

	// Token: 0x02000076 RID: 118
	[Serializable]
	public class Magazine
	{
		// Token: 0x04000475 RID: 1141
		public BaseProjectile.Magazine.Definition definition;

		// Token: 0x04000476 RID: 1142
		public int capacity;

		// Token: 0x04000477 RID: 1143
		public int contents;

		// Token: 0x04000478 RID: 1144
		[ItemSelector(ItemCategory.All)]
		public ItemDefinition ammoType;

		// Token: 0x06000790 RID: 1936 RVA: 0x0000844C File Offset: 0x0000664C
		public void ServerInit()
		{
			if (this.definition.builtInSize > 0)
			{
				this.capacity = this.definition.builtInSize;
			}
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x00048D50 File Offset: 0x00046F50
		public ProtoBuf.Magazine Save()
		{
			ProtoBuf.Magazine magazine = Pool.Get<ProtoBuf.Magazine>();
			if (this.ammoType == null)
			{
				magazine.capacity = this.capacity;
				magazine.contents = 0;
				magazine.ammoType = 0;
			}
			else
			{
				magazine.capacity = this.capacity;
				magazine.contents = this.contents;
				magazine.ammoType = this.ammoType.itemid;
			}
			return magazine;
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x0000846D File Offset: 0x0000666D
		public void Load(ProtoBuf.Magazine mag)
		{
			this.contents = mag.contents;
			this.capacity = mag.capacity;
			this.ammoType = ItemManager.FindItemDefinition(mag.ammoType);
		}

		// Token: 0x06000793 RID: 1939 RVA: 0x00008498 File Offset: 0x00006698
		public bool CanReload(BasePlayer owner)
		{
			return this.contents < this.capacity && owner.inventory.HasAmmo(this.definition.ammoTypes);
		}

		// Token: 0x02000077 RID: 119
		[Serializable]
		public struct Definition
		{
			// Token: 0x04000479 RID: 1145
			[Tooltip("Set to 0 to not use inbuilt mag")]
			public int builtInSize;

			// Token: 0x0400047A RID: 1146
			[InspectorFlags]
			[Tooltip("If using inbuilt mag, will accept these types of ammo")]
			public AmmoTypes ammoTypes;
		}
	}
}
