using System;

// Token: 0x02000324 RID: 804
public class StabilityEntity : DecayEntity
{
	// Token: 0x04001258 RID: 4696
	public bool grounded;

	// Token: 0x04001259 RID: 4697
	[NonSerialized]
	public float cachedStability;

	// Token: 0x0400125A RID: 4698
	[NonSerialized]
	public int cachedDistanceFromGround = int.MaxValue;

	// Token: 0x0600158B RID: 5515 RVA: 0x000123A4 File Offset: 0x000105A4
	public override void ResetState()
	{
		base.ResetState();
		this.cachedStability = 0f;
		this.cachedDistanceFromGround = int.MaxValue;
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x00084A0C File Offset: 0x00082C0C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.stabilityEntity != null)
		{
			this.cachedStability = info.msg.stabilityEntity.stability;
			this.cachedDistanceFromGround = info.msg.stabilityEntity.distanceFromGround;
			if (this.cachedStability <= 0f)
			{
				this.cachedStability = 0f;
			}
			if (this.cachedDistanceFromGround <= 0)
			{
				this.cachedDistanceFromGround = int.MaxValue;
			}
		}
	}
}
