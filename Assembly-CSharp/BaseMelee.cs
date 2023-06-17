using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using GameTips;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class BaseMelee : AttackEntity
{
	// Token: 0x040003F0 RID: 1008
	[Header("Melee")]
	public DamageProperties damageProperties;

	// Token: 0x040003F1 RID: 1009
	public List<DamageTypeEntry> damageTypes;

	// Token: 0x040003F2 RID: 1010
	public float maxDistance = 1.5f;

	// Token: 0x040003F3 RID: 1011
	public float attackRadius = 0.3f;

	// Token: 0x040003F4 RID: 1012
	public bool isAutomatic = true;

	// Token: 0x040003F5 RID: 1013
	[Header("Effects")]
	public GameObjectRef strikeFX;

	// Token: 0x040003F6 RID: 1014
	public bool useStandardHitEffects = true;

	// Token: 0x040003F7 RID: 1015
	[Header("NPCUsage")]
	public float aiStrikeDelay = 0.2f;

	// Token: 0x040003F8 RID: 1016
	public GameObjectRef swingEffect;

	// Token: 0x040003F9 RID: 1017
	public List<BaseMelee.MaterialFX> materialStrikeFX = new List<BaseMelee.MaterialFX>();

	// Token: 0x040003FA RID: 1018
	[Range(0f, 1f)]
	[Header("Other")]
	public float heartStress = 0.5f;

	// Token: 0x040003FB RID: 1019
	public ResourceDispenser.GatherProperties gathering;

	// Token: 0x040003FC RID: 1020
	[NonSerialized]
	private bool throwReady;

	// Token: 0x040003FD RID: 1021
	[Header("Throwing")]
	public bool canThrowAsProjectile;

	// Token: 0x040003FE RID: 1022
	public bool canAiHearIt;

	// Token: 0x040003FF RID: 1023
	public bool onlyThrowAsProjectile;

	// Token: 0x0600072C RID: 1836 RVA: 0x00046400 File Offset: 0x00044600
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseMelee.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00008036 File Offset: 0x00006236
	public override void GetAttackStats(HitInfo info)
	{
		info.damageTypes.Add(this.damageTypes);
		info.CanGather = this.gathering.Any();
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00046444 File Offset: 0x00044644
	public virtual void DoAttackShared(HitInfo info)
	{
		this.GetAttackStats(info);
		if (base.isClient && info.HitEntity == null)
		{
			TipCannotHarvest.HitNonEntity();
		}
		if (info.HitEntity != null)
		{
			using (TimeWarning.New("OnAttacked", 50L))
			{
				info.HitEntity.OnAttacked(info);
			}
		}
		if (info.DoHitEffects)
		{
			if (base.isServer)
			{
				using (TimeWarning.New("ImpactEffect", 20L))
				{
					Effect.server.ImpactEffect(info);
					return;
				}
			}
			using (TimeWarning.New("ImpactEffect", 20L))
			{
				Effect.client.ImpactEffect(info);
			}
		}
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00046520 File Offset: 0x00044720
	public override void OnInput()
	{
		base.OnInput();
		if (!this.IsFullyDeployed())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		bool flag = ownerPlayer.CanAttack();
		bool flag2 = ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY);
		bool flag3 = ownerPlayer.input.state.IsDown(BUTTON.FIRE_SECONDARY);
		if (this.viewModel && this.canThrowAsProjectile)
		{
			bool flag4 = flag3 && flag && !ownerPlayer.IsHeadUnderwater();
			this.viewModel.ironSights = flag4;
			this.viewModel.SetBool("aiming", flag4);
			ownerPlayer.modelState.aiming = flag4;
		}
		if (flag3 && this.canThrowAsProjectile && !ownerPlayer.IsHeadUnderwater())
		{
			if (flag2 && flag && this.throwReady && !ownerPlayer.IsHeadUnderwater())
			{
				ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Throw, "");
				if (this.viewModel)
				{
					this.viewModel.Trigger("throw");
				}
				this.throwReady = false;
				return;
			}
		}
		else if (flag2 && flag && !this.onlyThrowAsProjectile && (this.isAutomatic || ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY)))
		{
			bool flag5 = base.HasAttackCooldown();
			if ((this.isAutomatic || !flag5) && !flag5)
			{
				base.StartAttackCooldown(this.repeatDelay);
				ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Attack, "");
				if (this.viewModel)
				{
					this.viewModel.Play("attack");
					Analytics.MeleeStrikes++;
				}
			}
		}
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x0000805A File Offset: 0x0000625A
	public override void OnHolstered()
	{
		base.OnHolstered();
		this.throwReady = false;
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x000466C0 File Offset: 0x000448C0
	protected virtual void ProcessAttack(HitTest hit)
	{
		using (PlayerAttack playerAttack = Pool.Get<PlayerAttack>())
		{
			playerAttack.attack = hit.BuildAttackMessage();
			base.ServerRPC<PlayerAttack>("PlayerAttack", playerAttack);
			HitInfo hitInfo = new HitInfo();
			hitInfo.LoadFromAttack(playerAttack.attack, base.isServer);
			hitInfo.Initiator = base.GetOwnerPlayer();
			hitInfo.Weapon = this;
			hitInfo.WeaponPrefab = base.gameManager.FindPrefab(base.PrefabName).GetComponent<AttackEntity>();
			hitInfo.IsPredicting = true;
			hitInfo.damageProperties = this.damageProperties;
			hitInfo.DoHitEffects = this.useStandardHitEffects;
			this.DoAttackShared(hitInfo);
		}
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x00046774 File Offset: 0x00044974
	protected virtual void DoAttack()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		HitTest hitTest = new HitTest();
		hitTest.AttackRay = ownerPlayer.eyes.BodyRay();
		hitTest.MaxDistance = this.maxDistance;
		hitTest.BestHit = true;
		hitTest.damageProperties = this.damageProperties;
		hitTest.ignoreEntity = ownerPlayer;
		hitTest.Radius = 0f;
		hitTest.Forgiveness = Mathf.Min(this.attackRadius, 0.05f);
		hitTest.type = HitTest.Type.MeleeAttack;
		GameTrace.Trace(hitTest, 1269916417);
		ownerPlayer.BlockSprint(this.repeatDelay * 0.5f);
		if (!hitTest.DidHit)
		{
			hitTest.Forgiveness = Mathf.Max(0.05f, this.attackRadius);
			if (!GameTrace.Trace(hitTest, 1269916417))
			{
				return;
			}
		}
		if (!this.CanHit(hitTest))
		{
			return;
		}
		this.DoViewmodelImpact();
		this.ProcessAttack(hitTest);
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x00008069 File Offset: 0x00006269
	public virtual void DoViewmodelImpact()
	{
		if (this.viewModel)
		{
			this.viewModel.Play("attack2");
		}
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x00046858 File Offset: 0x00044A58
	public override void OnViewmodelEvent(string name)
	{
		if (name == "throw_ready")
		{
			this.throwReady = true;
		}
		if (name == "throw_cancel")
		{
			this.throwReady = false;
		}
		if (name == "Strike")
		{
			this.DoAttack();
		}
		if (name == "Throw")
		{
			this.DoThrow();
		}
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x00008088 File Offset: 0x00006288
	public ResourceDispenser.GatherPropertyEntry GetGatherInfoFromIndex(ResourceDispenser.GatherType index)
	{
		return this.gathering.GetFromIndex(index);
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool CanHit(HitTest info)
	{
		return true;
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x000468B4 File Offset: 0x00044AB4
	public float TotalDamage()
	{
		float num = 0f;
		foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
		{
			if (damageTypeEntry.amount > 0f)
			{
				num += damageTypeEntry.amount;
			}
		}
		return num;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool CanBeUsedInWater()
	{
		return true;
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x00046920 File Offset: 0x00044B20
	public string GetStrikeEffectPath(string materialName)
	{
		for (int i = 0; i < this.materialStrikeFX.Count; i++)
		{
			if (this.materialStrikeFX[i].materialName == materialName && this.materialStrikeFX[i].fx.isValid)
			{
				return this.materialStrikeFX[i].fx.resourcePath;
			}
		}
		return this.strikeFX.resourcePath;
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x00046998 File Offset: 0x00044B98
	internal void DoThrow()
	{
		if (!this.canThrowAsProjectile)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ownerPlayer.IsHeadUnderwater())
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		ItemModProjectile component = item.info.GetComponent<ItemModProjectile>();
		if (component == null)
		{
			return;
		}
		ProjectileShoot projectileShoot = new ProjectileShoot();
		projectileShoot.projectiles = new List<ProjectileShoot.Projectile>();
		projectileShoot.ammoType = item.info.itemid;
		Vector3 position = ownerPlayer.eyes.position;
		HowToThrow.ItemThrown();
		float num = component.projectileSpread;
		if (ownerPlayer.IsRunning())
		{
			num += 6f;
		}
		AttackEntity component2 = base.gameManager.FindPrefab(this).GetComponent<AttackEntity>();
		Projectile component3 = component.projectileObject.Get().GetComponent<Projectile>();
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(num, ownerPlayer.eyes.BodyForward(), true);
		Vector3 vector = modifiedAimConeDirection * component.GetRandomVelocity();
		int seed = ownerPlayer.NewProjectileSeed();
		int projectileID = ownerPlayer.NewProjectileID();
		Projectile projectile = this.CreateProjectile(component.projectileObject.resourcePath, position, modifiedAimConeDirection, vector);
		if (projectile != null)
		{
			projectile.seed = seed;
			projectile.owner = ownerPlayer;
			projectile.sourceWeaponPrefab = component2;
			projectile.sourceProjectilePrefab = component3;
			projectile.projectileID = projectileID;
		}
		ProjectileShoot.Projectile projectile2 = new ProjectileShoot.Projectile();
		projectile2.projectileID = projectileID;
		projectile2.startPos = position;
		projectile2.startVel = vector;
		projectile2.seed = seed;
		projectileShoot.projectiles.Add(projectile2);
		base.ServerRPC<ProjectileShoot>("CLProject", projectileShoot);
		Vector3 inheritedThrowVelocity = ownerPlayer.GetInheritedThrowVelocity();
		projectile.AdjustVelocity(inheritedThrowVelocity);
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x00046B34 File Offset: 0x00044D34
	private Projectile CreateProjectile(string prefabPath, Vector3 pos, Vector3 forward, Vector3 velocity)
	{
		GameObject gameObject = base.gameManager.CreatePrefab(prefabPath, pos, Quaternion.LookRotation(forward), true);
		if (gameObject == null)
		{
			return null;
		}
		Projectile component = gameObject.GetComponent<Projectile>();
		component.InitializeVelocity(velocity);
		return component;
	}

	// Token: 0x02000070 RID: 112
	[Serializable]
	public class MaterialFX
	{
		// Token: 0x04000400 RID: 1024
		public string materialName;

		// Token: 0x04000401 RID: 1025
		public GameObjectRef fx;
	}
}
