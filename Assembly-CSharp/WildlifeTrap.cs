using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200032C RID: 812
public class WildlifeTrap : StorageContainer
{
	// Token: 0x04001275 RID: 4725
	public float tickRate = 60f;

	// Token: 0x04001276 RID: 4726
	public GameObjectRef trappedEffect;

	// Token: 0x04001277 RID: 4727
	public float trappedEffectRepeatRate = 30f;

	// Token: 0x04001278 RID: 4728
	public float trapSuccessRate = 0.5f;

	// Token: 0x04001279 RID: 4729
	public List<ItemDefinition> ignoreBait;

	// Token: 0x0400127A RID: 4730
	public List<WildlifeTrap.WildlifeWeight> targetWildlife;

	// Token: 0x0400127B RID: 4731
	protected float nextEffectTime;

	// Token: 0x0600159A RID: 5530 RVA: 0x00012460 File Offset: 0x00010660
	public override void ResetState()
	{
		base.ResetState();
		this.nextEffectTime = 0f;
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x00012473 File Offset: 0x00010673
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		this.TrappedEffect();
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x00084B3C File Offset: 0x00082D3C
	public virtual void TrappedEffect()
	{
		if (this.HasCatch() && Time.realtimeSinceStartup >= this.nextEffectTime)
		{
			this.nextEffectTime = Time.realtimeSinceStartup + this.trappedEffectRepeatRate * Random.Range(0.8f, 1.2f);
			Effect.client.Run(this.trappedEffect.resourcePath, base.transform.position, base.transform.up, default(Vector3));
		}
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x00004723 File Offset: 0x00002923
	public bool HasCatch()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x0600159E RID: 5534 RVA: 0x000056A3 File Offset: 0x000038A3
	public bool IsTrapActive()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x0200032D RID: 813
	[Serializable]
	public class WildlifeWeight
	{
		// Token: 0x0400127C RID: 4732
		public TrappableWildlife wildlife;

		// Token: 0x0400127D RID: 4733
		public int weight;
	}

	// Token: 0x0200032E RID: 814
	public static class WildlifeTrapFlags
	{
		// Token: 0x0400127E RID: 4734
		public const BaseEntity.Flags Occupied = BaseEntity.Flags.Reserved1;
	}
}
