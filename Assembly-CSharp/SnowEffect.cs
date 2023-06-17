using System;

// Token: 0x0200041D RID: 1053
public class SnowEffect : WeatherEffect
{
	// Token: 0x06001974 RID: 6516 RVA: 0x000150B9 File Offset: 0x000132B9
	protected override float GetCurrentIntensity()
	{
		return Climate.GetSnow(base.transform.position);
	}
}
