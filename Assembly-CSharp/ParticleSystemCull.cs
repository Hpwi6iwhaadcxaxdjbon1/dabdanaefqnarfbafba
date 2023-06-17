using System;

// Token: 0x020003CF RID: 975
public class ParticleSystemCull : LODComponentParticleSystem
{
	// Token: 0x0400150A RID: 5386
	public float Distance = 100f;

	// Token: 0x06001883 RID: 6275 RVA: 0x0008D30C File Offset: 0x0008B50C
	protected override void SetLOD(int newlod)
	{
		if (this.curlod != newlod)
		{
			if (newlod == 0)
			{
				this.Show();
			}
			else
			{
				this.Hide();
			}
			this.curlod = newlod;
			return;
		}
		if (this.force)
		{
			if (newlod == 0)
			{
				this.Show();
			}
			else
			{
				this.Hide();
			}
			this.force = false;
		}
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x0001484D File Offset: 0x00012A4D
	protected override int GetLOD(float distance)
	{
		if (distance < LODUtil.VerifyDistance(this.Distance))
		{
			return 0;
		}
		return 1;
	}
}
