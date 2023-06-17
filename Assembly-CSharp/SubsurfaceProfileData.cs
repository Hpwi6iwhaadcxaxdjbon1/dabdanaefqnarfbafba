using System;
using UnityEngine;

// Token: 0x0200058D RID: 1421
[Serializable]
public struct SubsurfaceProfileData
{
	// Token: 0x04001C73 RID: 7283
	[Range(0.1f, 50f)]
	public float ScatterRadius;

	// Token: 0x04001C74 RID: 7284
	[ColorUsage(false, true, 1f, 1f, 1f, 1f)]
	public Color SubsurfaceColor;

	// Token: 0x04001C75 RID: 7285
	[ColorUsage(false, true, 1f, 1f, 1f, 1f)]
	public Color FalloffColor;

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06002079 RID: 8313 RVA: 0x000B0DDC File Offset: 0x000AEFDC
	public static SubsurfaceProfileData Default
	{
		get
		{
			return new SubsurfaceProfileData
			{
				ScatterRadius = 1.2f,
				SubsurfaceColor = new Color(0.48f, 0.41f, 0.28f),
				FalloffColor = new Color(1f, 0.37f, 0.3f)
			};
		}
	}

	// Token: 0x1700020F RID: 527
	// (get) Token: 0x0600207A RID: 8314 RVA: 0x000B0E34 File Offset: 0x000AF034
	public static SubsurfaceProfileData Invalid
	{
		get
		{
			return new SubsurfaceProfileData
			{
				ScatterRadius = 0f,
				SubsurfaceColor = Color.clear,
				FalloffColor = Color.clear
			};
		}
	}
}
