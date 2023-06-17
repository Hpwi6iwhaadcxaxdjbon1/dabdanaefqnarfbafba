using System;

// Token: 0x0200034F RID: 847
public class ElectricGenerator : IOEntity
{
	// Token: 0x04001309 RID: 4873
	public float electricAmount = 8f;

	// Token: 0x0600160D RID: 5645 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x0600160E RID: 5646 RVA: 0x00012A44 File Offset: 0x00010C44
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}
}
