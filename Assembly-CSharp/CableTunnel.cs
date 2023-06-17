using System;

// Token: 0x020000D4 RID: 212
public class CableTunnel : IOEntity
{
	// Token: 0x040006DC RID: 1756
	private const int numChannels = 4;

	// Token: 0x06000A79 RID: 2681 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool WantsPower()
	{
		return true;
	}
}
