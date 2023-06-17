using System;

// Token: 0x02000587 RID: 1415
[Serializable]
public struct ExtendGBufferParams
{
	// Token: 0x04001C4E RID: 7246
	public bool enabled;

	// Token: 0x04001C4F RID: 7247
	public static ExtendGBufferParams Default = new ExtendGBufferParams
	{
		enabled = false
	};
}
