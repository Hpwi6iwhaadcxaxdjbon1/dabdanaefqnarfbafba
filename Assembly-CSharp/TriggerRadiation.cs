using System;
using UnityEngine;

// Token: 0x0200040A RID: 1034
public class TriggerRadiation : TriggerBase
{
	// Token: 0x040015BA RID: 5562
	public TriggerRadiation.RadiationTier radiationTier = TriggerRadiation.RadiationTier.LOW;

	// Token: 0x040015BB RID: 5563
	public float RadiationAmountOverride;

	// Token: 0x040015BC RID: 5564
	public float radiationSize;

	// Token: 0x040015BD RID: 5565
	public float falloff = 0.1f;

	// Token: 0x06001944 RID: 6468 RVA: 0x00008E27 File Offset: 0x00007027
	public void Awake()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x0008F574 File Offset: 0x0008D774
	public float GetRadiationAmount()
	{
		if (this.RadiationAmountOverride > 0f)
		{
			return this.RadiationAmountOverride;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.MINIMAL)
		{
			return 2f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.LOW)
		{
			return 10f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.MEDIUM)
		{
			return 25f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.HIGH)
		{
			return 51f;
		}
		return 1f;
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x0008F5D8 File Offset: 0x0008D7D8
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, this.radiationSize);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radiationSize * (1f - this.falloff));
	}

	// Token: 0x0200040B RID: 1035
	public enum RadiationTier
	{
		// Token: 0x040015BF RID: 5567
		MINIMAL,
		// Token: 0x040015C0 RID: 5568
		LOW,
		// Token: 0x040015C1 RID: 5569
		MEDIUM,
		// Token: 0x040015C2 RID: 5570
		HIGH
	}
}
