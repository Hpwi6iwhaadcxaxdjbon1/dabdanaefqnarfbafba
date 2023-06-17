using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000325 RID: 805
public class TimedExplosive : BaseEntity
{
	// Token: 0x0400125B RID: 4699
	public float timerAmountMin = 10f;

	// Token: 0x0400125C RID: 4700
	public float timerAmountMax = 20f;

	// Token: 0x0400125D RID: 4701
	public float minExplosionRadius;

	// Token: 0x0400125E RID: 4702
	public float explosionRadius = 10f;

	// Token: 0x0400125F RID: 4703
	public bool canStick;

	// Token: 0x04001260 RID: 4704
	public bool onlyDamageParent;

	// Token: 0x04001261 RID: 4705
	public GameObjectRef explosionEffect;

	// Token: 0x04001262 RID: 4706
	public GameObjectRef stickEffect;

	// Token: 0x04001263 RID: 4707
	public GameObjectRef bounceEffect;

	// Token: 0x04001264 RID: 4708
	public bool explosionUsesForward;

	// Token: 0x04001265 RID: 4709
	public bool waterCausesExplosion;

	// Token: 0x04001266 RID: 4710
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	// Token: 0x04001267 RID: 4711
	[NonSerialized]
	private float lastBounceTime;

	// Token: 0x0600158E RID: 5518 RVA: 0x0000481F File Offset: 0x00002A1F
	public override float GetExtrapolationTime()
	{
		return 0f;
	}

	// Token: 0x0600158F RID: 5519 RVA: 0x00084A88 File Offset: 0x00082C88
	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = base.GetComponent<Collider>();
		if (component.enabled != wantsCollision)
		{
			component.enabled = wantsCollision;
		}
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool PhysicsDriven()
	{
		return true;
	}
}
