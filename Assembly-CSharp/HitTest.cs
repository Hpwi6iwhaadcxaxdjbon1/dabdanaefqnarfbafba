using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000399 RID: 921
public class HitTest
{
	// Token: 0x0400142F RID: 5167
	public HitTest.Type type;

	// Token: 0x04001430 RID: 5168
	public Ray AttackRay;

	// Token: 0x04001431 RID: 5169
	public float Radius;

	// Token: 0x04001432 RID: 5170
	public float Forgiveness;

	// Token: 0x04001433 RID: 5171
	public float MaxDistance;

	// Token: 0x04001434 RID: 5172
	public RaycastHit RayHit;

	// Token: 0x04001435 RID: 5173
	public bool MultiHit;

	// Token: 0x04001436 RID: 5174
	public bool BestHit;

	// Token: 0x04001437 RID: 5175
	public bool DidHit;

	// Token: 0x04001438 RID: 5176
	public DamageProperties damageProperties;

	// Token: 0x04001439 RID: 5177
	public GameObject gameObject;

	// Token: 0x0400143A RID: 5178
	public Collider collider;

	// Token: 0x0400143B RID: 5179
	public BaseEntity ignoreEntity;

	// Token: 0x0400143C RID: 5180
	public BaseEntity HitEntity;

	// Token: 0x0400143D RID: 5181
	public Vector3 HitPoint;

	// Token: 0x0400143E RID: 5182
	public Vector3 HitNormal;

	// Token: 0x0400143F RID: 5183
	public float HitDistance;

	// Token: 0x04001440 RID: 5184
	public Transform HitTransform;

	// Token: 0x04001441 RID: 5185
	public uint HitPart;

	// Token: 0x04001442 RID: 5186
	public string HitMaterial;

	// Token: 0x06001770 RID: 6000 RVA: 0x0008A774 File Offset: 0x00088974
	public Vector3 HitPointWorld()
	{
		if (this.HitEntity != null)
		{
			Transform transform = this.HitTransform;
			if (!transform)
			{
				transform = this.HitEntity.transform;
			}
			return transform.TransformPoint(this.HitPoint);
		}
		return this.HitPoint;
	}

	// Token: 0x06001771 RID: 6001 RVA: 0x0008A7C0 File Offset: 0x000889C0
	public Vector3 HitNormalWorld()
	{
		if (this.HitEntity != null)
		{
			Transform transform = this.HitTransform;
			if (!transform)
			{
				transform = this.HitEntity.transform;
			}
			return transform.TransformDirection(this.HitNormal);
		}
		return this.HitNormal;
	}

	// Token: 0x06001772 RID: 6002 RVA: 0x0008A80C File Offset: 0x00088A0C
	public Attack BuildAttackMessage()
	{
		uint num = 0U;
		uint num2 = 0U;
		if (this.HitTransform && this.HitEntity && this.HitEntity.transform != this.HitTransform)
		{
			num = StringPool.Get(this.HitTransform.name);
			if (num == 0U)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Unpooled bone name: ",
					this.HitTransform.name,
					" (",
					this.HitTransform.gameObject,
					")"
				}));
			}
		}
		if (!string.IsNullOrEmpty(this.HitMaterial))
		{
			num2 = StringPool.Get(this.HitMaterial);
			if (num2 == 0U)
			{
				Debug.LogWarning("Unpooled Material Name: " + this.HitMaterial);
			}
		}
		Attack attack = Pool.Get<Attack>();
		attack.pointStart = this.AttackRay.origin;
		attack.pointEnd = this.AttackRay.origin + this.AttackRay.direction * this.MaxDistance;
		attack.hitMaterialID = num2;
		if (this.DidHit)
		{
			if (this.HitEntity.IsValid())
			{
				Transform transform = this.HitTransform;
				if (!transform)
				{
					transform = this.HitEntity.transform;
				}
				attack.hitID = this.HitEntity.net.ID;
				attack.hitBone = num;
				attack.hitPartID = this.HitPart;
				attack.hitPositionWorld = transform.localToWorldMatrix.MultiplyPoint(this.HitPoint);
				attack.hitPositionLocal = this.HitPoint;
				attack.hitNormalWorld = transform.localToWorldMatrix.MultiplyVector(this.HitNormal);
				attack.hitNormalLocal = this.HitNormal;
			}
			else
			{
				attack.hitID = 0U;
				attack.hitBone = 0U;
				attack.hitPositionWorld = this.HitPoint;
				attack.hitPositionLocal = this.HitPoint;
				attack.hitNormalWorld = this.HitNormal;
				attack.hitNormalLocal = this.HitNormal;
			}
		}
		return attack;
	}

	// Token: 0x06001773 RID: 6003 RVA: 0x0008AA10 File Offset: 0x00088C10
	public void Clear()
	{
		this.type = HitTest.Type.Generic;
		this.AttackRay = default(Ray);
		this.Radius = 0f;
		this.Forgiveness = 0f;
		this.MaxDistance = 0f;
		this.RayHit = default(RaycastHit);
		this.MultiHit = false;
		this.BestHit = false;
		this.DidHit = false;
		this.damageProperties = null;
		this.gameObject = null;
		this.collider = null;
		this.ignoreEntity = null;
		this.HitEntity = null;
		this.HitPoint = default(Vector3);
		this.HitNormal = default(Vector3);
		this.HitDistance = 0f;
		this.HitTransform = null;
		this.HitPart = 0U;
		this.HitMaterial = null;
	}

	// Token: 0x0200039A RID: 922
	public enum Type
	{
		// Token: 0x04001444 RID: 5188
		Generic,
		// Token: 0x04001445 RID: 5189
		ProjectileEffect,
		// Token: 0x04001446 RID: 5190
		Projectile,
		// Token: 0x04001447 RID: 5191
		MeleeAttack,
		// Token: 0x04001448 RID: 5192
		Use
	}
}
