using System;

// Token: 0x0200041C RID: 1052
public class RainEffect : WeatherEffect
{
	// Token: 0x06001972 RID: 6514 RVA: 0x0001509F File Offset: 0x0001329F
	protected override float GetCurrentIntensity()
	{
		return Climate.GetRain(base.transform.position);
	}
}
