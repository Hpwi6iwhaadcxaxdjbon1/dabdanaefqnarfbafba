using System;
using UnityEngine;

// Token: 0x0200041E RID: 1054
public class UnderwaterEffect : WeatherEffect
{
	// Token: 0x06001976 RID: 6518 RVA: 0x00090284 File Offset: 0x0008E484
	protected override float GetCurrentIntensity()
	{
		if (WaterSystem.Collision != null && MainCamera.mainCamera != null && MainCamera.isWaterVisible)
		{
			return WaterLevel.Factor(new Bounds(MainCamera.mainCamera.transform.position, Vector3.one));
		}
		return 0f;
	}
}
