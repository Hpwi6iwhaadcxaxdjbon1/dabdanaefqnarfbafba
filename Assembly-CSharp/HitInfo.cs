using System;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200039B RID: 923
public class HitInfo
{
	// Token: 0x04001449 RID: 5193
	public BaseEntity Initiator;

	// Token: 0x0400144A RID: 5194
	public BaseEntity WeaponPrefab;

	// Token: 0x0400144B RID: 5195
	public AttackEntity Weapon;

	// Token: 0x0400144C RID: 5196
	public bool DoHitEffects = true;

	// Token: 0x0400144D RID: 5197
	public bool DoDecals = true;

	// Token: 0x0400144E RID: 5198
	public bool IsPredicting;

	// Token: 0x0400144F RID: 5199
	public bool UseProtection = true;

	// Token: 0x04001450 RID: 5200
	public Connection Predicted;

	// Token: 0x04001451 RID: 5201
	public bool DidHit;

	// Token: 0x04001452 RID: 5202
	public BaseEntity HitEntity;

	// Token: 0x04001453 RID: 5203
	public uint HitBone;

	// Token: 0x04001454 RID: 5204
	public uint HitPart;

	// Token: 0x04001455 RID: 5205
	public uint HitMaterial;

	// Token: 0x04001456 RID: 5206
	public Vector3 HitPositionWorld;

	// Token: 0x04001457 RID: 5207
	public Vector3 HitPositionLocal;

	// Token: 0x04001458 RID: 5208
	public Vector3 HitNormalWorld;

	// Token: 0x04001459 RID: 5209
	public Vector3 HitNormalLocal;

	// Token: 0x0400145A RID: 5210
	public Vector3 PointStart;

	// Token: 0x0400145B RID: 5211
	public Vector3 PointEnd;

	// Token: 0x0400145C RID: 5212
	public int ProjectileID;

	// Token: 0x0400145D RID: 5213
	public float ProjectileDistance;

	// Token: 0x0400145E RID: 5214
	public Vector3 ProjectileVelocity;

	// Token: 0x0400145F RID: 5215
	public Projectile ProjectilePrefab;

	// Token: 0x04001460 RID: 5216
	public PhysicMaterial material;

	// Token: 0x04001461 RID: 5217
	public DamageProperties damageProperties;

	// Token: 0x04001462 RID: 5218
	public DamageTypeList damageTypes = new DamageTypeList();

	// Token: 0x04001463 RID: 5219
	public bool CanGather;

	// Token: 0x04001464 RID: 5220
	public bool DidGather;

	// Token: 0x04001465 RID: 5221
	public float gatherScale = 1f;

	// Token: 0x06001775 RID: 6005 RVA: 0x00013AC0 File Offset: 0x00011CC0
	public bool IsProjectile()
	{
		return this.ProjectileID != 0;
	}

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06001776 RID: 6006 RVA: 0x00013ACB File Offset: 0x00011CCB
	public BasePlayer InitiatorPlayer
	{
		get
		{
			if (!this.Initiator)
			{
				return null;
			}
			return this.Initiator.ToPlayer();
		}
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06001777 RID: 6007 RVA: 0x0008AAD0 File Offset: 0x00088CD0
	public Vector3 attackNormal
	{
		get
		{
			return (this.PointEnd - this.PointStart).normalized;
		}
	}

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06001778 RID: 6008 RVA: 0x00013AE7 File Offset: 0x00011CE7
	public bool hasDamage
	{
		get
		{
			return this.damageTypes.Total() > 0f;
		}
	}

	// Token: 0x06001779 RID: 6009 RVA: 0x00013AFB File Offset: 0x00011CFB
	public HitInfo()
	{
	}

	// Token: 0x0600177A RID: 6010 RVA: 0x0008AAF8 File Offset: 0x00088CF8
	public HitInfo(BaseEntity attacker, BaseEntity target, DamageType type, float damageAmount, Vector3 vhitPosition)
	{
		this.Initiator = attacker;
		this.HitEntity = target;
		this.HitPositionWorld = vhitPosition;
		if (attacker != null)
		{
			this.PointStart = attacker.transform.position;
		}
		this.damageTypes.Add(type, damageAmount);
	}

	// Token: 0x0600177B RID: 6011 RVA: 0x00013B2E File Offset: 0x00011D2E
	public HitInfo(BaseEntity attacker, BaseEntity target, DamageType type, float damageAmount) : this(attacker, target, type, damageAmount, target.transform.position)
	{
	}

	// Token: 0x0600177C RID: 6012 RVA: 0x0008AB74 File Offset: 0x00088D74
	public void LoadFromAttack(Attack attack, bool serverSide)
	{
		this.HitEntity = null;
		this.PointStart = attack.pointStart;
		this.PointEnd = attack.pointEnd;
		if (attack.hitID > 0U)
		{
			this.DidHit = true;
			if (!serverSide)
			{
				this.HitEntity = (BaseNetworkable.clientEntities.Find(attack.hitID) as BaseEntity);
			}
			if (this.HitEntity)
			{
				this.HitBone = attack.hitBone;
				this.HitPart = attack.hitPartID;
			}
		}
		this.DidHit = true;
		this.HitPositionLocal = attack.hitPositionLocal;
		this.HitPositionWorld = attack.hitPositionWorld;
		this.HitNormalLocal = attack.hitNormalLocal;
		this.HitNormalWorld = attack.hitNormalWorld;
		this.HitMaterial = attack.hitMaterialID;
	}

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x0600177D RID: 6013 RVA: 0x0008AC38 File Offset: 0x00088E38
	public bool isHeadshot
	{
		get
		{
			if (this.HitEntity == null)
			{
				return false;
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return false;
			}
			if (baseCombatEntity.skeletonProperties == null)
			{
				return false;
			}
			SkeletonProperties.BoneProperty boneProperty = baseCombatEntity.skeletonProperties.FindBone(this.HitBone);
			return boneProperty != null && boneProperty.area == HitArea.Head;
		}
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x0600177E RID: 6014 RVA: 0x0008AC9C File Offset: 0x00088E9C
	public Translate.Phrase bonePhrase
	{
		get
		{
			if (this.HitEntity == null)
			{
				return null;
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return null;
			}
			if (baseCombatEntity.skeletonProperties == null)
			{
				return null;
			}
			SkeletonProperties.BoneProperty boneProperty = baseCombatEntity.skeletonProperties.FindBone(this.HitBone);
			if (boneProperty == null)
			{
				return null;
			}
			return boneProperty.name;
		}
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x0600177F RID: 6015 RVA: 0x0008AD00 File Offset: 0x00088F00
	public string boneName
	{
		get
		{
			Translate.Phrase bonePhrase = this.bonePhrase;
			if (bonePhrase != null)
			{
				return bonePhrase.english;
			}
			return "N/A";
		}
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06001780 RID: 6016 RVA: 0x0008AD24 File Offset: 0x00088F24
	public HitArea boneArea
	{
		get
		{
			if (this.HitEntity == null)
			{
				return (HitArea)(-1);
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return (HitArea)(-1);
			}
			return baseCombatEntity.SkeletonLookup(this.HitBone);
		}
	}

	// Token: 0x06001781 RID: 6017 RVA: 0x0008AD64 File Offset: 0x00088F64
	public Vector3 PositionOnRay(Vector3 position)
	{
		Ray ray = new Ray(this.PointStart, this.attackNormal);
		if (this.ProjectilePrefab == null)
		{
			return ray.ClosestPoint(position);
		}
		Sphere sphere;
		sphere..ctor(position, this.ProjectilePrefab.thickness);
		RaycastHit raycastHit;
		if (sphere.Trace(ray, ref raycastHit, float.PositiveInfinity))
		{
			return raycastHit.point;
		}
		return position;
	}

	// Token: 0x06001782 RID: 6018 RVA: 0x00013B46 File Offset: 0x00011D46
	public Vector3 HitPositionOnRay()
	{
		return this.PositionOnRay(this.HitPositionWorld);
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x0008ADC8 File Offset: 0x00088FC8
	public bool IsNaNOrInfinity()
	{
		return Vector3Ex.IsNaNOrInfinity(this.PointStart) || Vector3Ex.IsNaNOrInfinity(this.PointEnd) || Vector3Ex.IsNaNOrInfinity(this.HitPositionWorld) || Vector3Ex.IsNaNOrInfinity(this.HitPositionLocal) || Vector3Ex.IsNaNOrInfinity(this.HitNormalWorld) || Vector3Ex.IsNaNOrInfinity(this.HitNormalLocal) || Vector3Ex.IsNaNOrInfinity(this.ProjectileVelocity) || float.IsNaN(this.ProjectileDistance) || float.IsInfinity(this.ProjectileDistance);
	}
}
