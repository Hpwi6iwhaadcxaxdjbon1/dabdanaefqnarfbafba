using System;
using UnityEngine;

// Token: 0x02000737 RID: 1847
public class FixedRateStepped
{
	// Token: 0x040023DD RID: 9181
	public float rate = 0.1f;

	// Token: 0x040023DE RID: 9182
	public int maxSteps = 3;

	// Token: 0x040023DF RID: 9183
	internal float nextCall;

	// Token: 0x06002847 RID: 10311 RVA: 0x000CF588 File Offset: 0x000CD788
	public bool ShouldStep()
	{
		if (this.nextCall > Time.time)
		{
			return false;
		}
		if (this.nextCall == 0f)
		{
			this.nextCall = Time.time;
		}
		if (this.nextCall + this.rate * (float)this.maxSteps < Time.time)
		{
			this.nextCall = Time.time - this.rate * (float)this.maxSteps;
		}
		this.nextCall += this.rate;
		return true;
	}
}
