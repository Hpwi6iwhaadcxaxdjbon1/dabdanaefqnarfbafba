using System;
using UnityEngine;

// Token: 0x02000577 RID: 1399
[Serializable]
public struct FogSettings
{
	// Token: 0x04001C01 RID: 7169
	public Gradient ColorOverDaytime;

	// Token: 0x04001C02 RID: 7170
	public float Density;

	// Token: 0x04001C03 RID: 7171
	public float StartDistance;

	// Token: 0x04001C04 RID: 7172
	public float Height;

	// Token: 0x04001C05 RID: 7173
	public float HeightDensity;

	// Token: 0x04001C06 RID: 7174
	public static FogSettings Default = new FogSettings
	{
		ColorOverDaytime = new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(Color.gray, 0f),
				new GradientColorKey(Color.gray, 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		},
		Density = 0.001f,
		StartDistance = 0f,
		Height = 0f,
		HeightDensity = 0.5f
	};

	// Token: 0x06001FF7 RID: 8183 RVA: 0x000AEA50 File Offset: 0x000ACC50
	public static FogSettings Lerp(FogSettings source, FogSettings target, float t)
	{
		return new FogSettings
		{
			Density = Mathf.Lerp(source.Density, target.Density, t),
			StartDistance = Mathf.Lerp(source.StartDistance, target.StartDistance, t),
			Height = Mathf.Lerp(source.Height, target.Height, t),
			HeightDensity = Mathf.Lerp(source.HeightDensity, target.HeightDensity, t)
		};
	}
}
