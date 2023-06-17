using System;
using UnityEngine;

// Token: 0x020003CC RID: 972
public class ParticleCollisionLOD : LODComponentParticleSystem
{
	// Token: 0x04001502 RID: 5378
	[Horizontal(1, 0)]
	public ParticleCollisionLOD.State[] States;

	// Token: 0x0600187E RID: 6270 RVA: 0x0008D280 File Offset: 0x0008B480
	private void UpdateLOD(int newlod)
	{
		ParticleSystem.CollisionModule collision = this.particleSystem.collision;
		collision.enabled = (this.States[newlod].quality != ParticleCollisionLOD.QualityLevel.Disabled);
		if (collision.enabled)
		{
			collision.quality = this.States[newlod].quality;
		}
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x00014806 File Offset: 0x00012A06
	protected override void SetLOD(int newlod)
	{
		if (this.curlod != newlod)
		{
			this.UpdateLOD(newlod);
			this.curlod = newlod;
			return;
		}
		if (this.force)
		{
			this.UpdateLOD(newlod);
			this.force = false;
		}
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x0008D2D0 File Offset: 0x0008B4D0
	protected override int GetLOD(float distance)
	{
		for (int i = this.States.Length - 1; i >= 0; i--)
		{
			if (distance >= LODUtil.VerifyDistance(this.States[i].distance))
			{
				return i;
			}
		}
		return 0;
	}

	// Token: 0x020003CD RID: 973
	public enum QualityLevel
	{
		// Token: 0x04001504 RID: 5380
		Disabled = -1,
		// Token: 0x04001505 RID: 5381
		HighQuality,
		// Token: 0x04001506 RID: 5382
		MediumQuality,
		// Token: 0x04001507 RID: 5383
		LowQuality
	}

	// Token: 0x020003CE RID: 974
	[Serializable]
	public class State
	{
		// Token: 0x04001508 RID: 5384
		public float distance;

		// Token: 0x04001509 RID: 5385
		public ParticleCollisionLOD.QualityLevel quality = ParticleCollisionLOD.QualityLevel.Disabled;
	}
}
