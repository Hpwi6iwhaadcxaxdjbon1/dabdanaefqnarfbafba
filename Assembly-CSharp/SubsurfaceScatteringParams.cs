using System;

// Token: 0x02000585 RID: 1413
[Serializable]
public struct SubsurfaceScatteringParams
{
	// Token: 0x04001C45 RID: 7237
	public bool enabled;

	// Token: 0x04001C46 RID: 7238
	public SubsurfaceScatteringParams.Quality quality;

	// Token: 0x04001C47 RID: 7239
	public bool halfResolution;

	// Token: 0x04001C48 RID: 7240
	public float radiusScale;

	// Token: 0x04001C49 RID: 7241
	public static SubsurfaceScatteringParams Default = new SubsurfaceScatteringParams
	{
		enabled = true,
		quality = SubsurfaceScatteringParams.Quality.Medium,
		halfResolution = true,
		radiusScale = 1f
	};

	// Token: 0x02000586 RID: 1414
	public enum Quality
	{
		// Token: 0x04001C4B RID: 7243
		Low,
		// Token: 0x04001C4C RID: 7244
		Medium,
		// Token: 0x04001C4D RID: 7245
		High
	}
}
