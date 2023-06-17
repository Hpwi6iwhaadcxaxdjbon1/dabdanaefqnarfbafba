using System;
using UnityEngine;

// Token: 0x02000288 RID: 648
public class ScaleParticleSystem : ScaleRenderer
{
	// Token: 0x04000F19 RID: 3865
	public ParticleSystem pSystem;

	// Token: 0x04000F1A RID: 3866
	public bool scaleGravity;

	// Token: 0x04000F1B RID: 3867
	[NonSerialized]
	private float startSize;

	// Token: 0x04000F1C RID: 3868
	[NonSerialized]
	private float startLifeTime;

	// Token: 0x04000F1D RID: 3869
	[NonSerialized]
	private float startSpeed;

	// Token: 0x04000F1E RID: 3870
	[NonSerialized]
	private float startGravity;

	// Token: 0x06001272 RID: 4722 RVA: 0x00078DEC File Offset: 0x00076FEC
	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		this.startGravity = this.pSystem.gravityModifier;
		this.startSpeed = this.pSystem.startSpeed;
		this.startSize = this.pSystem.startSize;
		this.startLifeTime = this.pSystem.startLifetime;
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x00078E44 File Offset: 0x00077044
	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.pSystem.startSize = this.startSize * scale;
		this.pSystem.startLifetime = this.startLifeTime * scale;
		this.pSystem.startSpeed = this.startSpeed * scale;
		this.pSystem.gravityModifier = this.startGravity * scale;
	}
}
